using MasonryViewer.Common;
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

        public MainWindow()
        {
            InitializeComponent();
            Refresh(true);
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
            bool ret = false;
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
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
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
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
            var scrollViewer = sender as ScrollViewer;
            ScrollContentPresenter content = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            var vm = DataContext as MainWindowViewModel;
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
            Image image = sender as Image;
            UImage uImage = image.DataContext as UImage;

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
    }
}
