using Avalonia.Controls;
using BetterDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using Avalonia.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterDesktop.Misc;

namespace BetterDesktop.ViewModels
{
    /// <summary>View model for the settings window.</summary>
    [ObservableObject]
    internal partial class SettingsWindowViewModel
    {
        /// <summary>The settings of the icons. This will be updated by the View.</summary>
        [ObservableProperty]
        private Settings settings = new Settings();

        /// <summary>The old settigs.</summary>
        private Settings oldSettings;

        [ObservableProperty]
        /// <summary>If the settings have changed.</summary>
        private bool settingsHasChanged;

        public SettingsWindowViewModel(Settings settings)
        {
            Settings = settings;
            oldSettings = settings.Copy();

            CheckForChangedSettings();

            // when settings have changed.
            Settings.PropertyChanged += (s, e) =>
            {
                // if the current settings doesn't equal
                // the default values.
                CheckForChangedSettings();
            };
        }

        /// <summary>When OK is clicked.</summary>
        /// <param name="window">The window.</param>
        [RelayCommand]
        public void Ok(Window window)
        {
            // the option has changed for startup.
            if (oldSettings.StartupOnSystemStartup != Settings.StartupOnSystemStartup)
            {
                var startup = new SystemStartupManager();
                // previously no startup
                if (Settings.StartupOnSystemStartup)
                {
                    startup.CreateShortcut();
                }
                // no longer startup at system startup
                else
                {
                    startup.RemoveShortcut();
                }
            }

            window.Close();
        }

        [RelayCommand]
        /// <summary>Reset the settings</summary>
        public void ResetToDefaults()
        {
            // set to default.
            Settings = new Settings();
            // rehook the changed event.
            // when settings have changed.
            Settings.PropertyChanged += (s, e) =>
            {
                // if the current settings doesn't equal
                // the default values.
                CheckForChangedSettings();
            };
        }

        /// <summary>Closes the windows and sets settings to old one.</summary>
        [RelayCommand]
        public void Cancel(Window window)
        {
            Settings = oldSettings;
            window.Close();
        }

        /// <summary>Returns the settings property.</summary>
        public Settings GetNewSettings()
        {
            return Settings;
        }

        /// <summary>If the settings aren't the default settings.</summary>
        private void CheckForChangedSettings()
        {
            SettingsHasChanged = Settings != new Settings();
        }

    }
}
