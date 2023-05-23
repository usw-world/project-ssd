using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Effect_YBotMineSpell : MonoBehaviour, IPoolableObject {
	[SerializeField] private float explosionRadius = 3f;
	[SerializeField] private float explosionDelay = .25f;
	private float triggerTime = 0;

	[SerializeField] private float damageAmount = 35f;
	[SerializeField] private float forceScalar = 10f;
	[SerializeField] private float hittingDuration = .75f;


	[SerializeField] private CapsuleCollider capsuleCollider;
	[SerializeField] private DecalProjector delayProjector;
	[SerializeField] private DecalProjector rangeProjector;
	private float duration = 10f;
	private float lifetime = 0;
	private bool hasTriggered = false;
	private bool hasExplosion = false;
	
	[SerializeField] private ParticleSystem apearParticle;
	[SerializeField] private ParticleSystem idleParticle;
	[SerializeField] private ParticleSystem explosionParticle;
	private Coroutine inPoolCoroutine;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		apearParticle.Play();
		idleParticle.Play();
		lifetime = 0;
		triggerTime = 0;
		hasTriggered = false;

		rangeProjector.enabled = true;
		delayProjector.enabled = true;
		rangeProjector.size = new Vector3(explosionRadius*2, explosionRadius*2, rangeProjector.size.z);
		delayProjector.size = new Vector3(0, 0, delayProjector.size.z);
		explosionParticle.transform.localScale = Vector3.one * 1/2 * explosionRadius;

		capsuleCollider.radius = explosionRadius;
		capsuleCollider.height = explosionRadius*2 + 5;
	}
	private void Update() {
		if(hasExplosion)
			return;
			
		lifetime += Time.deltaTime;
		if(hasTriggered) {
			triggerTime = Mathf.Min(explosionDelay, triggerTime+Time.deltaTime);
			delayProjector.size = new Vector3(explosionRadius*2 * 1/explosionDelay * triggerTime, explosionRadius*2 * 1/explosionDelay * triggerTime, delayProjector.size.z);
			if(triggerTime >= explosionDelay) {
				Explosion();
			}
		}

		if(lifetime >= duration) {
			Disapear();
		}
	}
	private void OnTriggerEnter(Collider other) {
        if(!hasTriggered
		&& other.gameObject.layer == 7) { // Player Layer
			TriggerMine();
			Disapear();
        }
    }
	private void TriggerMine() {
		hasTriggered = true;
	}
	private void Disapear() {
		idleParticle.Stop();
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		inPoolCoroutine = StartCoroutine(InPoolCoroutine());
	}
	private void Explosion() {
		hasExplosion = true;
		Collider[] inners = Physics.OverlapSphere(transform.position, explosionRadius, 1<<7);
		for (int i=0; i<inners.Length; i++) {
			TPlayer player = inners[i].GetComponent<TPlayer>();
			if(player != null) {
				Vector3 forceVector = (inners[i].transform.position - this.transform.position).normalized * forceScalar;
				Damage damage = new Damage(
					this.damageAmount,
					this.hittingDuration,
					forceVector,
					Damage.DamageType.Normal
				);
				player.OnDamage(damage);
			}
		}
		explosionParticle.Play();
		rangeProjector.enabled = false;
		delayProjector.enabled = false;
		Disapear();
	}
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		explosionParticle.Stop();
		rangeProjector.enabled = false;
		delayProjector.enabled = false;
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
