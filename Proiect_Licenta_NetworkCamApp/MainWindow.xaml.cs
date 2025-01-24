using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Proiect_Licenta_NetworkCamApp.Sys;

using Proiect_Licenta_NetworkCamApp.View.SetariView;

using Proiect_Licenta_NetworkCamApp;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Globalization;
using Camera_Library;
using FileExplorer;
using Proiect_Licenta_NetworkCamApp.View;
using Proiect_Licenta_NetworkCamApp.Virtualization;








namespace Proiect_Licenta_NetworkCamApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AddCameras _addCameras;


        public MainWindow()
        {
            //DataContext = new MyViewModel();
            InitializeComponent();
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Maximized;
            _addCameras = new AddCameras(MainGrid);


            //MainFrame.Children.Add( new Virtualization.Virtualization_Main());
            LoadVirtualizationControl();

        }


        private void LoadVirtualizationControl()
        {
            // Șterge toate controalele din MainFrame, lăsând doar unul singur activ
            MainFrame.Content = null;

            // Creează o instanță a UserControl-ului și îl adaugă în MainFrame
            //var virtualizationControl = new Virtualization_Main();
            //MainFrame.Children.Add(virtualizationControl);
            MainFrame.Navigate(new Virtualization_Main());
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVirtualizationControl();
        }

      




        private void SetariRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Content.ToString() == "SETARI")
                {
                    MainFrame.Navigate(new NivelDoiSetari());
                }
                else if (radioButton.Content.ToString() == "STORAGE")
                {
                    MainFrame.Navigate(new NivelDoiStocare());
                }
                else if (radioButton.Content.ToString() == "CAMERE")
                {
                    // Deschide CameraPlayer ca o nouă fereastră
                    CameraPlayer cameraPlayer = new CameraPlayer();
                    MainFrame.Navigate(cameraPlayer);
                }
            }
        }




       

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /* metode pentru fereastra*/
        /*---------------------------------------------------------------------*/
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimizează fereastra
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized; // Maximizează fereastra
            }
            else
            {
                this.WindowState = WindowState.Normal; // Revine la dimensiunea normală
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Închide fereastra
        }

        private void FileExplorer_Click(object sender, RoutedEventArgs e)
        {
            ExplorerWindow explorerWindow = new ExplorerWindow(); explorerWindow.Show();

        }



        private void Playback_Click(object sender, RoutedEventArgs e)
        {
            CameraPlayer cameraPlayer = new CameraPlayer();
            // Check if CameraPlayer is already displayed
            if (MainFrame.Content is CameraPlayer)
            {
                return;
                // Exit if CameraPlayer is already displayed
            }
            // Display CameraPlayer in the Frame
            MainFrame.Navigate(cameraPlayer);
        }

        
            private void ListCameras_Click(object sender, RoutedEventArgs e)
        {
            CameraListControl cameralist = new CameraListControl();
            // Check if CameraPlayer is already displayed
            if (MainFrame.Content is CameraListControl)
            {
                return;
                // Exit if CameraPlayer is already displayed
            }
            // Display CameraPlayer in the Frame
            MainFrame.Navigate(cameralist);
        }


        private void VirtualPart_Click(object sender, RoutedEventArgs e)
        {
            // Verificați dacă Virtualization_Main este deja afișat
            if (MainFrame.Content is Virtualization. Virtualization_Main)
            {
                return; // Ieșiți dacă Virtualization_Main este deja afișat
            }

            // Inițializați și afișați Virtualization_Main în cadrul principal
            Virtualization. Virtualization_Main virtualMain = new Virtualization.Virtualization_Main();
            MainFrame.Navigate(virtualMain);
        }



        //--------------Gestionare lista camere----------------------------------

        // Eveniment pentru butonul de adaugare camera
        private void AddCameraButton_Click(object sender, RoutedEventArgs e)
        {
            // apelam metoda AddCamera din clasa AddCameras
            _addCameras.AddCamera();
        }
        public void DisplayControl(UserControl control)
        {

            MainFrame.Content = null ; // Elimină orice control existent
            MainFrame.Navigate(control); // Adaugă noul UserControl
        }


        //-------------End-Gestionare lista camere-------------------------------



        /*----------------------------------------------------------------------*/

        public static void NavigateToMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


    }
}