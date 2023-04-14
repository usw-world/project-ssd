using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill : MonoBehaviour
{
	public SkillInfo info;
	public SkillArea area;
	public SkillProperty property;

	public virtual void Use(float playerAP)
	{
		GameObject temp = Instantiate(info.effect);
		SkillEffect skillEffect = temp.GetComponent<SkillEffect>();
		skillEffect.Set(property, playerAP);
	}
}
[Serializable]
public class SkillInfo
{
	public GameObject effect;
	public Sprite skillImage;
	public string skillText;
}
[Serializable]
public class SkillArea
{
	public float skillDistance = 1;
	public float skillRange = 1;
}
[Serializable]
public class SkillProperty
{
	public float collTime;
	public float skillAP = 1;
	[HideInInspector] public float nowCollTime;
	public bool ready;
	public bool quickUse;
}