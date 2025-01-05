using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Proiect_Licenta_NetworkCamApp.View.SetariView;

using Proiect_Licenta_NetworkCamApp.View;
using Proiect_Licenta_NetworkCamApp.Sys;

namespace Proiect_Licenta_NetworkCamApp.Sys
{
    public class MyViewModel : ObservableObject
    {

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // Commands
        public ICommand SwitchToCameraCommand { get; }
        public ICommand SwitchToSetariCommand { get; }
        public ICommand SwitchToRecordCommand { get; }
        public ICommand SwitchToStorageCommand { get; }

        // Views (UserControls)
        public SetariView SetariView { get; }
        public RecordView RecordView { get; }
        public StorageView StorageView { get; }

        public MyViewModel()
        {
            // Instantiate the views
            SetariView = new SetariView();
            RecordView = new RecordView();
            StorageView = new StorageView();

            // Set the default view
            CurrentView = SetariView;

            // Instantiate the commands
            SwitchToSetariCommand = new RelayCommand(o => CurrentView = SetariView);
            SwitchToRecordCommand = new RelayCommand(o => CurrentView = RecordView);
            SwitchToStorageCommand = new RelayCommand(o => CurrentView = StorageView);
            //SwitchToSetariCommand = new RelayCommand(o => CurrentView = SetariView, o => CurrentView != SetariView);
        }
    }
}
