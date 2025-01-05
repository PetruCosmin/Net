using Proiect_Licenta_NetworkCamApp.Sys;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Proiect_Licenta_NetworkCamApp.View.SetariView
{
    /// <summary>
    /// Interaction logic for DatabaseViewer.xaml
    /// </summary>
    public partial class DatabaseViewer : Window
    {
        public DatabaseViewer()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Exemplu de date. În practică, vei încărca datele din baza de date.
            var records = new List<Record>
                    {
                        new Record { ID = 1, FileName = "Recording1.mp4", FileType = "Video", CreationDate = "2024-10-01", FileSize = "500MB" },
                        new Record { ID = 2, FileName = "Image1.jpg", FileType = "Image", CreationDate = "2024-10-02", FileSize = "2MB" }
                    };

            RecordDataGrid.ItemsSource = records;
        }
    }
}
