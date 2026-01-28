namespace Serene.Utils;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;


/// <summary>
/// Provides utility methods for validating strings and collections of strings.
/// </summary>
/// <remarks>
/// This static utility class includes methods to check whether a single string
/// or a list of strings is blank (null, empty, or whitespace). The overloaded
/// <see cref="IsBlank(List{string})"/> method treats null or empty lists as blank
/// and returns true if any string in the collection is blank. These methods
/// are useful for input validation across the application.
/// </remarks>
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