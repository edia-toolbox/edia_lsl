using edia.lsl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMarkerSender : MonoBehaviour
{
    public MarkerOutlet MarkerOutletTest;

	private void Start() {
        // StartCoroutine(MarkerSenderTypeOne());
		// StartCoroutine(MarkerSenderTypeTwo());
		StartCoroutine(MarkerSenderTypeThree());

	}

	IEnumerator MarkerSenderTypeOne() {
        while (true) {
            yield return new WaitForSecondsRealtime(Random.Range(0.3f, 4f));

            MarkerOutletTest.SendMarker("User On Marker");
        }
    }

	IEnumerator MarkerSenderTypeTwo() {
		while (true) {
			yield return new WaitForSecondsRealtime(Random.Range(2f, 5f));

			MarkerOutletTest.SendMarker("Stimuli Created");
		}
	}

	IEnumerator MarkerSenderTypeThree() {
	while (true) {
            yield return new WaitForSecondsRealtime(Random.Range(1f, 3f));
			MarkerOutletTest.SendMarker("Marker sent without specifications");
            MarkerOutletTest.SendMarker("Marker Sent Now", MarkerOutlet.MomentForMarker.Now);
			MarkerOutletTest.SendMarker("Marker Sent EndOfFrame", MarkerOutlet.MomentForMarker.EndOfFrame);
			MarkerOutletTest.SendMarker("Marker Sent Start Of Next Frame", MarkerOutlet.MomentForMarker.StartOfFrame);
			MarkerOutletTest.SendMarker("Marker Sent With 3 Frames Delay", MarkerOutlet.MomentForMarker.StartOfFrame, 3);
        }
    }
}
