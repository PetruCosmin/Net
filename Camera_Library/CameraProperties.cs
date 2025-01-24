using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Camera_Library
{
    

    public class CameraProperties : INotifyPropertyChanged
    {
        private string _cameraName_Properties;
        private int _usage;
        private string _size;
        private string _rtspUrl;
        private string _userCamera;
        private string _passwordCamera;
        private int _resolutionWidth;
        private int _resolutionHeight;
        private double _frameRate;
        private string _videoCodec;
        private bool _isEnabled;
        private DateTime _lastConnectionTime;
        private int _idCamera;
            
            public string CameraName_Properties
            {
                get => _cameraName_Properties;
                set
                {
                    if (_cameraName_Properties != value)
                    {
                        _cameraName_Properties = value;
                        OnPropertyChanged(nameof(CameraName_Properties));
                    }
                }
            }

          



        public int Usage
        {
            get => _usage;
            set { _usage = value; OnPropertyChanged(nameof(Usage)); }
        }

        public string Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(nameof(Size)); }
        }

        public string RtspUrl
        {
            get => _rtspUrl;
            set { _rtspUrl = value; OnPropertyChanged(nameof(RtspUrl)); }
        }

        public string UserCamera
        {
            get => _userCamera;
            set { _userCamera = value; OnPropertyChanged(nameof(UserCamera)); }
        }

        public string PasswordCamera
        {
            get => _passwordCamera;
            set { _passwordCamera = value; OnPropertyChanged(nameof(PasswordCamera)); }
        }

        public int ResolutionWidth
        {
            get => _resolutionWidth;
            set { _resolutionWidth = value; OnPropertyChanged(nameof(ResolutionWidth)); }
        }

        public int ResolutionHeight
        {
            get => _resolutionHeight;
            set { _resolutionHeight = value; OnPropertyChanged(nameof(ResolutionHeight)); }
        }

        public double FrameRate
        {
            get => _frameRate;
            set { _frameRate = value; OnPropertyChanged(nameof(FrameRate)); }
        }

        public string VideoCodec
        {
            get => _videoCodec;
            set { _videoCodec = value; OnPropertyChanged(nameof(VideoCodec)); }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        public DateTime LastConnectionTime
        {
            get => _lastConnectionTime;
            set { _lastConnectionTime = value; OnPropertyChanged(nameof(LastConnectionTime)); }
        }

        public int IdCamera
        {
            get => _idCamera;
            set { _idCamera = value; OnPropertyChanged(nameof(IdCamera)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
