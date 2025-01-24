using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using Camera_Library;
using Proiect_Licenta_NetworkCamApp.View.SetariView;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Input;

namespace Proiect_Licenta_NetworkCamApp.Sys
{
    public class AddCameras
    {
        private Grid MainGrid;
        private int cameraCounter = 1;
        public List<CameraProperties> camerasList = new List<CameraProperties>(); // Lista de camere
        private static string XmlFilePat = Xml_FilePath.SavedCamerasXmlPath;  // Locația fișierului XML
        public List<string> CameraNames { get; set; } = new List<string>(); //Lista cu numele camerei

        public AddCameras(Grid mainGrid)
        {
            MainGrid = mainGrid ?? throw new ArgumentNullException(nameof(mainGrid));

            // Populăm MainGrid cu camerele din XML la inițializare
            PopulateMainGrid();
        }

        private bool isAddingCamera = false;
        private HashSet<int> usedCameraIds = new HashSet<int>(); // Set pentru a urmări ID-urile utilizate

        public void AddCamera(string cameraName = null)
        {
            if (isAddingCamera) return;

            isAddingCamera = true;

            try
            {
                // Creează un obiect CameraProperties
                var cameraProperties = new CameraProperties();

                // Găsește primul ID disponibil
                int newCameraId = 0;
                while (usedCameraIds.Contains(newCameraId))
                {
                    newCameraId++;
                }

                // Actualizează ID-ul camerei și adaugă în set
                cameraProperties.IdCamera = newCameraId;
                usedCameraIds.Add(newCameraId);

                // Setează numele camerei
                cameraProperties.CameraName_Properties = string.IsNullOrEmpty(cameraName) ? $"Camera {newCameraId}" : cameraName;

                // Adaugă noua cameră în lista de camere
                camerasList.Add(cameraProperties);

                // Creează un Border pentru cameră folosind CameraProperties
                Border newCameraBorder = CreateCameraBorder(cameraProperties);
                newCameraBorder.MouseDown += CameraBorder_MouseDown;

                // Calculează numărul de coloane din MainGrid
                int columnCount = MainGrid.ColumnDefinitions.Count;

                // Adaugă o nouă ColumnDefinition pentru fiecare cameră
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                // Plasează Border-ul în Grid
                MainGrid.Children.Add(newCameraBorder);
                Grid.SetRow(newCameraBorder, 0); // Toate camerele sunt plasate în rândul 0
                Grid.SetColumn(newCameraBorder, columnCount); // Camera este plasată în coloana corespunzătoare

                // Salvează lista actualizată în XML
                SaveToXml();
            }
            finally
            {
                isAddingCamera = false; // Resetează flag-ul
            }
        }





        private Border CreateCameraBorder(CameraProperties cameraProperties)
        {
            Border cameraBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                BorderThickness = new Thickness(3),
                Background = new SolidColorBrush(Colors.White),
                Width = 200,
                Height = 90,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(5),
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 4,
                    Color = Colors.Gray,
                    Opacity = 0.5
                },
                Tag = cameraProperties.IdCamera // Ensure the Tag is set correctly
            };

            // Attach click event to Border
            cameraBorder.MouseLeftButtonUp += (s, e) => ShowCameraControl(cameraProperties);

            Grid cameraContent = new Grid();
            TextBlock cameraTitle = new TextBlock
            {
                Text = $"{cameraProperties.CameraName_Properties} (ID: {cameraProperties.IdCamera})",
                Foreground = Brushes.Black,
                FontSize = 18,
                Margin = new Thickness(10, 5, 0, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            Button closeButton = new Button
            {
                FontSize = 10,
                Content = "Close",
                Width = 70,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 5, 0),
                Tag = cameraBorder // Store the associated Border for reference
            };

            closeButton.Click += CloseButton_Click;

            cameraContent.Children.Add(cameraTitle);
            cameraContent.Children.Add(closeButton);

            cameraBorder.Child = cameraContent;

            return cameraBorder;
        }
        private Border selectedCameraBorder = null;

