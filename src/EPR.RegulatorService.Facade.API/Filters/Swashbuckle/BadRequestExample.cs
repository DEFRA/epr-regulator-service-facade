using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle;

[ExcludeFromCodeCoverage]
public class BadRequestExample : IExamplesProvider<ValidationProblemDetails>
{
    public ValidationProblemDetails GetExamples()
    {
        return new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = new Dictionary<string, string[]>()
              {
                { "Status", ["The Status field is required."] }
              },
        };
    }
}
