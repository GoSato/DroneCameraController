using UnityEngine;
using System.Collections;

public class CameraOrbitTest : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Transform _target2;

    [SerializeField]
    private float _xSpeed = 5.0f;
    [SerializeField]
    private float _ySpeed = 5.0f;

    [SerializeField]
    private float _yMinLimit = -360f;
    [SerializeField]
    private float _yMaxLimit = 360f;

    [SerializeField]
    private float _distanceMin = 0.5f;
    [SerializeField]
    private float _distanceMax = 15f;

    private float _x = 0.0f;
    private float _y = 0.0f;
    private float _distance = 5.0f;

    private void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (_target)
        {
            _x += Input.GetAxis("Mouse X") * _xSpeed;
            _y -= Input.GetAxis("Mouse Y") * _ySpeed;

            _y = ClampAngle(_y, _yMinLimit, _yMaxLimit);

            Quaternion rotation = Quaternion.Euler(_y, _x, 0);

            _distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * 5, _distanceMin, _distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -_distance);
            Vector3 position = rotation * negDistance + _target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private void Init()
    {
        _distance = Vector3.Distance(transform.position, _target.position);
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
    }

    public void SetTarget(Transform target)
    {
        _target = target;

        Init();
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}