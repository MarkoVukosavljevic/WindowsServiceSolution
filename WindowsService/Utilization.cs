using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
namespace WindowsService
{
    public class Utilization
    {

        public int GetCPUUtilization()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToInt32(obj["PercentProcessorTime"]);
                }
            }
            return 0; // Default value if not found.
        }

        public double GetDiskUtilization(string driveLetter = "C:")
        {
            DriveInfo drive = new DriveInfo(driveLetter);
            double percentUsed = ((double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize) * 100;
            return Math.Round(percentUsed, 2);
        }

        public float GetMemoryUtilization()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            float availableMemory = ramCounter.NextValue(); // in MB
            ComputerInfo compinfo = new ComputerInfo();
            float totalMemory = Convert.ToSingle(compinfo.TotalPhysicalMemory / (1024 * 1024)); // in MB
            float usedMemory = totalMemory - availableMemory;
            float percentUsed = (usedMemory / totalMemory) * 100;
            return percentUsed;
        }

    }
}
