using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System;

namespace Edia.Lsl {

	[RequireComponent(typeof(TimeSync))]
	public class Vector2Outlet : AFloatOutlet {

		public enum EmptyPackageOptions { Zero, LastValue, CustomValue }
		[Space(20)]
		[Header("What to send when no new vector2 values are recieved.")]
		public EmptyPackageOptions OnNoUpdateUse = EmptyPackageOptions.Zero;
		[Header("Custom float value to send as indicator there was no new data. Default=-99")]
		public float CustomValue = -99f;

		[Header("Flag that determines if the script is allowed to push new values onto the stream. Default TRUE")]
		public bool IsAllowed = true;

		Vector2 currentValues = new();
		Vector2 oldValues = new();

		public void Reset() {
			StreamName = "Unity.Vector2";
			StreamType = "Unity.Vector2";
			moment = MomentForSampling.FixedUpdate;
		}

		public override List<string> ChannelNames {
			get {
				List<string> chanNames = new List<string>();
				chanNames.AddRange(new string[] { "x", "y" });

				return chanNames;
			}
		}

		/// <summary>
		/// Call this method to supply the stream with new values.
		/// </summary>
		/// <param name="newValues">new vector2 values</param>
		public void UpdateValues(Vector2 newValues) {
			oldValues = currentValues;
			currentValues = newValues;
		}

		public void SetAllowedTo(bool onOff) {
			IsAllowed = onOff;
		}

		protected override bool BuildSample() {
			if (!IsAllowed) {
				sample[0] = OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.x : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
				sample[1] = OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.y : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
				return true;
			} else {
				sample[0] = currentValues.x != oldValues.x ? currentValues.x : OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.x : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f;
				sample[1] = currentValues.y != oldValues.y ? currentValues.y : OnNoUpdateUse == EmptyPackageOptions.LastValue ? currentValues.y : OnNoUpdateUse == EmptyPackageOptions.CustomValue ? CustomValue : 0f; 
				return true;
			}
		}
	}
}
