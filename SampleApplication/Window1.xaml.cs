using System;
using System.Windows;
using Microsoft.Win32;
using WPFMediaKit.DirectShow.MediaPlayers;


namespace SampleApplication
{
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
            videoCapElement.NewVideoSample += videoCapElement_NewVideoSample;
		}

        void videoCapElement_NewVideoSample(object sender, VideoSampleArgs e)
        {
            //e.VideoFrame.Save(@"c:\users\jmoney\desktop\test.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			mediaPlayer.Volume = 0.5;
		}

		private void mnuOpen_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();

			dlg.ShowDialog();

			string file = dlg.FileName;

			if (string.IsNullOrEmpty(file))
				return;

			mediaPlayer.Source = new Uri(file);
		}

		private void playButton_Click(object sender, RoutedEventArgs e)
		{
			mediaPlayer.Play();
		}

		private void pauseButton_Click(object sender, RoutedEventArgs e)
		{
			mediaPlayer.Pause();
		}
	}
}