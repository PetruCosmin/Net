using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;

namespace Camera_Library
{
    
    public class CameraViewModel : INotifyPropertyChanged
    {
        public string camera_name;
        private int _cameraId;
        private string _resolution;
        private readonly CameraSettingsManager _settingsManager;
        private static string DefaultFilePath = Xml_FilePath.SavedCamerasXmlPath;
        private string FilePath { get; set; }
        public string DisplayName_ID => $"{CameraName_Properties} (ID: {CameraId})";
        

        public ResolutionManager ResolutionManager { get; }

        public string CameraName_Properties
        {
            get => camera_name;
            set
            {
                if (camera_name != value)
                {
                    camera_name = value;
                    OnPropertyChanged(nameof(CameraName_Properties));
                }
            }
        }

        public int CameraId
        {
            get => _cameraId;
            set
            {
                if (_cameraId != value)
                {
                    _cameraId = value;
                    OnPropertyChanged(nameof(CameraId));
                }
            }
        }

        public string Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    OnPropertyChanged(nameof(Resolution));
                    UpdateResolution(value);
                }
            }
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand LoadSettingsCommand { get; }

        public ObservableCollection<string> Resolutions => ResolutionManager.Resolutions;
        public string SelectedResolution
        {
            get => ResolutionManager.SelectedResolution;
            set
            {
                if (ResolutionManager.SelectedResolution != value)
                {
                    ResolutionManager.SelectedResolution = value;
                    Resolution = value;
                }
            }
        }

        public CameraViewModel(string filePath = null)
        {
            FilePath = filePath ?? DefaultFilePath;
            _settingsManager = new CameraSettingsManager();
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            LoadSettingsCommand = new RelayCommand(LoadSettings);
            ResolutionManager = new ResolutionManager();
            Resolution = ResolutionManager.SelectedResolution;

            LoadSettings();
        }

        public void SaveSettings(object parameter = null)
        {
            if (string.IsNullOrWhiteSpace(CameraName_Properties))
            {
                MessageBox.Show("Numele camerei nu este setat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Încarcă camerele existente
            var existingCameras = _settingsManager.LoadCameras();

            // Găsește camera existentă cu același ID
            var existingCamera = existingCameras.FirstOrDefault(c => c.IdCamera == CameraId);

            if (existingCamera != null)
            {
                // Verifică dacă altă cameră are același nume
                var cameraWithSameName = existingCameras.FirstOrDefault(c => c.CameraName_Properties == CameraName_Properties && c.IdCamera != CameraId);
                if (cameraWithSameName != null)
                {
                    MessageBox.Show($"O cameră cu numele '{CameraName_Properties}' există deja.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Actualizează camera existentă
                existingCamera.CameraName_Properties = CameraName_Properties;
                existingCamera.ResolutionWidth = GetWidth(Resolution);
                existingCamera.ResolutionHeight = GetHeight(Resolution);
                existingCamera.FrameRate = 30; // Exemplu de valoare implicită
                existingCamera.IsEnabled = true;
                existingCamera.LastConnectionTime = DateTime.Now;
            }
            else
            {
                // Verifică dacă mai există camere cu același nume
                var cameraWithSameName = existingCameras.FirstOrDefault(c => c.CameraName_Properties == CameraName_Properties);
                if (cameraWithSameName != null)
                {
                    MessageBox.Show($"O cameră cu numele '{CameraName_Properties}' există deja.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Adaugă o nouă cameră
                existingCameras.Add(new CameraProperties
                {
                    IdCamera = CameraId,
                    CameraName_Properties = CameraName_Properties,
                    ResolutionWidth = GetWidth(Resolution),
                    ResolutionHeight = GetHeight(Resolution),
                    FrameRate = 30, // Exemplu de valoare implicită
                    IsEnabled = true,
                    LastConnectionTime = DateTime.Now
                });
            }

            // Salvează lista completă
            _settingsManager.SaveCameras(existingCameras);

          

            // Notifică utilizatorul despre salvare reușită
            MessageBox.Show($"Setările au fost salvate:\n\nNume Cameră: {CameraName_Properties}\nRezoluție: {Resolution}",
                            "Salvare Reușită", MessageBoxButton.OK, MessageBoxImage.Information);
        }





        public void LoadSettings(object parameter = null)
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Fișierul de setări nu există.");
                return;
            }

            var serializer = new XmlSerializer(typeof(List<CameraProperties>));
            try
            {
                using (var reader = new StreamReader(FilePath))
                {
                    var cameras = (List<CameraProperties>)serializer.Deserialize(reader);
                    if (cameras != null && cameras.Count > 0)
                    {
                        CameraName_Properties = cameras[0].CameraName_Properties;
                        Resolution = $"{cameras[0].ResolutionWidth}x{cameras[0].ResolutionHeight}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la încărcarea setărilor: {ex.Message}");
            }
        }

        public string GetCameraName()
        {
            return !string.IsNullOrWhiteSpace(CameraName_Properties) ? CameraName_Properties : "Nume camera necunoscut";
        }

        private int GetWidth(string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution)) return 0;
            var parts = resolution.Split('x');
            return parts.Length == 2 && int.TryParse(parts[0], out int width) ? width : 0;
        }

        private int GetHeight(string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution)) return 0;
            var parts = resolution.Split('x');
            return parts.Length == 2 && int.TryParse(parts[1], out int height) ? height : 0;
        }

        private void UpdateResolution(string selectedResolution)
        {
            Console.WriteLine($"Rezoluția selectată: {selectedResolution}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
