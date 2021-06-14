using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public class ImagePlus3 : BaseImage3
    {
        // Using a DependencyProperty as the backing store for Image. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapImage), typeof(ImagePlus3), new PropertyMetadata(null, propertyChanged));
        
        protected BitmapImage SourceImage;

        public BitmapImage Image
        {
            get { return SourceImage; }
            set
            {
                SourceImage = value;
                imageViewer.Source = SourceImage;
            }

        }

        private  static void propertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePlus3 imagePlus)
            {
                imagePlus.Image = e.NewValue as BitmapImage;
            }
        }
    }
}