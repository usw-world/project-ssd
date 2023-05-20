using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    // public abstract void OnDamage(GameObject origin, float amount);

    public abstract void OnDamage(Damage damage);
}
public struct Damage {
    public GameObject origin;
    public float amount;
    public float hittingDuration;
    public Vector3 forceVector;
    public DamageType damageType;

    public enum DamageType {
        Normal,
        Down
    }
    public Damage(GameObject origin, float amount, float hittingDuration, Vector3 forceVector, DamageType damageType) {
        this.origin = origin;
        this.amount = amount;
        this.hittingDuration = hittingDuration;
        this.forceVector = forceVector;
        this.damageType = damageType;
    }
}
