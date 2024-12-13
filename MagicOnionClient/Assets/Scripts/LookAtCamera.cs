using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera camera;
    void Update()
    {
        if(camera != null) transform.LookAt(camera.transform);
    }

    public void GetCamera(Camera playerCam)
    {
        camera = playerCam;
    }
}
