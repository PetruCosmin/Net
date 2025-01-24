using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Management;

namespace Proiect_Licenta_NetworkCamApp.Virtualization
{




    public class DriveLetterAssigner
    {
        private string DiskPath = @"C:\VirtualDisk\recordings.vhdx";
        
        public string DriveLetterAssigner_ { get; set; }  // Default drive letter
        public bool MountInFolder { get; set; } = false;
        public string MountFolder { get; set; } = string.Empty;
        public bool DoNotAssignDriveLetter { get; set; } = false;


        public DriveLetterAssigner(string diskPath)
        {
            DiskPath = diskPath ?? throw new ArgumentNullException(nameof(diskPath));
        }


        public void AssignDriveLetter()
        {
            if (string.IsNullOrEmpty(DriveLetterAssigner_))
            {
                MessageBox.Show("Please select a drive letter from the ComboBox.");
                return;
            }

            // Create DriveLetterAssigner object
            var assigner = new DriveLetterAssigner(DiskPath)
            {
                DriveLetterAssigner_ = DriveLetterAssigner_,
                MountInFolder = false
            };

            try
            {
                // Assign the drive letter using the DriveLetterAssigner class
                assigner.AssignDriveLetter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return;  // Ensure we exit here instead of continuing to avoid unnecessary actions
            }

            if (DoNotAssignDriveLetter)
            {
                Console.WriteLine("Drive letter assignment skipped.");
                return;
            }

            using (PowerShell powerShell = PowerShell.Create())
            {
                string command = $@"
        $partition = Get-Partition -DiskPath '{DiskPath}'
        if ($partition) {{
            # Remove any existing partition access path (if necessary)
            Remove-PartitionAccessPath -PartitionNumber $partition.PartitionNumber -Confirm:$false

            # Verify if the provided drive letter is available
            $existingDriveLetter = Get-Volume | Where-Object {{ $_.DriveLetter -eq '{DriveLetterAssigner_}' }}
            if ($existingDriveLetter) {{
                Write-Host 'The requested drive letter is already in use. Assigning an available letter instead.'
                 
                # Assign the first available drive letter from D-Z range
                $allLetters = 'D'..'Z'
                $usedLetters = (Get-Volume | Where-Object {{ $_.DriveLetter -ne $null }} | Select-Object -ExpandProperty DriveLetter)
                $availableLetters = $allLetters | Where-Object {{ $_ -notin $usedLetters }}
                $assignedLetter = $availableLetters | Select-Object -First 1

                Add-PartitionAccessPath -PartitionNumber $partition.PartitionNumber -DriveLetter $assignedLetter
                Write-Host 'Assigned drive letter: ' + $assignedLetter
            }} else {{
                # If the requested letter is available, assign it directly
                {(MountInFolder ? $"Add-PartitionAccessPath -PartitionNumber $partition.PartitionNumber -AccessPath '{MountFolder}'"
                                        : $"Add-PartitionAccessPath -PartitionNumber $partition.PartitionNumber -DriveLetter '{DriveLetterAssigner_}'")}
                Write-Host 'Assigned drive letter: ' + '{DriveLetterAssigner_}'
            }}
        }}";

                // Execute the PowerShell command
                powerShell.AddScript(command);
                var results = powerShell.Invoke();

                if (powerShell.HadErrors)
                {
                    var errors = string.Join("\n", powerShell.Streams.Error);
                    throw new InvalidOperationException($"Failed to assign drive letter: {errors}");
                }

                Console.WriteLine("Drive letter assigned successfully.");
            }
        }



    }
}
