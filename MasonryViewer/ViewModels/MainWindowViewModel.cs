using MasonryViewer.Common;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

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

        private ObservableCollection<UImage> uImages = new ObservableCollection<UImage>();
        public ObservableCollection<UImage> UImages
        {
            get { return uImages; }
            set { SetProperty(ref uImages, value); }
        }

        public DelegateCommand OpenSettingsFlyoutCommand { get; set; } = null;
        public DelegateCommand<UImage> LeftClickImageCommand { get; set; } = null;
        public DelegateCommand<UImage> LeftDoubleClickImageCommand { get; set; } = null;

        private string imageFolderPath = "";
        private List<string> imagePaths = new List<string>();

        private int nextShowImageIndex = 0;
        private int imagePanelWidth = 0;
        private UImage lastSelectedImage = null;

        public MainWindowViewModel()
        {
            Title = string.Format("{0} v{1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            OpenSettingsFlyoutCommand = new DelegateCommand(OpenSettingsFlyout);
            LeftClickImageCommand = new DelegateCommand<UImage>(LeftClickImage);
            LeftDoubleClickImageCommand = new DelegateCommand<UImage>(LeftDoubleClickImage);
        }

        public void OpenImageFolder()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Select Image Folder";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                imageFolderPath = dialog.SelectedPath;
                imagePaths = Directory.EnumerateFiles(imageFolderPath, "*.*", SearchOption.AllDirectories).
                    Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".png") ||
                    s.EndsWith(".tif") || s.EndsWith(".tiff") || s.EndsWith(".gif")).ToList();
            }
        }

        public void OnImagePanelSizeChanged(double width)
        {
            imagePanelWidth = (int)width;
        }

        public void OnImageCntPerLineChanged(double value)
        {
            ImageCntPerLine = (int)value;
        }

        public bool ShowMoreImage()
        {
            bool ret = false;
            for (int index = 0; index < ImageCntPerLine; ++index)
            {
                if (nextShowImageIndex >= imagePaths.Count)
                {
                    break;
                }

                UImage uImage = new UImage
                {
                    Path = imagePaths[nextShowImageIndex],
                    Width = imagePanelWidth / ImageCntPerLine - (int)(UImage.Margin.Left + UImage.BorderThickness.Left) * 2
                };
                UImages.Add(uImage);

                ret = true;
                ++nextShowImageIndex;
            };
            return ret;
        }

        public void Refresh()
        {
            UImages.Clear();
            nextShowImageIndex = 0;
            lastSelectedImage = null;
            ShowMoreImage();
        }

        private void OpenSettingsFlyout()
        {
            IsSettingsFlyoutOpen = !IsSettingsFlyoutOpen;
        }

        private void LeftClickImage(UImage uImage)
        {
            if (null != lastSelectedImage)
            {
                lastSelectedImage.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
            uImage.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b47cff"));
            lastSelectedImage = uImage;
        }

        private void LeftDoubleClickImage(UImage uImage)
        {

        }
    }
}
