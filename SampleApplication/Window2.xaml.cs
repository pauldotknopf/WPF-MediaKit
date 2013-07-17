using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using WPFMediaKit.DirectShow.Controls;

namespace SampleApplication
{
    public partial class TestMultipleVideosChangingReuseMediaUriElement : Window
    {
        DispatcherTimer _timer = new DispatcherTimer();
        private List<MediaUriElement> m_players = new List<MediaUriElement>();
        private List<string> _listOfVideoFiles;
        private int _playerToStartStopNext = 0;
        
        private const int MAX_PLAYERS = 9;
        private bool REUSE_PLAYERS = true;

        public TestMultipleVideosChangingReuseMediaUriElement()
        {
            InitializeComponent();
            MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(TestMultipleVideosChangingReuseMediaUriElement_MouseDoubleClick);
            MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(TestMultipleVideosChangingReuseMediaUriElement_MouseRightButtonDown);
        }

        void TestMultipleVideosChangingReuseMediaUriElement_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            _timer.Stop();
            panel.Children.Clear();
        }

        void TestMultipleVideosChangingReuseMediaUriElement_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
            _timer.Start();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateMediaUriElements();
            StartStopMediaUriElements();

            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private string GetRandomFileFromDirectory()
        {
            if (_listOfVideoFiles == null)
            {
                string directory = @"e:\Users\public\videos";
                _listOfVideoFiles = new List<string>();

                //string[] files1 = Directory.GetFiles(directory, "*.avi", SearchOption.AllDirectories);
                string[] files2 = Directory.GetFiles(directory, "*.wmv", SearchOption.AllDirectories);
                //string[] files3 = Directory.GetFiles(directory, "*.mkv", SearchOption.AllDirectories);

                //_listOfVideoFiles.AddRange(files1);
                _listOfVideoFiles.AddRange(files2);
                //_listOfVideoFiles.AddRange(files3);
            }

            var random = new Random();
            int i = random.Next(0, _listOfVideoFiles.Count - 1);
            return _listOfVideoFiles[i];
        }

        private void CreateMediaUriElements()
        {
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                m_players.Add(new MediaUriElement{Width=320, Height=240});
            }
        }

        private void StartStopMediaUriElements()
        {
            var player = m_players[_playerToStartStopNext];

            int pos = panel.Children.IndexOf(player);
            if(panel.Children.Contains(player))
                panel.Children.Remove(player);
            
            if (pos < 0)
                pos = panel.Children.Count;
            else if(!REUSE_PLAYERS)
            {
                m_players[_playerToStartStopNext].Close();
                m_players[_playerToStartStopNext] = new MediaUriElement { Width = 320, Height = 240 };
                player = m_players[_playerToStartStopNext];
            }

            player.BeginInit();
            player.Source = new Uri(GetRandomFileFromDirectory());
            panel.Children.Insert(pos, player);
            player.EndInit();

            Debug.WriteLine(player.Source.OriginalString);

            _playerToStartStopNext++;

            if (_playerToStartStopNext > MAX_PLAYERS - 1)
            {
                _playerToStartStopNext = 0;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            StartStopMediaUriElements();
           // GC.Collect(2);
           // GC.WaitForPendingFinalizers();
           // GC.Collect(2);
           //GC.Collect(2);
           // GC.WaitForPendingFinalizers();
           // GC.Collect(2);
           // GC.Collect(2);
           // GC.WaitForPendingFinalizers();
           // GC.Collect(2);
           // GC.Collect();
           // GC.WaitForPendingFinalizers();
           // GC.Collect();
        }
    } 
}
