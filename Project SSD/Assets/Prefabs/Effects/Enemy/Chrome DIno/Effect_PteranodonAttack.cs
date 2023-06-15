using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PteranodonAttack : MonoBehaviour, IPoolableObject {
	[SerializeField] private float damageAmount = 16f;
	[SerializeField] private float flyingSpeed = 2f;
	[SerializeField] private float hittingDuration = .45f;
	[SerializeField] private float forceScalar = 15f;
	private float duration = 5f;
	private float lifetime = 0;
	private bool isActive = false;

	private float hitInterval = .5f;
	private float lastHitTime = -.5f;
	
	[SerializeField] private ParticleSystem particle;
	private Coroutine inPoolCoroutine;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		particle.Play();
		lifetime = 0;
		isActive = true;
	}
	private void Update() {
		transform.Translate(Vector3.forward * flyingSpeed * Time.deltaTime);
		lifetime += Time.deltaTime;
		if(lifetime >= duration) {
			Disappear();
		}
	}
	private void OnTriggerEnter(Collider other) {
        if(isActive
		&& lastHitTime+hittingDuration < Time.time
		&& other.gameObject.layer == 7) { // Player Layer
			lastHitTime = Time.time;
			Vector3 forceVector = (other.transform.position - this.transform.position).normalized * forceScalar;
			Damage damage = new Damage(
				this.damageAmount,
				this.hittingDuration,
				forceVector,
				Damage.DamageType.Down
			);
			other.GetComponent<TPlayer>()?.OnDamage(damage);
        }
    }
	private void Disappear() {
		isActive = false;
		particle.Stop();
		inPoolCoroutine = StartCoroutine(InPoolCoroutine());
	}
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
