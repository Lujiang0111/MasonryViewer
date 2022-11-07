using Prism.Mvvm;

namespace MasonryViewer.Common
{
    public class UImage : BindableBase
    {
        private string path = "";
        public string Path
        {
            get { return path; }
            set { SetProperty(ref path, value); }
        }

        private int width = 0;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }
    }
}
