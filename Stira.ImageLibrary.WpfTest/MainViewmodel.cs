using Prism.Commands;
using Prism.Mvvm;
using Stira.ImageLibrary.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.WpfTest
{
    public class MainViewmodel : BindableBase
    {
        public MainViewmodel()
        {
            MouseClickCommand = new DelegateCommand<object>(MouseClicked);
            Image2 = new BitmapImage(new Uri(@"C:\Users\f.fagihi\Videos\Captures\Test.jpg"));
            RaisePropertyChanged(nameof(Image2));
        }

        public DelegateCommand<object> MouseClickCommand { get; }

        public BitmapImage Image2 { get; set; }

        private void MouseClicked(object mouseEvent)
        {
            if (mouseEvent is MouseArgs mouseArgs)
                Trace.WriteLine($"{mouseArgs.IsMouseDown} {mouseArgs.Position}");
        }
    }
}