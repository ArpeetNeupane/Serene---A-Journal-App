namespace Serene.Utils;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

public class Utils
{
    //getting the initials from the full name of the user
    public static string GetInitials(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return string.Empty;

        var parts = username.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = string.Concat(parts.Select(p => char.ToUpper(p[0])));

        return initials;
    }

    //getting the total word count from a provided document content
    public static int GetTotalWordCount(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 0;

        // Split on whitespace characters (space, tab, newline)
        var words = content
            .Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return words.Length;
    }
}

public class DateTimeUtils
{
    public static DateTime GetTodayLocalDate()
    {
        return DateTime.Now.Date;
    }

    public static DateTime GetUtcDateTime()
    {
        return DateTime.UtcNow;
    }
}

public class ValidationUtils
{
    public static bool IsBlank(string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    // Overriding IsBlank to check multiple strings
    public static bool IsBlank(List<string> values)
    {
        if (values == null || values.Count == 0)
            return true; // treat null/empty list as blank

        // Returns true if **any string is blank**
        return values.Any(string.IsNullOrWhiteSpace);
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        // At least one letter, one number, one symbol
        Regex PasswordRegex = new Regex(
            @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{6,}$",
            RegexOptions.Compiled
        );

        return PasswordRegex.IsMatch(password);
    }
}