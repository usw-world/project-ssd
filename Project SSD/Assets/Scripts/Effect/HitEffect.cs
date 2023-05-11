using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
	[SerializeField] float runTime = 5f;
	private void OnEnable()
	{
		Invoke("Hide", runTime);
	}
	void Hide() 
	{
		gameObject.SetActive(false); 
	}
}
