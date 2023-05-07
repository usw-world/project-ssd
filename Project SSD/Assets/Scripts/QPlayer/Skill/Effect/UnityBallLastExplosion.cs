using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBallLastExplosion : MonoBehaviour
{
	public void OnActive(float damageAmount) {
		Collider[] hit = null;
		hit = Physics.OverlapSphere(transform.position, transform.localScale.x, 1 << 8);
		for (int i = 0; i < hit.Length; i++)
		{
			IDamageable target = hit[i].GetComponent<IDamageable>();
			Damage damage = new Damage(
				this.gameObject,
				damageAmount,
				.3f,
				(hit[i].transform.position - transform.position).normalized * 2f,
				Damage.DamageType.Normal
			);
			target?.OnDamage(damage);
		}
		Invoke("Hide", 1f);
	}
	void Hide() {
		gameObject.SetActive(false);
	}
}
