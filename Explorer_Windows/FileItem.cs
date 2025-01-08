using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing; // Asigură-te că ai această importare
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

namespace FileExplorer
{
   

  
        public class FileItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Type { get; set; }
            public string Size { get; set; }
            public DateTime DateModified { get; set; }
            public string FilePath { get; set; }
            public string FileName => System.IO.Path.GetFileName(FilePath);
            public BitmapSource Icon { get; set; }
            public bool IsDirectory { get; internal set; }
        public string FullPath { get; set; }

        // Constructor implicit (fără parametri)
        public FileItem()
            {
                // Aici poți adăuga valori implicite, dacă dorești
                Name = string.Empty;
                Path = string.Empty;
                Type = string.Empty;
                Size = string.Empty;
                DateModified = DateTime.MinValue;
                FilePath = string.Empty;
                Icon = null;
            }

            // Constructor pentru directoare
            public FileItem(DirectoryInfo directoryInfo)
            {
                FilePath = directoryInfo.FullName;  // Setează calea completă
                Icon = GetFileIcon(FilePath);  // Obține icoana directorului
                Name = directoryInfo.Name;
                Path = directoryInfo.FullName;
                Type = "Directory";
                Size = "-";
                DateModified = directoryInfo.LastWriteTime;
            }

            // Constructor pentru fișiere
            public FileItem(FileInfo fileInfo)
            {
                FilePath = fileInfo.FullName;  // Setează calea completă
                Icon = GetFileIcon(FilePath);  // Obține icoana fișierului
                Name = fileInfo.Name;
                Path = fileInfo.FullName;
                Type = "File";
                Size = fileInfo.Length.ToString() + " bytes";
                DateModified = fileInfo.LastWriteTime;
            }

            // Metodă statică pentru obținerea icoanei unui fișier sau director
            public static BitmapSource GetFileIcon(string filePath)
            {
                // Folosește System.Drawing pentru a obține icoana
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);

                // Convertește icoana într-un BitmapSource pentru a fi utilizată în XAML
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                    );
            }



        }
    }
