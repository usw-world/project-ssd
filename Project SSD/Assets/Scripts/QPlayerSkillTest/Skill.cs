using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill : MonoBehaviour
{
	public SkillInfo info;
	public SkillArea area;
	public SkillProperty property;

	public virtual void Update()
	{
		if (property.nowCoolTime < property.coolTime)
			property.nowCoolTime += Time.deltaTime;
	}
	public virtual void Use() { }
	public virtual void Use(Vector3 target)
	{
		property.nowCoolTime = 0;
		GameObject temp = Instantiate(info.effect, target, Quaternion.Euler(0,0,0));
		SkillEffect skillEffect = temp.GetComponent<SkillEffect>();
		skillEffect.OnActive(property); ;
	}
	public virtual bool CanUse()
	{ 
		return (property.nowCoolTime >= property.coolTime) ? true : false;	
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
	public float distance = 1;
	public float range = 1;
}
[Serializable]
public class SkillProperty
{
	public float coolTime;
	public float skillAP = 1;
	[HideInInspector] public float nowCoolTime = 100f;
	public bool ready;
	public bool quickUse;
}