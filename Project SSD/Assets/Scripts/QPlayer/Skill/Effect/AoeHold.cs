using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

public class AoeHold : MonoBehaviour, IPoolableObject
{
    public MeshCollider holdCollider;

    private bool isOnDamage = false;

    private ParticleSystem particle;
    private IDamageable target;
    private Collider colliderEnemy;

    public GameObject explosion;
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DestoryTimer());
    }

    private void OnTriggerEnter(Collider enemy)
    {
        EnterEnemyOnAoe(enemy);
    }

    private void EnterEnemyOnAoe(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy") && isOnDamage == false)
        {
            this.colliderEnemy = enemy;
            target = enemy.gameObject.GetComponent<IDamageable>();

            if (AoeAttackDamage.GetInstance().isMultipleAttack)
            {
                StartCoroutine(AoeAttackDamage.GetInstance().DoMultipleAttack(target, enemy, this.transform));
            }
            else
            {
                AoeAttackDamage.GetInstance().DoGeneralDamageAttack(target, enemy, this.transform);
            }

            isOnDamage = true;

            this.transform.GetComponent<CapsuleCollider>().enabled = false;
            holdCollider.isTrigger = false;
        }
    }
    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(particle.startLifetime);
        PoolerManager.instance.InPool(GetType().ToString(), gameObject);

        if (AoeAttackDamage.GetInstance().isExplosion)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            AoeAttackDamage.GetInstance().DoExplosionDamageAttack(target, colliderEnemy, this.transform);
        }
    }

    public string GetKey()
    {
        return GetType().ToString();
    }

}
