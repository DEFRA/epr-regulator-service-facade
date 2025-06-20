namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class QueryNoteResponseDto
{
    public String Notes { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}
