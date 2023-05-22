using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_YBotShootingSpell : MonoBehaviour, IPoolableObject {
	[SerializeField] private float damageAmount = 25f;
	[SerializeField] private float flyingSpeed = 12f;
	[SerializeField] private float hittingDuration = .75f;
	[SerializeField] private float forceScalar = 10f;
	private float duration = 10f;
	private float lifetime = 0;
	private bool isActive = false;
	
	[SerializeField] private ParticleSystem flyingParticle;
	[SerializeField] private ParticleSystem explosionParticle;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		flyingParticle.Play();
		lifetime = 0;
		isActive = true;
	}
	private void Update() {
		transform.Translate(Vector3.forward * flyingSpeed * Time.deltaTime);

		lifetime += Time.deltaTime;
		if(lifetime >= duration) {
			Disapear();
		}
	}
	private void OnTriggerEnter(Collider other) {
        if(isActive
		&& other.gameObject.layer == 7) { // Player Layer
			Vector3 forceVector = (other.transform.position - this.transform.position).normalized * forceScalar;
			Damage damage = new Damage(
				this.gameObject,
				this.damageAmount,
				this.hittingDuration,
				forceVector,
				Damage.DamageType.Normal
			);
			other.GetComponent<TPlayer>()?.OnDamage(damage);
			Disapear();
        }
    }
	private void Disapear() {
		isActive = false;
		flyingParticle.Stop();
		explosionParticle.Play();
		StartCoroutine(InPoolCoroutine());
	}
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		explosionParticle.Stop();
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
