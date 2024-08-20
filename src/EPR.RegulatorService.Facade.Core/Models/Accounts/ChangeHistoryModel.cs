using EPR.RegulatorService.Facade.Core.Models.Applications;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts;

[ExcludeFromCodeCoverage]
public class ChangeHistoryModel
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public int OrganisationId { get; set; }

    public string? OrganisationName { get; set; }

    public string? Nation { get; set; }

    public string? OrganisationNumber { get; set; }

    public UserDetailsChangeModel? OldValues { get; set; }

    public UserDetailsChangeModel? NewValues { get; set; }

    public bool IsActive { get; set; } = false;

    public string? ApproverComments { get; set; }

    public int? ApprovedById { get; set; }

    public DateTimeOffset? DecisionDate { get; set; }

    public DateTimeOffset? DeclarationDate { get; set; }

    public Guid ExternalId { get; set; }

    public DateTimeOffset? CreatedOn { get; set; }

    public DateTimeOffset? LastUpdatedOn { get; set; }

    public string? Telephone { get; set; }

    public string? EmailAddress { get; set; }

    public string OrganisationType { get; set; }
    public string OrganisationReferenceNumber { get; set; }
    public string CompaniesHouseNumber { get; set; }

    public AddressModel BusinessAddress { get; set; }

}