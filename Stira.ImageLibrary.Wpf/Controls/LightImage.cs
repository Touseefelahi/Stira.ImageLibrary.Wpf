using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    /// <summary>
    /// Image for Raw Bytes
    /// </summary>
    public class LightImage : BaseImage
    {
        // Using a DependencyProperty as the backing store for ImagePtr. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty ImagePtrProperty =
            DependencyProperty.Register("ImagePtr", typeof(IntPtr), typeof(LightImage), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Height. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty HeightImageProperty =
            DependencyProperty.Register("HeightImage", typeof(int), typeof(LightImage), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for Width. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty WidthImageProperty =
            DependencyProperty.Register("WidthImage", typeof(int), typeof(LightImage), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for IsColored. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty IsColoredProperty =
            DependencyProperty.Register("IsColored", typeof(bool), typeof(LightImage), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for RawBytes. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty RawBytesProperty =
            DependencyProperty.Register("RawBytes", typeof(byte[]), typeof(LightImage), new PropertyMetadata(null));

        public byte[] RawBytes
        {
            get => (byte[])GetValue(RawBytesProperty);
            set
            {
                SetValue(RawBytesProperty, value);
                SourceImage.WritePixels(RectBitmap, RawBytes, WidthImage * NumberOfChannels, 0);
                ImageViewer.Source = SourceImage;
            }
        }

        public bool IsColored
        {
            get => (bool)GetValue(IsColoredProperty);
            set
            {
                if (value != IsColored)
                {
                    SetValue(IsColoredProperty, value);
                    NumberOfChannels = IsColored ? 3 : 1;
                    SetupImage();
                }
            }
        }

        public IntPtr ImagePtr
        {
            get => (IntPtr)GetValue(ImagePtrProperty);
            set
            {
                SetValue(ImagePtrProperty, value);
                SourceImage.WritePixels(RectBitmap, ImagePtr, WidthImage * HeightImage * NumberOfChannels, WidthImage * NumberOfChannels);
                ImageViewer.Source = SourceImage;
            }
        }

        public int HeightImage
        {
            get => (int)GetValue(HeightImageProperty);
            set
            {
                SetValue(HeightImageProperty, value);
                SetupImage();
            }
        }

        public int WidthImage
        {
            get => (int)GetValue(WidthImageProperty);
            set
            {
                SetValue(WidthImageProperty, value);
                SetupImage();
            }
        }

        private void SetupImage()
        {
            if (Width != 0 && HeightImage != 0)
            {
                const double dpi = 96;
                RectBitmap = new Int32Rect(0, 0, WidthImage, HeightImage);
                List<Color> colors = new List<Color> { Colors.Gray };
                BitmapPalette myPalette;
                PixelFormat format = PixelFormats.Gray8;
                if (NumberOfChannels == 3)
                {
                    colors = new List<Color> { Colors.Red, Colors.Green, Colors.Blue };
                    format = PixelFormats.Bgr24;
                }
                myPalette = new BitmapPalette(colors);
                SourceImage = new WriteableBitmap(WidthImage, HeightImage, dpi, dpi, format, myPalette);
            }
        }
    }
}