using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

public class Aoe : MonoBehaviour, IPoolableObject
{
    private float damageAmount;
    private bool isOnDamage = false;

    [HideInInspector] public ParticleSystem particle;
    [HideInInspector] public bool isTPlayerOnAoe = false;

    public void OnActive(float damage)
    {
        this.damageAmount = damage;
    }
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DestoryTimer());
    }


    private void OnTriggerStay(Collider enemy)
    {
        //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, new Vector3(transform.position.x, enemy.transform.position.y, transform.position.z), 0.1f); // 블랙홀 기능

        //if (particle.particleCount <= 0f)
        //{
        //    IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
        //    Damage damage = new Damage(gameObject, damageAmount, 1f, (enemy.transform.position - transform.position).normalized * 10f, Damage.DamageType.Normal);

        //    if (target != null)
        //        target.OnDamage(damage);

        //    return;
        //}
    }

    private void OnTriggerEnter(Collider coll)
    {
        EnterEnemyOnAoe(coll);
        EnterTPlayerOnAoe(coll);
    }

    private void EnterEnemyOnAoe(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy") && isOnDamage == false)
        {
            IDamageable target = coll.gameObject.GetComponent<IDamageable>();
            Damage damage = new Damage(damageAmount, 1f, (coll.transform.position - transform.position).normalized * 10f, Damage.DamageType.Normal);

            if (target != null)
                target.OnDamage(damage);

            isOnDamage = true;

        }
    }

    private void EnterTPlayerOnAoe(Collider coll)
    {
        if (coll.CompareTag("TPlayer"))
        {
            isTPlayerOnAoe = true;
        }
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
