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

        private static readonly int minScaleBasis = 1000;
        private static readonly int maxScaleBasis = 50000;

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
            vm.SetScale(((scale > 0) && (scale < 1)) ? (int)(scale * 10000) : (10000));
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
                int scaleBasis = vm.ScaleBasis;
                if (e.Delta > 0)
                {
                    scaleBasis = (scaleBasis / 1000 + 1) * 1000;
                }
                else if (e.Delta < 0)
                {
                    scaleBasis = (scaleBasis / 1000 - 1) * 1000;
                }

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
            ParentWindow.ScrollToImage(vm.ImageIndex, true);
            vm.TurnToLoading();
            Hide();
        }
    }
}
