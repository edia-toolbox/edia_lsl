using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;

namespace Edia.Lsl {

    public enum PoseFormat { PosEul6D, PosQuat7D, Transform12D }

	/// <summary>
	/// A LSL outlet which streams position and rotation data. 
	/// Based on the `PositionRotationOutlet.cs` from the LSL4Unity GitHub repository ([c:] 2021; Markus Fleck; https://github.com/labstreaminglayer/LSL4Unity), 
	/// </summary>
	[RequireComponent(typeof(TimeSync))]
	public class PosRotDefaultOutlet : AFloatOutlet {
        public PoseFormat transformFormat = PoseFormat.PosQuat7D;

        [Space(20)]
        [Header("none = current gameobject")]
        public Transform Target = null;

        private void Awake() {
            Target = Target == null ? gameObject.transform : Target;
        }

        public void Reset() {
            StreamName = "Unity.PosRot";
            StreamType = "Unity.Transform";
            moment = MomentForSampling.FixedUpdate;
        }

        public override List<string> ChannelNames {
            get {
                List<string> chanNames = new List<string>();
                if ((transformFormat == PoseFormat.PosEul6D) || (transformFormat == PoseFormat.PosQuat7D)) {
                    chanNames.AddRange(new string[] { "PosX", "PosY", "PosZ" });

                    if (transformFormat == PoseFormat.PosEul6D) {
                        chanNames.AddRange(new string[] { "Pitch", "Yaw", "Roll" });
                    }
                    else {
                        chanNames.AddRange(new string[] { "RotX", "RotY", "RotZ", "RotW" });
                    }
                }
                else if (transformFormat == PoseFormat.Transform12D) {
                    var pose = gameObject.transform.localToWorldMatrix;
                    for (int row_ix = 0; row_ix < 3; row_ix++) {
                        for (int col_ix = 0; col_ix < 4; col_ix++) {
                            chanNames.Add(string.Format("{0},{1}", row_ix, col_ix));
                        }
                    }
                }
                return chanNames;
            }
        }

        protected override void ExtendHash(Hash128 hash) {
            hash.Append(transformFormat.ToString());
        }

        protected override bool BuildSample() {
            if ((transformFormat == PoseFormat.PosEul6D) || (transformFormat == PoseFormat.PosQuat7D)) {
                var position = Target.position;
                sample[0] = position.x;
                sample[1] = position.y;
                sample[2] = position.z;

                if (transformFormat == PoseFormat.PosEul6D) {
                    var rotation = Target.eulerAngles;
                    sample[3] = rotation.x;
                    sample[4] = rotation.y;
                    sample[5] = rotation.z;
                }
                else {
                    var rotation = Target.rotation;
                    sample[3] = rotation.x;
                    sample[4] = rotation.y;
                    sample[5] = rotation.z;
                    sample[6] = rotation.w;
                }
            }
            else if (transformFormat == PoseFormat.Transform12D) {
                var pose = Target.localToWorldMatrix;
                for (int row_ix = 0; row_ix < 3; row_ix++) {
                    for (int col_ix = 0; col_ix < 4; col_ix++) {
                        sample[col_ix + 4 * row_ix] = pose[row_ix, col_ix];
                    }
                }
            }
            return true;
        }
    }
}