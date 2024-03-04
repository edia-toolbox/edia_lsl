using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using UnityEngine.InputSystem;
using System;

namespace edia.lsl {

	public class ControllerVector2Outlet : AFloatOutlet {

		Vector2 Values;

		[Header("Input Vector2 supplier")]
		public InputActionReference InputAction;

		private void OnEnable() {
			InputAction.action.performed += OnNewVector2DInput;
		}

		private void OnDisable() {
			InputAction.action.performed -= OnNewVector2DInput;
		}

		public void Reset() {
			StreamName = "Unity.XRController";
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

		public void OnNewVector2DInput(InputAction.CallbackContext obj) {
			Values = obj.ReadValue<Vector2>();
		}

		protected override bool BuildSample() {
			sample[0] = Values.x;
			sample[1] = Values.y;
			return true;
		}
	}
}
