using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Lightning : MonoBehaviour, IPoolableObject
{
	protected float damageAmount;
	protected bool isAttachmentDamage = false;
	protected bool isAttachmentInability = false;
	protected bool isBroadAttack = false;
	protected Attachment damage;
	protected Attachment inability;

	public void Initialize(float damageAmount)
	{
		this.damageAmount = damageAmount;
		isAttachmentDamage = false;
		isAttachmentInability = false;
		isBroadAttack = false;
	}
	public void Initialize(Effect_Lightning origin)
	{
		damageAmount = origin.damageAmount;
		isAttachmentDamage = origin.isAttachmentDamage;
		isAttachmentInability = origin.isAttachmentInability;
		isBroadAttack = origin.isBroadAttack;
		damage = origin.damage;
		inability = origin.inability;
	}

	#region 옵션 3 기절
	public void activeInability(Attachment attachment)
	{
		isAttachmentInability = true;
		inability = attachment;
	}
	#endregion 옵션 3 기절

	#region 옵션 4 지속데미지
	public void activeDamage(Attachment attachment)
	{
		isAttachmentDamage = true;
		damage = attachment;
	}
	#endregion 옵션 4 지속데미지

	public void ActiveBroadAttack(bool active)
	{
		isBroadAttack = active;
	}

	public void Run()
	{

	}

	public string GetKey()
	{
		return GetKey().ToString();
	}
}
