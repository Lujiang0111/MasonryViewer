using MasonryViewer.Extensions;
using Prism.Mvvm;
using System.Linq;

namespace MasonryViewer.ViewModels
{
    public class ImageViewerViewModel : BindableBase
    {
        public int ImageIndex { get; private set; } = 0;

        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string imagePath = "";
        public string ImagePath
        {
            get { return imagePath; }
            set { SetProperty(ref imagePath, value); }
        }

        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set { SetProperty(ref scale, value); }
        }

        public ImageViewerViewModel()
        {
            Title = "Image Viewer";
            TurnToLoading();
        }

        public void SetImageIndex(int imageIndex)
        {
            if ((imageIndex >= 0) && (imageIndex < ImageManager.Instance.ImagePaths.Count))
            {
                ImageIndex = imageIndex;
                ImagePath = ImageManager.Instance.ImagePaths[ImageIndex];
                char[] delimiterChars = { '/', '\\' };
                Title = ImagePath.Split(delimiterChars).LastOrDefault();
            }
        }

        public void TurnToLoading()
        {
            ImagePath = ImageManager.Instance.LoadingImage;
        }
    }
}
