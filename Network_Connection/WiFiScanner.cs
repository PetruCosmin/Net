using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Connection
{
    public class WiFiScanner
    {
        public void ScanForWiFiDevices(ObservableCollection<WiFiNetwork> wifiNetworks)
        {
            // Clear the existing networks in the collection
            // wifiNetworks.Clear();

            // Run the netsh command to get the list of WiFi networks
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show networks mode=bssid",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();


            // Add the networks to the ObservableCollection

        }


    }
}