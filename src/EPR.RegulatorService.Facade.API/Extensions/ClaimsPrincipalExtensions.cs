using Microsoft.Identity.Web;
using System.Security.Claims;

namespace EPR.RegulatorService.Facade.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string ClaimConstantEmails = "emails";

    public static Guid UserId(this ClaimsPrincipal user) => 
        Guid.Parse(user.Claims.Single(claim => claim.Type == ClaimConstants.ObjectId).Value);

    public static string Email(this ClaimsPrincipal user) => 
        user.Claims.Single(claim => claim.Type == ClaimConstantEmails).Value;
}
