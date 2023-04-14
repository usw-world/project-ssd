using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
	protected SkillProperty property;
	protected float playerAP;
	private void Start()
	{
		Destroy(gameObject, 1f);
	}
	virtual public void Set(SkillProperty property, float playerAP)
	{
		this.property = property;
		this.playerAP = playerAP;
	}
}
