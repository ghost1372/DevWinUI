namespace DevWinUIGallery.Views;

public sealed partial class GrowlPage : Page
{
    public GrowlPage()
    {
        this.InitializeComponent();
    }

    private void btnGrowl_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 11);
        switch (number)
        {
            case 1:
                Growl.Info("Hello");
                break;
            case 2:
                Growl.Info("Hello", "Info");
                break;
            case 3:
                Growl.Info2("Hello");
                break;
            case 4:
                Growl.Info2("Hello", "Info2");
                break;
            case 5:
                Growl.Warning("Hello");
                break;
            case 6:
                Growl.Warning("Hello", "Warning");
                break;
            case 7:
                Growl.Error("Hello");
                break;
            case 8:
                Growl.Error("Hello", "Error");
                break;
            case 9:
                Growl.Success("Hello");
                break;
            case 10:
                Growl.Success("Hello", "Success");
                break;
            case 11:
                Growl.Ask("Hello", (s, e) =>
                {
                    Growl.Info("Clicked");
                    return true;
                });
                break;
        }
    }

    private void btnGrowlInfo_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 5);
        switch (number)
        {
            case 1:
                Growl.Info(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo",
                });
                break;
            case 2:
                Growl.Info(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo",
                    UseBlueColorForInfo = true,
                });
                break;
            case 3:
                Growl.Warning(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Warning with GrowlInfo"
                });
                break;
            case 4:
                Growl.Error(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Error with GrowlInfo"
                });
                break;
            case 5:
                Growl.Success(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Success with GrowlInfo"
                });
                break;
        }
    }

    private void btnGrowlToken_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 11);
        switch (number)
        {
            case 1:
                Growl.InfoWithToken("Hello", "Test");
                break;
            case 2:
                Growl.InfoWithToken("Hello", "Info", "Test");
                break;
            case 3:
                Growl.Info2WithToken("Hello", "Test");
                break;
            case 4:
                Growl.Info2WithToken("Hello", "Info2", "Test");
                break;
            case 5:
                Growl.WarningWithToken("Hello", "Test");
                break;
            case 6:
                Growl.WarningWithToken("Hello", "Warning", "Test");
                break;
            case 7:
                Growl.ErrorWithToken("Hello", "Test");
                break;
            case 8:
                Growl.ErrorWithToken("Hello", "Error", "Test");
                break;
            case 9:
                Growl.SuccessWithToken("Hello", "Test");
                break;
            case 10:
                Growl.SuccessWithToken("Hello", "Success", "Test");
                break;
            case 11:
                Growl.AskWithToken("Hello", "Test", (s, e) =>
                {
                    Growl.Info("Clicked");
                    return true;
                });
                break;
        }
    }

    private void btnGrowlInfoToken_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 5);
        switch (number)
        {
            case 1:
                Growl.Info(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo",
                    Token = "Test"
                });
                break;
            case 2:
                Growl.Info(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo",
                    UseBlueColorForInfo = true,
                    Token = "Test"
                });
                break;
            case 3:
                Growl.Warning(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Warning with GrowlInfo",
                    Token = "Test"
                });
                break;
            case 4:
                Growl.Error(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Error with GrowlInfo",
                    Token = "Test"
                });
                break;
            case 5:
                Growl.Success(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Success with GrowlInfo",
                    Token = "Test"
                });
                break;
        }
    }

    private void btnGrowlClearAll_Click(object sender, RoutedEventArgs e)
    {
        Growl.Clear();
        Growl.Clear("Test");
    }
    private void btnGrowlGlobal_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 11);
        switch (number)
        {
            case 1:
                Growl.InfoGlobal("Hello");
                break;
            case 2:
                Growl.InfoGlobal("Hello", "Info");
                break;
            case 3:
                Growl.Info2Global("Hello");
                break;
            case 4:
                Growl.Info2Global("Hello", "Info2");
                break;
            case 5:
                Growl.WarningGlobal("Hello");
                break;
            case 6:
                Growl.WarningGlobal("Hello", "Warning");
                break;
            case 7:
                Growl.ErrorGlobal("Hello");
                break;
            case 8:
                Growl.ErrorGlobal("Hello", "Error");
                break;
            case 9:
                Growl.SuccessGlobal("Hello");
                break;
            case 10:
                Growl.SuccessGlobal("Hello", "Success");
                break;
            case 11:
                Growl.AskGlobal("Hello", (s, e) =>
                {
                    Growl.Info("Clicked");
                    return true;
                });
                break;
        }
    }

    private void btnGrowlInfoGlobal_Click(object sender, RoutedEventArgs e)
    {
        var number = new Random().Next(1, 5);
        switch (number)
        {
            case 1:
                Growl.InfoGlobal(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo"
                });
                break;
            case 2:
                Growl.InfoGlobal(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Info with GrowlInfo",
                    UseBlueColorForInfo = true
                });
                break;
            case 3:
                Growl.WarningGlobal(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Warning with GrowlInfo"
                });
                break;
            case 4:
                Growl.ErrorGlobal(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Error with GrowlInfo"
                });
                break;
            case 5:
                Growl.SuccessGlobal(new GrowlInfo
                {
                    ShowDateTime = true,
                    StaysOpen = true,
                    IsClosable = false,
                    Title = "Hello",
                    Message = "Success with GrowlInfo"
                });
                break;
        }
    }

    private void btnClearGlobalAll_Click(object sender, RoutedEventArgs e)
    {
        Growl.ClearGlobal();
    }
}
