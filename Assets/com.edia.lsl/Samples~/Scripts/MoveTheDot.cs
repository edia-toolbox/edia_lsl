using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheDot : MonoBehaviour
{
	public Vector2 CurrentPosition = new();
	public float CurrentX = 0f;
	public float CurrentY = 0f; 
	public Vector2 NewPosition = new();
	public float UpdateInterval = 2;
	private float elapsedTime;
	private float duration = 1;
	private float t = 0;

	/// <summary>
	/// Examples of a public field that can be 'watched' 
	/// </summary>
	public float Pulsation = 0;
	public float Scale = 0;

	void Start()
    {
		StartCoroutine(MoveTheDotRoutine());
	}

	IEnumerator MoveTheDotRoutine() {
		while (true) {
			NewPosition = new Vector2(Random.Range(-250f,250f), Random.Range(-250f, 250f));
			elapsedTime = 0;
			yield return new WaitForSecondsRealtime(UpdateInterval);
		}
	}

	private void Update() {

		Pulsation = Mathf.Sin(Time.time) * 1000f;
		Scale = Mathf.Cos(Time.time / 2) * 1000f;
		CurrentX = CurrentPosition.x;
		CurrentY = CurrentPosition.y;

		if (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			t = elapsedTime / duration;
		} 

		CurrentPosition = this.GetComponent<RectTransform>().anchoredPosition;
		this.GetComponent<RectTransform>().anchoredPosition =  Vector2.Lerp(CurrentPosition, NewPosition, t);
	}
}
