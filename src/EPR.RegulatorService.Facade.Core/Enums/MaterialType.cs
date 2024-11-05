using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum MaterialType
{
    [Display(Name = "")]
    None,
    [Display(Name = "GL")]
    Glass,
    [Display(Name = "AL")]
    Aluminium,
    [Display(Name = "ST")]
    Steel,
    [Display(Name = "PA")]
    PaperBoard,
    [Display(Name = "PL")]
    Plastic,
    [Display(Name = "WO")]
    Wood 
} 