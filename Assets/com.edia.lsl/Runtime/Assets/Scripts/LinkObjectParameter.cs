using System;
using UnityEngine;
using UnityEngine.Events;

namespace Edia.Lsl {

	/// <summary>
	/// This script grabs the parameter value of a component of the given targetobject on each update. \nAnd passes it along in a event hook configurable by the user.
	/// </summary>
	public class LinkObjectParameter : MonoBehaviour {
		// TODO Find better name for this script

		[Header("Target")]
		[Tooltip("The target object from which to grab the parameter")]
		public UnityEngine.Object targetObject;
		[Space(20)]

		[Header("Referencing the parameter as a string. \nThis is the search path on the gameobject.\n\nExample: RectTransform.AnchoredPosition -> Vector2 parameter")]
		[Tooltip("Case sensitive. Expects script API namings.")]
		public string PathToParameter = string.Empty;

		[Header("Method hooks")]
		public UnityEvent<float> OnFloatUpdate = new();
		public UnityEvent<Vector2> OnVector2Update = new();
		public UnityEvent<Vector3> OnVector3Update = new();
		public UnityEvent<int> OnIntUpdate = new();

		Type targetType = null;
		Type currentType = null;
		string[] path;
		object currentObject;

		private void Start() {
			targetType = targetObject.GetType();
			path = PathToParameter.Split('.');
		}

		void Update() {
			if (targetObject != null && !string.IsNullOrEmpty(PathToParameter)) {

				currentObject = targetObject;
				currentType = targetType;

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
					OnFloatUpdate.Invoke(floatValue);
				}
				else if (currentObject is Vector2 vector2Value) {
					OnVector2Update.Invoke(vector2Value);
				}
				else if (currentObject is Vector3 vector3Value) {
					OnVector3Update.Invoke(vector3Value);
				}
				else if (currentObject is int intValue) {
					OnIntUpdate.Invoke(intValue);
				}
			}
		}
	}
}