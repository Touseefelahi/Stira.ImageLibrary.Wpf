using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public class ImagePlus : BaseImage
    {
        // Using a DependencyProperty as the backing store for Image. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(WriteableBitmap), typeof(ImagePlus), new PropertyMetadata(null, propertyChanged));

        public WriteableBitmap Image
        {
            get { return SourceImage; }
            set
            {
                SourceImage = value;
                ImageViewer.Source = SourceImage;
            }
        }

        private static void propertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePlus imagePlus)
            {
                imagePlus.Image = e.NewValue as WriteableBitmap;
                imagePlus.RectBitmap = new Int32Rect(0, 0, (int)imagePlus.Image.Width, (int)imagePlus.Image.Height);
            }
        }
    }
}