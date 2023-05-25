using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AoeAttackDamage
{
    public bool isMultipleAttack = false;
    public bool isExplosion = false;

    private float multipleAttackDamage = 15f;
    private float explosionDamage = 100f;

    public float damageAmount { get; set; }

    private static AoeAttackDamage Instance = null;

    public static AoeAttackDamage GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AoeAttackDamage();
        }
        return Instance;
    }
    
    public void DoGeneralDamageAttack(IDamageable target, Collider enemy, Transform aoeTransform)
    {
        Damage damage = new Damage(Instance.damageAmount, 1f, (enemy.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
        target.OnDamage(damage);
    }
    public IEnumerator DoMultipleAttack(IDamageable target, Collider enemy, Transform aoeTransform)
    {
        for (int i = 0; i < 6; i++)
        {
            Damage damage = new Damage(Instance.multipleAttackDamage, 1f, (enemy.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
            target.OnDamage(damage);
            yield return new WaitForSeconds(0.5f);
        }
        isMultipleAttack = false;
    }
    public void DoExplosionDamageAttack(IDamageable target, Collider enemy, Transform aoeTransform)
    {
        Damage damage = new Damage(Instance.explosionDamage, 1f, (enemy.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
        target.OnDamage(damage);

        isExplosion = false;
    }
}
