using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    public abstract void OnDamage(GameObject origin, float amount);
}
public interface IGetAPable{
    public abstract float GetAP();
}
