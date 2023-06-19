using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class QPlayerCamera : PlayerCamera {

	[SerializeField] private Material blockHideMat;
	private Dictionary<MeshRenderer, Material> hideMeshRenderer = new Dictionary<MeshRenderer, Material>();
	protected override void Awake() {
        base.Awake();
        currentDistance = maxCameraDistance;
        ZoomOut();
    }
    public override void SetTarget(Transform target) {
        virtualCamera.Follow = target;
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			MeshRenderer meshRenderer;
			if (other.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				hideMeshRenderer.Add(meshRenderer, meshRenderer.material);
				meshRenderer.material = blockHideMat;
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			MeshRenderer meshRenderer;
			if (other.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				meshRenderer.material = hideMeshRenderer[meshRenderer];
			}
		}
	}
}
