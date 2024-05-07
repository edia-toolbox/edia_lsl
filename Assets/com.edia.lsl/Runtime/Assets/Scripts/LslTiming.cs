using Edia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LslTiming : MonoBehaviour, ILslTimer {
    public double GetTime() {
        return LSL.LSL.local_clock();
    }
}
