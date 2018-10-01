using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTest : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private void Update()
    {
        LookAt(_target.position);
    }

    private void LookAt(Vector3 target)
    {
        Vector3 z = (target - transform.position).normalized;
        Vector3 x = Vector3.Cross(Vector3.up, z).normalized;
        Vector3 y = Vector3.Cross(z, x).normalized;

        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = x.x; m[0, 1] = y.x; m[0, 2] = z.x;
        m[1, 0] = x.y; m[1, 1] = y.y; m[1, 2] = z.y;
        m[2, 0] = x.z; m[2, 1] = y.z; m[2, 2] = z.z;

        Quaternion rot = GetRotation(m);
        transform.rotation = rot;
    }

    private Quaternion GetRotation(Matrix4x4 m)
    {
        float[] elem = new float[4];
        elem[0] = m.m00 - m.m11 - m.m22 + 1.0f;
        elem[1] = -m.m00 + m.m11 - m.m22 + 1.0f;
        elem[2] = -m.m00 - m.m11 + m.m22 + 1.0f;
        elem[3] = m.m00 + m.m11 + m.m22 + 1.0f;

        int biggestIdx = 0;
        for (int i = 0; i < elem.Length; i++)
        {
            if (elem[i] > elem[biggestIdx])
            {
                biggestIdx = i;
            }
        }

        if (elem[biggestIdx] < 0)
        {
            Debug.Log("Wrong matrix.");
            return new Quaternion();
        }

        float[] q = new float[4];
        float v = Mathf.Sqrt(elem[biggestIdx]) * 0.5f;
        q[biggestIdx] = v;
        float mult = 0.25f / v;

        switch (biggestIdx)
        {
            case 0:
                q[1] = (m.m10 + m.m01) * mult;
                q[2] = (m.m02 + m.m20) * mult;
                q[3] = (m.m21 - m.m12) * mult;
                break;
            case 1:
                q[0] = (m.m10 + m.m01) * mult;
                q[2] = (m.m21 + m.m12) * mult;
                q[3] = (m.m02 - m.m20) * mult;
                break;
            case 2:
                q[0] = (m.m02 + m.m20) * mult;
                q[1] = (m.m21 + m.m12) * mult;
                q[3] = (m.m10 - m.m01) * mult;
                break;
            case 3:
                q[0] = (m.m21 - m.m12) * mult;
                q[1] = (m.m02 - m.m20) * mult;
                q[2] = (m.m10 - m.m01) * mult;
                break;
        }

        return new Quaternion(q[0], q[1], q[2], q[3]);
    }
}