namespace EPR.RegulatorService.Facade.Core.Extensions;

public static class StringExtensions
{
    public static string ToReferenceNumberFormat(this string? str)
    {
        if(str is null)
        {
            return string.Empty;
        }
        str = str.Replace(" ", "");

        int i = str.Length - 3;
        while (i > 0)
        {
            str = str.Insert(i, " ");
            i -= 3;
        }

        return str;
    }
}
