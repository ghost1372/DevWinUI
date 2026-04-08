// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Globalization;

namespace DevWinUI;

internal static class TimeSpanHelper
{
    public static string Convert(TimeSpan? time)
    {
        if (time is not TimeSpan ts)
        {
            return string.Empty;
        }

        // If user passed in a negative TimeSpan, normalize
        if (ts < TimeSpan.Zero)
        {
            ts = ts.Duration();
        }

        // Map the TimeSpan to a DateTime on today's date
        var dt = DateTime.Today.Add(ts);

        // This pattern automatically respects system 12/24-hour setting
        string pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

        return dt.ToString(pattern, CultureInfo.CurrentCulture);
    }
}
