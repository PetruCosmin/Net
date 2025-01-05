using LibVLCSharp.Shared;
using Network_Connection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;

using System.Linq;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LibVLCSharp;

/// <summary>
/// Interaction logic for Add_WifiConnection.xaml
/// Install-Package LibVLCSharp -Version 3.6.1
/// Install-Package Vlc.DotNet.Wpf -Version 3.1.0
/// Install-Package Vlc.DotNet.Core -Version 3.1.0
/// Install-Package LibVLCSharp.WPF -Version 3.6.1




/// </summary>


namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{
    /// <summary>
    /// Interaction logic for Add_WifiConnection.xaml
    /// </summary>
    /// 


    
    public partial class Add_WifiConnection : Page
    {

        public ObservableCollection<WiFiNetwork> WifiNetworks { get; set; }


        private LibVLC _libVLC;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;  // Fully qualified name to avoid ambiguity

        public Add_WifiConnection()
        {
            InitializeComponent();
            Core.Initialize();  // Initialize LibVLC

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;

            var scanner = new Network_Connection.WiFiScanner();
            scanner.ScanForWiFiDevices(WifiNetworks);


            WifiNetworks = new ObservableCollection<WiFiNetwork>();
            this.DataContext = this;  // Setăm DataContext pentru legătura datelor
        }




       

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Path to VLC libraries, adjust this to your setup
            var libVLCPath = @"C:\Users\petru\source\repos\RTSP_Linux\RTSP_Linux\Lib";

            // Initialize VLC with the path to the native libraries
            _libVLC = new LibVLC(new string[] { "--input-fast-seek", "--rtsp-tcp" });
            _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);  // Fully qualified name

            vlcVideoView.MediaPlayer = _mediaPlayer;

            // Start playing the RTSP stream
            var media = new Media(_libVLC, new Uri("rtsp://192.168.1.10:554/user=admin_password=tlJwpbo6_channel=1_stream=1_trackID=1"));
            _mediaPlayer.Play(media);
        }

        public void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up resources when the window is unloaded
            _mediaPlayer?.Dispose();
            _libVLC?.Dispose();
        }

        public void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            var scanner = new WiFiScanner();
            scanner.ScanForWiFiDevices(WifiNetworks);  // Apelează metoda pentru scanarea dispozitivelor WiFi
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Selectăm rețeaua curentă pe baza binding-ului
            var selectedNetwork = WifiNetworks?[0]; // Exemplu: Prima rețea, modifică după cerințe

            if (selectedNetwork == null)
            {
                MessageBox.Show("Please select a WiFi network to connect to.", "No Network Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Connecting to {selectedNetwork.SSID} with signal strength {selectedNetwork.SignalStrength}.");
        }
    }
}
