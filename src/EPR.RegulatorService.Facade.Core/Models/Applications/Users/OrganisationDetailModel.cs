using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Applications.Users;

[ExcludeFromCodeCoverage]
public class OrganisationDetailModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string OrganisationRole { get; set; }
    public string OrganisationType { get; set; }
    public int? NationId { get; set; }
}