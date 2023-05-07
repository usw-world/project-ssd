using System.Collections;
using Cinemachine;
using UnityEngine;

public abstract class PlayerCamera : MonoBehaviour {
    static public PlayerCamera instance;

    public float zoomInOutSensitivity;

    protected float currentDistance;
    public float maxCameraDistance;
    public float minCameraDistance;
    bool isUsing = false;

    [HideInInspector] public CinemachineBlend blend;
    [HideInInspector] public CinemachineVirtualCamera prevCamera;
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;
    [HideInInspector] public CinemachineFramingTransposer framingTransposer;

    protected virtual void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void Start() {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    public abstract void SetTarget(Transform target);

    #region zoom

    public virtual void ZoomIn() {
        currentDistance += -1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        framingTransposer.m_CameraDistance = currentDistance;
    }

    public virtual void ZoomOut() {
        currentDistance += 1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        framingTransposer.m_CameraDistance = currentDistance;
    }

    // value = Input.GetAxis 마우스휠
    public void Zoom(float value)
    {
        currentDistance -= value * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        framingTransposer.m_CameraDistance = currentDistance;
    }

    #endregion

    public void StartCutScene(GameObject targetCamera) {
        targetCamera.SetActive(true);
        UIManager.instance.StartCutScene();
    }

    public void EndCutScene(GameObject targetCamera) {
        targetCamera.SetActive(false);
        UIManager.instance.EndCutScene();
    }


    // 플레이어 에서 호출
    public virtual void MakeNoise(float value, float time)
    {
        StartCoroutine(SetNoise(value, time));
    }

    // noise 사용
    IEnumerator SetNoise(float value, float time)
    {
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = value;
        yield return new WaitForSeconds(time);
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }

    // 카메라 전환 
    public void ChangeCam(CinemachineVirtualCamera cam)
    {
        if (isUsing)
            return;
        prevCamera = virtualCamera;
        virtualCamera = cam;
        prevCamera.m_Priority = 9;
        virtualCamera.m_Priority = 11;
    }

    // 카메라 롤
    public void RevertCam() {
        virtualCamera = prevCamera;
        prevCamera = null;
        virtualCamera.m_Priority = 11;
    }


}