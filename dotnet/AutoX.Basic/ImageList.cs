#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

#endregion

#endregion

namespace AutoX.Basic
{
    public class ImageList
    {
        private static readonly ImageList Instance = new ImageList();
        private readonly Dictionary<string, BitmapImage> _container = new Dictionary<string, BitmapImage>();
        private readonly Dictionary<string, string> _paths = new Dictionary<string, string>();

        private ImageList()
        {
            //load the image from dir resources
            var di = new DirectoryInfo("Resources");
            if (!di.Exists)
                return;
            foreach (FileInfo fi in di.GetFiles("*.bmp"))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = File.OpenRead(fi.FullName);
                bitmap.EndInit();
                bitmap.Freeze();
                _container.Add(fi.Name.ToLower().Substring(0, fi.Name.Length - 4), bitmap);
                _paths.Add(fi.Name.ToLower().Substring(0, fi.Name.Length - 4), fi.FullName);
            }
        }

        public static ImageList GetInstance()
        {
            return Instance;
        }

        public string GetFileName(string name)
        {
            if (_paths.ContainsKey(name))
                return _paths[name];
            return null;
        }

        public BitmapImage Get(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return null;
            if (_container.ContainsKey(imageName.ToLower()))
                return _container[imageName.ToLower()];
            return null;
        }
    }
}