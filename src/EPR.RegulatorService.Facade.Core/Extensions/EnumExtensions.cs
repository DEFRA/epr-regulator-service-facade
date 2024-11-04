using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName<T>(this Enum enumValue) where T : Enum
        {
            string stringEnumeratedValue = string.Empty;
            if (Enum.IsDefined(typeof(T), enumValue) && enumValue.GetType().GetMember(enumValue.ToString()).Length == 1)
            {
                stringEnumeratedValue = enumValue.GetType().GetMember(enumValue.ToString())[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
            }
            return stringEnumeratedValue;
        }
    }
}
