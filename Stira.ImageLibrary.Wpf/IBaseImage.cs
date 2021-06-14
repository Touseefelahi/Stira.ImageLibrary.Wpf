using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public interface IBaseImage
    {
        public Int32Rect RectBitmap { get; }

        public int NumberOfChannels { get; }

        public WriteableBitmap SourceImage { get; }

        public Image ImageViewer { get; }
    }
}