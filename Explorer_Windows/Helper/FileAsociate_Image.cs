using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FileExplorer.Helper
{
    /// <summary>
    /// Converts a file path into an image corresponding to the file/folder type.
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class FileAsociate_Image : IValueConverter
    {
        // Instance de FileAsociate_Image pentru a evita crearea de instanțe noi
        private static readonly FileAsociate_Image fileAsociate_Image = new FileAsociate_Image();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            if (path == null)
                return null;

            string image;

            if (Directory.Exists(path))
            {
                // Este un director
                image = "folder_image.png";
            }
            else if (FileExists(path))
            {
                // Este un fișier - asociere pe baza extensiei
                var extension = Path.GetExtension(path).ToLower();

                switch (extension)
                {
                    case ".txt":
                        image = "txt_file.png"; // Imagine specifică pentru fișiere text
                        break;
                    case ".jpg":
                    case ".png":
                        image = "image_file.png"; // Imagine pentru fișiere de imagine
                        break;
                    case ".exe":
                        image = "exe_file.png"; // Imagine pentru fișiere executabile
                        break;
                    default:
                        image = "generic_file.png"; // Imagine generică pentru fișiere
                        break;
                }
            }
            else
            {
                // Calea nu există sau este invalidă
                image = "UnknownIcon.png";
            }

            // Corectarea calea pentru resurse
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{image}", UriKind.Absolute));
        }

        // ConvertBack nu este implementat, nu este necesar în acest context
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public static bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
        public static bool DirectoryExists(string directoryPath)
        {
            return System.IO.Directory.Exists(directoryPath);
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



    }
}
