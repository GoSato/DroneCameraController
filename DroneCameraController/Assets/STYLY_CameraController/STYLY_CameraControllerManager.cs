using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraControllerMode
{
    Manual,
    AutoOrbit,
}

public class STYLY_CameraControllerManager : MonoBehaviour
{
    [SerializeField]
    private STYLY_ManualCameraControllerUI _manualCameraControllerUI;

    [SerializeField]
    private STYLY_AutoOrbitCameraControllerUI _autoOrbitCameraControllerUI;

    private CameraControllerMode _curretCameraControllerMode = CameraControllerMode.Manual;

    private void Start()
    {
        _curretCameraControllerMode = CameraControllerMode.AutoOrbit;
        OpenOrCloseCameraControllerUI(false);
        _curretCameraControllerMode = CameraControllerMode.Manual;
        OpenOrCloseCameraControllerUI(true);
        OpenOrCloseCameraControllerUI(false, true);
    }

    public void OnOpenManualCameraControllerUI()
    {
        OpenOrCloseCameraControllerUI(false);
        _curretCameraControllerMode = CameraControllerMode.Manual;
        OpenOrCloseCameraControllerUI(true);
    }

    public void OnOpenAutoOrbitCameraControllerUI()
    {
        OpenOrCloseCameraControllerUI(false);
        _curretCameraControllerMode = CameraControllerMode.AutoOrbit;
        OpenOrCloseCameraControllerUI(true);
    }

    public void OnOpenOrCloseCameraControllerUI(Toggle toggle)
    {
        bool active = toggle.isOn;

        OpenOrCloseCameraControllerUI(active, true);
    }

    private void OpenOrCloseCameraControllerUI(bool active, bool onlyUI = false)
    {
        switch (_curretCameraControllerMode)
        {
            case CameraControllerMode.Manual:
                _manualCameraControllerUI.SetActive(active, onlyUI);
                break;
            case CameraControllerMode.AutoOrbit:
                _autoOrbitCameraControllerUI.SetActive(active, onlyUI);
                break;
        }
    }
}
