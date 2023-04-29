using Cinemachine;
using UnityEngine;

public abstract class PlayerCamera : MonoBehaviour {
    static public PlayerCamera instance;

    public float zoomInOutSensitivity;
    
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;

    protected virtual void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
        
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    public void Start() {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
    }
    public abstract void SetTarget(Transform target);
    public abstract void ZoomIn();
    public abstract void ZoomOut();
    
    public void StartCutScene(GameObject targetCamera) {
        targetCamera.SetActive(true);
        UIManager.instance.StartCutScene();
    }
    public void EndCutScene(GameObject targetCamera) {
        targetCamera.SetActive(false);
        UIManager.instance.EndCutScene();
    }
}