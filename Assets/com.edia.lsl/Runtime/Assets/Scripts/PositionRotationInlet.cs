using UnityEngine;
using LSL4Unity.Utils;

namespace Edia.Lsl {

    public class PostitionRotationInlet : AFloatInlet
    {
        public Vector3 Offset = new Vector3(1.0f, 1.0f, 0.0f);
        public bool UseOffset = false;

        private PoseFormat _transformFormat = PoseFormat.PosQuat7D;
        public PoseFormat TransformFormat { get { return _transformFormat; } }

        void Reset()
        {
            StreamName = "Unity.Pose";
        }

        protected override void OnStreamAvailable()
        {
            if (ChannelCount == 7)
                _transformFormat = PoseFormat.PosQuat7D;
            else if (ChannelCount == 6)
                _transformFormat = PoseFormat.PosEul6D;
            else if (ChannelCount == 12)
                _transformFormat = PoseFormat.Transform12D;
        }

        protected override void Process(float[] newSample, double timestamp)
        {
            if (TransformFormat == PoseFormat.Transform12D)
            {
                gameObject.transform.position = new Vector3(newSample[3], newSample[7], newSample[11]);
                // TODO: Update the matrix instead.
            }
            else
            {
                gameObject.transform.position = new Vector3(newSample[0], newSample[1], newSample[2]);
                if (TransformFormat == PoseFormat.PosQuat7D)
                    gameObject.transform.rotation = new Quaternion(newSample[3], newSample[4], newSample[5], newSample[6]);
            }
            if (UseOffset) gameObject.transform.position += Offset;
        }
    }

}