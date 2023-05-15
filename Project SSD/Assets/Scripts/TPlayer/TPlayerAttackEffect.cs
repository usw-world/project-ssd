using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffect : MonoBehaviour, IPooleableObject
{
	[SerializeField] private Mode mode;
	[SerializeField] private ETPlayerAttackEffect attackType;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private Vector3 damageZoneSize;
    [SerializeField] private Vector3 localPos;
	private string hitEffectKey;
	private void Awake()
	{
		hitEffectKey = hitEffect.GetComponent<IPooleableObject>().GetKey();
		PoolerManager.instance.InsertPooler(hitEffectKey, hitEffect, false);
	}
	public void OnActive(float damageAmount)
	{
		TPlayer tPlayer = TPlayer.instance;
		transform.parent = tPlayer.transform;
		transform.localPosition = localPos;
		transform.parent = null;
		transform.rotation = tPlayer.transform.rotation;

		StartCoroutine(Hide());

		Vector3 position = Vector3.zero;
		Collider[] hit = null;

		switch (mode)
		{
			case Mode.Nomal:
				position = tPlayer.transform.position - tPlayer.transform.forward + ( tPlayer.transform.forward * 2f ) + Vector3.up;
				hit = Physics.OverlapBox(position, damageZoneSize, tPlayer.transform.rotation, 1 << 8);
				break;
			case Mode.Dodge:
				position = tPlayer.transform.position - tPlayer.transform.forward - ( tPlayer.transform.forward * 2f ) + Vector3.up;
				hit = Physics.OverlapBox(position, damageZoneSize, tPlayer.transform.rotation, 1 << 8);
				break;
		}
		if (hit != null)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				IDamageable target = hit[i].GetComponent<IDamageable>();

				if (target != null)
				{
					Damage damage = new Damage(
						this.gameObject,
						damageAmount,
						.3f,
						(hit[i].transform.position - transform.position).normalized * .5f,
						Damage.DamageType.Normal
					);
					target.OnDamage(damage);

					GameObject hitEffect = PoolerManager.instance.OutPool(hitEffectKey);
					hitEffect.transform.position += Vector3.up;
					hitEffect.transform.parent = null;
					StartCoroutine(HideHitEffect(hitEffect));
				}
				
			}
		}
	}
	public string GetKey()
	{
		return GetType() + attackType.ToString();
	}
	private IEnumerator Hide()
	{
		yield return new WaitForSeconds(3f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
	}
	private IEnumerator HideHitEffect(GameObject hitEffect)
	{
		yield return new WaitForSeconds(3f);
		PoolerManager.instance.InPool(hitEffectKey, hitEffect);
	}
}
enum ETPlayerAttackEffect
{
	normal_1,
	normal_2,
	normal_3,
	normal_4,
	dodge
}
enum Mode
{
	Nomal, Dodge
}