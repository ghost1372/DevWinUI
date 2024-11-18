using System;
using System.Windows;
using System.Windows.Controls;
namespace DevWinUI_Template
{
    public partial class LibraryOptionUC : UserControl
    {
        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Unchecked;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LibraryOptionUC), new PropertyMetadata(default(string)));

        public LibraryOptionUC()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Checked?.Invoke(sender, e);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Unchecked?.Invoke(sender, e);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LibraryCheckBox.IsChecked = !LibraryCheckBox.IsChecked;
        }
    }
}
