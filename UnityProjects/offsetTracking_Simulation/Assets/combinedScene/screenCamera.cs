using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenCamera : MonoBehaviour
{

    public Camera mainCamera;
    public Camera screenCam;

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 positionDifference = mainCamera.transform.position - screenCam.transform.position;

        float rotY = (float) Math.Atan(positionDifference.y/positionDifference.z);
        float rotX = (float)Math.Atan(positionDifference.x / positionDifference.z);

        screenCam.transform.rotation = Quaternion.Euler(rotX, rotY, 0);*/
    }
}
