using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Serializable]
    public class CCamera
    {
        public float mouseScrollSpeed;
        public float maxCameraDisrance;
        public float minCameraDisrance;
        
        public GameObject camera;
        [HideInInspector] public float cameraDisrance;
        [HideInInspector] public CinemachineVirtualCamera CVC;
        [HideInInspector] public CinemachineFramingTransposer CFT;
        public CCamera(float MSS, float maxCD, float minCD)
        {
            mouseScrollSpeed = MSS;
            maxCameraDisrance = maxCD;
            minCameraDisrance = minCD;
        }

        public void Start()
        {
            CVC = camera.GetComponent<CinemachineVirtualCamera>();
            CFT = CVC.GetCinemachineComponent<CinemachineFramingTransposer>();
            cameraDisrance = maxCameraDisrance;
            CFT.m_CameraDistance = cameraDisrance;
        }
    }
}

