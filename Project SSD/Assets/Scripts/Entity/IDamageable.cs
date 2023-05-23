using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    // public abstract void OnDamage(GameObject origin, float amount);

    public abstract void OnDamage(Damage damage);
}
[System.Serializable]
public struct Damage {
    public float amount;
    public float hittingDuration;
    public Vector3 forceVector;
    public DamageType damageType;

    public enum DamageType {
        Normal,
        Down
    }
    public Damage(float amount, float hittingDuration, Vector3 forceVector, DamageType damageType) {
        this.amount = amount;
        this.hittingDuration = hittingDuration;
        this.forceVector = forceVector;
        this.damageType = damageType;
    }
}
