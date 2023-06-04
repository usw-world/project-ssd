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
	[SerializeField] protected bool previewDamageZone;
	protected GameObject previewBoxPrefab;

	protected void Awake()
	{
		previewBoxPrefab = Resources.Load("previewBoxPrefab") as GameObject;
	}
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

	#region 옵션 5 폭 증가
	public void ActiveBroadAttack(bool active)
	{
		isBroadAttack = active;
	}
	#endregion 옵션 5 폭 증가

	public virtual void Run()
	{
		Collider[] hit = null;
		Vector3 size;
		if (isBroadAttack)
		{
			size = new Vector3(2f, 3f, 10f);
			transform.localScale = Vector3.one * 2f;
		}
		else
		{
			size = new Vector3(1f, 3f, 10f);
			transform.localScale = Vector3.one;
		}
		float flinching = 1f;

		hit = Physics.OverlapBox(transform.position + transform.forward * 5f, size * 0.5f, QPlayer.instance.transform.rotation, 1 << 8);
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable target = hit[i].GetComponent<IDamageable>();
			Damage damage = new Damage(
				damageAmount,
				flinching,
				(hit[i].transform.position - transform.position).normalized * 5f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
			Enemy enemy;
			if (TryGetComponent<Enemy>(out enemy))
			{
				enemy.OnDamage(damage);
				if (isAttachmentDamage) enemy.AddAttachment(this.damage);
				if (isAttachmentInability) enemy.AddAttachment(inability);
			}
		}
		StartCoroutine(InPool());

		if (previewDamageZone)
		{
			GameObject previewBox = Instantiate(previewBoxPrefab, transform.position + transform.forward * 5f, QPlayer.instance.transform.rotation);
			previewBox.transform.localScale = size;
			Destroy(previewBox, 2f);
		}
	}
	protected virtual IEnumerator InPool()
	{
		yield return new WaitForSeconds(5f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
	}
	virtual public string GetKey()
	{
		return GetType().ToString();
	}
}
