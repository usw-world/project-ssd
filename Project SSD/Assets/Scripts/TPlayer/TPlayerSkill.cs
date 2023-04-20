using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkill : Skill
{
	PlayerStatus status;

	[SerializeField] float usingSP = 0f;
	[SerializeField] float usingMP = 0f;
	private void Start() 
	{
		status = TPlayer.instance.status; 
	}
	public override void Use()
	{
		property.nowCoolTime = 0;
		status.SP -= usingSP;
		status.MP -= usingMP;
		TPlayer.instance.status = status;
		if (info.effect != null)
		{
			GameObject temp = Instantiate(info.effect);
			SkillEffect skillEffect = temp.GetComponent<SkillEffect>();
			skillEffect.OnActive(property);
		}
	}
	public override bool CanUse()
	{
		print(gameObject.name + " CanUse ");
		if (ChackSP() && ChackMP() && ChackCoolTime()) 
			return true;
		return false;
	}
	bool ChackSP() 
	{
		if (status.SP >= usingSP)
			return true;
		return false;
	}
	bool ChackMP()
	{
		if (status.MP >= usingMP)
			return true;
		return false;
	}
	bool ChackCoolTime() {
		if (property.nowCoolTime >= property.coolTime)
			return true; ;
		return false;
	}
}
