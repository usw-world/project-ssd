using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraNoiseTest : MonoBehaviour
{
    public CinemachineVirtualCamera mainCam;
    public NoiseSettings test;


    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            CameraManager.instance.SwitchCameara(mainCam);
            CameraManager.instance.MakeNoise(3, 3);
        }
    }
}
