using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_DinoBeamHit : MonoBehaviour, IPoolableObject {
	[SerializeField] public float damageAmount = 8f;
	[SerializeField] public float hittingDuration = .1f;
	[SerializeField] private float forceScalar = 5f;
	public bool isActive = false;

	private float hitInterval = .2f;
	private float lastHitTime = -.3f;
	
    public string GetKey() {
        return GetType().ToString();
    }
	private void OnTriggerStay(Collider other) {
        if(isActive
		&& lastHitTime+hitInterval < Time.time
		&& other.gameObject.layer == 7) { // Player Layer
			lastHitTime = Time.time;
			Vector3 forceVector = (other.transform.position - this.transform.position).normalized * forceScalar;
			Damage damage = new Damage(
				this.damageAmount,
				this.hittingDuration,
				forceVector,
				Damage.DamageType.Normal
			);
			other.GetComponent<TPlayer>()?.OnDamage(damage);
        }
    }
}
