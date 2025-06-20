using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Filters.Swashbuckle;
using EPR.RegulatorService.Facade.API.HealthChecks;
using EPR.RegulatorService.Facade.API.Helpers;
using EPR.RegulatorService.Facade.API.Helpers;
using EPR.RegulatorService.Facade.API.Middlewares;
using EPR.RegulatorService.Facade.API.Middlewares;
using EPR.RegulatorService.Facade.API.Swagger;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using EPR.RegulatorService.Facade.API.Helpers;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using FluentValidation.AspNetCore;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationInsightsTelemetry()
    .AddHealthChecks();

builder.Services.RegisterComponents(builder.Configuration);

// Services & HttpClients
builder.Services.AddServicesAndHttpClients();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddFeatureManagement();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddFeatureManagement();

// Logging
builder.Services.AddLogging();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
    }, options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
    });


// Authorization
var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
builder.Services.AddAuthorizationBuilder().AddPolicy("AuthUser", policy);

// General Config
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<UpdateMaterialOutcomeRequestDtoValidator>();
    fv.AutomaticValidationEnabled = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.OperationFilter<AddAuthHeaderOperationFilter>();
    options.OperationFilter<SwashbuckleHeaderFilter>();
    options.OperationFilter<ExampleRequestsFilter>();
    options.DocumentFilter<FeatureEnabledDocumentFilter>();
    options.OperationFilter<FeatureGateOperationFilter>();
    options.EnableAnnotations();
});

// App
var app = builder.Build();


app.UseExceptionHandler(app.Environment.IsDevelopment() ? "/error-development" : "/error");
app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks(
    builder.Configuration.GetValue<string>("HealthCheckPath"),
    HealthCheckOptionBuilder.Build()).AllowAnonymous();

await app.RunAsync();