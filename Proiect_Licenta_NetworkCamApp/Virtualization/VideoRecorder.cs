using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Management.Automation;
using System.Diagnostics;
using System.Security.Principal;

namespace Proiect_Licenta_NetworkCamApp.Virtualization
{
   
        public class VideoRecorder
        {
            private string storagePath = @"C:\Recordings\"; // Calea către discul virtual montat

            public VideoRecorder()
            {
                // Verifică dacă folderul există; dacă nu, îl creează
                try
                {
                    if (!Directory.Exists(storagePath))
                    {
                        Directory.CreateDirectory(storagePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing storage path: {ex.Message}");
                }
            }

            public void SaveRecording(byte[] videoData, string fileName)
            {
                try
                {
                    if (!Directory.Exists(storagePath))
                    {
                        Console.WriteLine("Storage path not available. Ensure the virtual disk is mounted.");
                        return;
                    }

                    string filePath = System.IO.Path.Combine(storagePath, fileName);
                    File.WriteAllBytes(filePath, videoData);
                    Console.WriteLine($"Video saved at: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save video: {ex.Message}");
                }
            }

        }
    }
