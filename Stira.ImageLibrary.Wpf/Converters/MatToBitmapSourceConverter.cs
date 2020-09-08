using Emgu.CV;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public static class BitmapSourceConvert
    {
        public static BitmapSource ToBitmapSource(Mat image)
        {
            using System.Drawing.Bitmap source = image.ToBitmap();
            IntPtr ptr = source.GetHbitmap();

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ptr);
            return bs;
        }

        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ptr);
            return bs;
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
    }
}