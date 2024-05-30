using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheDot : MonoBehaviour
{
	public Vector2 currentPosition = new();
	public Vector2 NewPosition = new();
	public float UpdateInterval = 2;
	private float elapsedTime;
	private float duration;

	void Start()
    {
		StartCoroutine(MoveTheDotRoutine());
	}

	IEnumerator MoveTheDotRoutine() {
		while (true) {
			NewPosition = new Vector2(Random.Range(-250f,250f), Random.Range(-250f, 250f));
			yield return new WaitForSecondsRealtime(UpdateInterval);
		}
	}

	private void Update() {
		currentPosition = this.GetComponent<RectTransform>().anchoredPosition;
		this.GetComponent<RectTransform>().anchoredPosition =  Vector2.Lerp(currentPosition, NewPosition, Time.deltaTime);
	}
}
