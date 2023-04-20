using Avalonia.Controls;
using BetterDesktop.Misc;
using BetterDesktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDesktop.ViewModels
{
    /// <summary>View model for the app.</summary>
    [ObservableObject]
    internal partial class ApplicationViewModel
    {
        /// <summary>The main window.</summary>
        private MainWindow mainWindow;

        public ApplicationViewModel(MainWindow window)
        {
            mainWindow = window;

            // check if it has a startup shortcut.
            var startup = new SystemStartupManager();
            if (!startup.CheckForStartup())
            {
                startup.CreateShortcut();
            }
        }

        /// <summary>Show the main window from tray icon.</summary>
        [RelayCommand]
        public void OpenWindow()
        {
            mainWindow.Show();
        }

        /// <summary>Exit app through tray icon.</summary>
        [RelayCommand]
        public void Exit()
        {
            var vm = (MainWindowViewModel)mainWindow.DataContext;
            vm.forceClose = true;

            mainWindow.Close();
        }


    }
}
