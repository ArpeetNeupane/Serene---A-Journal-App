namespace Serene.Utils;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

public class ValidationUtils
{
    public static bool IsBlank(string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }


    //overload IsBlank to check multiple strings
    public static bool IsBlank(List<string> values)
    {
        if (values == null || values.Count == 0)
            return true; // treat null/empty list as blank

        //returns true if **any string is blank**
        return values.Any(string.IsNullOrWhiteSpace);
    }
}