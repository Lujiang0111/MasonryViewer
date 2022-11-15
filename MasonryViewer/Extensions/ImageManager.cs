using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MasonryViewer.Extensions
{
    public class ImageManager : BindableBase
    {
        public string LoadingImage { get; set; } = "pack://application:,,,/Resources/Loading.png";
        public string ImageFolderPath { get; set; } = "";
        public List<string> ImagePaths { get; set; } = new List<string>();

        private Thickness borderThickness = new Thickness(2);
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set { SetProperty(ref borderThickness, value); }
        }

        private Thickness margin = new Thickness(5);
        public Thickness Margin
        {
            get { return margin; }
            set { SetProperty(ref margin, value); }
        }

        public bool TryLoadImage(int index)
        {
            if ((index < 0) || (index >= ImagePaths.Count))
            {
                return false;
            }

            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.DecodePixelWidth = 1;
                bitmapImage.UriSource = new Uri(ImagePaths[index]);
                bitmapImage.EndInit();

                if ((bitmapImage.DpiX > 0) && (bitmapImage.DpiY > 0) &&
                    ((bitmapImage.DpiX * 10 < bitmapImage.DpiY) || (bitmapImage.DpiY * 10 < bitmapImage.DpiX)))
                {
                    throw new Exception();
                }
            }
            catch
            {
                ImagePaths.RemoveAt(index);
                return false;
            }

            return true;
        }

        public static ImageManager Instance { get; private set; }
        static ImageManager()
        {
            Instance = new ImageManager();
        }
    }
}
