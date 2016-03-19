using Server.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.Custom.Commands
{
    public class Recompile
    {
        private static string UpdateAndRestartFileName = "update_and_restart.bat";
        private static string FullPath = AppDomain.CurrentDomain.BaseDirectory;

        public static void Initialize()
        {
            CommandSystem.Register("recompile", AccessLevel.Administrator, new CommandEventHandler(Recompile_OnCommand));
        }

        private static void Recompile_OnCommand(CommandEventArgs e)
        {
            CommandLogging.WriteLine(e.Mobile, "{0} {1} shutting down server to recompile", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile));

            CommandSystem.Handle(e.Mobile, String.Format("{0}Save", CommandSystem.Prefix));

            string path = string.Join("", FullPath, UpdateAndRestartFileName);
            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);

            Core.Kill();
        }
    }
}
