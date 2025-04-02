using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EPR.RegulatorService.Facade.API.Helpers;

public class FeatureEnabledDocumentFilter(IFeatureManager featureManager) : IDocumentFilter
{
    public async void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new List<string>();

        foreach (var path in swaggerDoc.Paths)
        {

            var shouldRemove = await ShouldRemovePath(path.Key, path.Value.Operations, context);
            if (shouldRemove)
            {
                pathsToRemove.Add(path.Key);
            }
        }

        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
    }

    private async Task<bool> ShouldRemovePath(string pathKey, IDictionary<OperationType, OpenApiOperation> operations, DocumentFilterContext context)
    {
        foreach (var operation in operations)
        {
            var apiDescription = context.ApiDescriptions.FirstOrDefault(desc => desc.RelativePath!.Equals(pathKey.Trim('/'), StringComparison.InvariantCultureIgnoreCase));

            if (apiDescription != null)
            {
                var controllerActionDescriptor = apiDescription.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;

                if (controllerActionDescriptor != null)
                {
                    var isDisabled = await IsControllerOrActionDisabled(controllerActionDescriptor, apiDescription);
                    if (isDisabled)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private async Task<bool> IsControllerOrActionDisabled(ControllerActionDescriptor controllerActionDescriptor, ApiDescription apiDescription)
    {
        var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
        var controllerFeatureGate = controllerTypeInfo.GetCustomAttributes(typeof(FeatureGateAttribute), true).Cast<FeatureGateAttribute>().FirstOrDefault();

        var controllerFeaturesEnabled = await AreAllFeaturesEnabled(controllerFeatureGate?.Features ?? Array.Empty<string>());
        if (controllerFeatureGate != null && !controllerFeaturesEnabled)
        {
            return true;
        }

        var actionFeatureGate = apiDescription.ActionDescriptor.EndpointMetadata.OfType<FeatureGateAttribute>().FirstOrDefault();

        var actionFeaturesEnabled = await AreAllFeaturesEnabled(actionFeatureGate?.Features ?? Array.Empty<string>());
        if (actionFeatureGate != null && !actionFeaturesEnabled)
        {
            return true;
        }

        return false;
    }


    private async Task<bool> AreAllFeaturesEnabled(IEnumerable<string> features)
    {
        foreach (var feature in features)
        {
            if (!await featureManager.IsEnabledAsync(feature))
            {
                return false;
            }
        }
        return true;
    }
}
