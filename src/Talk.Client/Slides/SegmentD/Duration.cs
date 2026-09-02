using System.Text.RegularExpressions;

namespace Talk.Client.Slides;

/// <summary>
/// Parses and formats human durations like "1h 30m", "90m", "2h". Deliberately small: it exists so
/// <c>DurationInput</c> has a real string ↔ <see cref="TimeSpan"/> conversion of its own to own,
/// which is the whole reason a component would inherit <c>InputBase&lt;T&gt;</c> (see D2bInputBase).
/// </summary>
public static partial class Duration
{
    // Anchored, so trailing junk fails rather than being silently ignored ("90m please" is not 90m).
    // Both groups are optional individually, but the match is rejected below unless one is present —
    // otherwise the empty string would parse as zero.
    [GeneratedRegex(@"^\s*(?:(?<h>\d{1,4})\s*h)?\s*(?:(?<m>\d{1,5})\s*m)?\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex Pattern { get; }

    public static bool TryParse(string? text, out TimeSpan value)
    {
        value = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var match = Pattern.Match(text);
        if (!match.Success) return false;

        var hasHours = match.Groups["h"].Success;
        var hasMinutes = match.Groups["m"].Success;
        if (!hasHours && !hasMinutes) return false;   // matched only whitespace

        var hours = hasHours ? int.Parse(match.Groups["h"].Value) : 0;
        var minutes = hasMinutes ? int.Parse(match.Groups["m"].Value) : 0;

        value = new TimeSpan(hours, minutes, 0);
        return true;
    }

    /// <summary>The inverse, so an existing value shows up in the box the way you'd type it.</summary>
    public static string Format(TimeSpan value)
    {
        if (value == TimeSpan.Zero) return string.Empty;

        var hours = (int)value.TotalHours;
        var minutes = value.Minutes;

        return (hours, minutes) switch
        {
            (0, _) => $"{minutes}m",
            (_, 0) => $"{hours}h",
            _ => $"{hours}h {minutes}m",
        };
    }
}
