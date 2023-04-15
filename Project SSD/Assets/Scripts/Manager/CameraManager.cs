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
    public void CameraZoomInOut(bool zoomIn)
    {
        _cNowPlayerCamera.cameraDisrance += (zoomIn) ? -0.1f : 1f * _cNowPlayerCamera.mouseScrollSpeed * Time.deltaTime;

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


/* 
    MonoBehavior 상속받는 컴포넌트로 만드셔서 따로 파일 분리하시고(ex >> PlayerCamera.cs)
    상속 받는 'TPlayerCamera', 'QPlayerCamera' 만드신 뒤,
    TPlayerCamera에 (벽에 붙어서 안쪽이 투과되어 보일만큼 가까워 질 때)Player의 GameObject 자동으로 투명화하는
    기능 만들어두시면 됩니다.

    - usoock13 -
 */
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
    /* 
        다른 MonoBehavior class의 'void Start()'에서 호출되는 메서드입니다.
        따로 컴포넌트로 분리하실 때는 시그니쳐를 'viod Awake()' or 'void Start()'로 변경하시고 사용하길 권장한다네요. (by Hyunsoo218)

        ┌                   ┐    ┌                     ┐
        │ public void Set() │ >> │ public void Start() │
        └                   ┘    └                     ┘
        - usoock13 -
     */
    public void Set()
    {
        CVC = camera.GetComponent<CinemachineVirtualCamera>();
        CFT = CVC.GetCinemachineComponent<CinemachineFramingTransposer>();
        cameraDisrance = maxCameraDisrance;
        CFT.m_CameraDistance = cameraDisrance;
    }
}