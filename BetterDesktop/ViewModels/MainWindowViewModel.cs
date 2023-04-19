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

        /// <summary>The main window.</summary>
        private Window window;

        public bool forceClose;

        public MainWindowViewModel()
        {
            manager = new IconBackgroundManager();
            // create the manager's settings.
            manager.settings = new Settings();
            manager.Start();
        }

        /// <summary>Setup the window</summary>
        /// <param name="window">The window that this view model controls.</param>
        public void SetupWindow(Window window)
        {
            this.window = window;
            window.Closing += WindowClosing;
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

        /// <summary>Called when the window is closing.</summary>
        private void WindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.MinimizeToTrayOnClose && !forceClose)
            {
                e.Cancel = true;
                window.Hide();
            }
            else
            {
                // window is being closed for good, reset the wallpaper
                manager.Reset();
            }
        }

    }
}
