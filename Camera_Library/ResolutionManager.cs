using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;



namespace Camera_Library
{
    
    public class ResolutionManager : INotifyPropertyChanged
    {
        public ObservableCollection<string> Resolutions { get; set; }
        private string _selectedResolution;

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

        public ResolutionManager()
        {
            // Inițializăm rezoluțiile disponibile
            Resolutions = new ObservableCollection<string>
        {
            "1280x720",
            "640x480"
        };

            // Rezoluția implicită
            SelectedResolution = Resolutions[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}