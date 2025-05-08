namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationPaymentFeeRequestDto
{
    public string Material { get; init; }
    public string Regulator { get; init; }
    public DateTime SubmittedDate { get; set; }
    public string RequestorType { get; init; }
    public string Reference { get; init; }
}
