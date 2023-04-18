using Avalonia.Controls;
using BetterDesktop.IconBackgroundStuff;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        public MainWindowViewModel()
        {
            manager = new IconBackgroundManager();
            manager.Start();
        }

        /// <summary>Toggle the icon background.</summary>
        [RelayCommand]
        private void ToggleIconBackground()
        {
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
            // TODO
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
