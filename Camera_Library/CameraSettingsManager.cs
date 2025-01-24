using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Xml.Serialization;


namespace Camera_Library
{
    
    public class CameraSettingsManager
    {
        private static string FilePath = Xml_FilePath.SavedCamerasXmlPath;

        // Constructor care verifică dacă fișierul există, altfel creează unul gol
        public CameraSettingsManager()
        {
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(FilePath))
            {
                // Crearea unui fișier gol pentru a evita erori
                var cameras = new List<CameraProperties>();
                SaveCameras(cameras);
            }
        }

        // Salvează lista de camere într-un fișier XML
        public void SaveCameras(List<CameraProperties> cameras)
        {
            var serializer = new XmlSerializer(typeof(List<CameraProperties>));
            using (var writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, cameras);
            }

        }

        // Încarcă lista de camere din fișierul XML
        public List<CameraProperties> LoadCameras()
        {
            EnsureFileExists();

            var serializer = new XmlSerializer(typeof(List<CameraProperties>));
            using (var reader = new StreamReader(FilePath))
            {
                return (List<CameraProperties>)serializer.Deserialize(reader);
            }
        }
    }
}
