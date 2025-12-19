using Microsoft.Identity.Web;
using System.Security.Claims;

namespace EPR.RegulatorService.Facade.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string ClaimConstantEmails = "emails";

    /// <summary>
    /// Get UserId from claims, or return null if missing
    /// </summary>
    /// <returns>UserId from claims or null if missing</returns>
    public static Guid UserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.SingleOrDefault(claim => claim.Type == ClaimConstants.ObjectId);
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("UserId not found in claims");
        }
        return Guid.Parse(userIdClaim.Value);
    }

    public static string Email(this ClaimsPrincipal user) => 
        user.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ??
        /* Remove and switch above .SingleOrDefault() to .Single()
           when we migrate all environments to custom policy */
        user.Claims.Single(claim => claim.Type == ClaimConstantEmails).Value;
}
