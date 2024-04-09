using LSL4Unity.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edia.lsl {

	public class SendTimedMarkers : MonoBehaviour {
		private Material mat;
		private MarkerOutlet _markerOutlet;

		protected void Start() {
			Renderer rend = GetComponent<Renderer>();
			mat = rend.material;

			StartCoroutine(ChangerRoutine());

			_markerOutlet = GetComponent<MarkerOutlet>();
		}

		IEnumerator ChangerRoutine() {
			while (true) {
				yield return new WaitForSecondsRealtime(1.5f);

				mat.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

				// Will send the marker at the moment specified under MomentForSampling (on the MarkerOutlet component) in the editor 
				_markerOutlet.SendMarker("SpecifiedInEditor");

				// Will send the marker immediately
				_markerOutlet.SendMarker("Now", MarkerOutlet.MomentForMarker.Now);

				// Will send the marker at the end of the current frame
				_markerOutlet.SendMarker("EndOfThisFrame", MarkerOutlet.MomentForMarker.EndOfFrame);

				// Will send the marker at the start of the next frame
				_markerOutlet.SendMarker("StartOfNextFrame", MarkerOutlet.MomentForMarker.StartOfFrame);

				// Will send the marker with a delay of 4 frames (and then at the beginning of the 5th frame from now)
				int nFramesDelay = 4;
				_markerOutlet.SendMarker($"With{nFramesDelay}FramesDelay", nFramesDelay);
				
			}
		}
	}
}