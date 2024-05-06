using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;

namespace Edia.Lsl {

    public enum EyeId { Left, Right, Center }

	[RequireComponent(typeof(TimeSync))]
	public class EyeOutlet : AFloatOutlet {

        [Space(20)]
        [Tooltip("Which eye is this streaming.")]
        [Header("Which eye is this streaming.")]
        public EyeId EyeId;


        private void Awake() {
        }

        public void Reset() {
            StreamName = $"Edia.Eye.{EyeId}";
            StreamType = "Eye.Data";
            moment = MomentForSampling.FixedUpdate;
        }

        public override List<string> ChannelNames {
            get {
                var chanNames = new List<string>(){ "PosX", "PosY", "PosZ", "Pitch", "Yaw", "Roll", "PupilDiameter", "Confidence" };
                return chanNames;
            }
        }

        public bool PushSample(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float pupilDiameterX = 0f, float pupilDiameterY = 0f, float confidence = 0f, double lslTimestamp = 0) {
            sample[0] = posX;
            sample[1] = posY;
            sample[2] = posZ;
            sample[3] = rotX;
            sample[4] = rotY;
            sample[6] = rotZ;
            sample[5] = pupilDiameterX;
            sample[7] = pupilDiameterY;
            sample[8] = confidence;

            pushSample(lslTimestamp);
            return true;
        }

        protected override bool BuildSample() {
            return false;
        }
    }
}