using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Helpers.Validators;

public class NotNoneAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("The field is required.");
        }

        var enumType = value.GetType();
        if (!enumType.IsEnum)
        {
            throw new InvalidOperationException("The NotDefaultEnumAttribute can only be used on enum types.");
        }

        var defaultEnumValue = Activator.CreateInstance(enumType);
        return value.Equals(defaultEnumValue) ? new ValidationResult($"The field cannot be {defaultEnumValue}.") : ValidationResult.Success;
    }
}
