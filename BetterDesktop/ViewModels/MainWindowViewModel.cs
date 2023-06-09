﻿using Avalonia.Controls;
using Avalonia.Input;
using BetterDesktop.IconBackgroundStuff;
using BetterDesktop.Misc;
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

        /// <summary>If the window should close forcefully.</summary>
        public bool forceClose;

        /// <summary>If the shift key is pressed.</summary>
        private bool shiftPressed;

        public MainWindowViewModel()
        {
            manager = new IconBackgroundManager();
            
            // create the manager's settings.
            var settings = new SettingsLoader();
            manager.settings = settings.LoadSettings();
            manager.Start();
        }

        /// <summary>Setup the window</summary>
        /// <param name="window">The window that this view model controls.</param>
        public void SetupWindow(Window window)
        {
            this.window = window;
            window.Closing += WindowClosing;

            window.KeyDown += (s, e) =>
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    shiftPressed = true;
                }
            };
            window.KeyUp += (s, e) =>
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    shiftPressed = false;
                }
            };
        }

        /// <summary>Toggle the icon background.</summary>
        [RelayCommand]
        private void ToggleIconBackground()
        {
            // toggle
            iconBackgroundPainted = !iconBackgroundPainted;
            OnPropertyChanged(nameof(ToggleButtonText));

            if (!iconBackgroundPainted)
                manager.ResetWallpaper();
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
            // save settings
            var settings = new SettingsLoader();
            settings.SaveSettings(Settings);

            if (iconBackgroundPainted)
            {
                // restart the icons.
                manager.Start();
            }
        }

        /// <summary>Called when the window is closing.</summary>
        private void WindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.MinimizeToTrayOnClose && !forceClose && !shiftPressed)
            {
                e.Cancel = true;
                window.Hide();
            }
            else
            {
                // window is being closed for good, reset the wallpaper
                manager.Stop();
            }
        }

    }
}
