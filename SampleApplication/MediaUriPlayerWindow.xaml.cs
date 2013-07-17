using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SampleApplication
{
    public partial class MediaUriPlayerWindow : Window
    {
        public MediaUriPlayerWindow()
        {
            InitializeComponent();
           
            mediaPlayer.PreviewMouseLeftButtonDown += mediaPlayer_PreviewMouseLeftButtonDown;
        }

        private void mediaPlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
