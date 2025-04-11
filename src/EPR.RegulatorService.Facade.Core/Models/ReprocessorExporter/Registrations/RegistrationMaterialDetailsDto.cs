using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationMaterialDetailsDto
{
    public int Id { get; set; }   
    public int RegistrationId { get; set; }           
    public string MaterialName { get; set; }
    public ApplicationStatus? Status { get; set; }
}