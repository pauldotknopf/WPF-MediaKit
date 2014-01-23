using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFMediaKit.DirectShow.Controls;

namespace DirectShow.Controls
{
    public class OspreyLiveVideoElement : MediaElementBase
    {
        public OspreyLiveVideoElement()
        {
            MediaPlayerBase = new OspreyLiveVideo();
        }
    }
}
