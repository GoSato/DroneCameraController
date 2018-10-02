using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraControllerMode
{
    Manual,
    AutoOrbit,
}

public class STYLY_CameraControllerManager : MonoBehaviour
{
    [SerializeField]
    private ManualCameraController _manualController;

    [SerializeField]
    private AutoOrbitCameraController _autoController;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            UseManualCameraController();
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            UseAutoOrbitCameraController();
        }
    }

    public void UseManualCameraController()
    {
        _manualController.SetActive(true);
        _autoController.enabled = false;
    }

    public void UseAutoOrbitCameraController()
    {
        _manualController.SetActive(false);
        _autoController.enabled = true;
    }
}
