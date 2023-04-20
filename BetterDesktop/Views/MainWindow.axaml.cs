using Avalonia.Controls;
using System;

namespace BetterDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Hide window if started through system startup.</summary>
        protected override void OnOpened(EventArgs e)
        {
            if (App.StartedOnStartup)
            {
                Hide();
                App.StartedOnStartup = false;
            }
            base.OnOpened(e);
        }
    }
}