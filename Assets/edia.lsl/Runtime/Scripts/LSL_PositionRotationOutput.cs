using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using eDIA;

public class LSL_PositionRotationOutput : MonoBehaviour
{
    private StreamOutlet outlet;
    private float[] currentSample;
    public Transform zeObject;

    public string StreamName    = "";
    public string StreamType    = "";
    public string StreamId      = "";

    public bool isThere = false;

    // Start is called before the first frame update
    void Start()
    {
        StreamInfo streamInfo = new StreamInfo(StreamName, 
                                               StreamType, 
                                               6, // channel_count 
                                               1f / Time.deltaTime, //nominal_srate 
                                               LSL.channel_format_t.cf_float32, 
                                               StreamId);
        XMLElement chans = streamInfo.desc().append_child("channels");
        chans.append_child("channel").append_child_value("label", "pos_x");
        chans.append_child("channel").append_child_value("label", "pos_y");
        chans.append_child("channel").append_child_value("label", "pos_z");
        chans.append_child("channel").append_child_value("label", "rot_x");
        chans.append_child("channel").append_child_value("label", "rot_y");
        chans.append_child("channel").append_child_value("label", "rot_z");

        outlet = new StreamOutlet(streamInfo);
        currentSample = new float[6];

    }


    // FixedUpdate is a good hook for objects that are governed mostly by physics (gravity, momentum).
    // Update might be better for objects that are governed by code (stimulus, event).
    void Update()
    {
        Vector3 pos = zeObject.position;
        Vector3 rot = zeObject.rotation.eulerAngles;

        currentSample[0] = pos.x;
        currentSample[1] = pos.y;
        currentSample[2] = pos.z;
        currentSample[3] = rot.x;
        currentSample[4] = rot.y;
        currentSample[5] = rot.z;
        
        outlet.push_sample(currentSample);
    }
}