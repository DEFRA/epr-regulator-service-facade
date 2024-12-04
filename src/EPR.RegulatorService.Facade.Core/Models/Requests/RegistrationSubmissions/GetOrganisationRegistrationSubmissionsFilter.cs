﻿using EPR.RegulatorService.Facade.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class GetOrganisationRegistrationSubmissionsFilter
{
    public string? OrganisationName { get; set; }
    public string? OrganisationReference { get; set; }
    public string? OrganisationType { get; set; }
    public string? Statuses { get; set; }
    public string? RelevantYears { get; set; }

    [Required]
    [Range(1, 4, ErrorMessage = "The nationId must be valid")]
    public int NationId { get; set; }

    [Required]
    public int? PageNumber { get; set; } = 1;
    
    [Required]
    public int? PageSize { get; set; } = 20;

    [Required]
    [NotNull]
    [NotDefault]
    public Guid UserId { get; set; }

    public static implicit operator GetOrganisationRegistrationSubmissionsCommonDataFilter(GetOrganisationRegistrationSubmissionsFilter rhs) => new()
    {
        NationId = rhs.NationId,
        OrganisationName = rhs.OrganisationName,
        OrganisationReference = rhs.OrganisationReference,
        OrganisationType = rhs.OrganisationType,
        PageNumber = rhs.PageNumber,
        PageSize = rhs.PageSize,
        RelevantYears = rhs.RelevantYears,
        Statuses = rhs.Statuses
    };
}

[ExcludeFromCodeCoverage]
public class GetOrganisationRegistrationSubmissionsCommonDataFilter
{
    public string? OrganisationName { get; set; }
    public string? OrganisationReference { get; set; }
    public string? OrganisationType { get; set; }
    public string? Statuses { get; set; }
    public string? RelevantYears { get; set; }

    [Required]
    [Range(1, 4, ErrorMessage = "The nationId must be valid")]
    public int NationId { get; set; }

    [Required]
    public int? PageNumber { get; set; } = 1;

    [Required]
    public int? PageSize { get; set; } = 20;

    public string? ApplicationReferenceNumbers { get; set; }
}