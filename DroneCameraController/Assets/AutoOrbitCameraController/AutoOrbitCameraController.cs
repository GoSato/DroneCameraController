using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoOrbitCameraController : MonoBehaviour
{
    public enum CameraDirectionMode
    {
        LookAtTarget,   // 被写体の方にカメラを向ける
        LookAtOutside,  // 被写体の正反対側にカメラを向ける
        Manual,         // カメラの向きを角度で指定する
    }

    [Tooltip("被写体となるオブジェクト")]
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private CameraDirectionMode _cameraDirectionMode = CameraDirectionMode.LookAtTarget;

    [Tooltip("カメラの角度")]
    [SerializeField]
    private Vector3 _cameraRotation;

    [Tooltip("衛星軌道移動を逆回転に")]
    [SerializeField]
    private bool _enableReverse = false;

    [Tooltip("衛星軌道移動の速度")]
    [SerializeField]
    private float _speed = 1.0f;

    [Tooltip("Targetからのカメラの距離")]
    [SerializeField]
    private float _radius = 0.0f;

    [Tooltip("カメラの高さ調整")]
    [SerializeField]
    private float _height = 0.0f;
    
    private Camera _camera;
    private float _initialHeight;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _initialHeight = transform.position.y;
        _radius = Vector3.Distance(_target.transform.position, transform.position);

        _target = new GameObject("Camera Target").transform;
    }

    private void Update()
    {
        var speed = _enableReverse ? -_speed : _speed;

        if (_radius > 0.0f)
        {
            var targetToCamera = (_camera.transform.position - _target.position).normalized;
            var cameraPos = _target.position + targetToCamera * _radius;
            transform.position = cameraPos;
        }

        if(_height != 0.0f)
        {
            transform.position = new Vector3(transform.position.x, _initialHeight + _height, transform.position.z);
        }

        transform.RotateAround(_target.position, Vector3.up, Time.deltaTime * speed);

        switch (_cameraDirectionMode)
        {
            case CameraDirectionMode.LookAtTarget:
                _camera.transform.LookAt(_target);
                break;
            case CameraDirectionMode.LookAtOutside:
                var direction = _target.position - _camera.transform.position;
                var target = _camera.transform.position - direction;
                _camera.transform.LookAt(target);
                break;
            case CameraDirectionMode.Manual:
                _camera.transform.rotation = Quaternion.Euler(_cameraRotation);
                break;
        }
    }
}
