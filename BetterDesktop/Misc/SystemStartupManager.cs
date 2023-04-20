using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using File = System.IO.File;

namespace BetterDesktop.Misc
{
    internal class SystemStartupManager
    {
        /// <summary>The name of the app.</summary>
        public const string APP_NAME = "BetterDesktop";

        /// <summary>Check if the app is a startup app. </summary>
        /// <returns>True is it is a startup app.</returns>
        public bool CheckForStartup()
        {
            return File.Exists(GetShortcutPath());
        }

        /// <summary>Create the startup shortcut.</summary>
        // https://stackoverflow.com/a/4909475
        public void CreateShortcut()
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(GetShortcutPath());
            shortcut.Description = $"Opens The {APP_NAME} App";
            shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, $"{APP_NAME}.exe");
            shortcut.Arguments = "startup";
            shortcut.Save();
        }

        /// <summary>Remove the startup shortcut.</summary>
        public void RemoveShortcut()
        {
            if (CheckForStartup())
            {
                File.Delete(GetShortcutPath());
            }
        }

        /// <summary>Get the path of the startup shortcut.</summary>
        private string GetShortcutPath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(path, $"{APP_NAME}.lnk");
        }

    }
}
