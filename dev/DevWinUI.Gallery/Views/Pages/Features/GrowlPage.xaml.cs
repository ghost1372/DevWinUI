namespace DevWinUIGallery.Views;

public sealed partial class GrowlPage : Page
{
    private static readonly GrowlCardSentTimeVisibility[] SentTimeModes =
    {
        GrowlCardSentTimeVisibility.Visible,
        GrowlCardSentTimeVisibility.Hidden,
        GrowlCardSentTimeVisibility.OnHover
    };

    private static readonly TimeSpan?[] SentAges =
    {
        null,
        TimeSpan.FromMinutes(40),
        TimeSpan.FromHours(3),
        TimeSpan.FromDays(8),
        TimeSpan.FromDays(60)
    };

    private int _messageCount;

    public GrowlPage()
    {
        InitializeComponent();

        ShowAnimationBox.ItemsSource = Enum.GetValues<GrowlShowAnimation>();
        CloseAnimationBox.ItemsSource = Enum.GetValues<GrowlCloseAnimation>();
        GlobalShowAnimationBox.ItemsSource = Enum.GetValues<GrowlShowAnimation>();
        GlobalCloseAnimationBox.ItemsSource = Enum.GetValues<GrowlCloseAnimation>();

        ShowAnimationBox.SelectedItem = DefaultHost.DefaultShowAnimation;
        CloseAnimationBox.SelectedItem = DefaultHost.DefaultCloseAnimation;
        GlobalShowAnimationBox.SelectedItem = GrowlGlobal.DefaultShowAnimation;
        GlobalCloseAnimationBox.SelectedItem = GrowlGlobal.DefaultCloseAnimation;

        Loaded += (_, _) => Growl.SetDefault(DefaultHost);
    }

    private void Success_Click(object sender, RoutedEventArgs e)
    {
        Growl.ShowSuccess("Saved", "Your changes were saved.", true, GrowlCardAutoClose.Default);
    }

    private void AllSeverities_Click(object sender, RoutedEventArgs e)
    {
        Growl.ShowSuccess("Success", "Operation completed.");
        Growl.ShowWarning("Warning", "Check your settings.");
        Growl.ShowError("Error", "Could not connect.");
        Growl.ShowInfo("Info", "A new version is available.");
        Growl.ShowAttention("Attention", "Something needs your attention.");
    }

    private void AutoCloseBySeverity_Click(object sender, RoutedEventArgs e)
    {
        foreach (var severity in Enum.GetValues<GrowlCardSeverity>())
        {
            Growl.Show(new GrowlOptions
            {
                Title = severity.ToString(),
                SubTitle = "Closes only for Attention, Success and Info.",
                Severity = severity,
                AutoClose = GrowlCardAutoClose.Default,
                ShowCountdown = true
            });
        }
    }

    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        var handle = Growl.Show(new GrowlOptions
        {
            Title = "Downloading",
            SubTitle = "setup.exe",
            Severity = GrowlCardSeverity.Info,
            ShowProgress = true
        });

        for (int progress = 10; progress <= 100; progress += 10)
        {
            await Task.Delay(300);

            if (handle.IsClosed)
            {
                return;
            }

            handle.Update(progress: progress);
        }

