using MasonryViewer.Extensions;
using MasonryViewer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MasonryViewer.Views
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer
    {
        public MainWindow ParentWindow { get; set; } = null;

        private static readonly int minScaleBasis = 1000;
        private static readonly int maxScaleBasis = 50000;

        private Point scrollViewerStartPoint = new Point();
        private double scrollViewerStartHorizontalOffset = 0;
        private double scrollViewerStartVerticalOffset = 0;

        public ImageViewer()
        {
            InitializeComponent();
            Closing += ImageViewer_Closing;
        }

        public bool SetImage(int imageIndex)
        {
            var vm = DataContext as ImageViewerViewModel;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;

            if ((imageIndex < 0) || (imageIndex >= ImageManager.Instance.ImagePaths.Count))
            {
                return false;
            }

            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            vm.TurnToLoading();
            UpdateLayout();

            return vm.SetImageIndex(imageIndex);
        }

        private void ImageViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HideWindow();
            e.Cancel = true;
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            var image = sender as Image;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            ScrollContentPresenter content = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            double scale = Math.Min(content.ActualWidth / image.Source.Width, content.ActualHeight / image.Source.Height);
            vm.SetScale(((scale > 0) && (scale < 1)) ? (int)(scale * 10000) : (10000));
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ToPreviousImage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ToNextImage();
        }

        private void ImageScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var image = FindName("Image") as Image;
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                Point point = e.GetPosition(image);
                double xRatio = (image.ActualWidth > 0) ? (point.X / image.ActualWidth) : 0;
                xRatio = Math.Min(xRatio, 1);
                xRatio = Math.Max(xRatio, 0);

                double yRatio = (image.ActualHeight > 0) ? (point.Y / image.ActualHeight) : 0;
                yRatio = Math.Min(yRatio, 1);
                yRatio = Math.Max(yRatio, 0);

                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    ZoomOut();
                }

                scrollViewer.ScrollToHorizontalOffset(xRatio * scrollViewer.ScrollableWidth);
                scrollViewer.ScrollToVerticalOffset(yRatio * scrollViewer.ScrollableHeight);
            }
            else
            {
                if (e.Delta > 0)
                {
                    ToPreviousImage();
                }
                else if (e.Delta < 0)
                {
                    ToNextImage();
                }
            }
        }

        private void HideWindow()
        {
            var vm = DataContext as ImageViewerViewModel;
            vm.TurnToLoading();
            Hide();
        }

        private void ImageScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as ImageViewerViewModel;
            var scrollViewer = sender as ScrollViewer;
            if (e.ClickCount > 1)
            {
                ParentWindow.ScrollToImage(vm.ImageIndex);
                HideWindow();
            }
            else
            {
                scrollViewerStartPoint = e.GetPosition(scrollViewer);
                scrollViewerStartHorizontalOffset = scrollViewer.HorizontalOffset;
                scrollViewerStartVerticalOffset = scrollViewer.VerticalOffset;
                scrollViewer.CaptureMouse();
            }
        }

        private void ImageScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewerStartHorizontalOffset +
                    (scrollViewerStartPoint.X - e.GetPosition(scrollViewer).X));
                scrollViewer.ScrollToVerticalOffset(scrollViewerStartVerticalOffset +
                    (scrollViewerStartPoint.Y - e.GetPosition(scrollViewer).Y));
            }
        }

        private void ImageScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            scrollViewer.ReleaseMouseCapture();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void ZoomIn()
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            int scaleBasis = vm.ScaleBasis;
            scaleBasis = (scaleBasis / 1000 + 1) * 1000;
            SetScale(scaleBasis);
        }

        private void ZoomOut()
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            int scaleBasis = vm.ScaleBasis;
            scaleBasis = (scaleBasis / 1000 - 1) * 1000;
            SetScale(scaleBasis);
        }

        private void ToPreviousImage()
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            SetImage(vm.ImageIndex - 1);
        }

        private void ToNextImage()
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            SetImage(vm.ImageIndex + 1);
        }

        private void SetScale(int scaleBasis)
        {
            var vm = DataContext as ImageViewerViewModel;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;

            if (scaleBasis < minScaleBasis)
            {
                scaleBasis = minScaleBasis;
            }
            if (scaleBasis > maxScaleBasis)
            {
                scaleBasis = maxScaleBasis;
            }

            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            vm.SetScale(scaleBasis);
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as ImageViewerViewModel;
            if (Key.Up == e.Key)
            {
                ZoomIn();
            }
            else if (Key.Down == e.Key)
            {
                ZoomOut();
            }
            else if (Key.Left == e.Key)
            {
                ToPreviousImage();
            }
            else if ((Key.Right == e.Key) || (Key.Space == e.Key))
            {
                ToNextImage();
            }
            else if (Key.Escape == e.Key)
            {
                HideWindow();
            }
            else if (Key.Enter == e.Key)
            {
                ParentWindow.ScrollToImage(vm.ImageIndex);
                HideWindow();
            }
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            SetImage(vm.ImageIndex);
        }
    }
}
