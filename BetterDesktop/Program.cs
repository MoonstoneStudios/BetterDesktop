using Avalonia;
using BetterDesktop.ViewModels;
using System;

namespace BetterDesktop
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // check for args.
            if (args.Length > 0)
            {
                // if the first argument is "startup"
                // set that the app was run from the system startup system.
                if (args[0] == "startup")
                {
                    App.StartedOnStartup = true;
                }
            }

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
