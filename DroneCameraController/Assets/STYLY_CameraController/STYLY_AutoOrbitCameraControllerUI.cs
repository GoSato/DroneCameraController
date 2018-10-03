using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class STYLY_AutoOrbitCameraControllerUI : MonoBehaviour
{
    [SerializeField]
    private AutoOrbitCameraController _autoOrbitCameraController;

    private Vector3 _rotation = Vector3.zero;

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    public void SetActive(bool active, bool onlyUI = false)
    {
        if (!onlyUI)
        {
            _autoOrbitCameraController.enabled = active;
        }

        gameObject.SetActive(active);
        CanvasGroup.alpha = active ? 1 : 0;
        CanvasGroup.interactable = active;
        CanvasGroup.blocksRaycasts = active;
    }

    public void OnSetCameraDirectionMode(Dropdown dropdown)
    {
        AutoOrbitCameraController.CameraDirectionMode mode;
        switch (dropdown.value)
        {
            case 0:
            default:
                mode = AutoOrbitCameraController.CameraDirectionMode.LookAtTarget;
                break;
            case 1:
                mode = AutoOrbitCameraController.CameraDirectionMode.LookAtOutside;
                break;
            case 2:
                mode = AutoOrbitCameraController.CameraDirectionMode.Manual;
                break;
        }
        _autoOrbitCameraController.SetCameraDirectionMode(mode);
    }

    public void OnSetCameraRotationX(InputField inputField)
    {
        _rotation.x = StringToFloat(inputField.text);
        _autoOrbitCameraController.SetCamraRotation(_rotation);
    }

    public void OnSetCameraRotationY(InputField inputField)
    {
        _rotation.y = StringToFloat(inputField.text);
        _autoOrbitCameraController.SetCamraRotation(_rotation);
    }

    public void OnSetCameraRotationZ(InputField inputField)
    {
        _rotation.z = StringToFloat(inputField.text);
        _autoOrbitCameraController.SetCamraRotation(_rotation);
    }

    public void OnSetEnableReverse(Toggle toggle)
    {
        _autoOrbitCameraController.SetEnableReverse(toggle.isOn);
    }

    public void OnSetSpeed(Slider slider)
    {
        _autoOrbitCameraController.SetSpeed(Mathf.Lerp(1, 100, slider.value));
    }

    public void OnSetRadius(Slider slider)
    {
        _autoOrbitCameraController.SetRadius(Mathf.Lerp(-0.9f, 0.9f, slider.value));
    }

    public void OnSetHeight(Slider slider)
    {
        _autoOrbitCameraController.SetHeight(Mathf.Lerp(-20, 20, slider.value));
    }

    private float StringToFloat(string value)
    {
        int i;
        if (int.TryParse(value, out i))
        {
            return i;
        }
        else
        {
            Debug.Log("数値に変換できません");
            return 0;
        }
    }
}
