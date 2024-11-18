using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System.Collections.Generic;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template
{
    public class WinUIAppBlankUnitTestWizard : IWizard
    {
        SharedWizard WizardImplementation;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            WizardImplementation.ProjectFinishedGenerating(project);
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
            WizardImplementation.RunFinished();
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            WizardImplementation = new SharedWizard();
            WizardImplementation.RunStarted(automationObject, replacementsDictionary, "WinUIApp-Blank-UnitTest", false, false, false, true, true);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            if (!WizardImplementation.ShouldAddProjectItem())
            {
                return false;
            }

            else if (!WizardConfig.UseColorsDic && filePath.Contains("ThemeResources.xaml"))
            {
                return false;
            }
            else if (!WizardConfig.UseStylesDic && filePath.Contains("Styles.xaml"))
            {
                return false;
            }
            else if (!WizardConfig.UseConvertersDic && filePath.Contains("Converters.xaml"))
            {
                return false;
            }
            else if (!WizardConfig.UseFontsDic && filePath.Contains("Fonts.xaml"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
