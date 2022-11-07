using MasonryViewer.ViewModels;

namespace MasonryViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImagePanel_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.OnImagePanelSizeChanged(e.NewSize.Width);
        }

        private void ImageCntPerLineSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.OnImageCntPerLineChanged(e.NewValue);
        }

        private void ImageScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = sender as System.Windows.Controls.ScrollViewer;
            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)
            {
                vm.ShowMoreImage();
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
            }
        }
    }
}
