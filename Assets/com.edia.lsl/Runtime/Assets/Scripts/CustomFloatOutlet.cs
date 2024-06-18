using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System;
using System.Reflection;
using System.Linq;

namespace Edia.Lsl {

	[System.Serializable]
	public class MonitoringItem {
		public UnityEngine.Component TargetComponent = null;
		public string TargetField = string.Empty;
		[HideInInspector]
		public float _lastValue = 0f;
	}

	[RequireComponent(typeof(TimeSync))]
	public class CustomFloatOutlet : AFloatOutlet {

		public enum EmptyPackageOptions { Zero, LastValue, CustomValue }
		[Space(20)]
		[Header("What to send when no new vector2 values are recieved.")]
		public EmptyPackageOptions OnNoUpdateUse = EmptyPackageOptions.Zero;
		[Header("Custom float value to send as indicator there was no new data. Default=-99")]
		public float CustomValue = -99f;

		[Space(20)]
		public List<MonitoringItem> WatchList = new();

		FieldInfo _fieldInfo = null;
		float _targetFieldValue;

		//private void CheckCheck() {

		//	if (TargetComponent != null) {
		//		fieldInfo = TargetComponent.GetType().GetField(TargetField, BindingFlags.Public | BindingFlags.Instance);

		//		if (fieldInfo != null) {
		//			targetFieldValue = (float)fieldInfo.GetValue(TargetComponent);
		//			Debug.Log("Pulsation value (field): " + targetFieldValue.ToString());
		//		}
		//		else {
		//			Debug.LogError($"No public '{TargetField}' field found on the component.");
		//		}
		//	}
		//	else {
		//		Debug.LogError("Component reference is null.");
		//	}
		//}

		// TODO Extend with field.fieldtype so we can put in Vector2, Vector3, etc.

		//float[] GetTargetValues() {

		//	List<float> tmp = new();

		//	foreach (MonitoringItem item in WatchList) {

		//		if (item.TargetComponent != null) {
		//			_fieldInfo = item.TargetComponent.GetType().GetField(item.TargetField, BindingFlags.Public | BindingFlags.Instance);

		//			if (_fieldInfo != null) {
		//				Debug.Log($"Type: {_fieldInfo.FieldType.Name}");

		//				switch (_fieldInfo.FieldType.Name) {
		//					case "Single": // == float
		//						tmp.Add((float)_fieldInfo.GetValue(item.TargetComponent));
		//						break;
		//				}
		//			}
		//			else {
		//				Debug.LogError($"No public '{item.TargetField}' field found on the component.");
		//				return float.NaN;
		//			}
		//		}
		//		else {
		//			Debug.LogError("Component reference is null.");
		//			return float.NaN;
		//		}
		//	}
		//	return tmp.ToArray();
		//}

		//private void Update() {
		//	CheckCheck();
		//}

		float GetTargetValue(int index) {

			if (WatchList[index].TargetComponent != null) {

				_fieldInfo = WatchList[index].TargetComponent.GetType().GetField(WatchList[index].TargetField, BindingFlags.Public | BindingFlags.Instance);

				if (_fieldInfo != null) {
					float newValue = (float)_fieldInfo.GetValue(WatchList[index].TargetComponent);
					return newValue != WatchList[index]._lastValue ? newValue : OnNoUpdateUse == EmptyPackageOptions.LastValue ? newValue : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
					//return ((float)_fieldInfo.GetValue(WatchList[index].TargetComponent));
				}
				else {
					Debug.LogError($"No public '{WatchList[index].TargetField}' field found on the component.");
					return float.NaN;
				}
			}
			else {
				Debug.LogError("Component reference is null.");
				return float.NaN;
			}
		}

		public void Reset() {
			StreamName = "Unity.Floats";
			StreamType = "Unity.Floats";
			moment = MomentForSampling.EndOfFrame;
		}

		public override List<string> ChannelNames {
			get {
				List<string> chanNames = new List<string>();
				foreach (MonitoringItem m in WatchList) {
					chanNames.Add(m.TargetField);
				}

				return chanNames;
			}
		}

		protected override bool BuildSample() {

			for (int i = 0; i < WatchList.Count; i++) {
				sample[i] = GetTargetValue(i);
			}

			//sample[0] = currentValues.x != oldValues.x ? currentValues.x : OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.x : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
			//sample[1] = currentValues.y != oldValues.y ? currentValues.y : OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.y : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f; 
			return true;
		}
	}
}
