using System.Windows;

namespace Stira.ImageLibrary.Wpf
{
    public struct MouseArgs
    {
        /// <summary>
        /// To Detect Mouse down/Up event
        /// </summary>
        public bool IsMouseDown { get; set; }

        /// <summary>
        /// Current Mouse Position
        /// </summary>
        public Point Position { get; set; }
    }
}