using Prism.Commands;
using RtspPlayerControl;
using Stira.ImageLibrary.Wpf;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;

namespace Stira.ImageLibrary.WpfTest
{
    /// <summary>
    /// Interaction logic for OnvifWindow.xaml
    /// </summary>
    public partial class OnvifWindow : Window
    {
        private RtspPlayerModel rtspPlayer;

        public OnvifWindow()
        {
            InitializeComponent();
            rtspPlayer = new RtspPlayerModel("rtsp://192.168.10.90:554/stream1", "adminadmin", "adminadmin");
            _invalidateAction = Invalidate;
            rtspPlayer.VideoSource.FrameReceived += VideoSource_FrameReceived;
            rtspPlayer.Start();

        }
        private System.Windows.Media.Color _fillColor;
        private WriteableBitmap _writeableBitmap;
        private int _width;
        private int _height;
        private Int32Rect _dirtyRect;
        private TransformParameters _transformParameters;
        private readonly Action<IDecodedVideoFrame> _invalidateAction;

        private Task _handleSizeChangedTask = Task.CompletedTask;
        private CancellationTokenSource _resizeCancellationTokenSource = new CancellationTokenSource();

        private void VideoSource_FrameReceived(object sender, IDecodedVideoFrame e)
        {
            Application.Current.Dispatcher.Invoke(_invalidateAction, DispatcherPriority.Send, e);
        }

        private void Invalidate(IDecodedVideoFrame decodedVideoFrame)
        {
            if (lightImage2.writeableBitmap is null)
                if (!ReinitializeBitmap(decodedVideoFrame.Width, decodedVideoFrame.Height))
                    return;


            if (_width == 0 || _height == 0)
                return;

            lightImage2.writeableBitmap.Lock();

            try
            {
                decodedVideoFrame.TransformTo(lightImage2.writeableBitmap.BackBuffer, lightImage2.writeableBitmap.BackBufferStride, _transformParameters);
                lightImage2.AddDirtyRect();
            }
            finally
            {
                lightImage2.writeableBitmap.Unlock();
            }
        }
        private bool ReinitializeBitmap(int width, int height)
        {
            _width = width;
            _height = height;

            _transformParameters = new TransformParameters(RectangleF.Empty,
                    targetFrameSize: new System.Drawing.Size(_width, _height),
                    ScalingPolicy.Stretch, RtspPlayerControl.PixelFormat.Bgr24, ScalingQuality.FastBilinear);

            lightImage2.SetupBitmap(width, height);
            return (lightImage2.writeableBitmap != null);
        }


        public ICommand MouseClickCommand { get; }

        public DelegateCommand<object> MouseMoveCommand { get; }

        private void MouseMoved(object mouse)
        {
            if (mouse is MouseArgs mouseArgs)
                Trace.WriteLine($"{mouseArgs.IsMouseDown} {mouseArgs.Position}");
        }

        private void MouseClicked(object mouse)
        {
            if (mouse is MouseArgs mouseArgs)
                Trace.WriteLine($"{mouseArgs.IsMouseDown} {mouseArgs.Position}");
        }
    }
}
