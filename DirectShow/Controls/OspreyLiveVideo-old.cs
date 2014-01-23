using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace DirectShow.Controls
{
    public class OspreyLiveVideo : MediaPlayerBase
    {
        public OspreyLiveVideo()
        {
            BuildGraph();
        }

        private IAMCrossbar _crossbarFilterIAMCrossbar;
        private IGraphBuilder m_graph;
#if DEBUG
        private DsROTEntry m_dsRotEntry;
#endif

        /// <summary>
        /// Opens the media by initializing the DirectShow graph
        /// </summary>
        protected virtual void BuildGraph()
        {
            /* Make sure we clean up any remaining mess */
            FreeResources();

            try
            {
                /* Creates the GraphBuilder COM object */
                m_graph = new FilterGraphNoThread() as IGraphBuilder;

                if (m_graph == null)
                    throw new Exception("Could not create a graph");

                var renderer = CreateVideoRenderer(VideoRendererType.EnhancedVideoRenderer, m_graph, 2);

                var filterGraph = m_graph as IFilterGraph2;

                if (filterGraph == null)
                    throw new Exception("Could not QueryInterface for the IFilterGraph2");

                // add the crossbar
                var crossbarDevice =
                    DsDevice.GetDevicesOfCat(FilterCategory.AMKSCrossbar)
                        .SingleOrDefault(x => x.Name == "Osprey-100 Crossbar 1");
                if (crossbarDevice == null) throw new Exception("Couldn't find Osprey-100 Crossbar 1");
                IBaseFilter crossbarFilterIAMCrossbar;
                var hr = filterGraph.AddSourceFilterForMoniker(crossbarDevice.Mon, null, crossbarDevice.Name,
                    out crossbarFilterIAMCrossbar);
                DsError.ThrowExceptionForHR(hr);
                _crossbarFilterIAMCrossbar = crossbarFilterIAMCrossbar as IAMCrossbar;
                int currentRoute;
                _crossbarFilterIAMCrossbar.get_IsRoutedTo(0, out currentRoute);
                

                // add the video device
                var videoDevice =
                    DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                        .SingleOrDefault(x => x.Name == "Osprey-100 Video Device 1");
                if (videoDevice == null) throw new Exception("Couldn't find Osprey-100 Video Device 1");
                IBaseFilter deviceFilter;
                hr = filterGraph.AddSourceFilterForMoniker(videoDevice.Mon, null, videoDevice.Name,
                    out deviceFilter);
                DsError.ThrowExceptionForHR(hr);

                // connect the crossbar and device
                var crossbarOutput = DsFindPin.ByName(crossbarFilterIAMCrossbar, "0: Video Decoder Out");
                if (crossbarOutput == null) throw new Exception("Couldn't get the crossbar output pin");
                var videoDeviceInput = DsFindPin.ByName(deviceFilter, "Analog Video In");
                if (videoDeviceInput == null) throw new Exception("Couldn't get the video device input pin");
                hr = m_graph.Connect(crossbarOutput, videoDeviceInput);
                DsError.ThrowExceptionForHR(hr);

                /* We will want to enum all the pins on the source filter */
                IEnumPins pinEnum;

                hr = deviceFilter.EnumPins(out pinEnum);
                DsError.ThrowExceptionForHR(hr);

                IntPtr fetched = IntPtr.Zero;
                IPin[] pins = { null };

                /* Counter for how many pins successfully rendered */
                int pinsRendered = 0;

                /* Loop over each pin of the source filter */
                while (pinEnum.Next(pins.Length, pins, fetched) == 0)
                {
                    if (filterGraph.RenderEx(pins[0],
                                             AMRenderExFlags.RenderToExistingRenderers,
                                             IntPtr.Zero) >= 0)
                        pinsRendered++;

                    Marshal.ReleaseComObject(pins[0]);
                }

                if (pinsRendered == 0)
                    throw new Exception("Could not render any streams from the source Uri");

#if DEBUG
                /* Adds the GB to the ROT so we can view
                                 * it in graphedit */
                m_dsRotEntry = new DsROTEntry(m_graph);
#endif
                /* Configure the graph in the base class */
                SetupFilterGraph(m_graph);
            }
            catch (Exception ex)
            {
                /* This exection will happen usually if the media does
                 * not exist or could not open due to not having the
                 * proper filters installed */
                FreeResources();

                /* Fire our failed event */
                InvokeMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
            }

//            string fileSource = m_sourceUri.OriginalString;

//            if (string.IsNullOrEmpty(fileSource))
//                return;

//            try
//            {
//                /* Creates the GraphBuilder COM object */
//                m_graph = new FilterGraphNoThread() as IGraphBuilder;

//                if (m_graph == null)
//                    throw new Exception("Could not create a graph");

//                /* Add our prefered audio renderer */
//                InsertAudioRenderer(AudioRenderer);

//                IBaseFilter renderer = CreateVideoRenderer(VideoRenderer, m_graph, 2);

//                var filterGraph = m_graph as IFilterGraph2;

//                if (filterGraph == null)
//                    throw new Exception("Could not QueryInterface for the IFilterGraph2");

//                IBaseFilter sourceFilter;

//                /* Have DirectShow find the correct source filter for the Uri */
//                int hr = filterGraph.AddSourceFilter(fileSource, fileSource, out sourceFilter);
//                DsError.ThrowExceptionForHR(hr);

//                /* We will want to enum all the pins on the source filter */
//                IEnumPins pinEnum;

//                hr = sourceFilter.EnumPins(out pinEnum);
//                DsError.ThrowExceptionForHR(hr);

//                IntPtr fetched = IntPtr.Zero;
//                IPin[] pins = { null };

//                /* Counter for how many pins successfully rendered */
//                int pinsRendered = 0;

//                if (VideoRenderer == VideoRendererType.VideoMixingRenderer9)
//                {
//                    var mixer = renderer as IVMRMixerControl9;

//                    if (mixer != null)
//                    {
//                        VMR9MixerPrefs dwPrefs;
//                        mixer.GetMixingPrefs(out dwPrefs);
//                        dwPrefs &= ~VMR9MixerPrefs.RenderTargetMask;
//                        dwPrefs |= VMR9MixerPrefs.RenderTargetRGB;
//                        //mixer.SetMixingPrefs(dwPrefs);
//                    }
//                }

//                /* Test using FFDShow Video Decoder Filter
//                var ffdshow = new FFDShow() as IBaseFilter;

//                if (ffdshow != null)
//                    m_graph.AddFilter(ffdshow, "ffdshow");
//                */

//                /* Loop over each pin of the source filter */
//                while (pinEnum.Next(pins.Length, pins, fetched) == 0)
//                {
//                    if (filterGraph.RenderEx(pins[0],
//                                             AMRenderExFlags.RenderToExistingRenderers,
//                                             IntPtr.Zero) >= 0)
//                        pinsRendered++;

//                    Marshal.ReleaseComObject(pins[0]);
//                }

//                Marshal.ReleaseComObject(pinEnum);
//                Marshal.ReleaseComObject(sourceFilter);

//                if (pinsRendered == 0)
//                    throw new Exception("Could not render any streams from the source Uri");

//#if DEBUG
//                /* Adds the GB to the ROT so we can view
//                 * it in graphedit */
//                m_dsRotEntry = new DsROTEntry(m_graph);
//#endif
//                /* Configure the graph in the base class */
//                SetupFilterGraph(m_graph);

//                HasVideo = true;
//                /* Sets the NaturalVideoWidth/Height */
//                //SetNativePixelSizes(renderer);
//            }
//            catch (Exception ex)
//            {
//                /* This exection will happen usually if the media does
//                 * not exist or could not open due to not having the
//                 * proper filters installed */
//                FreeResources();

//                /* Fire our failed event */
//                InvokeMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
//            }

            InvokeMediaOpened();
        }

        protected override void FreeResources()
        {
#if DEBUG
            /* Remove us from the ROT */
            if (m_dsRotEntry != null)
            {
                m_dsRotEntry.Dispose();
                m_dsRotEntry = null;
            }
#endif

            /* We run the StopInternal() to avoid any 
             * Dispatcher VeryifyAccess() issues because
             * this may be called from the GC */
            StopInternal();

            /* Let's clean up the base 
             * class's stuff first */
            base.FreeResources();

            if (m_graph != null)
            {
                Marshal.ReleaseComObject(m_graph);
                m_graph = null;

                /* Only run the media closed if we have an
                 * initialized filter graph */
                InvokeMediaClosed(new EventArgs());
            }
        }
    }
}
