﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            DataContext = this;
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
        }
    }
}