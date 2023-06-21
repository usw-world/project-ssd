using UnityEngine;
using Cinemachine;

public class TPlayerCamera : PlayerCamera {
    private float hidingDistance = 1f;
    private float minTransparency = -1f, maxTransparency = 0f;

    private float mouseSensitivity = 300;

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
    }

    public void SetActiveMouseMove(bool active) {
        CinemachinePOV pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if(pov != null) {
            pov.m_VerticalAxis.m_MaxSpeed = active ? mouseSensitivity : 0;
            pov.m_HorizontalAxis.m_MaxSpeed = active ? mouseSensitivity : 0;
        }
    }
}
