#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#endregion

#endregion

namespace AutoX
{
    /// <summary>
    ///   Interaction logic for ImageDialog.xaml
    /// </summary>
    public sealed partial class ImageDialog : IDisposable
    {
        private MemoryStream _streamSource;

        public ImageDialog()
        {
            InitializeComponent();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_streamSource != null)
                _streamSource.Dispose();
        }

        #endregion

        public void SetImage(string value)
        {
            vImage = GetImage(value);
        }

        public void SetFile(string fileName)
        {
            _streamSource = new MemoryStream(File.ReadAllBytes(fileName));
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = _streamSource;
            bitmap.EndInit();
            vImage.Source = bitmap;

            //var bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(fileName);
            //bitmap.EndInit();
            //vImage.Source = bitmap;
        }

        private static Image GetImage(string binary64)
        {
            var buffer = Convert.FromBase64String(binary64);
            return GetImage(buffer);
        }

        private static Image GetImage(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            var bImage = new BitmapImage();
            bImage.BeginInit();
            bImage.StreamSource = ms;
            bImage.EndInit();
            var retImage = new Image {Source = bImage};
            ms.Close();

            return retImage;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (_streamSource != null)
            {
                _streamSource.Close();
            }
        }
    }
}