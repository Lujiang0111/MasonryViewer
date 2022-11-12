using MasonryViewer.Common;
using MasonryViewer.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace MasonryViewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand OpenSettingsFlyoutCommand { get; set; } = null;
        public DelegateCommand<UImage> CopyImageCommand { get; set; } = null;
        public int NextShowImageIndex { get; private set; } = 0;
        public int SelectedImageIndex { get; set; } = -1;

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

        private ObservableCollection<UImage> uImages = new ObservableCollection<UImage>();
        public ObservableCollection<UImage> UImages
        {
            get { return uImages; }
            set { SetProperty(ref uImages, value); }
        }

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceCaps", SetLastError = true)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private int imagePanelWidth = 0;
        private int screenDpiWidth = 0;

        public MainWindowViewModel()
        {
            Title = string.Format("{0} v{1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            OpenSettingsFlyoutCommand = new DelegateCommand(OpenSettingsFlyout);
            CopyImageCommand = new DelegateCommand<UImage>(CopyImage);

            var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            screenDpiWidth = GetDeviceCaps(desktop, 118);
        }

        public void OpenImageFolder()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Select Image Folder";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                ImageManager.Instance.ImageFolderPath = dialog.SelectedPath;
                ImageManager.Instance.ImagePaths = Directory.EnumerateFiles(ImageManager.Instance.ImageFolderPath, "*.*", SearchOption.AllDirectories).
                    Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".png") ||
                    s.EndsWith(".tif") || s.EndsWith(".tiff") || s.EndsWith(".gif")).ToList();
                ImageManager.Instance.ImagePaths.Sort();

                char[] delimiterChars = { '/', '\\' };
                Title = ImageManager.Instance.ImageFolderPath.Split(delimiterChars).LastOrDefault();
            }
        }

        public void OnImagePanelSizeChanged(double width)
        {
            imagePanelWidth = (int)width;
            for (int i = 0; i < UImages.Count; ++i)
            {
                UImages[i].Width = CalculateImageWidth();
            }
        }

        public bool ShowMoreImage()
        {
            bool ret = false;
            int newImageCnt = 0;
            while (newImageCnt < SettingManager.Instance.ImageCntPerLine)
            {
                if ((NextShowImageIndex < 0) || (NextShowImageIndex >= ImageManager.Instance.ImagePaths.Count))
                {
                    break;
                }

                if (!ImageManager.Instance.TryLoadImage(NextShowImageIndex))
                {
                    continue;
                }

                UImage uImage = new UImage(NextShowImageIndex)
                {
                    Path = ImageManager.Instance.ImagePaths[NextShowImageIndex],
                    Width = CalculateImageWidth(),
                    DecodeWidth = screenDpiWidth / SettingManager.Instance.ImageCntPerLine
                };
                UImages.Add(uImage);

                ++newImageCnt;
                ++NextShowImageIndex;
                ret = true;
            };
            return ret;
        }

        public void Refresh()
        {
            UImages.Clear();
            NextShowImageIndex = 0;
            SelectedImageIndex = -1;
        }

        public void SelectImage(int index)
        {
            if ((SelectedImageIndex >= 0) && (SelectedImageIndex < UImages.Count))
            {
                UImages[SelectedImageIndex].BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            if ((index >= 0) && (index < UImages.Count))
            {
                UImages[index].BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b47cff"));
                SelectedImageIndex = index;
            }
        }

        private int CalculateImageWidth()
        {
            return imagePanelWidth / SettingManager.Instance.ImageCntPerLine - (int)(ImageManager.Instance.Margin.Left + ImageManager.Instance.BorderThickness.Left) * 2;
        }

        private void OpenSettingsFlyout()
        {
            IsSettingsFlyoutOpen = !IsSettingsFlyoutOpen;
        }

        private void CopyImage(UImage uImage)
        {
            System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection
            {
                uImage.Path
            };
            System.Windows.Forms.Clipboard.SetFileDropList(paths);
        }
    }
}
