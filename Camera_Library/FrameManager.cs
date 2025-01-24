using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;


namespace Camera_Library
{
   

    public class FrameManager : INotifyPropertyChanged
    {
        private int _currentFrame;

        public int CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (_currentFrame != value)
                {
                    _currentFrame = value;
                    OnPropertyChanged(nameof(CurrentFrame));
                }
            }
        }

        public FrameManager()
        {
            // Inițializăm cadrul curent
            CurrentFrame = 30;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
