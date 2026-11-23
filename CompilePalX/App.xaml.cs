using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using CompilePalX.Configuration;
using ControlzEx.Theming;
using Microsoft.Win32;

namespace CompilePalX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ResourceDictionary palette = null!;
        private bool isDark;
        private string appearance = Settings.SystemAppearance;
        internal event EventHandler? AppearanceChanged;

	    protected override void OnStartup(StartupEventArgs e)
	    {
		    // catch all unhandled exceptions and log them
		    AppDomain.CurrentDomain.UnhandledException += (s, err) => { ExceptionHandler.LogException((Exception)err.ExceptionObject, false); };
		    DispatcherUnhandledException += (s, err) => { ExceptionHandler.LogException(err.Exception, false); };
		    TaskScheduler.UnobservedTaskException += (s, err) => { ExceptionHandler.LogException(err.Exception, false); };

            // force invariant culture so stack traces are always in english
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            // set working directory
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            // store path in registry
            RegistryManager.Write("Path", AppContext.BaseDirectory);

            ConfigurationManager.LoadSettings();
            palette = Resources.MergedDictionaries.Single(dictionary => dictionary.Contains("CompilePal.Brushes.Canvas"));
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            ApplyAppearance(ConfigurationManager.Settings.Appearance);
            base.OnStartup(e);
        }

        internal void ApplyAppearance(string preference)
        {
            appearance = preference;
            bool dark = preference switch
            {
                Settings.DarkAppearance => true,
                Settings.LightAppearance => false,
                _ => WindowsUsesDarkApps()
            };
            if (isDark == dark)
                return;

            string mode = dark ? "Dark" : "Light";
            var themeManager = ThemeManager.Current;
            var theme = themeManager.GetTheme($"{mode}.Orange")
                ?? throw new InvalidOperationException($"The bundled {mode} theme could not be loaded.");
            var source = new Uri($"/CompilePalX;component/Themes/{mode}.xaml", UriKind.Relative);
            var next = new ResourceDictionary { Source = source };
            Resources.MergedDictionaries[Resources.MergedDictionaries.IndexOf(palette)] = next;
            palette = next;
            var controls = Resources.MergedDictionaries.Single(dictionary => dictionary.Contains("CompilePal.Window"));
            themeManager.ChangeTheme(this, controls, theme);
            isDark = dark;
            AppearanceChanged?.Invoke(this, EventArgs.Empty);
        }

        private static bool WindowsUsesDarkApps()
        {
            try
            {
                return Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme", 1) is 0;
            }
            catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException or IOException)
            {
                return false;
            }
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (appearance == Settings.SystemAppearance)
                    ApplyAppearance(appearance);
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            base.OnExit(e);
        }
    }
}
