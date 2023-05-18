using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_YBotBomb : MonoBehaviour, IPoolableObject {
	[SerializeField] private float damageAmount = 10f;
	private float currentSpeed;
	
	[SerializeField] private ParticleSystem bombParticle;
	[SerializeField] private GameObject explosionGobj;
	
    public string GetKey() {
        return GetType().ToString();
    }

	private void OnEnable() {
		currentSpeed = 10;
	}
	private void Update() {
		transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);
		currentSpeed -= Time.deltaTime * 20;
	}
	private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 6
		&& currentSpeed < 0) {
			bombParticle.Stop();
			explosionGobj.SetActive(true);
        }
		StartCoroutine(InPoolCoroutine());
    }
	private IEnumerator InPoolCoroutine() {
		yield return new WaitForSeconds(5f);
		PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
	}
}
