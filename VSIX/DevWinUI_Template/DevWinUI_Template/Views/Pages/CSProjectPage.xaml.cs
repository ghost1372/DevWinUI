using System;
using System.Windows;
using System.Windows.Controls;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template
{
    public partial class CSProjectPage : Page
    {
        public CSProjectPage()
        {
            InitializeComponent();
        }

        private void Toggled(object sender, RoutedEventArgs e)
        {
            var optionUC = sender as OptionUC;
            AddOrRemoveElement(optionUC);
        }

        private void AddOrRemoveElement(OptionUC optionUC)
        {
            try
            {
                string keyValue = optionUC.Tag.ToString();
                if (optionUC.IsOn)
                {
                    WizardConfig.CSProjectElements.AddIfNotExists(keyValue, $"<{keyValue}>true</{keyValue}>");
                }
                else
                {
                    WizardConfig.CSProjectElements.TryGetValue(keyValue, out var valueExist);
                    if (!string.IsNullOrEmpty(valueExist))
                    {
                        WizardConfig.CSProjectElements.Remove(keyValue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void AddOrUpdateElementWithOnOffContent(OptionUC optionUC)
        {
            try
            {
                var key = optionUC.Tag.ToString();
                var onValue = optionUC.OnContent;
                var offValue = optionUC.OffContent;

                WizardConfig.CSProjectElements.TryGetValue(key, out var valueExist);
                if (!string.IsNullOrEmpty(valueExist))
                {
                    if (optionUC.IsOn)
                    {
                        WizardConfig.CSProjectElements.Update(key, $"<{key}>{onValue}</{key}>");
                    }
                    else
                    {
                        WizardConfig.CSProjectElements.Update(key, $"<{key}>{offValue}</{key}>");
                    }
                }
                else
                {
                    if (optionUC.IsOn)
                    {
                        WizardConfig.CSProjectElements.AddIfNotExists(key, $"<{key}>{onValue}</{key}>");
                    }
                    else
                    {
                        WizardConfig.CSProjectElements.AddIfNotExists(key, $"<{key}>{offValue}</{key}>");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void TrimModeOption_Toggled(object sender, RoutedEventArgs e)
        {
            WizardConfig.TrimMode = TrimModeOption.IsOn ? TrimModeOption.OnContent : TrimModeOption.OffContent;
        }

        private void OUAOT_Toggled(object sender, RoutedEventArgs e)
        {
            WizardConfig.PublishAot = OUAOT.IsOn;
        }
    }
}
