using UnityEngine;
using Cinemachine;

public class QPlayerCamera : PlayerCamera {
    
    [HideInInspector] public CinemachineTransposer transposer;
    [HideInInspector] public CinemachineHardLookAt hardLookAt;

    private float currentDistance = 0;
    public float maxCameraDistance = 15f;
    public float minCameraDistance = 10f;

    private bool isInHidingDistance = false;
    
    protected override void Awake() {
        base.Awake();
        currentDistance = minCameraDistance;
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        hardLookAt = virtualCamera.GetCinemachineComponent<CinemachineHardLookAt>();
        if(transposer==null || hardLookAt==null)
            Debug.LogWarning("Q Player Camera's body is must be 'Transposer' and aim is must be 'Hard Look At'.");
    }

    public override void SetTarget(Transform target) {
        virtualCamera.Follow = target;
    }
    public override void ZoomIn() {
        print(currentDistance);
        currentDistance += -1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        transposer.m_FollowOffset = new Vector3(0, currentDistance, -currentDistance);
    }

    public override void ZoomOut() {
        print(currentDistance);
        currentDistance += 1f * zoomInOutSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
        transposer.m_FollowOffset = new Vector3(0, currentDistance, -currentDistance);
    }
}
