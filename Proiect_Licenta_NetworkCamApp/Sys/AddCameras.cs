using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace Proiect_Licenta_NetworkCamApp.Sys
{
    public class AddCameras
    {
        private WrapPanel MainGrid; // Schimbăm numele pentru a reflecta WrapPanel-ul

        // Constructor care primește WrapPanel ca parametru
        public AddCameras(WrapPanel mainGrid)
        {
            MainGrid = mainGrid;
        }

        public void AddCamera()
        {

            Border newCameraBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),  // Gri deschis
                BorderThickness = new Thickness(3),  // Grosime pentru un efect reliefat
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),  // Fundal alb
                Width = 200,  // Setează lățimea
                Height = 90,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(5),  // Colțuri rotunjite
                Effect = new DropShadowEffect  // Efect de umbră pentru profunzime
                {
                    ShadowDepth = 4,
                    Color = Colors.Gray,
                    Opacity = 0.5
                }
            };

            // Conținutul camerei (TextBlock și Buton de închidere)
            Grid cameraContent = new Grid();
            TextBlock cameraTitle = new TextBlock
            {
                Text = $"Camera {GetNextCameraNumber()}",
                Foreground = Brushes.Black,
                FontSize = 18,
                Margin = new Thickness(10, 5, 0, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            Button closeButton = new Button
            {
                FontSize = 10,
                Content = "Close",
                Width = 70,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 5, 0)
            };

            closeButton.Click += (s, e) => MainGrid.Children.Remove(newCameraBorder);

            // Adaugă elementele în grila camerei
            cameraContent.Children.Add(cameraTitle);
            cameraContent.Children.Add(closeButton);

            // Asociază grila cu containerul noii camere
            newCameraBorder.Child = cameraContent;

            // Adaugă noua cameră în WrapPanel
            MainGrid.Children.Add(newCameraBorder);
        }


        private int GetNextCameraNumber()
        {
            return MainGrid.Children.OfType<Border>().Count() + 1;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = sender as Button;
            Border cameraToRemove = closeButton?.Tag as Border;

            if (cameraToRemove != null)
            {
                MainGrid.Children.Remove(cameraToRemove); // Elimină camera din WrapPanel
            }
        }
    }
}
