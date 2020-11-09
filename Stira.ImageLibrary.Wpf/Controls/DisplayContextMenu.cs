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

            AutoScroll = new MenuItem
            {
                Header = "Auto Scrollbar",
                IsCheckable = true,
                ToolTip = "If enabled it will display the scrollbar if the " +
                          "image size is bigger than the container"
            };
            ZoomIn = new MenuItem
            {
                Header = "Zoom In",
                ToolTip = "OneToOnePixel must be off to use this feature"
            };
            ZoomOut = new MenuItem
            {
                Header = "Zoom Out",
                ToolTip = "OneToOnePixel must be off to use this feature"
            };
            DefaultScale = new MenuItem
            {
                Header = "Fit On Screen"
            };
            Items.Add(OneToOnePixel);
            Items.Add(FlipVertical);
            Items.Add(FlipHorizontal);
            Items.Add(AutoScroll);
            Items.Add(ZoomIn);
            Items.Add(ZoomOut);
            Items.Add(DefaultScale);
        }

        public MenuItem OneToOnePixel { get; }
        public MenuItem FlipVertical { get; }
        public MenuItem FlipHorizontal { get; }

        public MenuItem AutoScroll { get; }
        public MenuItem ZoomIn { get; }
        public MenuItem ZoomOut { get; }
        public MenuItem DefaultScale { get; }
    }
}