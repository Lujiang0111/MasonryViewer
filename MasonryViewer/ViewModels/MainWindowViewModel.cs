using Prism.Commands;
using Prism.Mvvm;
using System.Reflection;

namespace MasonryViewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool isSettingsFlyoutOpen = false;
        public bool IsSettingsFlyoutOpen
        {
            get { return isSettingsFlyoutOpen; }
            set { SetProperty(ref isSettingsFlyoutOpen, value); }
        }

        private int imageCntPerLine = 4;
        public int ImageCntPerLine
        {
            get { return imageCntPerLine; }
            set { SetProperty(ref imageCntPerLine, value); }
        }

        public DelegateCommand OpenSettingsFlyoutCommand { get; set; } = null;

        public MainWindowViewModel()
        {
            Title = string.Format("{0} v{1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            OpenSettingsFlyoutCommand = new DelegateCommand(OpenSettingsFlyout);
        }

        private void OpenSettingsFlyout()
        {
            IsSettingsFlyoutOpen = !IsSettingsFlyoutOpen;
        }
    }
}
