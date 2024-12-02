﻿using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions.CommonData
{
    public class OrganisationRegistrationFilterRequest
    {
        public string? OrganisationNameCommaSeparated { get; set; }
        public string? OrganisationIDCommaSeparated { get; set; }
        public string? RelevantYearCommaSeparated { get; set; }
        public string? SubmissionStatusCommaSeparated { get; set; }
        public string? OrganisationTypesCommaSeparated { get; set; }

        [Required]
        public int PageNumber { get; set; }
        [Required]
        public int PageSize { get; set; }
    }
}
