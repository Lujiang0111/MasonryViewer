using MasonryViewer.Common;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        private ObservableCollection<UImage> uImages = new ObservableCollection<UImage>();
        public ObservableCollection<UImage> UImages
        {
            get { return uImages; }
            set { SetProperty(ref uImages, value); }
        }

        public DelegateCommand OpenImageFolderCommand { get; set; } = null;
        public DelegateCommand RefreshCommand { get; set; } = null;
        public DelegateCommand OpenSettingsFlyoutCommand { get; set; } = null;

        private string imageFolderPath = "";
        private List<string> images = new List<string>();
        private int imageNextShowIndex = 0;

        private int imagePanelWidth = 0;

        public MainWindowViewModel()
        {
            Title = string.Format("{0} v{1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            OpenImageFolderCommand = new DelegateCommand(OpenImageFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            OpenSettingsFlyoutCommand = new DelegateCommand(OpenSettingsFlyout);

            Refresh();
        }

        public void OnImagePanelSizeChanged(double width)
        {
            imagePanelWidth = (int)width;
        }

        public void OnImageCntPerLineChanged(double value)
        {
            ImageCntPerLine = (int)value;
            Refresh();
        }

        public void ShowMoreImage()
        {
            for (int index = 0; index < ImageCntPerLine; ++index)
            {
                if (imageNextShowIndex >= images.Count)
                {
                    return;
                }

                UImage uImage = new UImage
                {
                    Path = images[imageNextShowIndex],
                    Width = imagePanelWidth / ImageCntPerLine - 12
                };
                UImages.Add(uImage);

                ++imageNextShowIndex;
            };
        }

        private void OpenImageFolder()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Select Image Folder";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                imageFolderPath = dialog.SelectedPath;
                images = Directory.EnumerateFiles(imageFolderPath, "*.*", SearchOption.AllDirectories).
                    Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".png") ||
                    s.EndsWith(".tif") || s.EndsWith(".tiff") || s.EndsWith(".gif")).ToList();
                Refresh();
            }
        }

        private void OpenSettingsFlyout()
        {
            IsSettingsFlyoutOpen = !IsSettingsFlyoutOpen;
        }
        private void Refresh()
        {
            UImages.Clear();
            imageNextShowIndex = 0;
            ShowMoreImage();
        }

    }
}
