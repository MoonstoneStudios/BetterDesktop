using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Avalonia.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDesktop.Models
{
    /// <summary>The settings for the app.</summary>
    [ObservableObject]
    internal partial class Settings
    {

        /// <summary>The paint color</summary>
        [ObservableProperty]
        private Color paintColor = Color.FromArgb(90, 0, 0, 0);

        /// <summary>If the app should minimize to the system tray when the window is closed.</summary>
        [ObservableProperty]
        private bool minimizeToTrayOnClose = true;

    }
}
