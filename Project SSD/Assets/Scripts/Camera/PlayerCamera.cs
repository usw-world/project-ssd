using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    static public PlayerCamera instance;

    public float mouseScrollSpeed;
    public float maxCameraDisrance;
    public float minCameraDisrance;
    
    [HideInInspector] public float cameraDisrance;
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;
    [HideInInspector] public CinemachineFramingTransposer framingTransposer;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public void Start() {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        cameraDisrance = maxCameraDisrance;
        framingTransposer.m_CameraDistance = cameraDisrance;
    }
    public void SetFollow(Transform target) {
        virtualCamera.Follow = target;
    }
    public void CameraZoomInOut(bool zoomIn) {
        cameraDisrance += zoomIn ? -0.1f : 1f * mouseScrollSpeed * Time.deltaTime;
        cameraDisrance = Mathf.Clamp(cameraDisrance, minCameraDisrance, maxCameraDisrance);
        print(cameraDisrance);
        framingTransposer.m_CameraDistance = cameraDisrance;
    }
    public void StartCutScene(GameObject targetCamera) {
        targetCamera.SetActive(true);
        UIManager.instance.StartCutScene();
    }
    public void EndCutScene(GameObject targetCamera) {
        targetCamera.SetActive(false);
        UIManager.instance.EndCutScene();
    }
}

