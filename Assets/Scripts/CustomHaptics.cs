using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CustomHaptics : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;

    public void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
    }
}
