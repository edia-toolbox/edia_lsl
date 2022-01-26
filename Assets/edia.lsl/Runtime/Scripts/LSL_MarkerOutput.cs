using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eDIA;
using LSL;
using System;

public class LSL_MarkerOutput : MonoBehaviour
{
    private StreamOutlet outl = null;
    private StreamInfo inf = null;

	public string StreamName = "eDIA.MarkerStream";
	public string StreamType = "eDIA.MarkerStream";
	public string StreamId = "Markers";

    public bool isStreaming = false;    

    void Start() {
        inf = new StreamInfo(StreamName, StreamType, 1, 0, channel_format_t.cf_string, StreamId);
        outl = new StreamOutlet(inf);
        
        isStreaming = true;

        EventManager.StartListening("EvSendMarker", OnEvSendMarker);
        EventManager.StartListening("EvFinalizeSession", OnEvFinalizeSession);
    }

    void OnDestroy() {
        EventManager.StopListening("EvSendMarker", OnEvSendMarker);
        EventManager.StopListening("EvFinalizeSession", OnEvFinalizeSession);
    }

	private void OnEvFinalizeSession(eParam e)
	{
		KillStream();
	}

	void OnEvSendMarker (eParam e) {
        SendMarker(e.GetString());
    }

	public void SendMarker(string _msg)	{
        string[] sampleToSend = new string[] { _msg };
		outl.push_sample(sampleToSend);
	}

    void KillStream () {
        //TODO Close streams/sockets properly to not waste any system/netwerk resources

        if (outl != null)
            outl.Close();

        if (inf != null)
            inf.Close();
            
        outl = null;
        inf = null;

    }

    void OnApplicationQuit() {
        if (isStreaming)
            KillStream();
    }
}