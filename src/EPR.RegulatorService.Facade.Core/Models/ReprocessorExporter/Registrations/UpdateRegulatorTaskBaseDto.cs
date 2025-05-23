﻿using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public abstract class UpdateRegulatorTaskBaseDto
{
    [Required]
    public required string TaskName { get; set; }
    [Required]
    public required RegistrationTaskStatus Status { get; set; }
    [MaxLength(500)]
    public string? Comments { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
