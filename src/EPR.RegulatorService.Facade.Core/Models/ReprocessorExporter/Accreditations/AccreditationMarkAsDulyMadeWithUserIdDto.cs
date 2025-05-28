using System;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class AccreditationMarkAsDulyMadeWithUserIdDto : AccreditationMarkAsDulyMadeRequestDto
{
    public Guid DulyMadeBy { get; set; }
}
