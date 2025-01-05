using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace FileExplorer
{
    public class FileExplorerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FileItem> Files { get; set; } = new ObservableCollection<FileItem>();
        public ObservableCollection<FileItem> Drives { get; set; } = new ObservableCollection<FileItem>();

        private string _currentPath;
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
                LoadFilesAndDirectories(_currentPath);
            }
        }

        public FileExplorerViewModel()
        {
            LoadDrives();
        }

        private void LoadDrives()
        {
            Drives.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                Drives.Add(new FileItem(drive.RootDirectory));
            }
        }

        public void LoadFilesAndDirectories(string path)
        {
            Files.Clear();
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    Files.Add(new FileItem(new DirectoryInfo(dir)));
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    Files.Add(new FileItem(new FileInfo(file)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
