using MasonryViewer.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using System.Linq;

namespace MasonryViewer.ViewModels
{
    public class ImageViewerViewModel : BindableBase
    {
        public DelegateCommand CopyImageCommand { get; set; } = null;
        public int ImageIndex { get; private set; } = 0;
        public int ScaleBasis { get; private set; } = 0;

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
            private set { SetProperty(ref scale, value); }
        }

        public void SetScale(int scaleBasis)
        {
            ScaleBasis = scaleBasis;
            Scale = (double)ScaleBasis / 10000;
        }

        public ImageViewerViewModel()
        {
            Title = "Image Viewer";
            CopyImageCommand = new DelegateCommand(CopyImage);
            TurnToLoading();
        }

        public bool SetImageIndex(int imageIndex)
        {
            if ((imageIndex < 0) || (imageIndex >= ImageManager.Instance.ImagePaths.Count))
            {
                return false;
            }

            if (!ImageManager.Instance.TryLoadImage(imageIndex))
            {
                return false;
            }

            ImageIndex = imageIndex;
            ImagePath = ImageManager.Instance.ImagePaths[ImageIndex];
            char[] delimiterChars = { '/', '\\' };
            Title = ImagePath.Split(delimiterChars).LastOrDefault();
            return true;
        }

        public void TurnToLoading()
        {
            ImagePath = ImageManager.Instance.LoadingImage;
        }

        private void CopyImage()
        {
            System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection
            {
                ImagePath
            };
            System.Windows.Forms.Clipboard.SetFileDropList(paths);
        }
    }
}
