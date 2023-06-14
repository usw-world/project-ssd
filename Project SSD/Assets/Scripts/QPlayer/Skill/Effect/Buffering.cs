using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Buffering : MonoBehaviour, IPoolableObject
{
    public bool addShield = false;
    [FormerlySerializedAs("skillBuffer")] [FormerlySerializedAs("skill_Wheel")] public Skill_Buffering skillBuffering = null;
    
    public Buffering(Skill_Buffering skillBuffering)
    {
        this.skillBuffering = skillBuffering;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable enemy)) {
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
