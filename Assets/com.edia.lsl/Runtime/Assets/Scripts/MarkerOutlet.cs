using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL4Unity.Utils;
using LSL;
using LSL4Unity;

namespace edia.lsl {

    [ScriptOrder(-999)] // this needs to be executed as soon as possible in Update()
    [RequireComponent(typeof(TimeSync))]
    public class MarkerOutlet : AStringOutlet
    {

        private Queue<string> _markersQueued = new();
        private bool _isBuildSample = false;

        public enum MomentForMarker { StartOfFrame, EndOfFrame, Now }

		public override List<string> ChannelNames
        {
            get
            {
                List<string> chanNames = new List<string>{ "Event" };
                return chanNames;
            }
        }


        public void Reset()
        {
            StreamName = "Unity.Event";
            StreamType = "Markers";
            moment = MomentForSampling.EndOfFrame;
            IrregularRate = true;
        }


        protected override void Update() {
            if (_markersQueued.Count > 0) {
                int lengthQueue = _markersQueued.Count;
                for (int i = 0; i < lengthQueue; i++) {
                    var marker = _markersQueued.Dequeue();
                    pushMarker(marker);
                }
            }

            base.Update();

        }


        /// <summary>
        /// Send a marker over the configured LSL stream.
        /// </summary>
        /// <param name="value">String value for marker</param>
        public void SendMarker (string value) {
			sample[0] = value;
            _isBuildSample = true; // Trigger for actually sending sample in BuildSample method
        }


        /// <summary>
        /// Sends a marker immediately, at the end of the current frame, or at the start of the next frame based on the specified moment.
        /// Note that this will behave slightly differently than SendMarker(marker) without further arguments: with arguments it will ignore 
        /// a TimeSync component and just timestamp the marker with the current LSL time. It will also overrule the moment (MomentForSampling)
        /// which may have been set in the editor.
        /// </summary>
        /// <param name="marker">The marker to be sent.</param>
        /// <param name="moment">The moment during the frame to send the marker, 
        /// which can be immediately (Now), at the end of the current frame (EndOfFrame), 
        /// or at the start of the next frame (StartOfFrame), Defaults to Now.</param>
        public void SendMarker(string marker, MomentForMarker moment = MomentForMarker.Now) {
            switch (moment) {
                case MomentForMarker.StartOfFrame:
                    StartCoroutine(sendMarkerInXFrames(marker, 0));
                    break;
                case MomentForMarker.EndOfFrame:
                    sendMarkerAtEndOfFrame(marker);
                    break;
                case MomentForMarker.Now:
                    pushMarker(marker);
                    break;
            }
        }


        /// <summary>
        /// Sends a marker with an optional delay. 
        /// This version allows specifying a delay in frames, but currently only supports sending 
        /// at the start of a frame if there's a delay.
        /// A delay of 0 means the marker will be sent asap in the next frame.
        /// </summary>
        /// <param name="marker">The marker to be sent.</param>
        /// <param name="nFramesDelay">The number of frames to delay before sending the marker. 
        /// Must be non-negative. Defaults to 0, meaning marker is sent asap in the next frame.</param>
        public void SendMarker(string marker, int nFramesDelay = 0) {
            if (nFramesDelay < 0) {
                Debug.LogError("nFramesDelay must be greater than or equal to 0");
            }

            if (nFramesDelay >= 0) {
                StartCoroutine(sendMarkerInXFrames(marker, nFramesDelay));
            }
        }


        protected override bool BuildSample() {
            if (_isBuildSample) { 
                _isBuildSample = false;
                return true;
            }

            return false;
        }


        public void sendMarkerAtEndOfFrame(string marker) {
            StartCoroutine(pushMarkerAtEndOfFrame(marker));
        }

        
        public void sendMarkerNextFrame(string marker) {
            if (_markersQueued == null) {
                _markersQueued = new();
            }
            _markersQueued.Enqueue(marker);
        }


        void pushMarker(string marker) {
            var tmp = sample[0];
            sample[0] = marker;
            pushSample();
            sample[0] = tmp;
        }


        IEnumerator pushMarkerAtEndOfFrame(string marker) {
            yield return new WaitForEndOfFrame();
            pushMarker(marker);
        }


        IEnumerator sendMarkerInXFrames(string marker, int frames) {
            for (int i = 0; i < frames - 1; i++) {
                yield return null;
            }
            if (_markersQueued == null) {
                _markersQueued = new();
            }
            _markersQueued.Enqueue(marker);
        }
    }
}