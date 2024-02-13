using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;

    public class Vector2Outlet : AFloatOutlet
    {
        public void Reset()
        {
            StreamName = "Unity.Vector2";
            StreamType = "Unity.Transform";
            moment = MomentForSampling.FixedUpdate;
        }

        public override List<string> ChannelNames
        {
            get
            {
                List<string> chanNames = new List<string>();
                chanNames.AddRange(new string[] { "X", "Y" });
                
                return chanNames;
            }
        }

        protected override bool BuildSample()
        {
            var position = gameObject.transform.position;
            sample[0] = position.x;
            sample[1] = position.y;
            return true;
        }
    }
