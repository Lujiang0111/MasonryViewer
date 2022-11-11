using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;

namespace MasonryViewer.Extensions
{
    public class ImageManager : BindableBase
    {
        public string LoadingImage { get; set; } = "pack://application:,,,/Resources/Loading.png";
        public string ImageFolderPath { get; set; } = "";
        public List<string> ImagePaths { get; set; } = new List<string>();

        private Thickness borderThickness = new Thickness(1);
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

        public static ImageManager Instance { get; private set; }
        static ImageManager()
        {
            Instance = new ImageManager();
        }
    }
}
