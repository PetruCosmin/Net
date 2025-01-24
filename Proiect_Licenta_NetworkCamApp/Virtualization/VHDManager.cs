using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;



using System.Windows.Controls;
using System.Diagnostics;

namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
    public class VHDManager
    {
        public static CreazaPartitie creazaPartitie;

        // Metoda pentru setarea politicii de execuție și rularea unui script PowerShell
        public static void SetExecutionPolicyAndRunScript(string script)
        {
            try
            {
                using (PowerShell powerShell = PowerShell.Create())
                {
                    // Check current execution policy
                    powerShell.AddScript("Get-ExecutionPolicy");
                    var currentPolicy = powerShell.Invoke().FirstOrDefault()?.ToString();
                    Console.WriteLine("Current Execution Policy: " + currentPolicy);

                    // Only set the policy if it's not already RemoteSigned
                    if (currentPolicy != "RemoteSigned")
                    {
                        powerShell.Commands.Clear();
                        powerShell.AddScript("Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process");
                        powerShell.Invoke();
                        Console.WriteLine("Execution Policy set to RemoteSigned");
                    }

                    // Clear commands and add the main script
                    powerShell.Commands.Clear();
                    powerShell.AddScript(script);

                    // Run the main PowerShell script
                    var results = powerShell.Invoke();

                    // Check for errors in script execution
                    if (powerShell.HadErrors)
                    {
                        var errors = string.Join("\n", powerShell.Streams.Error);
                        throw new InvalidOperationException($"Script execution failed: {errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        // Metoda pentru crearea și montarea unui VHD
        public static void CreateAndMountVHD(string path, long sizeBytes, string driveLetter)
        {
            try
            {
                string selectedFileSystem = creazaPartitie?.GetSelectedFileSystem() ?? "NTFS"; // Fallback to NTFS if not set

                string script = $@"
                    Import-Module Storage

                    # Creare VHD dacă nu există deja
                    if (!(Test-Path -Path '{path}')) {{
                        New-VHD -Path '{path}' -SizeBytes {sizeBytes} -Dynamic
                    }}

                    # Montare VHD dacă nu este deja montat
                    $vhd = Get-VHD -Path '{path}'
                    if ($vhd.Attached -eq $false) {{
                        Mount-VHD -Path '{path}'
                    }}

                    # Verifică dacă există un disc RAW și creează o partiție
                    $disk = Get-Disk | Where-Object PartitionStyle -Eq 'RAW'
                    if ($disk) {{
                        Initialize-Disk -Number $disk.Number
                        $partition = New-Partition -DiskNumber $disk.Number -UseMaximumSize

                        # Atribuie litera de unitate specificată
                        Add-PartitionAccessPath -DiskNumber $disk.Number -PartitionNumber $partition.PartitionNumber -AccessPath ('{driveLetter}:\')

                        # Formatarea partiției folosind sistemul de fișiere selectat
                        Format-Volume -DriveLetter '{driveLetter}' -FileSystem {selectedFileSystem} -NewFileSystemLabel 'RecordingsDisk'
                        Write-Output '{driveLetter}'
                    }} else {{
                        Write-Output 'Disk already initialized and formatted.'
                    }}
                ";

                // Set execution policy and run script
                SetExecutionPolicyAndRunScript(script);

                MessageBox.Show("VHDX file created and mounted successfully with " + selectedFileSystem + " file system!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: Need Windows Pro or higher to use PowerShell scripts for VHD management\"  {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Metoda pentru crearea unei partiții cu o literă de unitate disponibilă
        public static void CreatePartitionWithAvailableDriveLetter(string vhdPath, long sizeBytes, string driveLetter)
        {
            try

            {

                string script = $@"
                    Import-Module Storage

                    # Verifică dacă VHD-ul există și creează-l dacă nu există
                    if (!(Test-Path -Path '{vhdPath}')) {{
                        New-VHD -Path '{vhdPath}' -SizeBytes {sizeBytes} -Dynamic
                    }}

                    # Montare VHD dacă nu este deja montat
                    $vhd = Get-VHD -Path '{vhdPath}'
                    if ($vhd.Attached -eq $false) {{
                        Mount-VHD -Path '{vhdPath}'
                    }}

                    # Inițializare disc dacă este RAW și crearea unei singure partiții
                    $disk = Get-Disk | Where-Object PartitionStyle -Eq 'RAW' | Select-Object -First 1
                    if ($disk) {{
                        Initialize-Disk -Number $disk.Number -PartitionStyle GPT
                        New-Partition -DiskNumber $disk.Number -UseMaximumSize
                    }}

                    # Atribuie litera de unitate specificată la prima partiție disponibilă
                    $partition = Get-Partition -DiskNumber $disk.Number | Where-Object DriveLetter -eq $null | Select-Object -First 1
                    if ($partition) {{
                        Add-PartitionAccessPath -DiskNumber $disk.Number -PartitionNumber $partition.PartitionNumber -AccessPath ('{driveLetter}:\')
                        Format-Volume -DriveLetter '{driveLetter}' -FileSystem NTFS -NewFileSystemLabel 'NewPartition'
                        Write-Output '{driveLetter}'
                    }} else {{
                        Write-Output 'Failed to assign a drive letter or create a single partition as needed.'
                    }}
                ";

                // Set execution policy and run script
                SetExecutionPolicyAndRunScript(script);

                MessageBox.Show("Partition created and assigned to drive letter successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare Litera nu a fost alocata!");
                MessageBox.Show($"An unexpected error occurred on creating partition with available drive letter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**********************************************/

       


    }
}
