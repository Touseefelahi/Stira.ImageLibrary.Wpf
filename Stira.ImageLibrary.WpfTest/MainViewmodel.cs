using Prism.Commands;
using Prism.Mvvm;
using Stira.ImageLibrary.Wpf;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.WpfTest
{
    public class MainViewmodel : BindableBase
    {
        public MainViewmodel()
        {
            MouseClickCommand = new DelegateCommand<object>(MouseClicked);
            MouseMovedCommand = new DelegateCommand<object>(MouseMoved);
            var Image = new BitmapImage(new Uri(@"C:\Users\Touseef\Pictures\25 14 March 21 Bullet position\25M1-1.mp4.00_06_26_17.Still001.png"));
            Image2 = new WriteableBitmap(Image);
            RaisePropertyChanged(nameof(Image2));
        }

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