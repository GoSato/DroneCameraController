using UnityEngine;
using System.Collections;

namespace Battlehub.RTHandles
{
    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class MouseOrbit : MonoBehaviour
    {
        private Camera m_camera;
        public Transform Target;
        public float Distance = 5.0f;
        public float XSpeed = 5.0f;
        public float YSpeed = 5.0f;

        public float YMinLimit = -360f;
        public float YMaxLimit = 360f;

        public float DistanceMin = .35f;
        private float DistanceMax = Mathf.Infinity;

        private float m_x = 0.0f;
        private float m_y = 0.0f;

        private void Awake()
        {
            m_camera = GetComponent<Camera>();
        }

        private void Start()
        {
            SyncAngles();
        }

        public void SyncAngles()
        {
            Vector3 angles = transform.eulerAngles;
            m_x = angles.y;
            m_y = angles.x;
        }

        private void LateUpdate()
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            deltaX = deltaX * XSpeed;
            deltaY = deltaY * YSpeed;

            m_x += deltaX;
            m_y -= deltaY;
            m_y = ClampAngle(m_y, YMinLimit, YMaxLimit);

            Zoom();
        }

        public void Zoom()
        {
            Quaternion rotation = Quaternion.Euler(m_y, m_x, 0);
            transform.rotation = rotation;

            float mwheel = Input.GetAxis("Mouse ScrollWheel");

            if (m_camera.orthographic)
            {
                m_camera.orthographicSize -= mwheel * m_camera.orthographicSize;
                if (m_camera.orthographicSize < 0.01f)
                {
                    m_camera.orthographicSize = 0.01f;
                }
            }

            Distance = Mathf.Clamp(Distance - mwheel * Distance, DistanceMin, DistanceMax);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);

            if (Distance <= DistanceMin)
            {
                if (mwheel > 0)
                {
                    Vector3 posDistance = new Vector3(0.0f, 0.0f, Distance);
                    Target.position = rotation * posDistance + Target.position;
                    transform.position = rotation * posDistance + transform.position;
                    UpdateTarget(Target);
                }
            }
            else
            {
                transform.position = rotation * negDistance + Target.position;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
            return Mathf.Clamp(angle, min, max);
        }

        public void UpdateTarget(Transform t)
        {
            Target = t;
            Distance = Vector3.Distance(Camera.main.transform.position, t.position);
        }
    }
}
