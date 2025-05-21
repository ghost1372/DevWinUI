using System.Collections;
using System.ComponentModel;

namespace DevWinUIGallery.Views;
public sealed partial class ValidationPage : Page
{
    public UserInfo UserInfo { get; }

    private void CoolButton_Click(object sender, RoutedEventArgs e)
    {
        frame.Navigate(typeof(ThemeManagerPage));
    }
    public ValidationPage()
    {
        this.InitializeComponent();
        UserInfo = new UserInfo();
    }
}

public sealed partial class UserInfo : ObservableObject, INotifyDataErrorInfo
{
    public UserInfo()
    {
        ValidateName(Name);
        ValidateMail(Mail);
    }

    [ObservableProperty]
    public partial string Name { get; set; }
    partial void OnNameChanged(string value)
    {
        ValidateName(value);
    }

    [ObservableProperty]
    public partial string Mail { get; set; }
    partial void OnMailChanged(string value)
    {
        ValidateMail(value);
    }

    private void ValidateName(string name)
    {
        var errors = new List<string>(3);
        if (string.IsNullOrEmpty(name))
            errors.Add("You'll need a name. I will not accept this.");
        else
        {
            if (name.Contains("1", StringComparison.InvariantCulture))
                errors.Add("Using Jungkook's favorite number is not allowed.");

            if (name.Length == 0)
                errors.Add("You'll need a name. I will not accept this.");
            else if (name.Length > 4)
                errors.Add("Your name is too long. Make it shorter.");

            if (name.Contains("LPK", StringComparison.InvariantCultureIgnoreCase))
                errors.Add("Name is forbidden for unknown reasons.");
        }
        

        SetErrors("Name", errors);
    }

    private void ValidateMail(string mail)
    {
        var errors = new List<string>(2);
        if (string.IsNullOrEmpty(mail))
            errors.Add("Invalid mail.");
        else
        {
            if (mail.Contains("1", StringComparison.InvariantCulture))
                errors.Add("Using Jungkook's favorite number is not allowed.");

            if (!mail.Contains("@", StringComparison.InvariantCultureIgnoreCase))
                errors.Add("Invalid mail.");
        }

        SetErrors("Mail", errors);
    }

    // Error validation
    private readonly Dictionary<string, ICollection<string>> _validationErrors = new();

    public bool HasErrors => _validationErrors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName) ||
            !_validationErrors.ContainsKey(propertyName))
            return null;

        return _validationErrors[propertyName];
    }

    private void SetErrors(string key, ICollection<string> errors)
    {
        if (errors.Any())
            _validationErrors[key] = errors;
        else
            _ = _validationErrors.Remove(key);

        OnErrorsChanged(key);
    }
}
