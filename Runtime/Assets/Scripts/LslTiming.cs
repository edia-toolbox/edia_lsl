using Edia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LslTiming : MonoBehaviour, ILslTimeAccessible {
    public double GetLslTime() {
        return LSL.LSL.local_clock();
    }
}


