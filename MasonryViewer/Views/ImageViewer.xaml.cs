using MasonryViewer.ViewModels;
using System;
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

        private static readonly int minScalePercent = 10;
        private static readonly int maxScalePercent = 500;

        public ImageViewer()
        {
            InitializeComponent();
            Closing += ImageViewer_Closing;
        }

        public void SetImage(int imageIndex)
        {
            var vm = DataContext as ImageViewerViewModel;
            vm.SetImageIndex(imageIndex);
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        private void ImageViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HideWindow();
            e.Cancel = true;
        }

        private void Image_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            var image = sender as Image;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            ScrollContentPresenter content = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            double scale = Math.Min(content.ActualWidth / image.Source.Width, content.ActualHeight / image.Source.Height);
            vm.Scale = ((scale > 0) && (scale < 1)) ? (scale) : (1);
        }

        private void PreviousButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            SetImage(vm.ImageIndex - 1);
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            SetImage(vm.ImageIndex + 1);
        }

        private void ImageScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ImageViewerViewModel vm = DataContext as ImageViewerViewModel;
            var scrollViewer = sender as ScrollViewer;
            var image = FindName("Image") as Image;

            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                int scalePercent = (int)(vm.Scale * 100);
                if (e.Delta > 0)
                {
                    scalePercent = (scalePercent / 10 + 1) * 10;
                }
                else if (e.Delta < 0)
                {
                    scalePercent = (scalePercent / 10 - 1) * 10;
                }

                if (scalePercent < minScalePercent)
                {
                    scalePercent = minScalePercent;
                }
                if (scalePercent > maxScalePercent)
                {
                    scalePercent = maxScalePercent;
                }

                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                vm.Scale = (double)scalePercent / 100;
            }
            else
            {
                if (e.Delta > 0)
                {
                    SetImage(vm.ImageIndex - 1);
                }
                else if (e.Delta < 0)
                {
                    SetImage(vm.ImageIndex + 1);
                }
            }
        }

        private void ImageScrollViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HideWindow();
        }

        private void HideWindow()
        {
            var vm = DataContext as ImageViewerViewModel;
            ParentWindow.ScrollToImageIndex(vm.ImageIndex, true);
            vm.TurnToLoading();
            Hide();
        }
    }
}
