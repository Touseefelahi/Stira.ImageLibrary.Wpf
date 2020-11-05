using System.Windows.Controls;

namespace Stira.ImageLibrary.Wpf
{
    internal class DisplayContextMenu : ContextMenu
    {
        public DisplayContextMenu()
        {
            OneToOnePixel = new MenuItem
            {
                Header = "1:1 Pixel",
                IsCheckable = true
            };

            FlipVertical = new MenuItem
            {
                Header = "Flip Vertical",
                IsCheckable = true
            };

            FlipHorizontal = new MenuItem
            {
                Header = "Flip Horizontal",
                IsCheckable = true
            };
            DefaultScale = new MenuItem
            {
                Header = "Default Scale"
            };
            Items.Add(OneToOnePixel);
            Items.Add(FlipVertical);
            Items.Add(FlipHorizontal);
            Items.Add(DefaultScale);
        }

        public MenuItem OneToOnePixel { get; }
        public MenuItem FlipVertical { get; }
        public MenuItem FlipHorizontal { get; }
        public MenuItem DefaultScale { get; }
    }
}