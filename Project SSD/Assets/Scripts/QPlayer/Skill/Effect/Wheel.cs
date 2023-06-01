using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour, IPoolableObject
{
    public float strength;
    public bool rotate = true;
    public Skill_wheel skill_Wheel = null;

    Wheel(float strength)
    {
        this.strength = strength;
    }
    public Wheel(float strength, Skill_wheel skill_Wheel)
    {
        this.strength= strength;
        this.skill_Wheel = skill_Wheel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out IDamageable enemy)) {
            Vector3 force = transform.forward * 15f;
            Damage damage = new Damage(
                10,
                .5f,
                force,
                Damage.DamageType.Normal
            );
            enemy.OnDamage(damage);
        }
    }

    private void Update()
    {
        // if(rotate)
            // transform.Rotate((new Vector3(0, 5, 0)));
    }

    public string GetKey()
    {
        return GetType().ToString();
    }
}
