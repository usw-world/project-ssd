using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_XBotJumpAttack : MonoBehaviour, IPoolableObject {
    [SerializeField] private float radius = 2.5f;
	[SerializeField] private float amount = 10f;

	public string GetKey() {
        return this.GetType().ToString();
    }

    private void OnEnable() {
        StartCoroutine(DisapearCoroutine());
        Collider[] inners = Physics.OverlapSphere(transform.position, radius, 1<<7);
        foreach(Collider inner in inners) {
			Damage damage = new Damage(
				gameObject, 
				amount, 
				0,
				Vector3.zero, 
				Damage.DamageType.Normal
			);
            inner.GetComponent<IDamageable>()?.OnDamage(damage);
        }
    }
    private IEnumerator DisapearCoroutine() {
        yield return new WaitForSeconds(5f);
        PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
    }
}
