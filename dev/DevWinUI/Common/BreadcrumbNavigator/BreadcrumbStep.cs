namespace DevWinUI;
public partial class BreadcrumbStep
{
    public BreadcrumbStep(string label, Type page)
    {
        Label = label;
        Page = page;
    }
    public BreadcrumbStep(string label, Type page, object parameter)
    {
        Label = label;
        Page = page;
        Parameter = parameter;
    }
    public string Label { get; }

    public Type Page { get; }
    public object Parameter { get; }

    public override string ToString() => Label;
}
