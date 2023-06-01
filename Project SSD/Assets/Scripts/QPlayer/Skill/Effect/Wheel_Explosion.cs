using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Explosion : MonoBehaviour, IPoolableObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out IDamageable enemy);
        Vector3 force = transform.forward * 15f;
        Damage damage = new Damage(10, .5f, force, Damage.DamageType.Normal);
        enemy.OnDamage(damage);
    }

    public string GetKey()
    {
        return GetType().ToString();
    }
}
