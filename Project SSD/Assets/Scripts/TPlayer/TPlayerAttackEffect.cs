using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffect : MonoBehaviour, IPoolableObject
{
	[SerializeField] private Mode mode;
	[SerializeField] private ETPlayerAttackEffect attackType;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private Vector3 damageZoneSize;
    [SerializeField] private Vector3 localPos;
    [SerializeField] private bool previewAttackZone = false;
	private GameObject previewBoxPrefab;
	private string hitEffectKey;

	private void Awake()
	{
		previewBoxPrefab = Resources.Load("previewBoxPrefab") as GameObject;
		hitEffectKey = hitEffect.GetComponent<IPoolableObject>().GetKey();
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
			case Mode.NonCgargong:
				position = tPlayer.transform.position - tPlayer.transform.forward + (tPlayer.transform.forward * 4f) + Vector3.up;
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
						damageAmount,
						.75f,
						(hit[i].transform.position - transform.position).normalized * 5f,
						Damage.DamageType.Normal
					);
					target.OnDamage(damage);

					GameObject hitEffect = PoolerManager.instance.OutPool(hitEffectKey);
					hitEffect.transform.position = hit[i].transform.position;
					hitEffect.transform.position += Vector3.up;
					hitEffect.transform.parent = null;
					StartCoroutine(HideHitEffect(hitEffect));
				}
			}
		}
		if (previewAttackZone)
		{
			GameObject previewBox = Instantiate(previewBoxPrefab, position, tPlayer.transform.rotation);
			previewBox.transform.localScale = damageZoneSize;
			Destroy(previewBox, 2f);
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
		yield return new WaitForSeconds(2f);
		PoolerManager.instance.InPool(hitEffectKey, hitEffect);
	}
}
enum ETPlayerAttackEffect
{
	LB_to_RT, LB_to_RT_Big,
	LM_to_RM, LM_to_RM_Big,
	LT_to_RB, LT_to_RB_Big,
	RB_to_LT, RB_to_LT_Big,
	RM_to_LM, RM_to_LM_Big,
	RT_to_LB, RT_to_LB_Big,
	NonCgargong,
	dodge
}
enum Mode
{
	Nomal, Dodge, NonCgargong
}