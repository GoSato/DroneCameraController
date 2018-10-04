using UnityEngine;

public static class InputPredefined
{
    public const string MOUSE_X = "Mouse X";
    public const string MOUSE_Y = "Mouse Y";
    public const string MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
    public const string HORIZONTAL_LEFT = "Horizontal Left";
    public const string VERTICAL_LEFT = "Vertical Left";
    public const string HORIZONTAL_RIGHT = "Horizontal Right";
    public const string VERTICAL_RIGHT = "Vertical Right";
    public const string BUMPER_LEFT_PRESS = "Bumper Left Press";
    public const string BUMPER_RIGHT_PRESS = "Bumper Right Press";
    public const string TRIGGER_LEFT_PRESS = "Trigger Left Press";
    public const string TRIGGER_RIGHT_PRESS = "Trigger Right Press";
    public const string JOYSTICK_LEFT_PRESS = "Joystick Left Press";
    public const string JOYSTICK_RIGHT_PRESS = "Joystick Right Press";
    public const string TRIGGER_LEFT = "Trigger Left";
    public const string TRIGGER_RIGHT = "Trigger Right";
}

[RequireComponent(typeof(Camera))]
public class ManualCameraController : MonoBehaviour
{
    public enum SpeedMode
    {
        Default,
        Slow,
        Turbo
    }

    public enum AxisMode
    {
        Global,
        Local,
    }

    public enum SmoothMode
    {
        Quick,
        Smooth,
    }

    [Header("前後左右の移動速度")]
    [SerializeField]
    private float _movingSensitivity = 1.0f;

    [Header("上下の移動速度")]
    [SerializeField]
    private float _hoveringSensitivity = 0.5f;

    [Header("カメラの向きの回転速度")]
    [SerializeField]
    private float _rotationSensitivity = 1.5f;

    [Header("マウスホイールの感度")]
    [SerializeField]
    private float _mouseWheelSensitivity = 100f;

    [Header("移動する際の軸")]
    [SerializeField]
    private AxisMode _axisMode = AxisMode.Global;

    [Header("Smoothに移動させるか")]
    [SerializeField]
    private SmoothMode _smoothMode = SmoothMode.Smooth;

    [Header("カーソルを隠すか")]
    [SerializeField]
    private bool _enableHideCursor = true;

    [Space(3)]
    [Header("ManualOrbit設定")]
    [Space(2)]

    [SerializeField]
    private Transform _target;

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

    private bool _active;

    private Camera _cam;
    private float _defaultFov;

    private SpeedMode _speedMode = SpeedMode.Default;

    private Vector3 _mousePosDelta;
    private Vector3 _nextPosition;
    private Vector3 _prevMousePos;
    private Transform _prevtransform;
    private Vector2 _elapsedAngles;

