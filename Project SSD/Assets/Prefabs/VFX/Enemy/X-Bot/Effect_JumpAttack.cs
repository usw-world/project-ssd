using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_JumpAttack : MonoBehaviour {
    [SerializeField] private float radius = 2.5f;
    private void OnEnable() {
        Collider[] inners = Physics.OverlapSphere(transform.position, radius, 1<<7);
        foreach(Collider inner in inners) {
            inner.GetComponent<TPlayer>()?.OnDamage(gameObject, 25f);
        }
    }
}
