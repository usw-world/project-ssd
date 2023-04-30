using Cinemachine;
using UnityEngine;

public abstract class PlayerCamera : MonoBehaviour {
    static public PlayerCamera instance;

    public float zoomInOutSensitivity;

    protected float currentDistance;
    public float maxCameraDistance;
    public float minCameraDistance;
    
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;
    [HideInInspector] public CinemachineFramingTransposer framingTransposer;

    protected virtual void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
        
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    public void Start() {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
    }
    public abstract void SetTarget(Transform target);
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
    
    public void StartCutScene(GameObject targetCamera) {
        targetCamera.SetActive(true);
        UIManager.instance.StartCutScene();
    }
    public void EndCutScene(GameObject targetCamera) {
        targetCamera.SetActive(false);
        UIManager.instance.EndCutScene();
    }
}