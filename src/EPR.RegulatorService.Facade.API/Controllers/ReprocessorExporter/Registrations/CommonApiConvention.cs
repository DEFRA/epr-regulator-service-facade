﻿using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations
{
    [ExcludeFromCodeCoverage]
    public static class CommonApiConvention
    {
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
        [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
        public static void Post() { }
    }
}
