using Edia.Lsl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMarkerSender : MonoBehaviour
{
    public MarkerOutlet MarkerOutletTest;

	private void Start() {
		StartCoroutine(MarkerSender());
	}
	

	IEnumerator MarkerSender() {
	while (true) {
            yield return new WaitForSecondsRealtime(Random.Range(1f, 3f));
			MarkerOutletTest.SendMarker("Marker sent without specifications"); // this might get sent in the current frame or the next one; not really controllable
            MarkerOutletTest.SendMarker("Marker Sent Now", MarkerOutlet.CustomMomentForMarker.Now); //gets sent immediately; might lead to jitter
			MarkerOutletTest.SendMarker("Marker Sent EndOfThisFrame", MarkerOutlet.CustomMomentForMarker.EndOfThisFrame); //gets sent at the end of the current frame
			MarkerOutletTest.SendMarker("Marker Sent Start Of Next Frame", MarkerOutlet.CustomMomentForMarker.StartOfNextFrame); //gets sent at the start of the next frame; least jitter in my tests
			MarkerOutletTest.SendMarker("Marker Sent With 3 Frames Delay", 3); //gets sent at the start of the (next + 3) frame; least jitter; can be used to circumvent latency
        }
    }
}
