using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stira.ImageLibrary.Wpf
{
    /// <summary>
    /// Image for Opencv Mat from EMGU library
    /// </summary>
    public class MatDisplay : Image
    {
        // Using a DependencyProperty as the backing store for Image. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(Mat), typeof(MatDisplay), new PropertyMetadata(0));

        /// <summary>
        /// Frame counter is actually beign used to update the frame from rawbytes array
        /// </summary>
        public static readonly DependencyProperty FrameCounterProperty =
            DependencyProperty.Register("FrameCounter", typeof(int), typeof(LightImage), new PropertyMetadata(0, MatUpdated));

        private WriteableBitmap SourceImage;

        private Int32Rect rectBitmap;

        public Mat Image
        {
            get { return (Mat)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
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
            if (Image.Width != rectBitmap.Width || Image.Height != rectBitmap.Height)
            {
                SetupImage();
            }
            SourceImage.WritePixels(rectBitmap, Image.GetData(), Image.Width * Image.NumberOfChannels, 0);
            Source = SourceImage;
        }

        private void SetupImage()
        {
            if (Image.Width != 0 && Image.Height != 0)
            {
                const double dpi = 96;
                rectBitmap = new Int32Rect(0, 0, Image.Width, Image.Height);
                List<Color> colors = new List<Color> { Colors.Gray };
                BitmapPalette myPalette;
                PixelFormat format = PixelFormats.Gray8;
                if (Image.NumberOfChannels == 3)
                {
                    colors = new List<Color> { Colors.Red, Colors.Green, Colors.Blue };
                    format = PixelFormats.Bgr24;
                }
                myPalette = new BitmapPalette(colors);
                SourceImage = new WriteableBitmap(Image.Width, Image.Height, dpi, dpi, format, myPalette);
            }
        }
    }
}