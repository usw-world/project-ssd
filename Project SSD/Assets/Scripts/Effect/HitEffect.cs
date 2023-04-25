using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : SkillEffect
{
	[SerializeField] float runTime = 5f;
	public override void OnActive(SkillProperty property)
	{
		Invoke("Hide", runTime);
	}
	void Hide() 
	{
		gameObject.SetActive(false); 
	}
}
