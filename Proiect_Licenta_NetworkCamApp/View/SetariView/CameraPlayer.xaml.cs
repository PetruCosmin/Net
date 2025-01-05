using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{

    /// <summary>
    /// Interaction logic for CameraPlayer.xaml
    /// </summary>
    public partial class CameraPlayer : UserControl
    {

        private bool isCameraListVisible = false;


        public CameraPlayer()
        {
            InitializeComponent();
        }



        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMediaElement.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMediaElement.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMediaElement.Stop();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Cod pentru descărcarea filmării
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CameraMediaElement.NaturalDuration.HasTimeSpan)
            {
                CameraMediaElement.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            }
        }




        private void Videolist_Click(object sender, RoutedEventArgs e)
        {
            // Toggle visibility of the CameraListPanel
            if (isCameraListVisible)
            {
                CameraListPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Clear any existing items in CameraListPanel to avoid duplicates
                CameraListPanel.Children.Clear();

                // Afișează camerele existente
                DisplayExistingCameras();

                CameraListPanel.Visibility = Visibility.Visible;
            }

            // Toggle the state
            isCameraListVisible = !isCameraListVisible;
        }

        private void DisplayExistingCameras()
        {
            // Exemplu de camere existente
            string[] existingCameras = { "Camera 1", "Camera 2", "Camera 3" };

            foreach (var camera in existingCameras)
            {
                TextBlock cameraTextBlock = new TextBlock
                {
                    Text = camera,
                    Margin = new Thickness(5)
                };

                CameraListPanel.Children.Add(cameraTextBlock);
            }
        }

    }


}

