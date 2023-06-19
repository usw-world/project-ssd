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
	private void SetTriggerSize() 
	{
		if (hideMeshRenderer.Count > 0)
		{
			GetComponent<BoxCollider>().size = new Vector3(10f, 5f, 15f); ;
		}
		else
		{
			GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 15f); ;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			MeshRenderer meshRenderer;
			if (other.gameObject.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				if (!hideMeshRenderer.ContainsKey(meshRenderer))
				{
					hideMeshRenderer.Add(meshRenderer, meshRenderer.material);
					meshRenderer.material = blockHideMat;
					SetTriggerSize();
				}
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			MeshRenderer meshRenderer;
			if (other.gameObject.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				if (hideMeshRenderer.ContainsKey(meshRenderer))
				{
					meshRenderer.material = hideMeshRenderer[meshRenderer];
					hideMeshRenderer.Remove(meshRenderer);
					SetTriggerSize();
				}
			}
		}
	}
}
