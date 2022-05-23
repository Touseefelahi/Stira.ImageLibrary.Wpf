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
            DataContext = new MainViewmodel();
            InitializeComponent();
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