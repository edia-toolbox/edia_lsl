using System;
using System.Collections;
using System.Collections.Generic;
using eDIA;
using LSL;
using UnityEngine;
using ViveSR.anipal.Eye;

public class LSL_EyeTrackingOutput : MonoBehaviour {

	private StreamOutlet outlet	= null;
	private StreamInfo streamInfo = null;
	private float[] currentSample;

	public string StreamName = "eDIA.Eyetracking";
	public string StreamType = "eDIA.Eyetracking";
	public string StreamId = "eDIA.Eyetracking";

	public bool isStreaming = false;

	private readonly GazeIndex[] idxPriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

	void Start() {
		// Listen to our ExperimentManager
		EventManager.StartListening("EvStartExperiment", OnEvStartExperiment);
		EventManager.StartListening("EvFinalizeSession", OnEvFinalizeSession);
	}

	void OnDestroy() {
		EventManager.StopListening("EvStartExperiment", OnEvStartExperiment);
		EventManager.StopListening("EvFinalizeSession", OnEvFinalizeSession);
	}

#region EVENT HANDLERS

	private void OnEvStartExperiment(eParam e)
	{
		StartStream();
	}


	private void OnEvFinalizeSession(eParam e)
	{
		// Stop the LSL stream
		KillStream();
	}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region STARTSTOPPERS

	void CreateStream () {

		streamInfo = new StreamInfo (StreamName,
			StreamType,
			10,
			1f / Time.deltaTime,
			LSL.channel_format_t.cf_float32,
			StreamId);

		XMLElement chans = streamInfo.desc ().append_child ("channels");
		chans.append_child ("channel").append_child_value ("label", "gaze_origin_x");
		chans.append_child ("channel").append_child_value ("label", "gaze_origin_y");
		chans.append_child ("channel").append_child_value ("label", "gaze_origin_z");
		chans.append_child ("channel").append_child_value ("label", "gaze_direction_x");
		chans.append_child ("channel").append_child_value ("label", "gaze_direction_y");
		chans.append_child ("channel").append_child_value ("label", "gaze_direction_z");

		chans.append_child ("channel").append_child_value ("label", "eye_openness_left");
		chans.append_child ("channel").append_child_value ("label", "eye_openness_right");
		chans.append_child ("channel").append_child_value ("label", "pupil_diameter_left");
		chans.append_child ("channel").append_child_value ("label", "pupil_diameter_right");

		outlet = new StreamOutlet (streamInfo);
		currentSample = new float[10];
	}

	void StartStream () {
		if (streamInfo == null) 
			CreateStream ();

		isStreaming = true;
	}

	void KillStream () {
		isStreaming = false;
		
		//TODO Close streams/sockets properly to not waste any system/netwerk resources
		outlet.Close();
		streamInfo.Close();
		outlet.Dispose();
		streamInfo.Dispose();
		outlet	= null;
		streamInfo = null;
	}


#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region SEND DATA

	// FixedUpdate is a good hook for objects that are governed mostly by physics (gravity, momentum).
	// Update might be better for objects that are governed by code (stimulus, event).
	void Update () {

		if (!isStreaming)
			return;

		Vector3 gazeOriginCombinedLocal = new Vector3 (float.NaN, float.NaN, float.NaN);
		Vector3 gazeDirectionCombinedLocal = new Vector3 (float.NaN, float.NaN, float.NaN);;

		bool gaze = false;
		// foreach (var idx in idxPriority)
		// {
		// 	gaze = SRanipal_Eye_v2.GetGazeRay(idx, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal);
		// 	if (gaze) break;
		// }

		gaze = SRanipal_Eye_v2.GetGazeRay (0, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal);

		if (gaze) {
			Vector3 gazeOriginCombined = XRrigUtilities.GetXRcam ().TransformPoint (gazeOriginCombinedLocal);
			Vector3 gazeDirectionCombined = XRrigUtilities.GetXRcam ().TransformDirection (gazeDirectionCombinedLocal);

			currentSample[0] = gazeOriginCombined.x;
			currentSample[1] = gazeOriginCombined.y;
			currentSample[2] = gazeOriginCombined.z;
			currentSample[3] = gazeDirectionCombined.x;
			currentSample[4] = gazeDirectionCombined.y;
			currentSample[5] = gazeDirectionCombined.z;
		} else {
			currentSample[0] = 0;
			currentSample[1] = 0;
			currentSample[2] = 0;
			currentSample[3] = 0;
			currentSample[4] = 0;
			currentSample[5] = 0;
		}

		VerboseData vEyeData = new VerboseData ();
		bool success = SRanipal_Eye_v2.GetVerboseData (out vEyeData);

		currentSample[6] = vEyeData.left.eye_openness;
		currentSample[7] = vEyeData.right.eye_openness;
		currentSample[8] = vEyeData.left.pupil_diameter_mm;
		currentSample[9] = vEyeData.right.pupil_diameter_mm;

		// Vector3 pos = zeObject.transform.position;
		// currentSample[0] = pos.x;
		// currentSample[1] = pos.y;
		// currentSample[2] = pos.z;
		outlet.push_sample (currentSample);
	}

	#endregion // -------------------------------------------------------------------------------------------------------------------------------
}