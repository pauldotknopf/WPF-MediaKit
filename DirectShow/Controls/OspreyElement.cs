using System;
using System.Windows;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace DirectShow.Controls
{
    public class OspreyElement : MediaElementBase
    {
        public OspreyElement()
        {
            OspreyPlayer.Dispatcher.BeginInvoke((Action)(() => OspreyPlayer.Open()));
            
            Play();
        }

        /// <summary>
        /// The current MediaUriPlayer
        /// </summary>
        protected OspreyMediaPlayer OspreyPlayer
        {
            get
            {
                return MediaPlayerBase as OspreyMediaPlayer;
            }
        }

        protected override MediaPlayerBase OnRequestMediaPlayer()
        {
            var player = new OspreyMediaPlayer();
            return player;
        }
    }
}
