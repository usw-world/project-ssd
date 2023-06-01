using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffect : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
	[SerializeField] bool enable = false;
	 
	private void Update() 
	{
		if(enable)
			transform.position = target.position + offset; 
	}
	public void Enable()
	{
		gameObject.SetActive(true);
		transform.parent = null;
		enable = true;
	}
	public void Disable() 
	{
		gameObject.SetActive(false);
		transform.position = target.position + offset;
		transform.parent = target;
		enable = false; 
	}
}
