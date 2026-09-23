using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Management.Deployment.Preview;

namespace PlantTracker.Wpf
{
    public static class PhotoStorage
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PlantTracker", "Photos");

        public static string Save(string sourcePath)
        {
            Directory.CreateDirectory(Folder);
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourcePath)}";
            string destination = Path.Combine(Folder, fileName);
            File.Copy(sourcePath, destination);
            return destination;
        }
        public static void Delete(string? path)
        {
            // solo borra archivos de mi carpeta, x si las moscas
            if (string.IsNullOrEmpty(path) || !path.StartsWith(Folder) || !File.Exists(path))
                return;

            try { File.Delete(path); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
