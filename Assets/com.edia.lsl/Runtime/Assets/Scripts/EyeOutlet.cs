using System.Collections.Generic;
using UnityEngine;
using LSL;

namespace Edia.Lsl {

    public class EyeOutlet : MonoBehaviour, ILslEyeOutlet {

        public enum EyeTrackingSamplingRate { ViveProEye_120Hz, QuestPro_30Hz, VarjoAero_100Hz, VarjoAero_200Hz, IDoNotKnow }

        [field: Space(20)]
        [Tooltip("Which eye is this streaming.")]
        [field: Header("Which eye?")]
        [SerializeField]
        private Edia.Constants.EyeId _eyeId = Constants.EyeId.CENTER;
        public Edia.Constants.EyeId EyeId {
            get => _eyeId;
            set {
                _eyeId = value;
            }
        }

        public string StreamName = "EDIA.Eye";
        public string StreamType = "Eye.Data";
        public bool IrregularRate = false;
        [Tooltip("How fast does your eye tracker provide samples?")]
        public EyeTrackingSamplingRate ExpectedSamplingRate = EyeTrackingSamplingRate.ViveProEye_120Hz;
        public int _channelCount { get { return _channelNames.Count; } }

        [Tooltip("Save rotation of eyeball in EulerAngles Angles or Quaternions?")]
        public RotationFormat EyeRotationFormat = RotationFormat.EulerAngles;

        // We'region only sending floats for now
        public channel_format_t Format { get { return channel_format_t.cf_float32; } }

        private StreamOutlet _outlet;
        private double[] _sample;
        private bool UniqueFromInstanceId = true;

        private List<string> _channelNames {
            get {
                var chanNames = new List<string>();
                if (EyeRotationFormat == RotationFormat.EulerAngles) {
                    chanNames = new List<string>() { "PosX", "PosY", "PosZ", "Pitch", "Yaw", "Roll", "PupilDiameter", "Openness", "Confidence", "TimestampET" };
                } else if (EyeRotationFormat == RotationFormat.Quaternion) {
                    chanNames = new List<string>() { "PosX", "PosY", "PosZ", "RotW", "RotX", "RotY", "RotZ", "PupilDiameter", "Openness", "Confidence", "TimestampET" };
                } else {
                    Debug.LogError("Unknown RotationFormat for EyePose.");
                }
                return chanNames;
            }
        }


        // Add an XML element for each channel. The automatic version adds only channel labels. Override to add unit, location, etc.
        private void FillChannelsHeader(XMLElement channels) {
            foreach (var chanName in _channelNames) {
                XMLElement chan = channels.append_child("channel");
                chan.append_child_value("label", chanName);
            }
        }

        private void Start() {
            _sample = new double[_channelCount];

            StreamName += $".{EyeId}";

            var hash = new Hash128();
            hash.Append(StreamName);
            hash.Append(StreamType);
            if (UniqueFromInstanceId)
                hash.Append(gameObject.GetInstanceID());

            double samplingRate = 0;
            if (!IrregularRate) {
                switch (ExpectedSamplingRate) {
                    case EyeTrackingSamplingRate.ViveProEye_120Hz:
                        samplingRate = 120;
                        break;
                    case EyeTrackingSamplingRate.QuestPro_30Hz:
                        samplingRate = 30;
                        break;
                    case EyeTrackingSamplingRate.VarjoAero_100Hz:
                        samplingRate = 100;
                        break;
                    case EyeTrackingSamplingRate.VarjoAero_200Hz:
                        samplingRate = 200;
                        break;
                    case EyeTrackingSamplingRate.IDoNotKnow:
                        IrregularRate = true;
                        samplingRate = 0;
                        Debug.Log("You did not specify an eye tracking sampling rate; setting this to an irregular stream.");
                        break;
                    default:
                        IrregularRate = true;
                        samplingRate = 0;
                        Debug.Log("No appropriate sampling rate set; setting this to an irregular stream.");
                        break;
                }
            }

            double dataRate = IrregularRate ? LSL.LSL.IRREGULAR_RATE : samplingRate;
            StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, _channelCount, dataRate, Format, hash.ToString());

            // Build XML header. See xdf wiki for recommendations: https://github.com/sccn/xdf/wiki/Meta-Data
            XMLElement acq_el = streamInfo.desc().append_child("acquisition");
            acq_el.append_child_value("manufacturer", "EDIA");
            XMLElement channels = streamInfo.desc().append_child("channels");
            FillChannelsHeader(channels);

