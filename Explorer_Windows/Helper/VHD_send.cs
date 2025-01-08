using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.Helper
{
    public class VHD_send
    {
        
        public static void Primeste_VHD(string currentPath)
        {
            MessageBox.Show("Metoda Selectata!");

            if (string.IsNullOrEmpty(currentPath))
            {
                MessageBox.Show("Calea fișierului este invalidă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Adaugă logica necesară pentru a procesa fișierul selectat
            MessageBox.Show($"Calea fișierului VHD primită: {currentPath}", "Primire VHD", MessageBoxButton.OK, MessageBoxImage.Information);

            // Exemplu: Poți folosi această cale pentru alte operațiuni
            // De exemplu, montarea unui VHD:
            // MountVHD(filePath); // Implementare separată pentru montare
        }
    }
}
