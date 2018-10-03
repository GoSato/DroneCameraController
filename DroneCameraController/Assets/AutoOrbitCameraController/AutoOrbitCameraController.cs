using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 衛星軌道でカメラを自動で動かす
/// </summary>
[RequireComponent(typeof(Camera))]
public class AutoOrbitCameraController : MonoBehaviour
{
    public enum CameraDirectionMode
    {
        LookAtTarget,   // 被写体の方にカメラを向ける
        LookAtOutside,  // 被写体の正反対側にカメラを向ける
        Manual,         // カメラの向きを角度で指定する
    }

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

    private Transform _target;
    private Camera _camera;
    private float _initialHeight;
    private float _initialRadius;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        _target = new GameObject("Camera Target").transform;
        _initialHeight = transform.position.y;
        _initialRadius = Vector3.Distance(_target.transform.position, transform.position);
        _height = 0.0f;
    }

    private void LateUpdate()
    {
        var speed = _enableReverse ? -_speed : _speed;

        var targetToCamera = (transform.position - _target.position).normalized;
        var cameraPos = _target.position + targetToCamera * (_initialRadius + (_initialRadius * _radius));
        transform.position = cameraPos;

        if (_height != 0.0f)
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
                var direction = _target.position - transform.position;
                var target = transform.position - direction;
                _camera.transform.LookAt(target);
                break;
            case CameraDirectionMode.Manual:
                transform.rotation = Quaternion.Euler(_cameraRotation);
                break;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    #region for UI
    public void SetCameraDirectionMode(CameraDirectionMode mode)
    {
        _cameraDirectionMode = mode;
    }

    public void SetCamraRotation(Vector3 rotation)
    {
        _cameraRotation = rotation;
    }

    public void SetEnableReverse(bool enable)
    {
        _enableReverse = enable;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public void SetHeight(float height)
    {
        _height = height;
    }
    #endregion
}
