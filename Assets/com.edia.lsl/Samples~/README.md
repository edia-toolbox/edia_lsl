# eDIA LSL demos

Imported from https://github.com/labstreaminglayer/LSL4Unity  
All scripts, etc. in this folder (as well as in `../Plugins/LSL`) underlie the according license and are intellectual property of the contributors to the .

---
---




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


