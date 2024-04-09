# eDIA LSL demos

### 1. Minimal Example

This is a reimplementation (with minor adaptations) of the `Complex Outlet Inlet Event` demo scene in the [LSL4Unity](https://github.com/labstreaminglayer/LSL4Unity) repository.  

Description in the original README (slightly adapted):
> A capsule continuously streams its pose over an outlet. Occassionally (every 2 seconds by default), the position is reset and new velocities are applied to the capsule.  
A cube has its pose set (+ a static offset) by the values coming in from an inlet, the very same stream that the capsule is sending out.  
Thus the cube should move the same as the capsule, except delayed by the capsule outlet -> cube inlet LSL transmission.  
A plane changes its colours occassionally (every ~3.4 seconds). The colour change events also stream out a marker string of the new colour.
By default, the marker is sent out using `WaitForEndOfFrame` (or the moment selected on the `MarkerOutlet` component).

This scene demonstrates how to stream out continuous (Pose) data, read in (here: the same) continuous data from a LSL stream, as well as how to send out non-continuous (string) markers.  
This is a good starting point to check whether principally setting up LSL outlets and inlets works in your project (without many dependencies required and a low level of code complexity).


### 2. Stream Cam Pose

Similar to the [Minimal Example](#1-minimal-example), just that now we are streaming out the pose (i.e., position and rotation) of the VR headset (you can use the `XR Device Simulator` GameObject in the scene to emulate a headset). The cube now reads in this stream and therefore exposes the pose of the VR headset (with a given offset along thw world's z-axis). 

### 3. Stream Controller Joystick 2D

Similar to the [Stream Cam Pose](#2-stream-cam-pose) demo, but this time we are streaming the pose of each of the users hands (see the `PositionRotationOutlet` components on the `LeaftHand` and `RightHand` GameObjects) as well as the 2D information about the joystick (or touchpad) input on each of the two controllers. There is no input/inlets in this demo. 