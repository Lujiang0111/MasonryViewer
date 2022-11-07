﻿using Prism.Mvvm;
using System.Windows;
using System.Windows.Media;

namespace MasonryViewer.Common
{
    public class UImage : BindableBase
    {
        static public Thickness BorderThickness { get; set; } = new Thickness(1);
        static public Thickness Margin { get; set; } = new Thickness(5);

        private string path = "";
        public string Path
        {
            get { return path; }
            set { SetProperty(ref path, value); }
        }

        private int width = 0;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private SolidColorBrush borderBrush = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush BorderBrush
        {
            get { return borderBrush; }
            set { SetProperty(ref borderBrush, value); }
        }
    }
}
