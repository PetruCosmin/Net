using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Camera_;



namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{

        public class StorageViewModel : INotifyPropertyChanged
        {
            private string usedStorage;
            private string freeStorage;
            private int cameraStorageUsage;
            private ObservableCollection<CameraProperties> cameras;

            public string UsedStorage
            {
                get { return usedStorage; }
                set
                {
                    usedStorage = value;
                    OnPropertyChanged("UsedStorage");
                }
            }

            public string FreeStorage
            {
                get { return freeStorage; }
                set
                {
                    freeStorage = value;
                    OnPropertyChanged("FreeStorage");
                }
            }

            public int CameraStorageUsage
            {
                get { return cameraStorageUsage; }
                set
                {
                    cameraStorageUsage = value;
                    OnPropertyChanged("CameraStorageUsage");
                }
            }

            public ObservableCollection<CameraProperties> Cameras
            {
                get { return cameras; }
                set
                {
                    cameras = value;
                    OnPropertyChanged("Cameras");
                }
            }

            public StorageViewModel()
            {
                // Initializare cu valori reale
                UsedStorage = "Used: 462 GB";
                FreeStorage = "Free: 491 GB";
                CameraStorageUsage = 60;

                Cameras = new ObservableCollection<CameraProperties>
            {
                new CameraProperties { Name = "Camera 1", Usage = 50, Size = "1 GB" },
                new CameraProperties { Name = "Camera 2", Usage = 70, Size = "1 GB" },
                new CameraProperties { Name = "Camera 3", Usage = 80, Size = "1 GB" }
            };
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }



