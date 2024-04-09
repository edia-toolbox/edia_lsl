using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edia.lsl {

	public class ChangePlaneColourSendMarker : MonoBehaviour {
		private Material mat;

		protected void Start() {
			Renderer rend = GetComponent<Renderer>();
			mat = rend.material;

			StartCoroutine(ChangerRoutine());
		}

		IEnumerator ChangerRoutine() {
			while (true) {
				yield return new WaitForSecondsRealtime(1.5f);

				mat.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

				// Will send the marker at the moment specified under MomentForSampling (on the MarkerOutlet component) in the editor 
				GetComponent<MarkerOutlet>().SendMarker(mat.color.ToString());
			}
		}
	}
}