using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TestLoader : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LoadRecorder();

    public void CallLoadRecorder()
    {
        Debug.Log("OnClick");
        LoadRecorder();
    }
}
