using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
    public static class PowerShellHelper
    {
   
       
            public static void InitializeDiskWithGpt()
            {
                try
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        // Setează politica de execuție pentru sesiunea curentă
                        powerShell.AddScript("Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force");

                        // Script pentru inițializarea discului cu GPT
                        powerShell.AddScript(@"
                        # Obține discul RAW și îl inițializează cu GPT
                        $disk = Get-Disk | Where-Object PartitionStyle -Eq 'RAW'
                        if ($disk) {
                            Initialize-Disk -Number $disk.Number -PartitionStyle GPT
                        } else {
                            Write-Output 'Disk is already initialized or not available as RAW.'
                        }
                    ");

                        var results = powerShell.Invoke();

                        if (powerShell.HadErrors)
                        {
                            var errors = powerShell.Streams.Error.Select(e => e.ToString()).ToList();
                            string errorMessage = string.Join("\n", errors);
                            MessageBox.Show($"Eroare la inițializarea discului:\n{errorMessage}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Discul a fost inițializat cu GPT cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"A apărut o eroare neașteptată la inițializare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static void InitializeAndPartitionDisk()
            {
                try
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        // Script pentru a crea o nouă partiție pe discul inițializat de fiecare dată când butonul este apăsat
                        powerShell.AddScript(@"
                        $disk = Get-Disk | Where-Object PartitionStyle -Eq 'GPT' | Where-Object IsOffline -eq $false
                        if ($disk) {
                            $freeSpace = $disk | Get-Partition | Where-Object SizeRemaining -gt 1MB | Sort-Object -Property SizeRemaining -Descending | Select-Object -First 1
                            if ($freeSpace) {
                                New-Partition -DiskNumber $disk.Number -Size 200MB -AssignDriveLetter | Format-Volume -FileSystem NTFS -NewFileSystemLabel 'VirtualPartition'
                            }
                        }
                    ");

                        var results = powerShell.Invoke();

                        if (powerShell.HadErrors)
                        {
                            var errors = powerShell.Streams.Error.Select(e => e.ToString()).ToList();
                            string errorMessage = string.Join("\n", errors);
                            MessageBox.Show($"Error creating new partition:\n{errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("New partition created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred while creating a new partition: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
