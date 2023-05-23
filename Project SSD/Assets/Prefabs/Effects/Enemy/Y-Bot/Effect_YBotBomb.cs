using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_YBotBomb : MonoBehaviour, IPoolableObject {
	[SerializeField] private float damageAmount = 10f;
	[SerializeField] private float damageAreaRadius = 1.5f;
	[SerializeField] private float forceScalar = 10f;
	[SerializeField] private float hittingDuration = .7f;
	private float currentSpeed;
	
	[SerializeField] private ParticleSystem bombParticle;
	[SerializeField] private ParticleSystem explosionParticle;
	private Coroutine inPoolCoroutine;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		if(inPoolCoroutine != null)
			StopCoroutine(inPoolCoroutine);
		currentSpeed = 10;
		inPoolCoroutine = StartCoroutine(InPoolCoroutine());
	}
	private void Update() {
		transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);
		currentSpeed -= Time.deltaTime * 20;
	}
	private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 6
		&& currentSpeed < 0) {
			bombParticle.Stop();
			explosionParticle.Play();
			AttackPlayer();
        }
    }
	private void AttackPlayer() {
		Collider[] inners = Physics.OverlapSphere(transform.position, damageAreaRadius, 1<<7);
		for(int i=0; i<inners.Length; i++) {
			IDamageable target = inners[i].GetComponent<IDamageable>();
			if(target != null) {
				Collider inner = inners[i];
				Vector3 force = Vector3.Scale(new Vector3(1, 0, 1), inner.transform.position - transform.position).normalized * forceScalar;
				Damage damage = new Damage(
					damageAmount,
					hittingDuration,
					force,
					Damage.DamageType.Normal
				);
				target.OnDamage(damage);
			}
		}
	}
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(10f);
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
