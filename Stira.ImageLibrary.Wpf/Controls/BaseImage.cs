using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public abstract class BaseImage : Image
    {
        protected Int32Rect rectBitmap;
        protected int numberOfChannels = 1;
        protected WriteableBitmap SourceImage;
        private readonly ScaleTransform scaleTransform;
        private readonly TranslateTransform translateTransform;
        private readonly DisplayContextMenu displayContextMenu;
        private bool isFlippedVertical, isOneToOnePixel, isFlippedHorizontal;
        private Point start, origin;

        protected BaseImage()
        {
            //Context menu
            displayContextMenu = new DisplayContextMenu();
            ContextMenu = displayContextMenu;
            ((MenuItem)ContextMenu.Items[0]).Click += OneToOnePixel;
            ((MenuItem)ContextMenu.Items[1]).Click += FlipVertical;
            ((MenuItem)ContextMenu.Items[2]).Click += FlipHorizontal;
            ((MenuItem)ContextMenu.Items[3]).Click += DefaultScale;

            //Mouse events zoom and pan
            MouseWheel += BaseImage_MouseWheel;
            MouseMove += Image_MouseMove;
            MouseLeftButtonDown += Image_MouseLeftButtonDown;
            MouseLeftButtonUp += Image_MouseLeftButtonUp;

            //Transform
            RenderTransformOrigin = new Point(0.5, 0.5);
            scaleTransform = new ScaleTransform();
            translateTransform = new TranslateTransform();
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(scaleTransform);
            myTransformGroup.Children.Add(translateTransform);
            RenderTransform = myTransformGroup;
            ClipToBounds = true;
        }

        private void DefaultScale(object sender, RoutedEventArgs e)
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
            if (displayContextMenu.OneToOnePixel.IsChecked)
            {
                OneToOnePixel(null, null);
                displayContextMenu.OneToOnePixel.IsChecked = false;
            }
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            translateTransform.X = 0;
            translateTransform.Y = 0;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            start = e.GetPosition(this);
            origin = new Point(translateTransform.X, translateTransform.Y);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                if (Math.Abs(scaleTransform.ScaleX) < 0.5 ||
                    Math.Abs(scaleTransform.ScaleY) < 0.5)
                {
                    return;
                }

                Vector v = start - e.GetPosition(this);
                if (isFlippedHorizontal)
                {
                    translateTransform.X = origin.X + v.X;
                }
                else
                {
                    translateTransform.X = origin.X - v.X;
                }

                if (isFlippedVertical)
                {
                    translateTransform.Y = origin.Y + v.Y;
                }
                else
                {
                    translateTransform.Y = origin.Y - v.Y;
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void BaseImage_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (displayContextMenu.OneToOnePixel.IsChecked)
            {
                OneToOnePixel(null, null);
                displayContextMenu.OneToOnePixel.IsChecked = false;
            }
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
        }

        private void FlipVertical(object sender, RoutedEventArgs e)
        {
            isFlippedVertical = !isFlippedVertical;
            scaleTransform.ScaleY = -1 * scaleTransform.ScaleY;
        }

        private void FlipHorizontal(object sender, RoutedEventArgs e)
        {
            isFlippedHorizontal = !isFlippedHorizontal;
            scaleTransform.ScaleX = -1 * scaleTransform.ScaleX;
        }

        private void OneToOnePixel(object sender, RoutedEventArgs e)
        {
            isOneToOnePixel = !isOneToOnePixel;
            if (isOneToOnePixel)
            {
                Stretch = Stretch.None;
                scaleTransform.ScaleY = isFlippedVertical ? -1 : 1;
                scaleTransform.ScaleX = isFlippedHorizontal ? -1 : 1;
            }
            else
            {
                Stretch = Stretch.Uniform;
            }
        }
    }
}