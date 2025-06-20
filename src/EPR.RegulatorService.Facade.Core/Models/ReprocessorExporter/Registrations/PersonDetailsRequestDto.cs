namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class PersonDetailsRequestDto
{
    public Guid?  OrgId { get; set; }
    public List<Guid> UserIds  { get; set; }
}
