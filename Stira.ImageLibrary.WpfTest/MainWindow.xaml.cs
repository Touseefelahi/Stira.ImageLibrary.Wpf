using Prism.Commands;
using Stira.ImageLibrary.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Stira.ImageLibrary.WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewmodel();
            return;
            image.WidthImage = 2048;
            image.HeightImage = 1536;
            image.IsColored = true;
            var rawBytes = new byte[image.WidthImage * image.HeightImage * 3];
            for (int r = 0; r < image.HeightImage; r++)
            {
                for (int c = 0; c < image.WidthImage; c++)
                {
                    if (r % 2 == 0)
                    {
                        rawBytes[r * c] = 255;
                    }
                }
            }
            image.RawBytes = rawBytes;
            MouseClickCommand = new DelegateCommand<object>(MouseClicked);
            Image2 = new BitmapImage(new Uri(@"C:\Users\touse\Pictures\90 14 March 21 Bullet position\90M1-1.png"));
            image3.Image = Image2;
            // MouseMoveCommand = new DelegateCommand<object>(MouseMoved);
        }
        public BitmapImage Image2 { get; set; }
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