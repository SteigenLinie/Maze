using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchAndZoom : MonoBehaviour
{
    [SerializeField] private float MouseZoomSpeed = 15.0f;
    [SerializeField] private float TouchZoomSpeed = 0.1f;
    [SerializeField] private float ZoomMinBound = 0.1f;
    [SerializeField] private float ZoomMaxBound = 179.9f;
    [SerializeField] private Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            Debug.Log("touchSupported");
            // Pinch to zoom
            if (Input.touchCount == 2)
            {

                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Zoom(deltaDistance, TouchZoomSpeed);
            }
        }
        else
        {
            Debug.Log("touchNotSupported");
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            
            if(scroll != 0)
                Zoom(scroll, MouseZoomSpeed);
        }



       /* if (cam.fieldOfView < ZoomMinBound)
        {
            cam.orthographicSize = 0.1f;
        }
        else
        if (cam.fieldOfView > ZoomMaxBound)
        {
            cam.orthographicSize = 179.9f;
        }*/
    }

    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        Debug.Log(deltaMagnitudeDiff);
        Debug.Log(cam.orthographicSize);
        var size = cam.orthographicSize + deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        cam.orthographicSize = Math.Clamp(size, ZoomMinBound, ZoomMaxBound);
    }
}