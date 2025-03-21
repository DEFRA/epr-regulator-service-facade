using System;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.Submissions
{
    public class PomResubmissionPaycalParametersDto
    {
        public bool IsResubmission { get; set; }
        public DateTime? ResubmissionDate { get; set; }
        public int? MemberCount { get; set; }
        public string? Reference { get; set; }
        public string? NationCode { get; set; }
        public bool ReferenceNotAvailable { get; set; }
        public bool ReferenceFieldNotAvailable { get; set; }
    }
}