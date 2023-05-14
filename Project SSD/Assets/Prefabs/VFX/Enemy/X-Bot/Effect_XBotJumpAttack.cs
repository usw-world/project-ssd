using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_XBotJumpAttack : MonoBehaviour, IPoolerableObject {
    [SerializeField] private float radius = 2.5f;

    public string GetKey() {
        return this.GetType().ToString();
    }

    private void OnEnable() {
        StartCoroutine(DisapearCoroutine());
        Collider[] inners = Physics.OverlapSphere(transform.position, radius, 1<<7);
        foreach(Collider inner in inners) {
            inner.GetComponent<TPlayer>()?.OnDamage(gameObject, 25f);
        }
    }
    private IEnumerator DisapearCoroutine() {
        yield return new WaitForSeconds(5f);
        PoolerManager.instance.InPool(this.GetKey(), this.gameObject);
    }
}
