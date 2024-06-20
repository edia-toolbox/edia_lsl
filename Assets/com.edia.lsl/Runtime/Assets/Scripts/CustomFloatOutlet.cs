using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System;
using System.Reflection;
using System.Linq;

namespace Edia.Lsl {

	/// <summary>
	/// Definition of one pointer to a float field to monitor.
	/// </summary>
	[System.Serializable]
	public class MonitoringItem {
		public UnityEngine.Component TargetComponent = null;
		public string TargetField = string.Empty;
		[HideInInspector]
		public float _lastValue = 0f;
	}

	/// <summary>
	/// A LSL outlet which streams one stream with all float values listed in the watchlist
	/// </summary>
	[RequireComponent(typeof(TimeSync))]
	public class CustomFloatOutlet : AFloatOutlet {

		public enum EmptyPackageOptions { Zero, LastValue, CustomValue }
		[Space(20)]
		[Header("References to components and fields")]
		public List<MonitoringItem> WatchList = new();

		[Space(20)]
		[Header("What to send when no new vector2 values are recieved.")]
		public EmptyPackageOptions OnNoUpdateUse = EmptyPackageOptions.Zero;
		[Header("Custom float value to send as indicator there was no new data. Default=-99")]
		public float CustomValue = -99f;

		FieldInfo _fieldInfo = null;
		float _targetFieldValue;

		/// <summary>Gets float value from given components<>field combo in the watchlist</summary>
		/// <param name="index">Index in watchlist</param>
		/// <returns>Float value, float.NaN if not found</returns>
		float GetTargetValue(int index) {

			if (WatchList[index].TargetComponent != null) {

				_fieldInfo = WatchList[index].TargetComponent.GetType().GetField(WatchList[index].TargetField, BindingFlags.Public | BindingFlags.Instance);

				if (_fieldInfo != null) {
					float newValue = (float)_fieldInfo.GetValue(WatchList[index].TargetComponent);
					return newValue != WatchList[index]._lastValue ? newValue : OnNoUpdateUse == EmptyPackageOptions.LastValue ? newValue : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
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
			return true;
		}
	}
}
