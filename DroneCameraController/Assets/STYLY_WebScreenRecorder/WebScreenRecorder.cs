using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public static class WebScreenRecorder
{
    [DllImport("__Internal")]
    private static extern void StartOrStopRecording();

    public static void CallStartOrStopRecording()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartOrStopRecording();
#endif
    }
}
