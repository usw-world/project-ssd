using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boomber_Explosion : MonoBehaviour
{
    public Enemy_Bomber parent;
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out TPlayer player);
        other.TryGetComponent(out IDamageable damageable);
        
        if (player == null || damageable == null)
            return;
        Vector3 force = transform.forward * 15f;
        Damage damage = new Damage(5, .5f, force, Damage.DamageType.Normal);
        damageable.OnDamage(damage);
        parent.Explode();

    }
}
