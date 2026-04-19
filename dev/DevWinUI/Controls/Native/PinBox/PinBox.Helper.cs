namespace DevWinUI;
public partial class PinBox
{
    private void UnRegisterPropertyChanged(DependencyProperty dependencyProperty, long token)
    {
        UnregisterPropertyChangedCallback(dependencyProperty, token);
    }

    private void RegisterPropertyChanged(DependencyProperty dependencyProperty, long token, DependencyPropertyChangedCallback propertyChangedCallback)
    {
        token = RegisterPropertyChangedCallback(dependencyProperty, propertyChangedCallback);
    }

    private Border GetBorderElement(PasswordBox passwordBox)
    {
        Border border = passwordBox.FindDescendant<Border>();
        if (border != null && border.Name.Equals("BorderElement"))
        {
            return border;
        }
        else
        {
            return passwordBox.FindDescendants().Where(element => ((FrameworkElement)element).Name.Equals("BorderElement")).Cast<Border>().FirstOrDefault();
        }
    }

    private string GetPassword()
    {
        return GetPassword(Password);
    }

    private string GetPassword(string value)
    {
        if (_uniformGrid == null)
            return null;

        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        var newPassword = value.Length > PasswordLength ? value.Substring(0, PasswordLength) : value;
        return newPassword;
    }

    private string GetPasswordChar(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return "●";
        }
        else
        {
            return newValue;
        }
    }

    public int GetCurrentPasswordBoxIndex(PasswordBox passwordBox)
    {
        return _uniformGrid.Children.IndexOf(passwordBox);
    }

    public List<PasswordBox> GetPinBoxList()
    {
        return _uniformGrid.Children.Cast<PasswordBox>().ToList();
    }
}
