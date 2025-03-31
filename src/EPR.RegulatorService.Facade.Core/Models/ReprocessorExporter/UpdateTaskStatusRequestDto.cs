
using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Attributes;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;

public class UpdateTaskStatusRequestDto
{
    [Required]
    public RegistrationTaskStatus Status { get; set; }

    [StringLength(200)]
    [RequiredIf("Status", "Queried", ErrorMessage = "Comments field is required if 'Status' is queried.")]
    public string Comments { get; set; } = null;
}
