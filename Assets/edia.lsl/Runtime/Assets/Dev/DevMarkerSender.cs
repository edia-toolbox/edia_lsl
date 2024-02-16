using edia.lsl;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class DevMarkerSender : MonoBehaviour
{
    public MarkerOutlet MarkerOutletTest;

	private void Start() {
        StartCoroutine(MarkerSenderTypeOne());
		StartCoroutine(MarkerSenderTypeTwo());
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
}
