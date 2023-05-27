using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAttackDamage
{
    public bool isMultipleAttack = false;
    public bool isExplosion = false;

    public float multipleAttackDamage = 15f;
    public float explosionDamage = 100f;
    public int damageFrequency = 6;
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
    
    public void DoGeneralDamageAttack(Collider enemy, Transform aoeTransform)
    {
        IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
        Damage damage = new Damage(Instance.damageAmount, 1f, (enemy.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
        
        target.OnDamage(damage);
    }

    public IEnumerator DoMultipleAttack(Collider enemy, Transform aoeTransform)
    {
        IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
        for (int i = 0; i < damageFrequency; i++)
        {
            Damage damage = new Damage(Instance.multipleAttackDamage, 1f, (enemy.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
           
            target.OnDamage(damage);
            yield return new WaitForSeconds(0.5f);
        }
        isMultipleAttack = false;
    }

    public void DoExplosionDamageAttack(HashSet<Collider> attackedEnemies, Transform aoeTransform)
    {
        foreach (Collider c in attackedEnemies)
        {
            IDamageable target = c.gameObject.GetComponent<IDamageable>();
            Damage damage = new Damage(Instance.explosionDamage, 1f, (c.transform.position - aoeTransform.transform.position).normalized * 10f, Damage.DamageType.Normal);
           
            target.OnDamage(damage);
        }
        isExplosion = false;
    }
}
