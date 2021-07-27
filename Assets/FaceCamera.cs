using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;

    private Transform _targetCamera;

    // Start is called before the first frame update
    void Start()
    {
        _targetCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var direction = _targetCamera.position - transform.position;

        if (!x) direction.x = 0;
        if (!y) direction.y = 0;
        if (!z) direction.z = 0;

        var rotation = Quaternion.LookRotation(direction);

        transform.rotation = rotation;
    }
}
