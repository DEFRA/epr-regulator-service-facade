using Microsoft.FeatureManagement.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EPR.RegulatorService.Facade.API.Helpers;
public class FeatureGateOperationFilter(IFeatureManager featureManager) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var featureGateAttributes = context.MethodInfo.GetCustomAttributes(typeof(FeatureGateAttribute), false) as FeatureGateAttribute[];

        if (featureGateAttributes != null)
        {
            foreach (var featureGateAttribute in featureGateAttributes)
            {
                foreach (var featureName in featureGateAttribute.Features)
                {
                    var featureEnabled = featureManager.IsEnabledAsync(featureName).Result;

                    if (!featureEnabled)
                    {
                        operation.Deprecated = true;
                        operation.Description += " (This feature is currently disabled)";
                    }
                }
            }
        }
    }
}
