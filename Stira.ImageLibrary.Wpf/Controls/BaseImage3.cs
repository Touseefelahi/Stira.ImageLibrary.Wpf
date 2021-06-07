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
         
        public BaseImage3()
        {
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
            grid.Children.Add(imageViewer);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            grid.LayoutTransform = transformGroup;
            Content = grid;
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

    }
}