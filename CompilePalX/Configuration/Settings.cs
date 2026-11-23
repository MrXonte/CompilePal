using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilePalX.Configuration
{
    public class Settings : ICloneable, IEquatable<GameConfiguration>
    {
        public const string SystemAppearance = "System";
        public const string LightAppearance = "Light";
        public const string DarkAppearance = "Dark";

        private string appearance = SystemAppearance;
        public string Appearance
        {
            get => appearance;
            set => appearance = value is LightAppearance or DarkAppearance ? value : SystemAppearance;
        }

        public string ErrorSourceURL { get; set; } = "https://www.interlopers.net/includes/errorpage/errorChecker.txt";
        public int ErrorCacheExpirationDays { get; set; } = 7;
        public bool PlaySoundOnCompileCompletion { get; set; } = true;

        // not directly editable by user, set in MainWindow.xaml.cs on shutdown
        public string? MapListHeight { get; set; } = null;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool Equals(GameConfiguration? other)
        {
            throw new NotImplementedException();
        }
    }
}
