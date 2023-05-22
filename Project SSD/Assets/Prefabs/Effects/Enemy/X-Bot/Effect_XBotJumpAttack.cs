using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_XBotJumpAttack : MonoBehaviour, IPoolableObject {
    [SerializeField] private float radius = 2.5f;
	[SerializeField] private float amount = 10f;

	public string GetKey() {
        return this.GetType().ToString();
    }
    
    public void AttackArea(Vector3 center) {
        transform.position = center;
        StartCoroutine(DisapearCoroutine());
        Collider[] inners = Physics.OverlapSphere(transform.position, radius, ~(1<<8));
        foreach(Collider inner in inners) {
			Damage damage = new Damage(
				gameObject, 
				amount, 
				1.5f,
				Vector3.zero, 
				Damage.DamageType.Down
			);
            inner.GetComponent<IDamageable>()?.OnDamage(damage);
        }
    }
    private IEnumerator DisapearCoroutine() {
        yield return new WaitForSeconds(5f);
        PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
    }
}
