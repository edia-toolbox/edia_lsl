using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System;

namespace Edia.Lsl {

	[RequireComponent(typeof(TimeSync))]
	public class Vector2Outlet : AFloatOutlet {

		public enum EmptyPackageValues { Zero, LastValue }
		[Space(20)]
		[Header("What to send when no new vector2 values are recieved.")]
		public EmptyPackageValues OnNoUpdateUse = EmptyPackageValues.Zero;

		Vector2 currentValues;
		Vector2 oldValues;

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

		protected override bool BuildSample() {
			sample[0] = currentValues.x != oldValues.x ? currentValues.x : OnNoUpdateUse == EmptyPackageValues.LastValue ? currentValues.x : 0f;
			sample[1] = currentValues.y != oldValues.y ? currentValues.y : OnNoUpdateUse == EmptyPackageValues.LastValue ? currentValues.y : 0f;
			return true;
		}
	}
}
