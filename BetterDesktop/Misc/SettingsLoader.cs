using BetterDesktop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDesktop.Misc
{
    internal class SettingsLoader
    {
        /// <summary>Path to settings folder.</summary>
        private string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               SystemStartupManager.APP_NAME);

        /// <summary>Loads the settings.</summary>
        public Settings LoadSettings()
        {
            // get the settings file.
            if (Directory.Exists(SettingsPath))
            {
                try
                {
                    // get settings file and deserialize.
                    var json = File.ReadAllText(Path.Combine(SettingsPath, "Settings.json"));
                    return JsonConvert.DeserializeObject<Settings>(json);
                }
                catch
                {
                    // something went wrong, return default settings.
                    return new Settings();
                }
            }

            // settings doesn't exist, set to default. Save.
            var defaultSettings = new Settings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        /// <summary>Save the settings</summary>
        public void SaveSettings(Settings settings)
        {
            if (!Directory.Exists(SettingsPath))
            {
                Directory.CreateDirectory(SettingsPath);
            }

            // serialize and save.
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(Path.Combine(SettingsPath, "Settings.json"), json);
        }

    }
}
