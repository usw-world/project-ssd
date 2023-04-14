using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
	protected SkillProperty property;
	protected float playerAP;

	virtual public void Set(SkillProperty property, float playerAP)
	{
		this.property = property;
		this.playerAP = playerAP;
	}
}
