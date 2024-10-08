﻿using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string stringEnumeratedValue = string.Empty;
            if (Enum.IsDefined(typeof(SubmissionType), enumValue) && enumValue.GetType().GetMember(enumValue.ToString()).Length == 1)
            {
                stringEnumeratedValue = enumValue.GetType().GetMember(enumValue.ToString())[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
            }
            return stringEnumeratedValue;
        }
    }
}
