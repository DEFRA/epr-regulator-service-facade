using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus
{
    public class AntiVirusDetails
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public SubmissionType SubmissionType { get; set; }
    }
}
