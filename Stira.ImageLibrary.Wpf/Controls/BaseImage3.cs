using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public abstract class BaseImage3 : ScrollViewer
    {
        // Using a DependencyProperty as the backing store for MouseClickEvents. This enables
        // animation, styling, binding, etc...
        public static readonly DependencyProperty MouseClickEventsProperty =
            DependencyProperty.Register("MouseClickEvents", typeof(ICommand), typeof(BaseImage3), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for MouseMovementEvent. This enables
        // animation, styling, binding, etc...
        public static readonly DependencyProperty MouseMovementEventProperty =
            DependencyProperty.Register("MouseMovementEvent", typeof(ICommand), typeof(BaseImage3), new PropertyMetadata(null));

        public ICommand MouseMovementEvent
        {
            get => (ICommand)GetValue(MouseMovementEventProperty);
            set => SetValue(MouseMovementEventProperty, value);
        }

        public ICommand MouseClickEvents
        {
            get => (ICommand)GetValue(MouseClickEventsProperty);
            set => SetValue(MouseClickEventsProperty, value);
        }

        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;

        public Grid grid;
        public Image imageViewer;
        public ScaleTransform scaleTransform;
        private DisplayContextMenu displayContextMenu;

        public BaseImage3()
        {

            //Context menu
            displayContextMenu = new DisplayContextMenu();
            ContextMenu = displayContextMenu;
            ((MenuItem)ContextMenu.Items[0]).Click += OneToOnePixel;
            ((MenuItem)ContextMenu.Items[1]).Click += FlipVertical;
            ((MenuItem)ContextMenu.Items[2]).Click += FlipHorizontal;
            ((MenuItem)ContextMenu.Items[3]).Click += AutoScroll;
            ((MenuItem)ContextMenu.Items[4]).Click += ZoomIn;
            ((MenuItem)ContextMenu.Items[5]).Click += ZoomOut;
            ((MenuItem)ContextMenu.Items[6]).Click += FitOnScreen;

            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            grid = new Grid();
            imageViewer = new Image();
            scaleTransform = new ScaleTransform(1,1);
            ScrollChanged += BaseImage3_ScrollChanged;
            MouseLeftButtonUp += BaseImage3_MouseLeftButtonUp;
            PreviewMouseLeftButtonUp += BaseImage3_MouseLeftButtonUp;
            PreviewMouseWheel += BaseImage3_PreviewMouseWheel;
            PreviewMouseLeftButtonDown += BaseImage3_PreviewMouseLeftButtonDown;
            MouseMove += BaseImage3_MouseMove;
            SizeChanged += BaseImage3_SizeChanged;
            grid.Children.Add(imageViewer);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            grid.LayoutTransform = transformGroup;
            grid.RenderTransformOrigin = new Point(0.5, 0.5);
            grid.Width = ActualWidth;
            grid.Height = ActualHeight;
            Content = grid;
        }



        private void BaseImage3_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
            {
                grid.Width = e.NewSize.Width;
            }
            if (e.NewSize.Height > 0)
            {
                grid.Height = e.NewSize.Height;
            }
        }

        private void BaseImage3_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(this);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                ScrollToHorizontalOffset(HorizontalOffset - dX);
                ScrollToVerticalOffset(VerticalOffset - dY);
            }
        }

        private void BaseImage3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(this);
            if (mousePos.X <= ViewportWidth && mousePos.Y < ViewportHeight)
            {
                Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(this);
            }
        }

        private void BaseImage3_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isOneToOnePixel)
            {
                return;
            }

            if (e.OriginalSource.GetType() != typeof(Image))
            {
                return;
            }

            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            if (e.Delta != 0)
            {

                if (e.Delta > 0)
                {
                    scaleTransform.ScaleX += 1;
                    scaleTransform.ScaleY += 1;
                }
                if (e.Delta < 0 && scaleTransform.ScaleX > 1)
                {
                    scaleTransform.ScaleX -= 1;
                    scaleTransform.ScaleY -= 1;
                }
                var centerOfViewport = new Point(ViewportWidth / 2,
                                 ViewportHeight / 2);
                lastCenterPositionOnTarget = TranslatePoint(centerOfViewport, grid);
            }
            e.Handled = true;
        }


        private void BaseImage3_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(ViewportWidth / 2, ViewportHeight / 2);
                        Point centerOfTargetNow = TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;

                    double newOffsetX = HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    ScrollToHorizontalOffset(newOffsetX);
                    ScrollToVerticalOffset(newOffsetY);
                }
            }

        }

        private void BaseImage3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
            ReleaseMouseCapture();
            lastDragPoint = null;
        }

        private bool isFlippedVertical, isOneToOnePixel, isFlippedHorizontal, isAutoScrollOn;

        private void FlipVertical(object sender, RoutedEventArgs e)
        {
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
                imageViewer.Width = grid.Width;
                imageViewer.Height = grid.Height;
            }
            else
            {
                imageViewer.Stretch = Stretch.Uniform;
                imageViewer.Width = ActualWidth;
                imageViewer.Height = ActualHeight;
            }
        }

        private void AutoScroll(object sender, RoutedEventArgs e)
        {
            isAutoScrollOn = !isAutoScrollOn;
            if (isAutoScrollOn)
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
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


        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (!isOneToOnePixel)
            {
                Zoom(-0.1);
            }
        }

        private void Zoom(double v)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            scaleTransform.ScaleX += v;
            scaleTransform.ScaleY += v;

            var centerOfViewport = new Point(ViewportWidth / 2,
                             ViewportHeight / 2);
            lastCenterPositionOnTarget = TranslatePoint(centerOfViewport, grid);

        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            if (!isOneToOnePixel)
            {
                Zoom(0.1);
            }
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
                grid.Width = ActualWidth;
                grid.Height = ActualHeight;
            }
            RemoveFlipping();
        }
    }
}