            _outlet = new StreamOutlet(streamInfo);
        }

        public double GetLslTime() {
            return LSL.LSL.local_clock();
        }

        private void Awake() {
        }


        ///<summary>
        /// Builds and pushes a sample with eye tracking data.
        /// </summary>
        /// <param name="eyePositionLocal">The local eye position.</param>
        /// <param name="eyeRotationLocalEuler">The local eye rotation in Euler Angles.</param>
        /// <param name="pupilDiameter">The diameter of the pupil. Default is 0f.</param>
        /// <param name="openness">The openness of the eye. Default is 0f.</param>
        /// <param name="confidence">The confidence level of the eye tracking data. Default is 0f.</param>
        /// <param name="timestampEt">The eye tracker timestamp. Default is 0f.</param>
        /// <param name="timestampLsl">The LSL timestamp. Default is 0 and will timestamp the sample on sending.</param>
        public void PushSample(Vector3 eyePositionLocal, Vector3 eyeRotationLocalEuler, float pupilDiameter = 0f, float openness = 0f,
                               float confidence = 0f, double timestampEt = 0f, double timestampLsl = 0) {

            if (EyeRotationFormat == RotationFormat.EulerAngles) {

                _sample[0] = eyePositionLocal.x;
                _sample[1] = eyePositionLocal.y;
                _sample[2] = eyePositionLocal.z;
                _sample[3] = eyeRotationLocalEuler.x;
                _sample[4] = eyeRotationLocalEuler.y;
                _sample[5] = eyeRotationLocalEuler.z;
                _sample[6] = pupilDiameter;
                _sample[7] = openness;
                _sample[8] = confidence;
                _sample[9] = timestampEt;

                pushSample(timestampLsl);

            } else if (EyeRotationFormat == RotationFormat.Quaternion) {

                Quaternion rotAsQuaternion = Quaternion.Euler(eyeRotationLocalEuler.x, eyeRotationLocalEuler.y, eyeRotationLocalEuler.z);

                PushSample(eyePositionLocal, rotAsQuaternion, pupilDiameter, openness, confidence, timestampEt, timestampLsl);
            }
        }


        ///<summary>
        /// Builds and pushes a sample with eye tracking data.
        /// </summary>
        /// <param name="eyePositionLocal">The local eye position.</param>
        /// <param name="eyeRotationLocalQuaternion">The local eye rotation as Quaternion.</param>
        /// <param name="pupilDiameter">The diameter of the pupil. Default is 0f.</param>
        /// <param name="openness">The openness of the eye. Default is 0f.</param>
        /// <param name="confidence">The confidence level of the eye tracking data. Default is 0f.</param>
        /// <param name="timestampEt">The eye tracker timestamp. Default is 0f.</param>
        /// <param name="timestampLsl">The LSL timestamp. Default is 0 and will timestamp the sample on sending.</param>
        public void PushSample(Vector3 eyePositionLocal, Quaternion eyeRotationLocalQuaternion, float pupilDiameter = 0f, float openness = 0f,
                               float confidence = 0f, double timestampEt = 0f, double timestampLsl = 0) {

            if (EyeRotationFormat == RotationFormat.Quaternion) {
                _sample[0] = eyePositionLocal.x;
                _sample[1] = eyePositionLocal.y;
                _sample[2] = eyePositionLocal.z;
                _sample[3] = eyeRotationLocalQuaternion.w; // watch out for wxyz sequence — see header!
                _sample[4] = eyeRotationLocalQuaternion.x;
                _sample[5] = eyeRotationLocalQuaternion.y;
                _sample[6] = eyeRotationLocalQuaternion.z;
                _sample[7] = pupilDiameter;
                _sample[8] = openness;
                _sample[9] = confidence;
                _sample[10] = timestampEt;

                pushSample(timestampLsl);
            } else if (EyeRotationFormat == RotationFormat.EulerAngles) {

                Vector3 rotAsEuler = eyeRotationLocalQuaternion.eulerAngles;

                PushSample(eyePositionLocal, rotAsEuler, pupilDiameter, openness, confidence, timestampEt, timestampLsl);
            }
        }


        private void pushSample(double timestamp = 0) {
            if (_outlet == null)
                return;
            _outlet.push_sample(_sample, timestamp);
        }
    }
}