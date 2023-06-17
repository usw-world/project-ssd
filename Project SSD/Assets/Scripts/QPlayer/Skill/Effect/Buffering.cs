using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Buffering : MonoBehaviour, IPoolableObject
{
    public bool addShield = false;
    public Skill_Buffering skillBuffering = null;
    
    public Buffering(Skill_Buffering skillBuffering)
    {
        this.skillBuffering = skillBuffering;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable enemy) && !other.gameObject.layer.Equals(1 << 7 | 1<<11)) {
            Vector3 force = transform.forward * 15f;
            Damage damage = new Damage(
                10,
                .5f,
                force,
                Damage.DamageType.Normal
            );
            enemy.OnDamage(damage);
            if(addShield && TPlayer.instance != null)
                TPlayer.instance.AddShield(new TPlayerShield(5));
        }
    }



    public string GetKey()
    {
        return GetType().ToString();
    }
    
}