    [Range(0f, 1f)] private float _smoothness = 0.001f;
    private float _noSmoothness = 0f;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _defaultFov = _cam.fieldOfView;
        _elapsedAngles = transform.localEulerAngles;
    }

    private void Start()
    {
        SetActive(true);
    }

    public void SetActive(bool active)
    {
        _active = active;
        if(!_enableHideCursor)
        {
            return;
        }

        if (!_active)
        {
            _mousePosDelta = Vector2.zero;
            _prevMousePos = Vector3.zero;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            _prevMousePos = Input.mousePosition;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void LateUpdate()
    {
        if (!_active)
            return;

        // スピードダウン
        if (Input.GetButtonDown(InputPredefined.JOYSTICK_LEFT_PRESS) || Input.GetKeyDown(KeyCode.F))
        {
            switch (_speedMode)
            {
                case SpeedMode.Default:
                    _speedMode = SpeedMode.Slow;
                    break;
                case SpeedMode.Turbo:
                    _speedMode = SpeedMode.Default;
                    break;
                default:
                    break;
            }
        }

        // スピードアップ
        if (Input.GetButtonDown(InputPredefined.JOYSTICK_RIGHT_PRESS) || Input.GetKeyDown(KeyCode.G))
        {
            switch (_speedMode)
            {
                case SpeedMode.Default:
                    _speedMode = SpeedMode.Turbo;
                    break;
                case SpeedMode.Slow:
                    _speedMode = SpeedMode.Default;
                    break;
                default:
                    break;
            }
        }

        _mousePosDelta = Input.mousePosition - _prevMousePos;
        _prevMousePos = Input.mousePosition;
        _nextPosition = transform.position;

        // 速度の設定
        var movingSpeed = _movingSensitivity;
        var hoveringSpeed = _hoveringSensitivity;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            _speedMode = SpeedMode.Turbo;
        }
        else
        {
            _speedMode = SpeedMode.Default;
        }

        switch (_speedMode)
        {
            case SpeedMode.Slow:
                movingSpeed = _movingSensitivity * 0.5f;
                hoveringSpeed = _hoveringSensitivity * 0.5f;
                break;
            case SpeedMode.Default:
                movingSpeed = _movingSensitivity * 1.0f;
                hoveringSpeed = _hoveringSensitivity * 1.0f;
                break;
            case SpeedMode.Turbo:
                movingSpeed = _movingSensitivity * 2.0f;
                hoveringSpeed = _hoveringSensitivity * 2.0f;
                break;
            default:
                break;
        }
        movingSpeed *= transform.lossyScale.x;

        var smoothness = _smoothMode == SmoothMode.Smooth ? _smoothness : _noSmoothness;

        var deltaTime = Time.deltaTime;

        var rightDir = transform.right;
        var upDir = _axisMode == AxisMode.Global ? Vector3.up : transform.up;
        var forwardDir = _axisMode == AxisMode.Global ? Vector3.ProjectOnPlane(transform.forward, Vector3.up) : transform.forward;

        var leftJoystick = new Vector2(Input.GetAxis(InputPredefined.HORIZONTAL_LEFT),
            Input.GetAxis(InputPredefined.VERTICAL_LEFT));
        var rightJoystick = new Vector2(Input.GetAxis(InputPredefined.HORIZONTAL_RIGHT),
            Input.GetAxis(InputPredefined.VERTICAL_RIGHT));

        var trigger = 0f;
        if (Input.GetButton(InputPredefined.TRIGGER_LEFT_PRESS))
        {
            trigger = -_hoveringSensitivity * (Input.GetAxis(InputPredefined.TRIGGER_LEFT) + 1) / 2.0f;
        }
        else if (Input.GetButton(InputPredefined.TRIGGER_RIGHT_PRESS))
        {
            trigger = _hoveringSensitivity * (Input.GetAxis(InputPredefined.TRIGGER_RIGHT) + 1) / 2.0f;
        }

        if (Input.GetMouseButton(1) || leftJoystick.sqrMagnitude > 0f || rightJoystick.sqrMagnitude > 0f || trigger != 0f)
        {

            // キーボード移動
            if (Input.GetKey(KeyCode.W))
                _nextPosition = _nextPosition + forwardDir * movingSpeed;

            if (Input.GetKey(KeyCode.A))
                _nextPosition = _nextPosition - rightDir * movingSpeed;

            if (Input.GetKey(KeyCode.S))
                _nextPosition = _nextPosition - forwardDir * movingSpeed;

            if (Input.GetKey(KeyCode.D))
                _nextPosition = _nextPosition + rightDir * movingSpeed;

            if (Input.GetKey(KeyCode.Q))
                _nextPosition = _nextPosition - upDir * hoveringSpeed;

            if (Input.GetKey(KeyCode.E))
                _nextPosition = _nextPosition + upDir * hoveringSpeed;

            // コントローラー移動
            _nextPosition += rightDir * leftJoystick.x * movingSpeed + forwardDir * leftJoystick.y * movingSpeed + upDir * trigger * movingSpeed;

            // マウス回転
            _elapsedAngles.y += Input.GetAxis(InputPredefined.MOUSE_X) * _rotationSensitivity;
            _elapsedAngles.x += -Input.GetAxis(InputPredefined.MOUSE_Y) * _rotationSensitivity;

            // コントローラー回転
            _elapsedAngles.y += rightJoystick.x * _rotationSensitivity;
            _elapsedAngles.x += -rightJoystick.y * _rotationSensitivity;

            transform.localRotation = SmoothDamp.Pow(transform.localRotation,
                Quaternion.Euler(
                    new Vector3(_elapsedAngles.x, _elapsedAngles.y, 0f)), smoothness, deltaTime);
        }
        else if(Input.GetMouseButton(2))
        {
            var mouseDelta = _mousePosDelta;
            _nextPosition -= rightDir * mouseDelta.x * movingSpeed * 0.1f;
            _nextPosition -= upDir * mouseDelta.y * movingSpeed * 0.1f;
            //Debug.Log("mouseDelta : " + mouseDelta);
        }
        else if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
        {
            if (_target)
            {
                _axisMode = AxisMode.Local;
                _smoothMode = SmoothMode.Quick;

                _x += Input.GetAxis(InputPredefined.MOUSE_X) * _xSpeed;
                _y -= Input.GetAxis(InputPredefined.MOUSE_Y) * _ySpeed;

                _y = ClampAngle(_y, _yMinLimit, _yMaxLimit);

                Quaternion rotation = Quaternion.Euler(_y, _x, 0);

                _distance = Vector3.Distance(transform.position, _target.position);

                //_distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * 5, _distanceMin, _distanceMax);

                Vector3 negDistance = new Vector3(0.0f, 0.0f, -_distance);



                Vector3 position = rotation * negDistance + _target.position;

                transform.rotation = rotation;
                _nextPosition = position;
            }
        }

        if (Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y < Screen.height && Input.mousePosition.y > 0)
        {
            // マウスホイール前後移動
            _nextPosition += forwardDir * Input.GetAxis(InputPredefined.MOUSE_SCROLLWHEEL) * movingSpeed * _mouseWheelSensitivity;
        }

        transform.position = SmoothDamp.Pow(transform.position, _nextPosition, smoothness, deltaTime);

        // ズーム
        if (Input.GetKey(KeyCode.C) || Input.GetButton(InputPredefined.BUMPER_RIGHT_PRESS))
        {
            _cam.fieldOfView = SmoothDamp.Pow(_cam.fieldOfView, _defaultFov * 0.5f, 0.01f, deltaTime);
        }
        else if (Input.GetKey(KeyCode.Z) || Input.GetButton(InputPredefined.BUMPER_LEFT_PRESS))
        {
            _cam.fieldOfView = SmoothDamp.Pow(_cam.fieldOfView, _defaultFov * 1.5f, 0.01f, deltaTime);
        }
        else
        {
            _cam.fieldOfView = SmoothDamp.Pow(_cam.fieldOfView, _defaultFov, 0.01f, deltaTime);
        }
    }

    public void SetAxisMode(AxisMode mode)
    {
        _axisMode = mode;
    }

    public void SetOrbitTarget(Transform target)
    {
        _target = target;

        InitOrbit();
    }

    private void InitOrbit()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
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