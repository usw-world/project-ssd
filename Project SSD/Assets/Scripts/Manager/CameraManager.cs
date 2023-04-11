using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager CM;

    [SerializeField] private PlayerCamera TPlayer = new PlayerCamera(2f, 3f, 1f);
    [SerializeField] private PlayerCamera QPlayer = new PlayerCamera(2f, 15f, 2f);
    private PlayerCamera _cNowPlayerCamera;

    private CameraManager() { }
    private void Awake() => CM = this;
    private void Start()
    {
        TPlayer.Set();
        _cNowPlayerCamera = TPlayer;
    }
    public void CameraZoomInOut(float axis)
    {
        _cNowPlayerCamera.cameraDisrance += -axis * _cNowPlayerCamera.mouseScrollSpeed;

        _cNowPlayerCamera.cameraDisrance = Mathf.Clamp(_cNowPlayerCamera.cameraDisrance, _cNowPlayerCamera.minCameraDisrance, _cNowPlayerCamera.maxCameraDisrance);

        _cNowPlayerCamera.CFT.m_CameraDistance = _cNowPlayerCamera.cameraDisrance;
    }
    public void StartCutScene(GameObject camera)
    {
        camera.SetActive(true);
        UIManager.UIM.StartCutScene();
    }
    public void EndCutScene(GameObject camera)
    {
        camera.SetActive(false);
        UIManager.UIM.EndCutScene();
    }
}
[Serializable]
class PlayerCamera
{
    public float mouseScrollSpeed;
    public float maxCameraDisrance;
    public float minCameraDisrance;
    public GameObject camera;
    [HideInInspector] public float cameraDisrance;
    [HideInInspector] public CinemachineVirtualCamera CVC;
    [HideInInspector] public CinemachineFramingTransposer CFT;
    public PlayerCamera(float MSS, float maxCD, float minCD)
    {
        mouseScrollSpeed = MSS;
        maxCameraDisrance = maxCD;
        minCameraDisrance = minCD;
    }
    public void Set()
    {
        CVC = camera.GetComponent<CinemachineVirtualCamera>();
        CFT = CVC.GetCinemachineComponent<CinemachineFramingTransposer>();
        cameraDisrance = maxCameraDisrance;
        CFT.m_CameraDistance = cameraDisrance;
    }
}