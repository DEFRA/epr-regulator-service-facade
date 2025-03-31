using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationTaskStatus
{
    [Display(Name = "Complete")]
    Complete = 1,

    [Display(Name = "Queried")]
    Queried = 2,
}