        private void CameraBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cast sender to Border
            Border clickedBorder = sender as Border;

            if (clickedBorder != null)
            {
                // Unselect the previously selected camera
                if (selectedCameraBorder != null)
                {
                    selectedCameraBorder.BorderBrush = Brushes.Transparent;
                }

                // Select the new camera
                selectedCameraBorder = clickedBorder;
                selectedCameraBorder.BorderBrush = Brushes.Blue; // Change the color to indicate selection

                // Optionally, do something with the selected camera's properties
                var selectedCamera = camerasList.FirstOrDefault(c => c.IdCamera == (int)clickedBorder.Tag);
                if (selectedCamera != null)
                {
                    // Handle selection logic, e.g., update UI or store selected camera properties
                    MessageBox.Show($"Selected Camera:\n\nName: {selectedCamera.CameraName_Properties}\nID: {selectedCamera.IdCamera}",
                                    "Camera Selected", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Call ShowCameraControl with the selected camera's name
                    ShowCameraControl(selectedCamera);
                }
            }
        }

       




        private void SaveToXml()
        {
            try
            {
                if (camerasList.Count == 0)
                {
                    Console.WriteLine("No cameras to save.");
                    return;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<CameraProperties>));
                using (StreamWriter writer = new StreamWriter(XmlFilePat))
                {
                    serializer.Serialize(writer, camerasList);
                }
                Console.WriteLine("XML file saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving XML: {ex.Message}");
                MessageBox.Show($"Error saving XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void SetCameraName(int cameraId, string newCameraName)
        {
            // Verificăm dacă numele camerei nu este gol
            if (string.IsNullOrEmpty(newCameraName))
            {
                MessageBox.Show("Numele camerei nu poate fi gol.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Căutăm camera în lista de camere după ID
            var camera = camerasList.FirstOrDefault(c => c.IdCamera == cameraId);

            // Dacă nu s-a găsit camera, afișăm un mesaj de eroare
            if (camera == null)
            {
                MessageBox.Show($"Camera cu ID-ul {cameraId} nu a fost găsită.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Modificăm numele camerei
            camera.CameraName_Properties = newCameraName;

            // Actualizăm UI-ul pentru a reflecta schimbările (actualizarea camerei în UI)
            UpdateCameraDisplay(cameraId, newCameraName);

            // Salvăm lista actualizată de camere în fișierul XML
            SaveToXml();

        }
        private void UpdateCameraDisplay(int cameraId, string newCameraName)
        {
            // Căutăm Border-ul care conține camera dorită
            foreach (var child in MainGrid.Children)
            {
                // Verificăm dacă elementul curent este de tip Border și are tag-ul corespunzător
                if (child is Border border && border.Tag != null && border.Tag.ToString() == cameraId.ToString())
                {
                    // Căutăm TextBlock-ul din Border care conține numele camerei
                    var cameraContent = border.Child as Grid;
                    var cameraTitle = cameraContent?.Children.OfType<TextBlock>().FirstOrDefault();

                    if (cameraTitle != null)
                    {
                        // Actualizăm textul cu noul nume al camerei
                        cameraTitle.Text = $"{newCameraName} - ID: {cameraId}";
                    }

                    // Iesim din bucla for dacă am găsit și actualizat camera
                    break;
                }
            }
        }
        public void PopulateMainGrid()
        {
            // Încărcăm camerele din XML
            LoadFromXml();

            // Verificăm dacă lista de camere nu este goală
            if (camerasList.Count > 0)
            {
                foreach (var camera in camerasList)
                {
                    // Creăm Border-ul pentru fiecare cameră
                    Border cameraBorder = CreateCameraBorder(camera);

                    cameraBorder.MouseDown += CameraBorder_MouseDown;

                    // Calculăm numărul de coloane existente în MainGrid
                    int columnCount = MainGrid.ColumnDefinitions.Count;

                    // Adăugăm o nouă coloană pentru fiecare cameră
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                    // Plasăm Border-ul în Grid
                    MainGrid.Children.Add(cameraBorder);
                    Grid.SetRow(cameraBorder, 0); // Toate camerele sunt plasate pe rândul 0
                    Grid.SetColumn(cameraBorder, columnCount); // Fiecare cameră va fi plasată într-o coloană diferită
                }
            }
            else
            {
                Console.WriteLine("No cameras to populate.");
            }
        }
        private void LoadFromXml()
        {
            try
            {
                if (File.Exists(XmlFilePat))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CameraProperties>));
                    using (StreamReader reader = new StreamReader(XmlFilePat))
                    {
                        camerasList = (List<CameraProperties>)serializer.Deserialize(reader);
                    }
                    Console.WriteLine("XML file loaded successfully.");
                }
                else
                {
                    Console.WriteLine("XML file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading XML: {ex.Message}");
                MessageBox.Show($"Error loading XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void ShowCameraControl(CameraProperties cameraProperties)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                CameraControl cameraControl = new CameraControl(mainWindow.MainGrid)
                {
                    CameraProperties = cameraProperties
                };

                mainWindow.DisplayControl(cameraControl); // Ensure DisplayControl is implemented in MainWindow
            }
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button closeButton && closeButton.Tag is Border cameraToRemove)
            {
                // Ensure MainGrid and camerasList are not null
                if (MainGrid == null || camerasList == null)
                {
                    Console.WriteLine("MainGrid or camerasList is null.");
                    return;
                }

                // Verificăm dacă Border-ul asociat este valid
                Console.WriteLine($"Camera to remove: {cameraToRemove.Tag}");

                // Ensure cameraToRemove.Tag is not null
                if (cameraToRemove.Tag == null)
                {
                    Console.WriteLine("cameraToRemove.Tag is null.");
                    return;
                }

                // Căutăm camera în lista de camere folosind un identificator unic
                CameraProperties cameraToDelete = camerasList.FirstOrDefault(camera => camera.IdCamera.ToString() == cameraToRemove.Tag.ToString());

                // Dacă am găsit camera, o eliminăm din lista de camere
                if (cameraToDelete != null)
                {
                    Console.WriteLine($"Removing camera: {cameraToDelete.CameraName_Properties}");

                    // Elimină camera din lista de camere
                    camerasList.Remove(cameraToDelete);

                    // Șterge Border-ul din Grid
                    MainGrid.Children.Remove(cameraToRemove);

                    // Salvează lista actualizată în XML
                    SaveToXml();

                }
                else
                {
                    Console.WriteLine("Camera not found in the list.");
                }
            }
            else
            {
                Console.WriteLine("Invalid button click or invalid Tag.");
            }
        }



        public void RefreshMainGrid()
        {
            // Clear the current content of MainGrid
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();

            CameraSettingsManager camerasLoad = new CameraSettingsManager();

            // Reload the cameras from the XML file
            var camerasFromXml = camerasLoad.LoadCameras();

            // Populate MainGrid with the latest camera data
            foreach (var camera in camerasFromXml)
            {
                // Add the camera to the cameras list
                //camerasList.Add(camera);

                // Create a Border for the camera
                Border newCameraBorder = CreateCameraBorder(camera);

                // Attach MouseDown event handler
                newCameraBorder.MouseDown += CameraBorder_MouseDown;

                // Calculate the number of columns
                int columnCount = MainGrid.ColumnDefinitions.Count;

                // Add a new ColumnDefinition for each camera
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                // Place the Border in the Grid
                MainGrid.Children.Add(newCameraBorder);
                Grid.SetRow(newCameraBorder, 0); // All cameras are placed in row 0
                Grid.SetColumn(newCameraBorder, columnCount); // Camera is placed in the corresponding column
            }
        }









    }
}
