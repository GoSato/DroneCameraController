using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Recorder : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LoadRecorder();

    public void CallLoadRecorder()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Load Recorder Button.");
        LoadRecorder();
#endif
    }
}
