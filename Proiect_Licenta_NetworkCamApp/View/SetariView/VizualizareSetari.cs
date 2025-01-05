using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{
    public class VizualizareSetari : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Languages
        public ObservableCollection<string> AvailableLanguages { get; set; }
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { _selectedLanguage = value; OnPropertyChanged(nameof(SelectedLanguage)); }
        }

        // Time Zones
        public ObservableCollection<string> AvailableTimeZones { get; set; }
        private string _selectedTimeZone;
        public string SelectedTimeZone
        {
            get { return _selectedTimeZone; }
            set { _selectedTimeZone = value; OnPropertyChanged(nameof(SelectedTimeZone)); }
        }

        // Date Formats
        public ObservableCollection<string> AvailableDateFormats { get; set; }
        private string _selectedDateFormat;
        public string SelectedDateFormat
        {
            get { return _selectedDateFormat; }
            set { _selectedDateFormat = value; OnPropertyChanged(nameof(SelectedDateFormat)); }
        }

        // System Date and Time
        private string _systemDate;
        public string SystemDate
        {
            get { return _systemDate; }
            set { _systemDate = value; OnPropertyChanged(nameof(SystemDate)); }
        }

        private string _systemTime;
        public string SystemTime
        {
            get { return _systemTime; }
            set { _systemTime = value; OnPropertyChanged(nameof(SystemTime)); }
        }

        // Device Name and Number
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; OnPropertyChanged(nameof(DeviceName)); }
        }

        private int _deviceNo;
        public int DeviceNo
        {
            get { return _deviceNo; }
            set { _deviceNo = value; OnPropertyChanged(nameof(DeviceNo)); }
        }

        // Auto Log Out
        private int _autoLogOut;
        public int AutoLogOut
        {
            get { return _autoLogOut; }
            set { _autoLogOut = value; OnPropertyChanged(nameof(AutoLogOut)); }
        }

        // Menu Output Mode
        public ObservableCollection<string> AvailableMenuOutputModes { get; set; }
        private string _selectedMenuOutputMode;
        public string SelectedMenuOutputMode
        {
            get { return _selectedMenuOutputMode; }
            set { _selectedMenuOutputMode = value; OnPropertyChanged(nameof(SelectedMenuOutputMode)); }
        }

        // Enable Wizard
        private bool _enableWizard;
        public bool EnableWizard
        {
            get { return _enableWizard; }
            set { _enableWizard = value; OnPropertyChanged(nameof(EnableWizard)); }
        }

        // Enable Password
        private bool _enablePassword;
        public bool EnablePassword
        {
            get { return _enablePassword; }
            set { _enablePassword = value; OnPropertyChanged(nameof(EnablePassword)); }
        }

        public VizualizareSetari()
        {
            // Initialize lists with sample data
            AvailableLanguages = new ObservableCollection<string> { "English", "French", "German" };
            AvailableTimeZones = new ObservableCollection<string> { "GMT+01:00", "GMT+02:00", "GMT+03:00" };
            AvailableDateFormats = new ObservableCollection<string> { "DD-MM-YYYY", "MM-DD-YYYY" };
            AvailableMenuOutputModes = new ObservableCollection<string> { "Auto", "Manual" };

            // Set default values
            SelectedLanguage = "English";
            SelectedTimeZone = "GMT+01:00";
            SelectedDateFormat = "DD-MM-YYYY";
            SystemDate = DateTime.Now.ToString("dd-MM-yyyy");
            SystemTime = DateTime.Now.ToString("HH:mm:ss");
            DeviceName = "Device001";
            DeviceNo = 255;
            AutoLogOut = 5;
            EnableWizard = true;
            EnablePassword = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
