using System;
using UnityEngine;

public class ThirdPersonCameraControl : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)] private float _angularSpeed = 1f;

    [SerializeField] private Transform _target;

    private float _angleY;

    private void Start()
    {
        _angleY = transform.rotation.y;
    }

    private void LateUpdate()
    {
         _angleY -= Input.GetAxis("Mouse Y") * _angularSpeed;
        _angleY += Input.GetAxis("Mouse Y") * _angularSpeed;

        transform.position = _target.transform.position;
        var targetRotation = _target.transform.rotation;
        //stransform..RotateAround(new Vector3(0, targetRotation.y, 0));
    }
}
