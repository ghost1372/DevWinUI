﻿namespace DevWinUIGallery.Views;

public sealed partial class CalendarWithClockPage : Page
{
    public CalendarWithClockPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        TxtDateTime.Text = CalendarWithClockSample.SelectedDateTimeString;
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var tag = (sender as RadioButton).Tag;
        var displayMode = GeneralHelper.GetEnum<TimePickerDisplayMode>(tag?.ToString());
        CalendarWithClockSample2.TimePickerDisplayMode = displayMode;
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        TxtDateTime2.Text = CalendarWithClockSample2.SelectedDateTimeString;
    }
}
