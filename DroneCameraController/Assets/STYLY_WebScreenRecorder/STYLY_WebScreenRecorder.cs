using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//using Battlehub.RTHandles;

/// <summary>
/// WebScreenRecorderのSTYLY用Wrapperクラス
/// WebGL PluginとしてWebScreenRecorderPluginを使用
/// </summary>
public class STYLY_WebScreenRecorder : MonoBehaviour
{
    [Tooltip("録画スタート/ストップに使うショートカットキーの組み合わせ")]
    [SerializeField]
    private List<KeyCode> _triggerList = new List<KeyCode> { KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.Space };

    [Tooltip("録画時に非表示にしたいオブジェクトを登録(canvasは自動で非表示にするのでcanvas以外)")]
    [SerializeField]
    private List<GameObject> _hideObjectList;

    private List<bool> _isTriggerDownList;
    private bool _isRecording = false;

    private Canvas[] _canvasComponents;

    //private BaseHandle[] _handleComponents;

    //private LightGizmo[] _lightGizmoComponents;

    // ショートカットキーの条件を満たした直後はショートカットキーを押し続けている判定になってしまうのでいずれかのキーがupされるまではロック状態にする
    private bool _isLock = false;

    private void Start()
    {
        InitTrigger();
    }

    private void Update()
    {
        for(int i = 0; i < _triggerList.Count; i++)
        {
            if(Input.GetKeyDown(_triggerList[i]))
            {
                _isTriggerDownList[i] = true;
            }

            if(Input.GetKeyUp(_triggerList[i]))
            {
                _isTriggerDownList[i] = false;
                _isLock = false;
            }
        }

        // ショートカットキーの条件をすべて満たしているか否かのフラグ
         var isCompleteShortcutKey = _isTriggerDownList.All(a => a == true);

        if (isCompleteShortcutKey && !_isLock)
        {
            CompleteShortcutKey();
            _isLock = true;
        }
    }

    private void InitTrigger()
    {
        _isTriggerDownList = new List<bool>();
        for (int i = 0; i < _triggerList.Count; i++)
        {
            _isTriggerDownList.Add(false);
        }
    }

    private void FindAllCanvas()
    {
        _canvasComponents = GameObject.FindObjectsOfType<Canvas>();
    }

    private void FindAllHandle()
    {
        //_handleComponents = GameObject.FindObjectsOfType<BaseHandle>();
    }

    private void FindLightGizamo()
    {
        //_lightGizmoComponents = GameObject.FindObjectsOfType<LightGizmo>();
    }

    /// <summary>
    /// ショートカットキーの組み合わせ条件を満たした
    /// </summary>
    private void CompleteShortcutKey()
    {
        if(_isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        Debug.Log("Start Recording");
        _isRecording = true;

        // シーン遷移のタイミング等でcanvasが増える可能性を考慮してこのタイミングで取得
        FindAllCanvas();
        FindAllHandle();
        FindLightGizamo();

        HideScreenUI();

        WebScreenRecorder.CallStartOrStopRecording();
    }

    private void StopRecording()
    {
        Debug.Log("Stop Recording");
        _isRecording = false;

        ShowScreenUI();

        WebScreenRecorder.CallStartOrStopRecording();
    }

    private void ShowScreenUI()
    {
        SetActiveCanvas(true);
        SetActiveHideObjecet(true);
        SetActiveHandle(true);
        SetActiveLightGizmo(true);
    }

    private void HideScreenUI()
    {
        SetActiveCanvas(false);
        SetActiveHideObjecet(false);
        SetActiveHandle(false);
        SetActiveLightGizmo(false);
    }

    /// <summary>
    ///  canvasの表示非表示切り替え
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveCanvas(bool active)
    {
        foreach (var canvas in _canvasComponents)
        {
            canvas.enabled = active;
        }
    }

    /// <summary>
    /// あらかじめインスペクターから設定したHide対象のオブジェクトの表示非表示切り替え
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveHideObjecet(bool active)
    {
        foreach(var target in _hideObjectList)
        {
            target.SetActive(active);
        }
    }

    /// <summary>
    /// Position,Rotation,Scale用のGizmoの表示非表示切り替え
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveHandle(bool active)
    {
        //foreach(var handle in _handleComponents)
        //{
        //    handle.enabled = active;
        //}
    }

    /// <summary>
    /// ライトのGizmoの表示非表示切り替え
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveLightGizmo(bool active)
    {
        //foreach(var lightGizmo in _lightGizmoComponents)
        //{
        //    lightGizmo.enabled = active;
        //}
    }
}
