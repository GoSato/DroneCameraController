using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private bool _enableLookAtTarget = true;

    [SerializeField]
    private Vector3 _cameraRotation;

    [SerializeField]
    private bool _enableReverse = false;

    [SerializeField]
    private float _speed = 1.0f;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        var speed = _enableReverse ? -_speed : _speed;

        transform.RotateAround(_target.position, Vector3.up, Time.deltaTime * speed);

        if (_enableLookAtTarget)
        {
            _camera.transform.LookAt(_target);
        }
        else
        {
            _camera.transform.rotation = Quaternion.Euler(_cameraRotation);
        }
    }
}
