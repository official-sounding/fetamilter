using System.Text.RegularExpressions;

namespace Data.Extensions;

public static partial class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) { return input; }

        var startUnderscores = HasStartUnderscores().Match(input);
        return startUnderscores + PascalCaseWordBoundary().Replace(input, "$1_$2").ToLower();
    }

    [GeneratedRegex(@"^_+")]
    private static partial Regex HasStartUnderscores();
    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex PascalCaseWordBoundary();
}
