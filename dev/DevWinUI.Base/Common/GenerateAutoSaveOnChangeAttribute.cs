namespace DevWinUI;

/// <summary>
/// Triggers the source generator in the Nucs.JsonSettings.AutosaveGenerator package. It is applied to classes and is
/// not inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateAutoSaveOnChangeAttribute : Attribute
{
}
