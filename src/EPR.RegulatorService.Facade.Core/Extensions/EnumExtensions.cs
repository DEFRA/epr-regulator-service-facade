using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string stringEnumeratedValue = string.Empty;
            if (enumValue.GetType().GetMember(enumValue.ToString())[0] != null)
            {
                stringEnumeratedValue = enumValue.GetType().GetMember(enumValue.ToString())[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
            }
            return stringEnumeratedValue;
        }
    }
}
