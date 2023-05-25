using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

public class Aoe : MonoBehaviour, IPoolableObject
{
    private bool isOnDamage = false;

    private ParticleSystem particle;

    private List<Collider> enemies = new List<Collider>();

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DestoryTimer());
    }

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemies.Add(enemy);
        }
        print(isOnDamage);
        if (isOnDamage == false)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnterEnemyInAoe(enemies[i]);
            }
            isOnDamage = true;
        }
        print(isOnDamage + " aaa");
    }

    private void EnterEnemyInAoe(Collider enemy)
    {
            IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
            if (target != null)
                AoeAttackDamage.GetInstance().DoGeneralDamageAttack(target, enemy, transform);
    }

    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(particle.startLifetime);
        PoolerManager.instance.InPool(GetType().ToString(), gameObject);
    }

    public string GetKey()
    {
        return GetType().ToString();
    }

}
