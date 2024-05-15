﻿namespace EPR.RegulatorService.Facade.Core.Enums;

using System.ComponentModel.DataAnnotations;

public enum SubmissionType
{
    [Display(Name = "pom")]
    Producer = 1,
    [Display(Name = "registration")]
    Registration = 2
}
