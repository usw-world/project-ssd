using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_YBotShootingSpell : MonoBehaviour, IPoolableObject {
	[SerializeField] private float damageAmount = 25f;
	[SerializeField] private float flyingSpeed = 12f;
	
	[SerializeField] private ParticleSystem flyingParticle;
	[SerializeField] private ParticleSystem explosionParticle;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		/*  */
	}
	private void Update() {
		transform.Translate(Vector3.forward * flyingSpeed * Time.deltaTime);
	}
	private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 7) { // Player Layer
			flyingParticle.Stop();
			explosionParticle.Play();
        }
		StartCoroutine(InPoolCoroutine());
    }
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
