namespace DevWinUI;

public partial class EmptyStringToObjectConverter : EmptyObjectToObjectConverter
{
    protected override bool CheckValueIsEmpty(object value)
    {
        return string.IsNullOrEmpty(value?.ToString());
    }
}
