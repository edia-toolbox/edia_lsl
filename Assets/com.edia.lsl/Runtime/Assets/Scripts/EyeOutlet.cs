using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using System.Xml.Linq;
using UnityEditor.PackageManager.UI;

namespace Edia.Lsl {

    public enum EyeIdLsl { Left, Right, Center }

	public class EyeOutlet : MonoBehaviour {

        [Space(20)]
        [Tooltip("Which eye is this streaming.")]
        [Header("Which eye is this streaming.")]
        public EyeIdLsl EyeIdLsl;



        public string StreamName;
        public string StreamType;
        public bool IrregularRate = false;
        public int SamplingRate;
        private bool UniqueFromInstanceId = true;

        private List<string> _channelNames {
            get {
                var chanNames = new List<string>() { "PosX", "PosY", "PosZ", "Pitch", "Yaw", "Roll", "PupilDiameter", "PupilDiameterX", "PupilDiameterY", "Confidence" };
                return chanNames;
            }
        }
        public int _channelCount { get { return _channelNames.Count; } }

        // We'region only sending floats for now
        public channel_format_t Format { get { return channel_format_t.cf_float32; } }

        private StreamOutlet _outlet;
        private float[] _sample;

        // Add an XML element for each channel. The automatic version adds only channel labels. Override to add unit, location, etc.
        private void FillChannelsHeader(XMLElement channels) {
            foreach (var chanName in _channelNames) {
                XMLElement chan = channels.append_child("channel");
                chan.append_child_value("label", chanName);
            }
        }

        private void Start() {
            _sample = new float[_channelCount];

            var hash = new Hash128();
            hash.Append(StreamName);
            hash.Append(StreamType);
            if (UniqueFromInstanceId)
                hash.Append(gameObject.GetInstanceID());

            double dataRate = IrregularRate ? LSL.LSL.IRREGULAR_RATE : SamplingRate;
            StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, _channelCount, dataRate, Format, hash.ToString());

            // Build XML header. See xdf wiki for recommendations: https://github.com/sccn/xdf/wiki/Meta-Data
            XMLElement acq_el = streamInfo.desc().append_child("acquisition");
            acq_el.append_child_value("manufacturer", "EDIA");
            XMLElement channels = streamInfo.desc().append_child("channels");
            FillChannelsHeader(channels);

            _outlet = new StreamOutlet(streamInfo);
        }

        private void Awake() {
        }

        public bool PushSample(float posX, float posY, float posZ,
                               float rotX, float rotY, float rotZ,
                               float pupilDiameter = 0f,
                               float confidence = 0f,
                               float timestampEt = 0f,
                               double timestampLsl = 0) {
            _sample[0] = posX;
            _sample[1] = posY;
            _sample[2] = posZ;
            _sample[3] = rotX;
            _sample[4] = rotY;
            _sample[5] = rotZ;
            _sample[6] = pupilDiameter;
            _sample[9] = confidence;
            _sample[10] = timestampEt;

            pushSample(timestampLsl);
            return true;
        }


        private void pushSample(double timestamp = 0) {
            if (_outlet == null)
                return;
            _outlet.push_sample(_sample, timestamp);
        }
    }
}