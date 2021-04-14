﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public class ImagePlus : BaseImage2
    {
        // Using a DependencyProperty as the backing store for Image. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapImage), typeof(ImagePlus), new PropertyMetadata(null, propertyChanged));

        public BitmapImage Image
        {
            get { return SourceImage; }
            set
            {
                SourceImage = value;
                imageViewer.Source = SourceImage;
            }
        }

        private static void propertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePlus imagePlus)
            {
                imagePlus.Image = e.NewValue as BitmapImage;
                imagePlus.rectBitmap = new Int32Rect(0, 0, (int)imagePlus.Image.Width, (int)imagePlus.Image.Height);
            }
        }
    }
}