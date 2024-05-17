using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using LSL;
using LSL4Unity.Utils;
using System.Xml.Linq;
using System;

namespace Edia.Lsl {

    public enum PoseFormatEdia { PosEul6D, PosQuat7D }

	[RequireComponent(typeof(TimeSync))]
	public class EdiaPositionRotationOutlet : AFloatOutlet {
        public PoseFormatEdia transformFormat = PoseFormatEdia.PosQuat7D;

        [Space(20)]
        [Tooltip("E.g. 90 for the HTC Vive")]
        [Header("Refresh rate of the Headset/display (in Hz)")]
        public float DisplayRefreshRate = 90; // TODO: implement more user friendly

        [Space(20)]
        [Tooltip("Leave empty to use current gameobject")]
        [Header("Leave empty to use current gameobject")]
        public Transform Target = null;

        public bool UseLocalSpace = false;  //TODO: make more robust

        private void Awake() {
            Target = Target == null ? gameObject.transform : Target;
        }

        public void Reset() {
            string locStr = UseLocalSpace ? ".local" : "";
            StreamName = $"Unity.PosRot{locStr}";
            StreamType = "Unity.Transform";
            moment = MomentForSampling.FixedUpdate;
        }

        public override List<string> ChannelNames {
            get {
                List<string> chanNames = new List<string>();
                if ((transformFormat == PoseFormatEdia.PosEul6D) || (transformFormat == PoseFormatEdia.PosQuat7D)) {
                    chanNames.AddRange(new string[] { "PosX", "PosY", "PosZ" });

                    if (transformFormat == PoseFormatEdia.PosEul6D) {
                        chanNames.AddRange(new string[] { "Pitch", "Yaw", "Roll" });
                    }
                    else {
                        chanNames.AddRange(new string[] { "RotX", "RotY", "RotZ", "RotW" });
                    }
                }
                return chanNames;
            }
        }

        protected override void Start() {
            timeSync = gameObject.GetComponent<TimeSync>();
            sample = new float[ChannelCount];

            var hash = new Hash128();
            hash.Append(StreamName);
            hash.Append(StreamType);
            hash.Append(moment.ToString());
            hash.Append(gameObject.GetInstanceID());
            ExtendHash(hash);

            double dataRate = IrregularRate ? LSL.LSL.IRREGULAR_RATE : GetSamplingRateFor(moment);
            StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, ChannelCount, dataRate, Format, hash.ToString());

            // Build XML header. See xdf wiki for recommendations: https://github.com/sccn/xdf/wiki/Meta-Data
            XMLElement acq_el = streamInfo.desc().append_child("acquisition");
            acq_el.append_child_value("manufacturer", "LSL4Unity");
            XMLElement channels = streamInfo.desc().append_child("channels");
            FillChannelsHeader(channels);

            outlet = new StreamOutlet(streamInfo);

        }

        private double GetSamplingRateFor(MomentForSampling moment) {
            float samplingRateInHertz = 0;

            if (moment == MomentForSampling.FixedUpdate)
                samplingRateInHertz = 1000 / (1000 * Time.fixedDeltaTime);

            if (moment == MomentForSampling.Update || moment == MomentForSampling.LateUpdate || moment == MomentForSampling.EndOfFrame) {

                samplingRateInHertz = DisplayRefreshRate;
            }
            return samplingRateInHertz;
        }


        protected override void ExtendHash(Hash128 hash) {
            hash.Append(transformFormat.ToString());
        }

        protected override bool BuildSample() {
            var position = UseLocalSpace ? Target.localPosition : Target.position;

            sample[0] = position.x;
            sample[1] = position.y;
            sample[2] = position.z;

            if (transformFormat == PoseFormatEdia.PosEul6D) {
                var rotation = UseLocalSpace ? Target.localRotation.eulerAngles : Target.eulerAngles;
                sample[3] = rotation.x;
                sample[4] = rotation.y;
                sample[5] = rotation.z;
            }
            else {
                var rotation = UseLocalSpace ? Target.localRotation : Target.rotation;
                sample[3] = rotation.x;
                sample[4] = rotation.y;
                sample[5] = rotation.z;
                sample[6] = rotation.w;
            }
        return true;
        }
    }
}