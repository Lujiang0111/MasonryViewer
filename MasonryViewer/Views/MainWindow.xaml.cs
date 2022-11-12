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
        private Point imageStartPoint = new Point();
        private ImageViewer imageViewer = null;
        private double scrollViewerWidth = -1;
        public MainWindow()
        {
            Closing += MainWindow_Closing;

            InitializeComponent();

            var vm = DataContext as MainWindowViewModel;
            Refresh();

            var ImageCntPerLineSlider = FindName("ImageCntPerLineSlider") as Slider;
            ImageCntPerLineSlider.Value = SettingManager.Instance.ImageCntPerLine;

            var MouseWheelPrecisionSlider = FindName("MouseWheelPrecisionSlider") as Slider;
            MouseWheelPrecisionSlider.Value = SettingManager.Instance.MouseWheelPrecision;
            MouseWheelPrecisionSlider.Ticks.Clear();
            for (int i = (int)MouseWheelPrecisionSlider.Minimum + 1; i < (int)MouseWheelPrecisionSlider.Maximum; ++i)
            {
                MouseWheelPrecisionSlider.Ticks.Add(i);
            }
            MouseWheelPrecisionSlider.ValueChanged += MouseWheelPrecisionSlider_ValueChanged;
        }

        public void ScrollToImage(int imageIndex)
        {
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;

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

            int vcerticalAreaCnt;
            do
            {
                vcerticalAreaCnt = (vm.NextShowImageIndex - 1) / SettingManager.Instance.ImageCntPerLine;
                double vcerticalOffset = (vcerticalAreaCnt > 0)
                    ? (scrollViewer.ScrollableHeight / vcerticalAreaCnt * (imageIndex / SettingManager.Instance.ImageCntPerLine))
                    : 0;
                scrollViewer.ScrollToVerticalOffset(vcerticalOffset);
                UpdateLayout();
            } while ((vm.NextShowImageIndex - 1) / SettingManager.Instance.ImageCntPerLine != vcerticalAreaCnt);

            vm.SelectImage(imageIndex);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
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
                int vcerticalAreaCnt = (vm.NextShowImageIndex - 1) / SettingManager.Instance.ImageCntPerLine;
                double vcerticalOffsetTh = (vcerticalAreaCnt > 0)
                    ? (scrollViewer.ScrollableHeight / vcerticalAreaCnt * (vcerticalAreaCnt - 1))
                    : 0;

                if (scrollViewer.VerticalOffset >= vcerticalOffsetTh)
                {
                    ret = vm.ShowMoreImage();
                }
            }
            return ret;
        }

        private void Refresh()
        {
            var vm = DataContext as MainWindowViewModel;
            vm.Refresh();

            var scrollViewer = FindName("ImageScrollViewer") as ScrollViewer;
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToTop();
            }

            ShowMoreImage();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void OpenImageFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.OpenImageFolder();
            Refresh();
        }

        private void ImageScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var scrollViewer = sender as ScrollViewer;
            ScrollContentPresenter content = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            vm.OnImagePanelSizeChanged(content.ActualWidth);

            if (scrollViewerWidth <= 0)
            {
                scrollViewerWidth = e.NewSize.Width;
            }
            else if (Math.Abs(e.NewSize.Width - scrollViewerWidth) > 1)
            {
                double newVerticalOffset = scrollViewer.VerticalOffset * e.NewSize.Width / scrollViewerWidth;
                scrollViewerWidth = e.NewSize.Width;
                scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            }
        }

        private void ImageBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var border = sender as Border;
            var uImage = border.DataContext as UImage;

            imageStartPoint = e.GetPosition(null);
            if (1 == e.ClickCount)
            {
                vm.SelectImage(uImage.Index);
            }
            else if (e.ClickCount > 1)
            {
                ShowImageViewer(uImage.Index);
            }
        }

        private void ImageBorder_MouseMove(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var uImage = border.DataContext as UImage;

            Vector diff = imageStartPoint - e.GetPosition(null);
            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                string[] files = { uImage.Path };
                DataObject dataObject = new DataObject(DataFormats.FileDrop, files);
                DragDrop.DoDragDrop(border, dataObject, DragDropEffects.Copy);
            }
        }

        private void ImageCntPerLineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var slider = sender as Slider;
            if ((int)slider.Value != SettingManager.Instance.ImageCntPerLine)
            {
                SettingManager.Instance.SetImageCntPerLine((int)slider.Value);
                int selectedImageIndex = vm.SelectedImageIndex;
                Refresh();
                ScrollToImage(selectedImageIndex);
            }
        }

        private void Border_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            var border = sender as Border;
            var uImage = border.DataContext as UImage;
            vm.SelectImage(uImage.Index);
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (Key.Up == e.Key)
            {
                ScrollToImage(vm.SelectedImageIndex - SettingManager.Instance.ImageCntPerLine);
            }
            else if (Key.Down == e.Key)
            {
                ScrollToImage(vm.SelectedImageIndex + SettingManager.Instance.ImageCntPerLine);
            }
            else if (Key.Left == e.Key)
            {
                ScrollToImage(vm.SelectedImageIndex - 1);
            }
            else if (Key.Right == e.Key)
            {
                ScrollToImage(vm.SelectedImageIndex + 1);
            }
            else if ((Key.Enter == e.Key) || (Key.Space == e.Key))
            {
                ShowImageViewer(vm.SelectedImageIndex);
            }
        }

        private void ShowImageViewer(int imageIndex)
        {
            if ((imageIndex < 0) || (imageIndex >= ImageManager.Instance.ImagePaths.Count))
            {
                return;
            }

            if (null == imageViewer)
            {
                imageViewer = new ImageViewer();
                imageViewer.ParentWindow = this;
            }

            imageViewer.SetImage(imageIndex);

            if (!imageViewer.IsVisible)
            {
                imageViewer.Show();
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => imageViewer.Focus()));
        }

        private void ImageScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void ImageScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            e.Handled = true;
            if (e.Delta > 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta * SettingManager.Instance.MouseWheelPrecision);
            }
            else if (e.Delta < 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta * SettingManager.Instance.MouseWheelPrecision);
            }
        }

        private void MouseWheelPrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            SettingManager.Instance.SetMouseWheelPrecision(slider.Value);
        }
    }
}
