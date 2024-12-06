namespace DevWinUI_Template;
public class TemplateConfig
{
    public bool IsMVVM { get; set; }
    public bool HasNavigationView { get; set; }
    public bool IsBlank { get; set; }
    public bool IsTest { get; set; }
    public bool HasPages { get; set; }
    public TemplateType TemplateType { get; set; }
}
public enum TemplateType
{
    WinUIApp_Blank_UnitTest,
    WinUIApp_Blank,
    WinUIApp_MVVM_NavigationView,
    WinUIApp_MVVM,
    WinUIApp_NavigationView,
    WinUIApp
}
