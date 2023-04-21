using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Skill : MonoBehaviour
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
	public virtual void Use(Vector3 target) { }
	public abstract bool CanUse();
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