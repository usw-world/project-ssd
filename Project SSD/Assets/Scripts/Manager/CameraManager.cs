using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager CM;

    [SerializeField] private TPlayerCamera.CCamera TPlayer;
    [SerializeField] private PlayerCamera.CCamera QPlayer = new PlayerCamera.CCamera(2f, 15f, 2f);
    private PlayerCamera.CCamera _cNowPlayerCamera;

    private CameraManager() { }
    private void Awake() => CM = this;
    private void Start()
    {
        TPlayer.Start();
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

