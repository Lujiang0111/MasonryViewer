using MasonryViewer.Common;
using MasonryViewer.Extensions;
using MasonryViewer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MasonryViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private double imageScrollViewerMaxHeight = 0;
        private Point imageStartPoint = new Point();
        private ImageViewer imageViewer = null;

        public MainWindow()
        {
            Closing += MainWindow_Closing;

            InitializeComponent();
            Refresh(true);
        }

        public void ScrollToImageIndex(int imageIndex, bool isSelect)
        {
            var vm = DataContext as MainWindowViewModel;
            if ((imageIndex < 0) || (imageIndex >= ImageManager.Instance.ImagePaths.Count)
                || (vm.NextShowImageIndex <= 0))
            {
                return;
            }

            while (vm.NextShowImageIndex <= imageIndex)
            {
                if (!vm.ShowMoreImage())
                {
                    break;
                }
            }
            UpdateLayout();

            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight /
                (vm.NextShowImageIndex / vm.ImageCntPerLine) * (imageIndex / vm.ImageCntPerLine) +
                scrollViewer.ActualHeight / 2);

            if (isSelect)
            {
                vm.SelectImage(imageIndex);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ImageCntPerLineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.OnImageCntPerLineChanged(e.NewValue);
            Refresh(false);
        }

        private void ImageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ShowMoreImage();
        }

        private bool ShowMoreImage()
        {
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;

            bool ret = false;
            if (null != scrollViewer)
            {
                if (imageScrollViewerMaxHeight > scrollViewer.ScrollableHeight)
                {
                    imageScrollViewerMaxHeight = scrollViewer.ScrollableHeight;
                }

                if (scrollViewer.VerticalOffset >= imageScrollViewerMaxHeight)
                {
                    imageScrollViewerMaxHeight = scrollViewer.ScrollableHeight;
                    ret = vm.ShowMoreImage();
                }
            }
            return ret;
        }

        private void Refresh(bool isResetLastSelectedImageIndex)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.Refresh(isResetLastSelectedImageIndex);

            imageScrollViewerMaxHeight = 0;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToTop();
            }

            ShowMoreImage();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh(true);
        }

        private void OpenImageFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.OpenImageFolder();
            Refresh(true);
        }

        private void ImageScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = sender as ScrollViewer;
            ScrollContentPresenter content = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            vm.OnImagePanelSizeChanged(content.ActualWidth);

            double newVerticalOffset = (e.PreviousSize.Width > 0) ? (scrollViewer.VerticalOffset * e.NewSize.Width / e.PreviousSize.Width) : (0);
            scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imageStartPoint = e.GetPosition(null);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            var uImage = image.DataContext as UImage;

            var mpos = e.GetPosition(null);
            Vector diff = imageStartPoint - mpos;
            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                string[] files = { uImage.Path };
                DataObject dataObject = new DataObject(DataFormats.FileDrop, files);
                DragDrop.DoDragDrop(image, dataObject, DragDropEffects.Copy);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var image = sender as Image;
            var uImage = image.DataContext as UImage;
            if (1 == e.ClickCount)
            {
                vm.SelectImage(uImage.Index);
            }
            else if (e.ClickCount > 1)
            {
                if (null == imageViewer)
                {
                    imageViewer = new ImageViewer();
                    imageViewer.ParentWindow = this;
                }

                imageViewer.SetImage(uImage.Index);

                if (!imageViewer.IsActive)
                {
                    imageViewer.Show();
                }
            }
        }
    }
}
