using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CompilePalX.Configuration;

namespace CompilePalX
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = ConfigurationManager.Settings.Clone();
        }

        private void AppearancePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedValue is string preference)
                ((App)Application.Current).ApplyAppearance(preference);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SaveSettings((Settings)DataContext);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            ((App)Application.Current).ApplyAppearance(ConfigurationManager.Settings.Appearance);
            base.OnClosed(e);
        }

        private readonly Regex numberRegex = new Regex("[^0-9]+");
        private void ErrorCacheDurationDays_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numberRegex.IsMatch(e.Text);
        }
    }
}
