using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.WpfTest
{
    public class MainViewmodel : BindableBase
    {

        public MainViewmodel()
        {
            Image2 = new BitmapImage(new Uri(@"C:\Users\touse\Pictures\90 14 March 21 Bullet position\90M1-1.png"));
            RaisePropertyChanged(nameof(Image2));
            Image2 = new BitmapImage(new Uri(@"C:\Users\touse\Pictures\90 14 March 21 Bullet position\90M1-1.png"));
            RaisePropertyChanged(nameof(Image2));
        }

        public BitmapImage Image2 { get; set; }

    }
}
