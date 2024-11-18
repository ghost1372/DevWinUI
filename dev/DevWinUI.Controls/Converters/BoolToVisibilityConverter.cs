// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DevWinUI;

/// <summary>
/// This class converts a boolean value into a Visibility enumeration.
/// </summary>
public partial class BoolToVisibilityConverter : BoolToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
    /// </summary>
    public BoolToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }
}
