using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideTest : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Transform _anchor;

    // target - center - anchor

    private void Update()
    {
        var direction = _target.position - transform.position;

        _anchor.position = transform.position - direction;
    }
}
