using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerAttackEffectFlyTrail : TPlayerAttackEffect
{
	[SerializeField] private List<ParticleSystem> particles;
	[SerializeField] private List<Transform> trails;
	public override void OnActive(float damageAmount)
	{
		TPlayer tPlayer = TPlayer.instance;
		transform.parent = tPlayer.transform;
		transform.localPosition = Vector3.up * 0.5f;
		transform.parent = null;
		transform.rotation = tPlayer.transform.rotation;

		StartCoroutine(Hide());
	}
	private IEnumerator Hide()
	{
		foreach (var item in particles){
			ParticleSystem.EmissionModule emission = item.emission;
			emission.rateOverDistance = 10;
		}
		foreach (var item in trails){
			item.localPosition = Vector3.zero;
		}
		yield return new WaitForSeconds(2f);
		foreach (var item in particles){
			ParticleSystem.EmissionModule emission = item.emission;
			emission.rateOverDistance = 0;
		}
		yield return new WaitForSeconds(2f);
		PoolerManager.instance.InPool(GetKey(), gameObject);
	}
}
