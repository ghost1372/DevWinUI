﻿using System.Collections.Generic;
using DevWinUI_Template.WizardUI;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace DevWinUI_Template;

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
        WizardImplementation.RunStarted(automationObject, replacementsDictionary, new TemplateConfig { IsBlank = true, IsTest = true, TemplateType = TemplateType.WinUIApp_Blank_UnitTest });
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
