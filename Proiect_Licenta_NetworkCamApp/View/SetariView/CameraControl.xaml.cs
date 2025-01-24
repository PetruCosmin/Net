using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Camera_Library;
//using Camera_.Proiect_Licenta_NetworkCamApp.View.SetariView;
using Proiect_Licenta_NetworkCamApp.Sys;



namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl, INotifyPropertyChanged
    {
        private CameraProperties _cameraProperties;
        private string camera_name;
        private Grid _mainGrid;
        private string _selectedResolution;
        public ObservableCollection<string> Resolutions { get; set; } = new ObservableCollection<string>();
        public ResolutionManager ResolutionManager { get; set; }
        public FrameManager FrameManager { get; set; }


        public CameraProperties CameraProperties
        {
            get => _cameraProperties;
            set
            {
                if (_cameraProperties != value)
                {
                    _cameraProperties = value;
                    OnPropertyChanged(nameof(CameraProperties));
                    OnPropertyChanged(nameof(DisplayName_ID)); // Ensure PropertyChanged is raised

                }
            }
        }

       

        public string DisplayName_ID => $"{CameraProperties?.CameraName_Properties} (ID: {CameraProperties?.IdCamera})";
        
        public static readonly DependencyProperty CameraNameProperty =
            DependencyProperty.Register(
                "CameraName",
                typeof(string),
                typeof(CameraControl),
                new PropertyMetadata(string.Empty, OnCameraNameChanged));

        public string CameraName_Control
        {
            get => camera_name;
            set
            {
                if (camera_name != value)
                {
                    camera_name = value;
                    OnPropertyChanged(nameof(CameraName_Control));
                }
            }
        }
        public string SelectedResolution 
        {
            get => _selectedResolution;
            set 
            {
                if (_selectedResolution != value)
                {
                    _selectedResolution = value; 
                    OnPropertyChanged(nameof(SelectedResolution));
                }
            }
        }


        public CameraControl(Grid mainGrid)
        {
            InitializeComponent();
            _mainGrid = mainGrid; // Assign MainGrid
            DataContext = this;
            //this.DataContext = new CameraViewModel();
            // Instantiate managers
            ResolutionManager = new ResolutionManager();
            FrameManager = new FrameManager();
            this.Loaded += CameraControl_Loaded;
        }

        private void CameraControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CameraProperties != null)
            {
                CameraNameTextBlock.Text = CameraProperties.CameraName_Properties;
                CameraName_Control = CameraProperties.CameraName_Properties;

                if (!Resolutions.Any())
                {
                    Resolutions.Add("1920x1080");
                    Resolutions.Add("1280x720");
                }

                Resolutions.Clear();
                foreach (var resolution in Resolutions)
                {
                    Resolutions.Add(resolution);
                }

                SelectedResolution = CameraProperties.CameraName_Properties ?? Resolutions.FirstOrDefault();
            }
        }


        private static void OnCameraNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CameraControl control && e.NewValue is string newValue)
            {
                control.CameraName_Control = newValue;
            }
        }

        private void OnSaveSettingsClick(object sender, RoutedEventArgs e)
        {
            if (CameraProperties != null)
            {
                // Update camera properties
                CameraProperties.CameraName_Properties = CameraName_Control;
                // Save settings to XML
                SaveSettings();
                // Pass MainGrid to AddCameras constructor
                var refresh = new AddCameras(_mainGrid);
                refresh. RefreshMainGrid();
            }
            else
            {
                MessageBox.Show("No camera selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveSettings()
        {
            if (CameraProperties != null)
            {
                var settingsManager = new CameraSettingsManager();
                var cameras = settingsManager.LoadCameras();
                var existingCamera = cameras.FirstOrDefault(c => c.IdCamera == CameraProperties.IdCamera);

                if (existingCamera != null)
                {
                    existingCamera.CameraName_Properties = CameraProperties.CameraName_Properties;
                    // Update other properties as necessary
                }

                settingsManager.SaveCameras(cameras);

                MessageBox.Show($"Settings have been saved:\n\nCamera Name: {CameraProperties.CameraName_Properties}",
                                "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*

        Grid MainGrid = new Grid();

        public void DisplayControl(UserControl control)
        {
            MainGrid.Children.Clear(); // Elimină orice control existent
            MainGrid.Children.Add(control); // Adaugă noul UserControl

        }

        */

    }
}

