using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Connection
{
    public class WiFiNetwork
    {

        public string SSID { get; set; }
        public string BSSID { get; set; }
        public string SignalStrength { get; set; }
        public string RadioType { get; set; }   // Tipul de radio (802.11g/n/ac)
        public string Frequency { get; set; }   // Frecvența radio (2.4 GHz / 5 GHz)


        // Metodă pentru a parsa ieșirea comenzii netsh și a crea obiecte WiFiNetwork
        public List<WiFiNetwork> ParseWiFiNetworks(string output)
        {
            var networks = new List<WiFiNetwork>();
            var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            WiFiNetwork currentNetwork = null;

            foreach (var line in lines)
            {
                if (line.Contains("SSID"))
                {
                    currentNetwork = new WiFiNetwork();
                    currentNetwork.SSID = line.Split(new[] { " : " }, StringSplitOptions.None)[1].Trim();
                    networks.Add(currentNetwork);
                }
                else if (line.Contains("Signal") && currentNetwork != null)
                {
                    currentNetwork.SignalStrength = line.Split(new[] { " : " }, StringSplitOptions.None)[1].Trim();
                }
                else if (line.Contains("Radio type") && currentNetwork != null)
                {
                    string radioType = line.Split(new[] { " : " }, StringSplitOptions.None)[1].Trim();
                    currentNetwork.RadioType = radioType;
                    currentNetwork.Frequency = GetFrequencyByRadioType(radioType);
                }
            }

            return networks;
        }

        public string GetFrequencyByRadioType(string radioType)
        {
            switch (radioType)
            {
                case "802.11g":
                    return "2.4 GHz";
                case "802.11n":
                    return "2.4 GHz / 5 GHz";
                case "802.11ac":
                    return "5 GHz";
                default:
                    return "Unknown";
            }
        }


    }
}
