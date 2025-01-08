﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Virtualization;




namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
    /// <summary>
    /// Interaction logic for CreazaPartitie.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for CreazaPartitie.xaml
    /// </summary>
    public partial class CreazaPartitie : UserControl
    {
        private string diskPath = @"C:\VirtualDisk\recordings.vhdx";
        public VHDManager vhdManager;
        public DriveLetterAssigner DriveLetterAssigner_ { get; set; }




        public CreazaPartitie()
        {
            InitializeComponent();
            vhdManager = new VHDManager();

            if (!IsAdministrator())
            {
                RunAsAdministrator();
                return;
            }

            PopulateDriveLetterComboBox();
        }

        private void PopulateDriveLetterComboBox()
        {
            DriveLetterComboBox.Items.Clear();
            var availableLetters = GetAvailableDriveLetters();
            foreach (var letter in availableLetters)
            {
                DriveLetterComboBox.Items.Add(new ComboBoxItem { Content = letter });
            }
            if (DriveLetterComboBox.Items.Count > 0)
            {
                ((ComboBoxItem)DriveLetterComboBox.Items[0]).IsSelected = true;
            }
        }

        private List<string> GetAvailableDriveLetters()
        {
            var letters = new List<string>();
            try
            {
                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.AddScript(@"
                $assignedLetters = (Get-Volume | Where-Object DriveLetter | Select-Object -ExpandProperty DriveLetter)
                'D'..'Z' | Where-Object { $_ -notin $assignedLetters }
            ");

                    var results = powerShell.Invoke();
                    letters.AddRange(results.Select(r => r.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while retrieving available drive letters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return letters;
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get disk path
                string diskPath = DiskPathTextBox.Text;
                if (string.IsNullOrEmpty(diskPath))
                {
                    MessageBox.Show("Please enter the disk path.");
                    return;
                }

                // Configure drive letter assignment
                var driveLetterAssigner = new DriveLetterAssigner(diskPath);
                if (AssignDriveLetterRadio.IsChecked == true)
                {
                    driveLetterAssigner.DriveLetterAssigner_ = (string)((ComboBoxItem)DriveLetterComboBox.SelectedItem).Content;
                }
                else if (MountInFolderRadio.IsChecked == true)
                {
                    driveLetterAssigner.MountInFolder = true;
                    driveLetterAssigner.MountFolder = FolderPathTextBox.Text;
                }
                else if (NoDriveLetterRadio.IsChecked == true)
                {
                    driveLetterAssigner.DoNotAssignDriveLetter = true;
                }

                // Configure formatting options
                var formatterSettings = new VolumeFormatterSettings
                {
                    FileSystem = (string)((ComboBoxItem)FileSystemComboBox.SelectedItem).Content,
                    AllocationUnitSize = (string)((ComboBoxItem)AllocationUnitSizeComboBox.SelectedItem).Content,
                    VolumeLabel = VolumeLabelTextBox.Text,
                    QuickFormat = QuickFormatCheckBox.IsChecked == true,
                    EnableCompression = EnableCompressionCheckBox.IsChecked == true,
                    DoNotFormat = DoNotFormatCheckBox.IsChecked == true
                };

                // Create formatter and format volume
                var volumeFormatter = new VolumeFormatter(diskPath, driveLetterAssigner, formatterSettings);
                volumeFormatter.FormatVolume();

                MessageBox.Show("Volume formatted successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void AllocationUnitSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSize = ((ComboBoxItem)AllocationUnitSizeComboBox.SelectedItem).Content.ToString();
            MessageBox.Show($"Selected allocation unit size: {selectedSize}");
        }

        // Property for volume size
        private int _volumeSize = 1006;

        public int VolumeSize
        {
            get => _volumeSize;
            set
            {
                _volumeSize = value;
                SimpleVolumeSizeTextBox.Text = _volumeSize.ToString();
            }
        }

        // Command to increase volume size
        public ICommand IncreaseValue => new RelayCommand(_ => VolumeSize += 1);
        // Command to decrease volume size
        public ICommand DecreaseValue => new RelayCommand(_ => VolumeSize = Math.Max(8, VolumeSize - 1)); // Minimum 8 MB


        private void FormatVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            var DriveLetter_ = DriveLetterAssigner_.DriveLetterAssigner_;
            // Creează și montează VHD-ul
            VHDManager.CreateAndMountVHD(diskPath, 1 * 1024 * 1024 * 1024, DriveLetter_); // 1GB VHD, formatted as NTFS
                                                                                          // 1GB

            // Inițializează și particionează discul
            InitializeDiskWithGpt();
            InitializeAndPartitionDisk();

            // Alocă litera de unitate
            //DriveLetterAssigner.AssignDriveLetter();

            NavigateToMainWindow();




        }

        private void DriveLetterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DriveLetterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Creează o instanță a clasei DriveLetterAssigner
                DriveLetterAssigner_ = new DriveLetterAssigner("your-disk-path-here")
                {
                    DriveLetterAssigner_ = selectedItem.Content.ToString() // Setează litera selectată
                };

                Console.WriteLine($"Selected Drive Letter: {DriveLetterAssigner_.DriveLetterAssigner_}");
            }
        }



        public string GetSelectedFileSystem()
        {
            if (FileSystemComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
            }
            return "NTFS"; // Valoarea implicită în caz că nu este nimic selectat
        }


        private void InitializeDiskWithGpt()
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



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (AssignDriveLetterRadio != null && DriveLetterComboBox != null && FolderPathTextBox != null)
            {
                if (sender == AssignDriveLetterRadio)
                {
                    DriveLetterComboBox.IsEnabled = true;
                    FolderPathTextBox.IsEnabled = false;

                    // Atribuie litera selectată la DriveLetterAssigner_
                    if (DriveLetterComboBox.SelectedItem is ComboBoxItem selectedItem)
                    {
                        // Inițializează DriveLetterAssigner_ și setează litera selectată
                        DriveLetterAssigner_ = new DriveLetterAssigner("your-disk-path-here")
                        {
                            DriveLetterAssigner_ = selectedItem.Content.ToString()
                        };

                        Console.WriteLine($"Assigned Drive Letter: {DriveLetterAssigner_.DriveLetterAssigner_}");
                    }
                }
                else if (sender == MountInFolderRadio)
                {
                    DriveLetterComboBox.IsEnabled = false;
                    FolderPathTextBox.IsEnabled = true;
                }
                else if (sender == NoDriveLetterRadio)
                {
                    DriveLetterComboBox.IsEnabled = false;
                    FolderPathTextBox.IsEnabled = false;
                }
            }
        }




        private void InitializeAndPartitionDisk()
        {
            try
            {
                using (PowerShell powerShell = PowerShell.Create())
                {
                    // Script to create a new partition on the initialized disk each time button is pressed
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

        private bool IsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RunAsAdministrator()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(processInfo);
                Application.Current.Shutdown();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Administrator privileges are required.", "Privileges Required", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }


        public static void NavigateToMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }



    }
}

