namespace DevWinUI;

// This is an attribute that triggers the source generator in the Nucs.JsonSettings.AutosaveGenerator package

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateAutoSaveOnChangeAttribute : Attribute
{
}