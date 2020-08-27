## Welcome to STIRA Image Library for WPF
This library have a control that can display raw bytes as image int ImageControl 
in WPF
### Usage
Set the Resolution of input Image as WidthImage and HeightImage to in Image control.
For grayscall images fill the RawBytes from Row Column (0, 0) to (MaxWidth, MaxHeight).
For colored images set IsColored to true and fill the RawBytes in RGB format.

### Light Image

      xmlns:il="clr-namespace:Stira.ImageLibrary.Wpf;assembly=Stira.ImageLibrary.Wpf"
      
      <il:LightImage Margin="5,50,5,5" RawBytes="{Binding RawBytes}"
                     Width="1024" Height="768"
                     WidthImage="{Binding Camera.Width}"
                     HeightImage="{Binding Camera.Height}"
                     IsColored="False"
                     FrameCounter="{Binding FrameCounter}" />

And in the backend we must Update FrameCounter to refresh the image at every new frame
e.g.

        public byte[] RawBytes { get; set; }

        public int FrameCounter
        {
            get => frameCounter;
            set => SetProperty(ref frameCounter, value);
        }

        private void FrameReady(byte[] rawBytes)
        {
            if (rawBytes != null)
            {
                RawBytes = rawBytes;
                RaisePropertyChanged(nameof(RawBytes));
                FrameCounter++;
            }
        }