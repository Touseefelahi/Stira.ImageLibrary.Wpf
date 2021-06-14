using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.Wpf
{
    public abstract class BaseImage : ScrollViewer, IBaseImage
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MouseClickEvents. This enables
        /// animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MouseClickEventsProperty =
            DependencyProperty.Register("MouseClickEvents", typeof(ICommand), typeof(BaseImage), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for MouseMovementEvent. This enables
        /// animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MouseMovementEventProperty =
            DependencyProperty.Register("MouseMovementEvent", typeof(ICommand), typeof(BaseImage), new PropertyMetadata(null));

        public Grid grid;

        public ScaleTransform scaleTransform;

        private readonly DisplayContextMenu displayContextMenu;
        private Point? lastCenterPositionOnTarget;

        private Point? lastMousePositionOnTarget;

        private Point? lastDragPoint;
        private bool isFlippedVertical, isOneToOnePixel, isFlippedHorizontal, isAutoScrollOn;

        protected BaseImage()
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
            ImageViewer = new Image();
            scaleTransform = new ScaleTransform(1, 1);
            ScrollChanged += BaseImage3_ScrollChanged;
            PreviewMouseLeftButtonUp += BaseImage3_MouseLeftButtonUp;
            PreviewMouseWheel += BaseImage3_PreviewMouseWheel;
            PreviewMouseLeftButtonDown += BaseImage3_PreviewMouseLeftButtonDown;
            MouseMove += BaseImage3_MouseMove;
            SizeChanged += BaseImage3_SizeChanged;
            grid.Children.Add(ImageViewer);
            TransformGroup transformGroup = new();
            transformGroup.Children.Add(scaleTransform);
            grid.LayoutTransform = transformGroup;
            grid.RenderTransformOrigin = new Point(0.5, 0.5);
            grid.Width = ActualWidth;
            grid.Height = ActualHeight;
            Content = grid;
        }

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

        public Int32Rect RectBitmap { get; protected set; }
        public int NumberOfChannels { get; protected set; } = 1;
        public WriteableBitmap SourceImage { get; protected set; }
        public Image ImageViewer { get; protected set; }

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
            MouseMovementEvent?.Execute(new MouseArgs() { IsMouseDown = IsMouseCaptured, Position = GetPoint(e) });
        }

        private void BaseImage3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            if (mousePos.X <= ViewportWidth && mousePos.Y < ViewportHeight)
            {
                Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(this);
            }
            MouseClickEvents?.Execute(new MouseArgs() { IsMouseDown = true, Position = GetPoint(e) });
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
                    scaleTransform.ScaleX++;
                    scaleTransform.ScaleY++;
                }
                if (e.Delta < 0 && scaleTransform.ScaleX > 1)
                {
                    scaleTransform.ScaleX--;
                    scaleTransform.ScaleY--;
                }
                Point centerOfViewport = new(ViewportWidth / 2,
                                 ViewportHeight / 2);
                lastCenterPositionOnTarget = TranslatePoint(centerOfViewport, grid);
            }
            e.Handled = true;
        }

        private bool IsOutOfImage()
        {
            Point point = new()
            {
                X = Mouse.GetPosition(ImageViewer).X * RectBitmap.Width / ImageViewer.ActualWidth,
                Y = Mouse.GetPosition(ImageViewer).Y * RectBitmap.Height / ImageViewer.ActualHeight,
            };
            return point.X > RectBitmap.Width || point.X < 0 || point.Y > RectBitmap.Height || RectBitmap.Height < 0;
        }

        private Point GetPoint(MouseEventArgs e)
        {
            return new Point()
            {
                X = e.GetPosition(ImageViewer).X * RectBitmap.Width / ImageViewer.ActualWidth,
                Y = e.GetPosition(ImageViewer).Y * RectBitmap.Height / ImageViewer.ActualHeight,
            };
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
                        Point centerOfViewport = new(ViewportWidth / 2, ViewportHeight / 2);
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

                    double newOffsetX = HorizontalOffset - (dXInTargetPixels * multiplicatorX);
                    double newOffsetY = VerticalOffset - (dYInTargetPixels * multiplicatorY);

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
            MouseClickEvents?.Execute(new MouseArgs() { IsMouseDown = false, Position = GetPoint(e) });
        }

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
                ImageViewer.Stretch = Stretch.None;
                scaleTransform.ScaleY = 1;
                scaleTransform.ScaleX = 1;
                ImageViewer.Width = grid.Width;
                ImageViewer.Height = grid.Height;
            }
            else
            {
                ImageViewer.Stretch = Stretch.Uniform;
                ImageViewer.Width = ActualWidth;
                ImageViewer.Height = ActualHeight;
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

            Point centerOfViewport = new(ViewportWidth / 2, ViewportHeight / 2);
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