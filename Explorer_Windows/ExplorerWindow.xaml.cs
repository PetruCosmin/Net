using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using FileExplorer.File;
using FileExplorer;

using FileExplorer.Helper;


namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class ExplorerWindow : Window
    {
        private Stack<string> backStack = new();
        private Stack<string> forwardStack = new();
        public string currentPath = string.Empty;
        public BitmapSource Icon { get; set; }
        

        public ExplorerWindow()
        {
            InitializeComponent();

           Loaded += MainWindow_Loaded;
          
          
        }
      
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load drives into the DrivesListBox
            foreach (var drive in Directory.GetLogicalDrives())
            {
                DrivesListBox.Items.Add(drive);
            }
        }
        
        private void SidebarListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SidebarListBox.SelectedItem is ListBoxItem sidebarItem)
            {
                NavigateToPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + $"\\{sidebarItem.Content}");
            }
            else if (DrivesListBox.SelectedItem is ListBoxItem driveItem)
            {
                NavigateToPath(driveItem.Content.ToString()); // Navighează la unitatea selectată
            }
        }




        
        private void DrivesListBox_SelectionChanged(object sender, MouseButtonEventArgs e)
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


        private void PathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                currentPath = "C:\\";
                NavigateToPath(PathTextBox.Text);
            }
        }


        private void DrivesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DrivesListBox.SelectedItem is string drive)
            {
                NavigateToPath(drive); // Navighează la unitatea selectată
                e.Handled = true; // Previne alte acțiuni legate de apăsarea tastei
            }
        }
        private void SidebarListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SidebarListBox.SelectedItem is ListBoxItem sidebarItem)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + $"\\{sidebarItem.Content}";
                if (Directory.Exists(path))
                {
                    NavigateToPath(path); // Navighează la folderul selectat
                }
                else
                {
                    MessageBox.Show("Calea selectată nu este validă sau nu există.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                e.Handled = true;
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


            VHD_send. Primeste_VHD(currentPath);
        }

        

        


        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileListView.SelectedItem is FileExplorer.FileItem selectedItem)
            {
                if (selectedItem.Type == "Folder")
                {
                    NavigateToPath(selectedItem.FullPath); // Navighează la folderul selectat
                }
                else
                {
                    try
                    {
                       
                        VHD_send. Primeste_VHD(currentPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Eroare la executarea fișierului: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        private void NavigateToPath(string path, bool addToHistory = true)
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
                    FileListView.Items.Add(new FileExplorer. FileItem
                    {
                        Name = System.IO.Path.GetFileName(dir),
                        FullPath = dir,
                        Type = "Folder",
                        Icon = GetFolderIcon() // Aici se va folosi iconul extras
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static BitmapSource GetFolderIcon()
        {
            try
            {
                // Calea către folderul System32
                string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\WinSxS\amd64_microsoft-windows-dxp-deviceexperience_31bf3856ad364e35_10.0.20348.2849_none_ef67066c2c259c24";

                // Extrage iconul asociat cu folderul Windows
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(system32Path + @"\Folder.ico"))
                {
                    if (icon != null)
                    {
                        // Convertirea iconului în BitmapSource pentru WPF
                        return Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            new Int32Rect(0, 0, icon.Width, icon.Height),
                            BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la obținerea iconului: {ex.Message}");
            }

            return null;
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
                return new FileItem
                {
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    Icon = File.FileItem.GetFileIcon(path)
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






        private void UpdateStatus()
        {
            StatusSelectedItems.Content = $"Selected Items: {FileListView.SelectedItems.Count}";
            StatusOperations.Content = "Ongoing Operations: 0";
        }
    }
}




