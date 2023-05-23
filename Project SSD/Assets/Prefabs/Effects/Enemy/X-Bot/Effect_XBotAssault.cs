using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_XBotAssault : MonoBehaviour {
	[SerializeField] private float amount = 10f;
	[SerializeField] private float forceScalar = 35f;
	private bool hasHit = false;

	private void OnEnable() {
		hasHit = false;
	}
	private void OnTriggerEnter(Collider other) {
        if(!hasHit
		&& other.gameObject.layer == 7) {
			Vector3 force = (other.transform.position - transform.position).normalized * forceScalar;
			Damage damage = new Damage(
				amount,
				.75f,
				force,
				Damage.DamageType.Normal
			); 
			other.GetComponent<IDamageable>()?.OnDamage(damage);
			hasHit = true;
        }
    }
}
