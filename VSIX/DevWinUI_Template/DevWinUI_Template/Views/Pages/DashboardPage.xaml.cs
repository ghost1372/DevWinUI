﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += DashboardPage_Loaded;
    }

    private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (WizardConfig.IsBlank)
        {
            if (tgJsonSettings != null)
            {
                tgJsonSettings.IsEnabled = false;
            }
        }
    }

    private void cmbVersionMechanism_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbVersionMechanism = sender as ComboBox;
        WizardConfig.UsePreReleaseVersion = cmbVersionMechanism.SelectedIndex != 0;

        //if (LibrariesPage.Instance != null)
        //{
        //    LibrariesPage.Instance.CreateBoxes();
        //}
    }

    private void cmbNetVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        WizardConfig.DotNetVersion = (cmbNetVersion.SelectedItem as ComboBoxItem).Tag.ToString();

        //if (LibrariesPage.Instance != null)
        //{
        //    LibrariesPage.Instance.CreateBoxes();
        //}
    }

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
//        DialogResult = true;

  //      Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
    }

    private void Cancel()
    {
        Resources?.Clear();
        Application.Current?.Resources?.Clear();
        //DialogResult = false;
    }


    private void cmbTargetFrameworkVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        WizardConfig.TargetFrameworkVersion = (cmbTargetFrameworkVersion.SelectedItem as ComboBoxItem).Tag.ToString();
    }

    private void tgUnPackaged_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.IsUnPackagedMode = tgUnPackaged.IsOn;
    }

    private void tgNullable_Toggled(object sender, RoutedEventArgs e)
    {
        WizardConfig.Nullable = tgNullable.IsOn ? tgNullable.OnContent?.ToString() : tgNullable.OffContent?.ToString();
    }

    private void tgJsonSettings_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseJsonSettings = tgJsonSettings.IsOn;
    }

    private void tgSolutionFolder_Toggled(object sender, System.Windows.RoutedEventArgs e)
    {
        WizardConfig.UseSolutionFolder = tgSolutionFolder.IsOn;
    }

    private void txtSolutionFolderName_TextChanged(object sender, TextChangedEventArgs e)
    {
        var txt = sender as TextBox;
        if (!string.IsNullOrEmpty(txt.Text))
        {
            WizardConfig.SolutionFolderName = txt.Text;
        }
        else
        {
            WizardConfig.SolutionFolderName = WizardConfig.SolutionFolderNameDefault;
        }
    }
}