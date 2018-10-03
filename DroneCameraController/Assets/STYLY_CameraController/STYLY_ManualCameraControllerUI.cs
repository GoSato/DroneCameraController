using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class STYLY_ManualCameraControllerUI : MonoBehaviour
{
    [SerializeField]
    private ManualCameraController _manualCameraController;

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
            _manualCameraController.SetActive(active);
        }

        gameObject.SetActive(active);
        CanvasGroup.alpha = active ? 1 : 0;
        CanvasGroup.interactable = active;
        CanvasGroup.blocksRaycasts = active;
    }

    public void OnSetAxisMode(Dropdown dropdown)
    {
        ManualCameraController.AxisMode _axisMode;

        switch(dropdown.value)
        {
            case 0:
            default:
                _axisMode = ManualCameraController.AxisMode.Local;
                break;
            case 1:
                _axisMode = ManualCameraController.AxisMode.Global;
                break;
        }

        _manualCameraController.SetAxisMode(_axisMode);
    }
}
