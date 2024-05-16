using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System;

namespace Edia.Lsl {
	[RequireComponent(typeof(TimeSync))]
	public class Vector2Outlet : AFloatOutlet {

		Vector2 currentValues;

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
		/// <param name="newValues">Update vector2 data</param>
		public void SetNewVector2DValues(Vector2 newValues) {
			currentValues = newValues;
		}

		protected override bool BuildSample() {
			sample[0] = currentValues.x;
			sample[1] = currentValues.y;
			return true;
		}
	}
}
