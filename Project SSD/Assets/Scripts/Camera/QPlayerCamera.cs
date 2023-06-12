using UnityEngine;
using Cinemachine;

public class QPlayerCamera : PlayerCamera {
    protected override void Awake() {
        base.Awake();
        currentDistance = maxCameraDistance;
        ZoomOut();
    }

    public override void SetTarget(Transform target) {
        virtualCamera.Follow = target;
    }


}
