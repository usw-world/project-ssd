using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffect : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected Vector3 offset;
	protected virtual void Start()
	{
		transform.SetParent(null);
	}
	protected virtual void Update() 
	{
		transform.position = target.position + offset;
		transform.rotation = target.rotation;
	}
	public virtual void Enable()
	{
		ParticleSystem.EmissionModule emission = GetComponent<ParticleSystem>().emission;
		emission.rateOverDistance = 5f;
	}
	public virtual void Disable() 
	{
		ParticleSystem.EmissionModule emission = GetComponent<ParticleSystem>().emission;
		emission.rateOverDistance = 0f;
	}
}
