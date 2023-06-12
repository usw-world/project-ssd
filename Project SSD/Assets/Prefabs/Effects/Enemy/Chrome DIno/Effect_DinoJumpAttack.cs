using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_DinoJumpAttack : MonoBehaviour, IPoolableObject {
    [SerializeField] public float radius = 5f;
	[SerializeField] public float amount = 30f;
    [SerializeField] private ParticleSystem piecesParticle;
    [SerializeField] private ParticleSystem crackParticle;

	public string GetKey() {
        return this.GetType().ToString();
    }

    private void Awake() {

    }
    private void OnEnable() {
        ParticleSystem.ShapeModule shape = piecesParticle.shape;
        shape.radius = this.radius;
        ParticleSystem.MainModule main = crackParticle.main;
        main.startSize = this.radius*2;
    }
    
    public void AttackArea(Vector3 center) {
        transform.position = center;
        StartCoroutine(DisapearCoroutine());
        Collider[] inners = Physics.OverlapSphere(transform.position, radius, ~(1<<8));
        foreach(Collider inner in inners) {
			Damage damage = new Damage(
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
