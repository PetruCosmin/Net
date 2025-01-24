using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading.Tasks;



namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
    
        public class VolumeFormatterSettings
        {
            public string FileSystem { get; set; } = "NTFS";
            public string AllocationUnitSize { get; set; } = "Default"; // or specify in bytes
            public string VolumeLabel { get; set; } = "Cam_Project";
            public bool QuickFormat { get; set; } = true;
            public bool EnableCompression { get; set; } = false;
            public bool DoNotFormat { get; set; } = false;

            public VolumeFormatterSettings() { }

            public VolumeFormatterSettings(string fileSystem, string allocationUnitSize, string volumeLabel, bool quickFormat, bool enableCompression)
            {
                FileSystem = fileSystem;
                AllocationUnitSize = allocationUnitSize;
                VolumeLabel = volumeLabel;
                QuickFormat = quickFormat;
                EnableCompression = enableCompression;
            }
        }

}
