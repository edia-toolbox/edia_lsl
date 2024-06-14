using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;
using LSL;
using LSL4Unity.Utils;
using System.Xml.Linq;

namespace Edia.Lsl {

	public class EyeOutlet : MonoBehaviour, ILslPusher {

        public enum EyeTrackingSamplingRate {ViveProEye_120Hz, QuestPro_72Hz, QuestPro_90Hz, VarjoAero_100Hz, VarjoAero_200Hz, IDoNotKnow}

        [field: Space(20)]
        [Tooltip("Which eye is this streaming.")]
        [field: Header("Which eye?")]
        [SerializeField]
        private Constants.EyeId _eyeId = Constants.EyeId.CENTER;
        public Constants.EyeId EyeId { 
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

        [Tooltip("Save rotation of eyeball in Euler Angles or Quaternions?")]
        public PoseFormatEdia EyePoseFormat = PoseFormatEdia.PosQuat7D;

        // We'region only sending floats for now
        public channel_format_t Format { get { return channel_format_t.cf_float32; } }

        private StreamOutlet _outlet;
        private float[] _sample;
        private bool UniqueFromInstanceId = true;

        private List<string> _channelNames {
            get {
                var chanNames = new List<string>();
                if (EyePoseFormat == PoseFormatEdia.PosEul6D) {
                    chanNames = new List<string>() { "PosX", "PosY", "PosZ", "Pitch", "Yaw", "Roll", "PupilDiameter", "Confidence", "TimestampET" };
                } else if (EyePoseFormat == PoseFormatEdia.PosQuat7D) {
                    chanNames = new List<string>() { "PosX", "PosY", "PosZ", "RotW", "RotX", "RotY", "RotZ", "PupilDiameter", "Confidence", "TimestampET" };
                } else {
                    Debug.LogError("Unknown PoseFormat for EyePose.");
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
            _sample = new float[_channelCount];

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
                    case EyeTrackingSamplingRate.QuestPro_72Hz:
                        samplingRate = 72;
                        break;
                    case EyeTrackingSamplingRate.QuestPro_90Hz:
                        samplingRate = 90;
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

        public double GetTime() {
            return LSL.LSL.local_clock();
        }

        private void Awake() {
        }

        public void PushSample(float posX, float posY, float posZ,
                               float rotX, float rotY, float rotZ,
                               float pupilDiameter = 0f,
                               float confidence = 0f,
                               float timestampEt = 0f,
                               double timestampLsl = 0) {
            ///<summary>
            /// Builds and pushes a sample with eye tracking data.
            /// </summary>
            /// <param name="posX">The x-coordinate of the local eye position.</param>
            /// <param name="posY">The y-coordinate of the local eye position.</param>
            /// <param name="posZ">The z-coordinate of the local eye position.</param>
            /// <param name="rotX">The x-component of the local eye rotation in euler Angles.</param>
            /// <param name="rotY">The y-component of the local eye rotation in euler Angles.</param>
            /// <param name="rotZ">The z-component of the local eye rotation in euler Angles.</param>
            /// <param name="pupilDiameter">The diameter of the pupil. Default is 0f.</param>
            /// <param name="confidence">The confidence level of the eye tracking data. Default is 0f.</param>
            /// <param name="timestampEt">The eye tracker timestamp. Default is 0f.</param>
            /// <param name="timestampLsl">The LSL timestamp. Default is 0 and will timestamp the sample on sending.</param>
            _sample[0] = posX;
            _sample[1] = posY;
            _sample[2] = posZ;
            _sample[3] = rotX;
            _sample[4] = rotY;
            _sample[5] = rotZ;
            _sample[6] = pupilDiameter;
            _sample[7] = confidence;
            _sample[8] = timestampEt;

            pushSample(timestampLsl);
        }

        public void PushSample(float posX, float posY, float posZ,
                               float rotW, float rotX, float rotY, float rotZ,
                               float pupilDiameter = 0f,
                               float confidence = 0f,
                               float timestampEt = 0f,
                               double timestampLsl = 0) {
            ///<summary>
            /// Builds and pushes a sample with eye tracking data.
            /// </summary>
            /// <param name="posX">The x-coordinate of the local eye position.</param>
            /// <param name="posY">The y-coordinate of the local eye position.</param>
            /// <param name="posZ">The z-coordinate of the local eye position.</param>
            /// <param name="rotW">The w-component of the local eye rotation in quaternions.</param>
            /// <param name="rotX">The x-component of the local eye rotation in quaternions.</param>
            /// <param name="rotY">The y-component of the local eye rotation in quaternions.</param>
            /// <param name="rotZ">The z-component of the local eye rotation in quaternions.</param>
            /// <param name="pupilDiameter">The diameter of the pupil. Default is 0f.</param>
            /// <param name="confidence">The confidence level of the eye tracking data. Default is 0f.</param>
            /// <param name="timestampEt">The eye tracker timestamp. Default is 0f.</param>
            /// <param name="timestampLsl">The LSL timestamp. Default is 0 and will timestamp the sample on sending.</param>
            _sample[0] = posX;
            _sample[1] = posY;
            _sample[2] = posZ;
            _sample[3] = rotW;
            _sample[4] = rotX;
            _sample[5] = rotY;
            _sample[6] = rotZ;
            _sample[7] = pupilDiameter;
            _sample[8] = confidence;
            _sample[9] = timestampEt;

            pushSample(timestampLsl);
        }




        private void pushSample(double timestamp = 0) {
            if (_outlet == null)
                return;
            _outlet.push_sample(_sample, timestamp);
        }
    }
}