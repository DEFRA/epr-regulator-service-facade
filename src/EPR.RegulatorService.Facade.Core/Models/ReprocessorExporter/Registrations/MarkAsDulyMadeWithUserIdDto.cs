using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class MarkAsDulyMadeWithUserIdDto
{
    public DateTime DulyMadeDate { get; set; }
    public DateTime DeterminationDate { get; set; }
    public Guid UserId { get; set; }
}
