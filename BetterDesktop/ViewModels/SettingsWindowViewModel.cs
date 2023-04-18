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
        private Settings Settings { get; set; } = new Settings();
        
        /// <summary>The old settigs.</summary>
        private Settings oldSettings;

        public SettingsWindowViewModel(Settings settings)
        {
            Settings = settings;
            oldSettings = settings.Copy();
        }

        /// <summary>When OK is clicked.</summary>
        /// <param name="window">The window.</param>
        [RelayCommand]
        public void Ok(Window window)
        {
            window.Close();
        }

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

    }
}