        handle.Update(card => card.AutoClose = GrowlCardAutoClose.Success);
        handle.Update(title: "Downloaded", severity: GrowlCardSeverity.Success, showProgress: false, restartAutoClose: true);
    }

    private void Key_Click(object sender, RoutedEventArgs e)
    {
        _messageCount++;

        Growl.Show(new GrowlOptions
        {
            Key = "messages",
            Title = $"{_messageCount} new messages",
            SubTitle = "The same key updates the existing card.",
            Severity = GrowlCardSeverity.Info
        });
    }

    private void Rainbow_Click(object sender, RoutedEventArgs e)
    {
        Growl.Show(new GrowlOptions
        {
            Title = "Rainbow",
            SubTitle = "Colorful indicator and icon",
            Severity = GrowlCardSeverity.Info,
            ColorStyle = GrowlCardColorStyle.Rainbow,
            AutoClose = GrowlCardAutoClose.Default
        });
    }

    private void Action_Click(object sender, RoutedEventArgs e)
    {
        Growl.Show(new GrowlOptions
        {
            Title = "Update available",
            SubTitle = "Version 2.4.0 is ready to install",
            Severity = GrowlCardSeverity.Info,
            ActionContent = new Button { Content = "Install now" }
        });
    }

    private void TestHost_Click(object sender, RoutedEventArgs e)
    {
        Growl.GetHost("Test")?.ShowInfo("Test host", "Shown on the host with key Test.", true, GrowlCardAutoClose.Default);
    }

    private void Stacked_Toggled(object sender, RoutedEventArgs e)
    {
        if (DefaultHost is null || TestHost is null)
        {
            return;
        }

        var mode = ((ToggleSwitch)sender).IsOn ? GrowlHostLayoutMode.Stacked : GrowlHostLayoutMode.List;

        DefaultHost.LayoutMode = mode;
        TestHost.LayoutMode = mode;
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        Growl.ClearAll();
        Growl.ClearAll("Test");
    }

    private void Global_Click(object sender, RoutedEventArgs e)
    {
        Growl.ShowSuccessGlobal("Global notification", "Shown outside the app window", true, GrowlCardAutoClose.Default);
    }

    private void GlobalOptions_Click(object sender, RoutedEventArgs e)
    {
        Growl.Show(new GrowlOptions
        {
            Title = "Global notification",
            SubTitle = "Bottom right of the monitor under the cursor",
            Severity = GrowlCardSeverity.Info,
            Scope = GrowlScope.Global,
            Position = GrowlPosition.BottomRight,
            Monitor = GrowlMonitor.Cursor,
            ShowAnimation = GrowlShowAnimation.Snap,
            CloseAnimation = GrowlCloseAnimation.Snap,
            AutoClose = GrowlCardAutoClose.Default,
            ShowCountdown = true
        });
    }

    private void ClearGlobal_Click(object sender, RoutedEventArgs e)
    {
        GrowlGlobal.ClearAll();
    }

    private void ShowAgain_Click(object sender, RoutedEventArgs e)
    {
        Card.IsOpen = true;
    }

    private void AutoCloseFlag_Click(object sender, RoutedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        var value = GrowlCardAutoClose.None;

        if (AutoCloseAttention.IsChecked == true) value |= GrowlCardAutoClose.Attention;
        if (AutoCloseSuccess.IsChecked == true) value |= GrowlCardAutoClose.Success;
        if (AutoCloseWarning.IsChecked == true) value |= GrowlCardAutoClose.Warning;
        if (AutoCloseError.IsChecked == true) value |= GrowlCardAutoClose.Error;
        if (AutoCloseInfo.IsChecked == true) value |= GrowlCardAutoClose.Info;

        Card.AutoClose = value;
    }

    private void DurationBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Card is null || double.IsNaN(args.NewValue))
        {
            return;
        }

        Card.AutoCloseDuration = TimeSpan.FromSeconds(args.NewValue);
    }

    private void Placement_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.IndicatorPlacement = (GrowlCardIndicatorPlacement)((ComboBox)sender).SelectedIndex;
    }

    private void ProgressType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.ProgressType = (GrowlCardProgressType)((ComboBox)sender).SelectedIndex;
    }

    private void GradientIntensity_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.GradientIntensity = (GrowlCardGradientIntensity)((ComboBox)sender).SelectedIndex;
    }

    private void IconStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.IconStyle = (GrowlCardIconStyle)((ComboBox)sender).SelectedIndex;
    }

    private void Severity_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.Severity = (GrowlCardSeverity)((ComboBox)sender).SelectedIndex;
    }

    private void ColorStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.ColorStyle = (GrowlCardColorStyle)((ComboBox)sender).SelectedIndex;
    }

    private void CountdownSeverity_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.CountdownSeverity = (GrowlCardCountdownSeverity)((ComboBox)sender).SelectedIndex;
    }

    private void CountdownColorStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.CountdownColorStyle = (GrowlCardCountdownColorStyle)((ComboBox)sender).SelectedIndex;
    }

    private void CountdownPlacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.CountdownPlacement = (GrowlCardCountdownPlacement)((ComboBox)sender).SelectedIndex;
    }

    private void DisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        var mode = (GrowlCardDisplayMode)((ComboBox)sender).SelectedIndex;
        bool banner = mode == GrowlCardDisplayMode.Banner;

        Card.DisplayMode = mode;
        Card.HorizontalAlignment = banner ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;
        Card.VerticalAlignment = banner ? VerticalAlignment.Top : VerticalAlignment.Center;
        Card.Margin = banner ? new Thickness(24) : new Thickness(0);
    }

    private void SentTimeVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        Card.SentTimeVisibility = SentTimeModes[((ComboBox)sender).SelectedIndex];
    }

    private void SentAt_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Card is null)
        {
            return;
        }

        var age = SentAges[((ComboBox)sender).SelectedIndex];
        Card.SentAt = age is { } value ? DateTimeOffset.Now - value : null;
    }

    private void ShowAnimation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DefaultHost is null || TestHost is null || ((ComboBox)sender).SelectedItem is not GrowlShowAnimation value)
        {
            return;
        }

        DefaultHost.DefaultShowAnimation = value;
        TestHost.DefaultShowAnimation = value;
    }

    private void CloseAnimation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DefaultHost is null || TestHost is null || ((ComboBox)sender).SelectedItem is not GrowlCloseAnimation value)
        {
            return;
        }

        DefaultHost.DefaultCloseAnimation = value;
        TestHost.DefaultCloseAnimation = value;
    }

    private void GlobalShowAnimation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is GrowlShowAnimation value)
        {
            GrowlGlobal.DefaultShowAnimation = value;
        }
    }

    private void GlobalCloseAnimation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is GrowlCloseAnimation value)
        {
            GrowlGlobal.DefaultCloseAnimation = value;
        }
    }
}
