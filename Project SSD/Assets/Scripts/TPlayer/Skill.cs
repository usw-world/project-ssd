using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Skill : MonoBehaviour
{
	public SkillInfo info;
	public float areaAmout = 3f;
	public SkillProperty property;

	public virtual void Update()
	{
		if (property.nowCoolTime < property.coolTime)
			property.nowCoolTime += Time.deltaTime;
	}
	public virtual void Use() { print("함수가 오버라이드 되지 않았습니다"); }
	public virtual void Use(float damage) { print("함수가 오버라이드 되지 않았습니다"); }
	public virtual void Use(Vector3 target) { print("함수가 오버라이드 되지 않았습니다"); }
	public abstract bool CanUse();
	public abstract AimType GetAimType();
	public abstract SkillSize GetAreaAmout();
}
public enum AimType {
	None, // 조준 없음
	Arrow, // 화살표로 표시
	Area // 원으로 표시
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
public class SkillProperty
{
	public float coolTime;
	public float skillAP = 1;
	[HideInInspector] public float nowCoolTime = 100f;
    public bool ready;
	public bool quickUse;
}
public struct SkillSize
{
	public float x, y;
	public SkillSize(float x, float y)
	{
		this.x = x;
		this.y = y;
	}
}