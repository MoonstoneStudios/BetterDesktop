using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetterDesktop.ViewModels;
using BetterDesktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetterDesktop
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();

            // setup the window after initialization and the window is shown.
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            {
                var vm = (MainWindowViewModel)d.MainWindow.DataContext;
                vm.SetupWindow(d.MainWindow);

                // add the view model.
                DataContext = new ApplicationViewModel(d.MainWindow as MainWindow);
            }
        }
    }
}