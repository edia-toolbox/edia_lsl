using System;
using UnityEngine;
using UnityEngine.Events;

public class LinkObjectParameter : MonoBehaviour {

	[Header("Object from which we grab a parameter")]
	public UnityEngine.Object targetObject;
	[Space(20)]
	[Header("TargetObjects parameter to grab. I.e. RectTransform.anchoredPosition\nValid are: Int, Float, Vector2, Vector3")]
	public string PathToParameter = string.Empty;

	[Serializable] public class Input2DValueChanged : UnityEvent<Vector2> { } // event definition to fire

	[Header("Methods to fire when value changes")]
	public Input2DValueChanged OnVector2InputValueChanged; // event link in inspector

	Type targetType = null;
	string[] path;

	private void Start() {
		targetType = targetObject.GetType();
		path = PathToParameter.Split('.');
	}

	void Update() {
		if (targetObject != null && !string.IsNullOrEmpty(PathToParameter)) {

			object currentObject = targetObject;
			Type currentType = targetType;

			foreach (var part in path) {
				if (currentObject is GameObject gameObject) {
					// Check if the part is a component name
					currentObject = gameObject.GetComponent(part);
					if (currentObject != null) {
						currentType = currentObject.GetType();
						continue;
					}
				}

				var property = currentType.GetProperty(part);
				if (property != null) {
					currentObject = property.GetValue(currentObject);
					currentType = property.PropertyType;
				}
				else {
					var field = currentType.GetField(part);
					if (field != null) {
						currentObject = field.GetValue(currentObject);
						currentType = field.FieldType;
					}
					else {
						Debug.LogError($"Property or Field '{part}' not found on '{currentType.Name}'");
						return;
					}
				}
			}

			// Now currentObject should hold the final value
			if (currentObject is float floatValue) {
				Debug.Log($"Linked Value: {floatValue}");
			}
			else if (currentObject is Vector2 vector2Value) {
				Debug.Log($"Linked Value: {vector2Value}");
				OnVector2InputValueChanged.Invoke(vector2Value);
			}
			else if (currentObject is Vector3 vector3Value) {
				Debug.Log($"Linked Value: {vector3Value}");
			}
			else if (currentObject is int intValue) {
				Debug.Log($"Linked Value: {intValue}");
			}

			// Add more cases as needed for other types
		}
	}
}
