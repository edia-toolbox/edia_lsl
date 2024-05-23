using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL4Unity.Utils;
using LSL;
using LSL4Unity;

namespace Edia.Lsl {

    [ScriptOrder(-999)] // this needs to be executed as soon as possible in Update()
    [RequireComponent(typeof(TimeSync))]
    public class MarkerOutlet : AStringOutlet
    {

        private Queue<string> _markersQueued = new();
        private bool _isBuildSample = false;

        public enum CustomMomentForMarker { StartOfNextFrame, EndOfThisFrame, Now }

		public override List<string> ChannelNames
        {
            get
            {
                var chanNames = new List<string>{ "Event" };
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
                    PushMarker(marker);
                }
            }

            base.Update();

        }


        /// <summary>
        /// Send a marker over the configured LSL stream.
        /// The timing of the marker will be determined by the public MomentForSampling property (not the CustomMomentForMarker).
        /// </summary>
        /// <param name="value">String value for marker</param>
        public void SendMarker (string value) {
			sample[0] = value;
            _isBuildSample = true; // Trigger for actually sending sample in BuildSample method
        }


        /// <summary>
        /// Sends a marker immediately, at the end of the current frame, or at the start of the next frame based on the specified customMomentForMarker.
        /// Note that this will behave slightly differently than SendMarker(marker) without further arguments: with arguments it will ignore 
        /// a TimeSync component and just timestamp the marker with the current LSL time. It will also overrule the customMomentForMarker (MomentForSampling)
        /// which may have been set in the editor.
        /// </summary>
        /// <param name="marker">The marker to be sent.</param>
        /// <param name="customMomentForMarker">The customMomentForMarker to send the marker, 
        /// which can be immediately (Now), at the end of the current frame (EndOfThisFrame), 
        /// or at the start of the next frame (StartOfNextFrame).</param>
        public void SendMarker(string marker, CustomMomentForMarker customMomentForMarker) {
            switch (customMomentForMarker) {
                case CustomMomentForMarker.StartOfNextFrame:
                    StartCoroutine(SendMarkerInXFrames(marker, 0));
                    break;
                case CustomMomentForMarker.EndOfThisFrame:
                    SendMarkerAtEndOfFrame(marker);
                    break;
                case CustomMomentForMarker.Now:
                    PushMarker(marker);
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
        /// Must be non-negative. 0 means the marker is sent asap in the next frame.</param>
        public void SendMarker(string marker, int nFramesDelay) {
            if (nFramesDelay < 0) {
                Debug.LogError("nFramesDelay must be greater than or equal to 0");
            }
            else {
                StartCoroutine(SendMarkerInXFrames(marker, nFramesDelay));
            }
        }


        protected override bool BuildSample() {
            if (!_isBuildSample) {
                return false;
            }
            _isBuildSample = false;
            return true;
        }


        private void SendMarkerAtEndOfFrame(string marker) {
            StartCoroutine(PushMarkerAtEndOfFrame(marker));
        }

        
        private void SendMarkerNextFrame(string marker) {
            _markersQueued ??= new();
            _markersQueued.Enqueue(marker);
        }


        private void PushMarker(string marker) {
            var tmp = sample[0];
            sample[0] = marker;
            pushSample();
            sample[0] = tmp;
        }


        private IEnumerator PushMarkerAtEndOfFrame(string marker) {
            yield return new WaitForEndOfFrame();
            PushMarker(marker);
        }


        private IEnumerator SendMarkerInXFrames(string marker, int frames) {
            for (int i = 0; i < frames - 1; i++) {
                yield return null;
            }
            _markersQueued ??= new();
            _markersQueued.Enqueue(marker);
        }
    }
}