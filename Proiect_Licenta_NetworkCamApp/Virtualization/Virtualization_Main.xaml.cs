using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

using System.Management.Automation;
using System.Diagnostics;
using System.Security.Principal;
using FileExplorer.Helper;
using FileExplorer;
using System.Windows.Interop;


namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
    /// <summary>
    /// Interaction logic for Virtualization_Main.xaml
    /// </summary>
    public partial class Virtualization_Main : Page
    {


        private Stack<string> backStack = new();
        private Stack<string> forwardStack = new();
        public string currentPath = string.Empty;
        public BitmapSource Icon { get; set; }
        public FileAsociate_Image fileAsociateImage;
        


        private string storagePath = @"C:\VirtualDisk\";

        public Virtualization_Main()
        {
            
                InitializeComponent();

            //fileAsociateImage = new FileAsociate_Image();

            Loaded += MainWindow_Loaded;
           

            LoadDirectories(storagePath);
            currentPath = "C:\\";


        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load drives into the DrivesListBox
            foreach (var drive in Directory.GetLogicalDrives())
            {
                DrivesListBox.Items.Add(drive);
            }
        }

        private void LoadDirectories(string path)
            {
                if (!Directory.Exists(path))
                {
                    MessageBox.Show("Virtual disk not mounted. 'C:\\VirtualDisk\\' Please mount the disk using the button to access recordings.",
                                    "Disk Not Mounted", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var rootDirectoryInfo = new DirectoryInfo(path);
                var rootItem = CreateDirectoryNode(rootDirectoryInfo);
                FileListView.Items.Add(rootItem);
            }

            private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
            {
                var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = directoryInfo };
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directoryNode.Items.Add(CreateDirectoryNode(directory));
                }
                return directoryNode;
            }

            private void DirectoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            {
                if (e.NewValue is TreeViewItem selectedItem && selectedItem.Tag is DirectoryInfo directoryInfo)
                {
                    LoadFiles(directoryInfo);
                }
            }
        private void LoadFiles(DirectoryInfo directoryInfo)
        {
            FileListView.Items.Clear();
            foreach (var file in directoryInfo.GetFiles())
            {
                FileListView.Items.Add(new
                {
                    Name = file.Name,
                    Size = (file.Length / 1024).ToString("N0") + " KB",
                    Type = file.Extension,
                    DateModified = file.LastWriteTime.ToString("g") // Format date as desired
                });
            }
        }



        //*********************************************************************************



        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == DrivesListBox && DrivesListBox.SelectedItem is string drive && Directory.Exists(drive))
                {
                    NavigateToPath(drive);
                }
                else if (sender == SidebarListBox && SidebarListBox.SelectedItem is ListBoxItem sidebarItem)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + $"\\{sidebarItem.Content}";
                    if (Directory.Exists(path))
                    {
                        NavigateToPath(path);
                    }
                    else
                    {
                        MessageBox.Show("Calea selectată nu este validă sau nu există.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (sender == PathTextBox)
                {
                    string path = PathTextBox.Text;
                    if (Directory.Exists(path))
                    {
                        NavigateToPath(path);
                    }
                    else
                    {
                        MessageBox.Show("Calea introdusă nu este validă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (sender == FileListView && FileListView.SelectedItem is FileItem selectedItem)
                {
                    string selectedFilePath = System.IO.Path.Combine(currentPath, selectedItem.Name);

                    if (Directory.Exists(selectedFilePath))
                    {
                        NavigateToPath(selectedFilePath);
                    }
                    else
                    {
                        try
                        {
                            VHD_send.Primeste_VHD(selectedFilePath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error executing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else if (e.Key == Key.Delete && sender == FileListView)
            {
                MessageBox.Show("Ștergere element selectat!");
            }
        }



        //******************************************************************************



        //---------------------

        public void NavigateToPath(string path, bool addToHistory = true)
        {
            
            try
            {
                if (addToHistory && !string.IsNullOrEmpty(currentPath))
                {
                    backStack.Push(currentPath);
                }

                currentPath = path;
                PathTextBox.Text = path;
                FileListView.Items.Clear();

                // Load directories
                foreach (var dir in Directory.GetDirectories(path))
                {
                    FileListView.Items.Add(new FileExplorer.FileItem
                    {
                        Name = System.IO.Path.GetFileName(dir),
                        FullPath = dir,
                        Type = "Folder",
                        Icon = FileAsociate_Image.GetFolderIcon() // Aici se va folosi iconul extras
                    });
                }
                

                // Load files using CreateFileItem method
                foreach (var file in Directory.GetFiles(path))
                {
                    var fileItem = CreateFileItem(file);
                    if (fileItem != null)
                    {
                        FileListView.Items.Add(fileItem);
                    }
                }

                UpdateStatus();
                FileListView.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateStatus()
        {
            StatusSelectedItems.Content = $"Selected Items: {FileListView.SelectedItems.Count}";
            StatusOperations.Content = "Ongoing Operations: 0";
        }
        public FileItem CreateFileItem(string path)
        {
            try
            {
                // Verify if the file exists
                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show($"File does not exist: {path}");
                    return null;
                }

                var fileInfo = new FileInfo(path);

                // Ensure the FileItem is created properly
                return new  FileItem
                {
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    Icon = FileItem.GetFileIcon(path)
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Unauthorized access to file: {path} - {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la crearea FileItem pentru: {path} - {ex.Message}");
                return null;
            }
        }


        //------------------------------


        private void WrapPanel_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                // Handler pentru evenimentul de schimbare a focusului
            }


        //--------------------------------------------------------------
        public void DrivesListBox_SelectionClick(object sender, MouseButtonEventArgs e)
        {
            if (DrivesListBox.SelectedItem is string drive && Directory.Exists(drive))
            {
                NavigateToPath(drive); // Navighează la unitatea selectată
            }
            else
            {
                MessageBox.Show("Unitatea selectată nu este validă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            // Creează un dialog de selectare a fișierelor
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Selectează un fișier",
                Filter = "Toate fișierele (*.*)|*.*", // Poți specifica alte filtre, ex: "Fișiere VHD (*.vhd)|*.vhd"
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) // Folder inițial
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Trimite calea către metoda Primeste_VHD
                Primeste_VHD(currentPath);

               // MessageBox.Show($"Fișier selectat: {selectedFilePath}", "Fișier Selectat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            */


            VHD_send.Primeste_VHD(currentPath);
        }
        // Handler pentru dublu-click pe FileListView
        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileListView.SelectedItem is FileExplorer.FileItem selectedItem)
            {
                string selectedFilePath = System.IO.Path.Combine(currentPath, selectedItem.Name);

                if (Directory.Exists(selectedFilePath))
                {
                    // Navigăm la director
                    NavigateToPath(selectedFilePath);
                }
                else
                {
                    // Deschidem fișierul
                    try
                    {
                        System.Diagnostics.Process.Start(selectedFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Handler pentru SidebarListBox
        private void SidebarListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SidebarListBox.SelectedItem is ListBoxItem sidebarItem)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + $"\\{sidebarItem.Content}";

                if (Directory.Exists(path))
                {
                    NavigateToPath(path);
                    // Setați focusul pe panoul Sidebar
                    UserPanel.Focus();
                }
                else
                {
                    MessageBox.Show("Calea selectată nu este validă sau nu există.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

       

        private void Main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == DrivesListBox && DrivesListBox.SelectedItem is string drive && Directory.Exists(drive))
            {
                NavigateToPath(drive);
            }
            else if (sender == SidebarListBox && SidebarListBox.SelectedItem is ListBoxItem sidebarItem)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + $"\\{sidebarItem.Content}";
                if (Directory.Exists(path))
                {
                    NavigateToPath(path);
                }
                else
                {
                    MessageBox.Show("Calea selectată nu este validă sau nu există.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (sender == PathTextBox)
            {
                string path = PathTextBox.Text;
                if (Directory.Exists(path))
                {
                    NavigateToPath(path);
                }
                else
                {
                    MessageBox.Show("Calea introdusă nu este validă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (sender == FileListView && FileListView.SelectedItem is FileItem selectedItem)
            {
                string selectedFilePath = System.IO.Path.Combine(currentPath, selectedItem.Name);

                if (Directory.Exists(selectedFilePath))
                {
                    NavigateToPath(selectedFilePath);
                }
                else
                {
                    try
                    {
                        VHD_send.Primeste_VHD(selectedFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error executing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }




        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (backStack.Count > 0)
            {
                forwardStack.Push(currentPath);
                NavigateToPath(backStack.Pop(), addToHistory: false);
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (forwardStack.Count > 0)
            {
                backStack.Push(currentPath);
                NavigateToPath(forwardStack.Pop(), addToHistory: false);
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            
            var parentPath = Directory.GetParent(currentPath)?.FullName ?? "C:\\";

            NavigateToPath(parentPath);
        }


      
        private void CreateVhdButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Navigate to CreazaPartitie page
            var createPartitionPage = new Virtualization. CreazaPartitie();
            this.Content = createPartitionPage;
        }

        private void DrivesListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

       
        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualizare status pentru numărul de elemente selectate
            StatusSelectedItems.Content = $"Selected Items: {FileListView.SelectedItems.Count}";
        }


       

        private void CreateAttachVHDButton_Click(object sender, RoutedEventArgs e)
        {
            ////
            ///Metoda care foloseste disk Management pentru sisteme
            ///care nu suporta Hyper-V

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "diskmgmt.msc", 
                    UseShellExecute = true
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while trying to open Disk Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       



        //---------------------------------



    }


}

