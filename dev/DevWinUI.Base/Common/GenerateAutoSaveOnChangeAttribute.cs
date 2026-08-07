namespace DevWinUI;

/// <summary>
/// Triggers the source generator in the Nucs.JsonSettings.AutosaveGenerator package. It is applied to classes and is
/// not inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[Obsolete("This attribute is deprecated and will be removed in the next update. Please use the nucs.JsonSettings.AutoSave package instead.")]
public class GenerateAutoSaveOnChangeAttribute : Attribute
{
}
