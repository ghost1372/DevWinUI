﻿namespace WinUICommunity;

/// <summary>
/// Loading control allows to show an loading animation with some xaml in it.
/// </summary>
public partial class Loading
{
    /// <summary>
    /// Identifies the <see cref="IsLoading"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
        nameof(IsLoading), typeof(bool), typeof(Loading), new PropertyMetadata(default(bool), IsLoadingPropertyChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the control is in the loading state.
    /// </summary>
    /// <remarks>Set this to true to show the Loading control, false to hide the control.</remarks>
    public bool IsLoading
    {
        get { return (bool)GetValue(IsLoadingProperty); }
        set { SetValue(IsLoadingProperty, value); }
    }

    private static void IsLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Loading control)
        {
            if (control._presenter == null)
            {
                if (control.GetTemplateChild("ContentGrid") is FrameworkElement content)
                {
                    control._presenter = content;
                }
            }

            control?.Update();
        }
    }
}