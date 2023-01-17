using System;
using System.Collections;
using System.Collections.Generic;
using LSL;
using UnityEngine;
using eDIA;

namespace edia.lsl {

	public class LSL_MarkerOutput : MonoBehaviour {

		private StreamOutlet outl = null;
		private StreamInfo inf = null;

		public string StreamName = "eDIA.MarkerStream";
		public string StreamType = "eDIA.MarkerStream";
		public string StreamId = "Markers";

		bool isStreaming = false;

#region STARTSTOPPERS

		void Start () {
			inf = new StreamInfo (StreamName, StreamType, 1, 0, channel_format_t.cf_string, StreamId);
			outl = new StreamOutlet (inf);

			isStreaming = true;

			EventManager.StartListening (eDIA.Events.DataHandlers.EvSendMarker, OnEvSendMarker);
			// EventManager.StartListening (eDIA.Events..Core.EvFinalizeSession, OnEvFinalizeSession);
		}

		void OnDestroy () {
			EventManager.StopListening (eDIA.Events.DataHandlers.EvSendMarker, OnEvSendMarker);
			// EventManager.StopListening (eDIA.Events.Core.EvFinalizeSession, OnEvFinalizeSession);
		}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region EVENT LISTENERS

		void OnEvFinalizeSession (eParam e) {
			KillStream ();
		}

		void OnEvSendMarker (eParam e) {
			SendMarker (e.GetString ());
		}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region LSL


		public void SendMarker (string _msg) {
			string[] sampleToSend = new string[] { _msg };
			outl.push_sample (sampleToSend);
		}

		void KillStream () {
			//TODO Close streams/sockets properly to not waste any system/netwerk resources

			if (outl != null)
				outl.Close ();

			if (inf != null)
				inf.Close ();

			outl = null;
			inf = null;

		}

		void OnApplicationQuit () {
			if (isStreaming)
				KillStream ();
		}
	}

#endregion // -------------------------------------------------------------------------------------------------------------------------------

}