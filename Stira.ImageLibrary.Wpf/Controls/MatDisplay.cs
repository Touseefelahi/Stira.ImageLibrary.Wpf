using Emgu.CV;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    /// <summary>
    /// Image for Opencv Mat from EMGU library
    /// </summary>
    public class MatDisplay : BaseImage
    {
        // Using a DependencyProperty as the backing store for Image. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(Mat), typeof(MatDisplay), new PropertyMetadata(null));

        /// <summary>
        /// Frame counter is actually begin used to update the frame from rawbytes array
        /// </summary>
        public static readonly DependencyProperty FrameCounterProperty =
            DependencyProperty.Register("FrameCounter", typeof(int), typeof(MatDisplay), new PropertyMetadata(0, MatUpdated));

        public Mat Image
        {
            get => (Mat)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public int FrameCounter
        {
            get => (int)GetValue(FrameCounterProperty);
            set => SetValue(FrameCounterProperty, value);
        }

        private static void MatUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatDisplay matImage)
            {
                matImage.ShowImage();
            }
        }

        private void ShowImage()
        {
            if (Image.Width != RectBitmap.Width || Image.Height != RectBitmap.Height || NumberOfChannels != Image.NumberOfChannels)
            {
                SetupImage();
            }
            SourceImage.WritePixels(RectBitmap,
                Image.DataPointer, //Buffer
                Image.Width * Image.NumberOfChannels * Image.Height, //Total buffer size
                Image.Width * Image.NumberOfChannels); //Stride
            ImageViewer.Source = SourceImage;
        }

        private void SetupImage()
        {
            if (Image.Width != 0 && Image.Height != 0)
            {
                const double dpi = 96;
                RectBitmap = new Int32Rect(0, 0, Image.Width, Image.Height);
                List<Color> colors = new List<Color> { Colors.Gray };
                BitmapPalette myPalette;
                NumberOfChannels = Image.NumberOfChannels;
                PixelFormat format = PixelFormats.Gray8;
                if (Image.NumberOfChannels == 3)
                {
                    colors = new List<Color> { Colors.Blue, Colors.Green, Colors.Red };
                    format = PixelFormats.Bgr24;
                }
                myPalette = new BitmapPalette(colors);
                SourceImage = new WriteableBitmap(Image.Width, Image.Height, dpi, dpi, format, myPalette);
            }
        }
    }
}