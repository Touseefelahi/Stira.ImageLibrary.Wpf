using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Stira.ImageLibrary.Wpf
{
    /// <summary>
    /// Image for Raw Bytes
    /// </summary>
    public class LightImage2 : BaseImage3
    {
        public WriteableBitmap writeableBitmap;
        private Int32Rect dirtyRect;

        public void AddDirtyRect()
        {
            writeableBitmap.AddDirtyRect(dirtyRect);
        }

        public void SetupBitmap(int width, int height)
        {
            writeableBitmap = new WriteableBitmap(width, height, 0, 0, PixelFormats.Bgr24, null);

            dirtyRect = new Int32Rect(0, 0, width, height);

            RenderOptions.SetBitmapScalingMode(writeableBitmap, BitmapScalingMode.NearestNeighbor);

            writeableBitmap.Lock();

            try
            {
                writeableBitmap.AddDirtyRect(dirtyRect);
            }
            finally
            {
                writeableBitmap.Unlock();
            };
            imageViewer.Source = writeableBitmap;
        }
    }
}