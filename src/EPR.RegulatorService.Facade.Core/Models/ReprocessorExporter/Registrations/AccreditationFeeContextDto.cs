﻿using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class AccreditationFeeContextDto
{
    public Guid AccreditationId { get; set; }
    public Guid OrganisationId { get; set; }
    public int NationId { get; set; }
    public string MaterialName { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public PrnTonnageType PrnTonnage { get; set; }
    public ApplicationOrganisationType ApplicationType { get; init; }
    public string SiteAddress { get; set; }
    public DateTime SubmittedDate { get; set; }
}