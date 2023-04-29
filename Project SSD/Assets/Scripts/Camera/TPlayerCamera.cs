using UnityEngine;
using Cinemachine;

public class TPlayerCamera : PlayerCamera {
    
    [HideInInspector] public CinemachineFramingTransposer framingTransposer;

    private float hidingDistance = 1f;
    private float minTransparency = -1f, maxTransparency = 0f;

    private float currentDistance;
    public float maxCameraDistance;
    public float minCameraDistance;

    private bool isInHidingDistance = false;
    
    protected override void Awake() {
        base.Awake();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = maxCameraDistance;
    }

    private void Update() {
        FadeInOut();
    }

    private void ChangeTransparency(float transparency)
    {
        Renderer[] renderers = TPlayer.instance.GetComponentsInChildren<Renderer>();
        foreach (Renderer childRenderer in renderers) {
            childRenderer.material.SetFloat("_Tweak_transparency", CheckOverTransparency(transparency));
            childRenderer.material.SetColor("_BaseColor", new Color(1f, 1f, 1f, CheckOverTransparency(transparency) + 1f));
        }
    }

    private float CheckOverTransparency(float transparency)
    {
        if (transparency < minTransparency + 0.5)
            transparency = minTransparency;
        else if (transparency > maxTransparency)
            transparency = maxTransparency;

        return transparency;
    }

    private void FadeInOut() {
        float cameraAndPlayerDistance = Vector3.Distance(TPlayer.instance.transform.position + new Vector3(0, 1), Camera.main.transform.position);
        
        if (cameraAndPlayerDistance < hidingDistance) {
            isInHidingDistance = true;
            ChangeTransparency(cameraAndPlayerDistance - hidingDistance);
        } else if (cameraAndPlayerDistance > hidingDistance && isInHidingDistance) {
            isInHidingDistance = false;
            ChangeTransparency(0f);
        }
    }

    public override void SetTarget(Transform target) {
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
    }
    public override void ZoomIn() {
        currentDistance += -1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        framingTransposer.m_CameraDistance = currentDistance;
    }

    public override void ZoomOut() {
        currentDistance += 1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        framingTransposer.m_CameraDistance = currentDistance;
    }
}
