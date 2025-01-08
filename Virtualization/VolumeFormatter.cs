using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Virtualization
{
    
        public class VolumeFormatter
        {
            public string DiskPath { get; set; }
            public DriveLetterAssigner DriveLetterAssigner_ { get; set; }
            public VolumeFormatterSettings FormatterSettings { get; set; }

            public VolumeFormatter(string diskPath, DriveLetterAssigner driveLetterAssigner, VolumeFormatterSettings formatterSettings)
            {
                DiskPath = diskPath;
                DriveLetterAssigner_ = driveLetterAssigner;
                FormatterSettings = formatterSettings;
            }

            public void FormatVolume()
            {
                if (FormatterSettings.DoNotFormat)
                {
                    Console.WriteLine("Formatting skipped.");
                    return;
                }

                try
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        // Crearea unei noi partiții pe VHD
                        string createPartitionScript = $@"
                    $disk = Get-Disk -Path '{DiskPath}'
                    $partition = New-Partition -DiskNumber $disk.Number -UseMaximumSize";

                        powerShell.AddScript(createPartitionScript);
                        powerShell.Invoke();

                        if (powerShell.HadErrors)
                        {
                            var errors = string.Join("\n", powerShell.Streams.Error);
                            throw new InvalidOperationException($"Failed to create partition: {errors}");
                        }

                        // Apelăm metoda de atribuire a literei de unitate
                        DriveLetterAssigner_.AssignDriveLetter();

                        // Formatăm partiția folosind litera selectată
                        string formatCommand = $@"
                    $partition = Get-Partition -DiskNumber $disk.Number | Where-Object DriveLetter -eq '{DriveLetterAssigner_.DriveLetterAssigner_}'
                    if ($partition) {{
                        Format-Volume -DriveLetter '{DriveLetterAssigner_.DriveLetterAssigner_}' -FileSystem '{FormatterSettings.FileSystem}' 
                            -AllocationUnitSize {(FormatterSettings.AllocationUnitSize == "Default" ? "4096" : FormatterSettings.AllocationUnitSize)}
                            -NewFileSystemLabel '{FormatterSettings.VolumeLabel}' 
                            {(FormatterSettings.QuickFormat ? "-Confirm:$false" : "")} 
                            {(FormatterSettings.EnableCompression ? "-EnableCompression" : "")}
                    }}";

                        powerShell.AddScript(formatCommand);
                        var results = powerShell.Invoke();

                        if (powerShell.HadErrors)
                        {
                            var errors = string.Join("\n", powerShell.Streams.Error);
                            throw new InvalidOperationException($"Failed to format volume: {errors}");
                        }

                        Console.WriteLine("Volume created, formatted, and drive letter assigned successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }
        }

    }
