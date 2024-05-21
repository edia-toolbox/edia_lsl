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
        /// <summary>
        /// A LSL outlet which streams position and rotation data. 
        /// 
        /// Based on the `PositionRotationOutlet.cs` from the LSL4Unity GitHub repository ([c:] 2021; Markus Fleck; https://github.com/labstreaminglayer/LSL4Unity), 
        /// extended for usage within the EDIA framework (Felix Klotzsche, 2024).
        /// </summary>
        
        public PoseFormatEdia transformFormat = PoseFormatEdia.PosQuat7D;

        [Space(20)]
        [Tooltip("E.g. 90 for the HTC Vive")]
        [Header("Refresh rate of the Headset/display (in Hz)")]
        public float DisplayRefreshRate = 90; // TODO: This could be more user friendly

        [Space(20)]
        [Tooltip("Leave empty to use current gameobject")]
        [Header("Tracked object (default: current GameObject) and Origin of tracking space.")]
        public Transform TrackedObject = null;

        public bool UseLocalSpace = false;
        [Tooltip("Only used, when using Local Space. Leave empty to use parent of the current gameObject.")]
        public Transform Origin;

        private void Awake() {
            TrackedObject = TrackedObject == null ? gameObject.transform : TrackedObject;

            if (UseLocalSpace) {
                if (Origin == null) {
                    if (TrackedObject.transform.parent != null) {
                        Origin = TrackedObject.transform.parent;
                    } else {
                        Debug.LogWarning($"You specified to use LocalSpace but did not provide an Origin nor does the tracked GameObject ({TrackedObject.name}) have a parent. Setting up an empty at the World Origin.");
                        GameObject worldOrigin = new(name: "WorldOrigin");
                        worldOrigin.transform.position = Vector3.zero;
                        worldOrigin.transform.rotation = Quaternion.identity;
                        Origin = worldOrigin.transform;
                    }
                }
                string locStr = UseLocalSpace ? $".LocalTo{Origin.name}" : "";
                StreamName = StreamName + locStr;
            }
            else {
                if (Origin != null) {
                    Debug.LogWarning("You specified an Origin but did not indicate to use Local Space; ignoring the origin and using World Space.");
                }
            }
        }

        public void Reset() {
            StreamName = "Unity.PosRot";
            StreamType = "Unity.Transform";
            moment = MomentForSampling.EndOfFrame;
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
            acq_el.append_child_value("manufacturer", "EDIA");
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

            
            var position = UseLocalSpace ? Origin.InverseTransformPoint(TrackedObject.position) : TrackedObject.position;
            var rotation = UseLocalSpace ? Quaternion.Inverse(Origin.rotation) * TrackedObject.rotation : TrackedObject.rotation;

            sample[0] = position.x;
            sample[1] = position.y;
            sample[2] = position.z;

            if (transformFormat == PoseFormatEdia.PosEul6D) {
                sample[3] = rotation.eulerAngles.x;
                sample[4] = rotation.eulerAngles.y;
                sample[5] = rotation.eulerAngles.z;
            }
            else {
                sample[3] = rotation.x;
                sample[4] = rotation.y;
                sample[5] = rotation.z;
                sample[6] = rotation.w;
            }
        return true;
        }
    }
}