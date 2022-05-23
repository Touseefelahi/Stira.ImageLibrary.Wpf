using Prism.Commands;
using Prism.Mvvm;
using Stira.ImageLibrary.Wpf;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.WpfTest
{
    public class MainViewmodel : BindableBase
    {
        public MainViewmodel()
        {
            int rows = 480;
            int columns = 640;
            MouseClickCommand = new DelegateCommand<object>(MouseClicked);
            MouseMovedCommand = new DelegateCommand<object>(MouseMoved);
            var Image = new BitmapImage(new Uri(@"E:\Pictures\TestSample.png"));
            Image2 = new WriteableBitmap(Image);
            RaisePropertyChanged(nameof(Image2));
            RawBytes = new byte[rows * columns];

            Task.Run(async () =>
            {
                FrameCounter = 0;
                while (FrameCounter++ < 1000)
                {
                    int pixelIndex = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            RawBytes[pixelIndex++] = (byte)(i + FrameCounter);
                        }
                    }
                    RaisePropertyChanged(nameof(FrameCounter));
                    await Task.Delay(20);
                }
            });
        }

        public int FrameCounter { get; set; }
        public byte[] RawBytes { get; set; }
        public DelegateCommand<object> MouseClickCommand { get; }

        public DelegateCommand<object> MouseMovedCommand { get; }

        public WriteableBitmap Image2 { get; set; }

        private void MouseMoved(object mouseEvent)
        {
            if (mouseEvent is MouseArgs mouseArgs)
            {
                Trace.WriteLine($"{mouseArgs.IsMouseDown} {mouseArgs.Position}");
            }
        }

        private void MouseClicked(object mouseEvent)
        {
            if (mouseEvent is MouseArgs mouseArgs)
            {
                Trace.WriteLine($"{mouseArgs.IsMouseDown} {mouseArgs.Position}");
            }
        }
    }
}