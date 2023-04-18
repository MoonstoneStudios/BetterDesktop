using Avalonia.Controls;
using BetterDesktop.IconBackgroundStuff;
using BetterDesktop.Models;
using BetterDesktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace BetterDesktop.ViewModels
{
    [ObservableObject]
    public partial class MainWindowViewModel
    {
        /// <summary>The text for the toggle button.</summary>
        public string ToggleButtonText
        {
            get 
            {
                return iconBackgroundPainted ? "Turn off" : "Turn on";
            }
        }
        
        /// <summary>If the icon background is being painted.</summary>
        private bool iconBackgroundPainted = true;

        /// <summary>The manager.</summary>
        private IconBackgroundManager manager;

        /// <summary>A shorthand for the icon manager settings.</summary>
        private Settings Settings => manager.settings;

        public MainWindowViewModel()
        {
            manager = new IconBackgroundManager();
            // create the manager's settings.
            manager.settings = new Settings();
            manager.Start();
        }

        /// <summary>Toggle the icon background.</summary>
        [RelayCommand]
        private void ToggleIconBackground()
        {
            // toggle
            iconBackgroundPainted = !iconBackgroundPainted;
            OnPropertyChanged(nameof(ToggleButtonText));

            if (!iconBackgroundPainted)
                manager.Reset();
            else
                manager.Start();
        }

        /// <summary>Open Settings window.</summary>
        [RelayCommand]
        private void OpenSettings()
        {
            // create a new settings window
            var settingsWindow = new SettingsWindow();
            var vm = new SettingsWindowViewModel(Settings);
            settingsWindow.DataContext = vm;

            settingsWindow.Show();
            settingsWindow.Closed += SettingsClosed;
        }

        /// <summary>When the settings window is closed.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsClosed(object? sender, EventArgs e)
        {
            var settingsWindow = ((SettingsWindow)sender).DataContext as SettingsWindowViewModel;
            manager.settings = settingsWindow.GetNewSettings();
            // restart the icons.
            manager.Start();
        }

        /// <summary>Close the app.</summary>
        /// <param name="window">The window.</param>
        [RelayCommand]
        private void Close(Window window)
        {
            manager.Reset();
            // minimize to tray later.
            window.Close();
        }

    }
}
