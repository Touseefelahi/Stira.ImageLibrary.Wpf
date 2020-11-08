using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public abstract class BaseImage : ScrollViewer
    {
        protected Int32Rect rectBitmap;
        protected int numberOfChannels = 1;
        protected WriteableBitmap SourceImage;
        protected Image imageViewer;
        private readonly ScaleTransform scaleTransform;
        private readonly DisplayContextMenu displayContextMenu;
        private bool isFlippedVertical, isOneToOnePixel, isFlippedHorizontal;
        private Point start, origin;

        private UIElement reference;

        private Size baseSize;

        protected BaseImage()
        {
            imageViewer = new Image();
            //Context menu
            displayContextMenu = new DisplayContextMenu();
            ContextMenu = displayContextMenu;
            ((MenuItem)ContextMenu.Items[0]).Click += OneToOnePixel;
            ((MenuItem)ContextMenu.Items[1]).Click += FlipVertical;
            ((MenuItem)ContextMenu.Items[2]).Click += FlipHorizontal;
            ((MenuItem)ContextMenu.Items[3]).Click += FitOnScreen;

            //Mouse events zoom and pan
            PreviewMouseWheel += BaseImage_MouseWheel;
            PreviewMouseMove += Image_MouseMove;
            PreviewMouseLeftButtonDown += Image_MouseLeftButtonDown;
            PreviewMouseLeftButtonUp += Image_MouseLeftButtonUp;

            //Transform
            scaleTransform = new ScaleTransform() { CenterX = 0.5, CenterY = 0.5 };
            imageViewer.RenderTransformOrigin = new Point(0.5, 0.5);
            imageViewer.RenderTransform = scaleTransform;

            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            IsDeferredScrollingEnabled = true;

            Content = imageViewer;
            Loaded += BaseImage_Loaded;
            SizeChanged += BaseImage_SizeChanged;
        }

        private void BaseImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
            {
                imageViewer.Width = e.NewSize.Width;
            }
            if (e.NewSize.Height > 0)
                imageViewer.Height = e.NewSize.Height;
        }

        private void BaseImage_Loaded(object sender, RoutedEventArgs e)
        {
            reference = this;
        }

        private void FitOnScreen(object sender, RoutedEventArgs e)
        {
            RemoveTransform();
            if (displayContextMenu.OneToOnePixel.IsChecked)
            {
                OneToOnePixel(null, null);
                displayContextMenu.OneToOnePixel.IsChecked = false;
            }
            else
            {
                imageViewer.Stretch = Stretch.Uniform;
                imageViewer.Width = this.ActualWidth;
                imageViewer.Height = this.ActualHeight;
            }
            RemoveFlipping();
        }

        private void RemoveFlipping()
        {
            if (displayContextMenu.FlipVertical.IsChecked)
            {
                FlipVertical(null, null);
                displayContextMenu.FlipVertical.IsChecked = false;
            }
            if (displayContextMenu.FlipHorizontal.IsChecked)
            {
                FlipHorizontal(null, null);
                displayContextMenu.FlipHorizontal.IsChecked = false;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(reference);
            origin = new Point(HorizontalOffset, VerticalOffset);
            CaptureMouse();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Vector v = start - e.GetPosition(reference);
                ScrollToVerticalOffset(origin.Y + v.Y);
                ScrollToHorizontalOffset(origin.X + v.X);
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void BaseImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isOneToOnePixel) return;
            double zoom = e.Delta > 0 ? .1 : -.1;
            if (Math.Abs(scaleTransform.ScaleX + zoom) < 0.2 ||
                Math.Abs(scaleTransform.ScaleY + zoom) < 0.2)
            {
                return;
            }
            if (isFlippedHorizontal)
            {
                scaleTransform.ScaleX -= zoom;
            }
            else
            {
                scaleTransform.ScaleX += zoom;
            }

            if (isFlippedVertical)
            {
                scaleTransform.ScaleY -= zoom;
            }
            else
            {
                scaleTransform.ScaleY += zoom;
            }

            imageViewer.Width = this.ActualWidth * Math.Abs(scaleTransform.ScaleX);
            imageViewer.Height = ActualHeight * Math.Abs(scaleTransform.ScaleY);

            e.Handled = true;
        }

        private void FlipVertical(object sender, RoutedEventArgs e)
        {
            //ZoomToNormal();
            isFlippedVertical = !isFlippedVertical;
            scaleTransform.ScaleY = -1 * scaleTransform.ScaleY;
        }

        private void RemoveTransform()
        {
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
        }

        private void FlipHorizontal(object sender, RoutedEventArgs e)
        {
            //ZoomToNormal();
            isFlippedHorizontal = !isFlippedHorizontal;
            scaleTransform.ScaleX = -1 * scaleTransform.ScaleX;
        }

        private void ZoomToNormal()
        {
            scaleTransform.ScaleY = isFlippedVertical ? -1 : 1;
            scaleTransform.ScaleX = isFlippedHorizontal ? -1 : 1;
        }

        private void OneToOnePixel(object sender, RoutedEventArgs e)
        {
            isOneToOnePixel = !isOneToOnePixel;
            if (isOneToOnePixel)
            {
                ZoomToNormal();
                imageViewer.Stretch = Stretch.None;
                scaleTransform.ScaleY = 1;
                scaleTransform.ScaleX = 1;
                imageViewer.Width = rectBitmap.Width;
                imageViewer.Height = rectBitmap.Height;
            }
            else
            {
                imageViewer.Stretch = Stretch.Uniform;
                imageViewer.Width = this.ActualWidth;
                imageViewer.Height = this.ActualHeight;
                //imageViewer.Width = imageViewer.Height = double.NaN;
            }
        }
    }
}