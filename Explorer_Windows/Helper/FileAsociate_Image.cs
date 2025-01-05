using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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

    }
}
