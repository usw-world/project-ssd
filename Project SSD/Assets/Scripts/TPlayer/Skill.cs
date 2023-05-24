using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Skill : MonoBehaviour
{
	public AimType aimType;
	public SkillInfo info;
	public SkillArea area;
	public SkillProperty property;

	public virtual void Update()
	{
		if (property.nowCoolTime < property.coolTime)
			property.nowCoolTime += Time.deltaTime;
	}
	public virtual void Use() { print("함수가 오버라이드 되지 않았습니다"); }
	public virtual void Use(Vector3 target) { print("함수가 오버라이드 되지 않았습니다"); }
	public abstract bool CanUse();
}
public enum AimType {
	None, // 기본값
	Arrow, // 구현 안됨
	Area // 아직 못씀
}
[Serializable]
public class SkillInfo
{
	public GameObject effect;
	public Sprite skillImage;
	public string skillText;
	public string name;
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
    [HideInInspector] public bool isUseSkill = false;
    public bool ready;
	public bool quickUse;
}