using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL4Unity.Utils;
using LSL;

namespace edia.lsl {
 
    public class MarkerOutlet : AStringOutlet
    {
        public float ResetInterval = 3.4f;
        private float elapsed_time = 0.0f;
        private Material mat;
		private bool isBuildSample = false;

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
        /// <summary>
        /// Send a marker over the configured LSL stream.
        /// </summary>
        /// <param name="value">String value for marker</param>
        public void SendMarker (string value) {
			sample[0] = value;
            isBuildSample = true; // Trigger for actually sending sample in BuildSample method
        }

        protected override bool BuildSample() {
            if (isBuildSample) { isBuildSample = false;
                return true;
            }

            return false;
        }
    }
}