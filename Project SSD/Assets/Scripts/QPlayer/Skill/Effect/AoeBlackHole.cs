using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeBlackHole : MonoBehaviour, IPoolableObject
{
    private ParticleSystem particle;
    private IDamageable target;
    private Collider colliderEnemy;

    public GameObject explosion;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DestoryTimer());
    }

    private void OnTriggerStay(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, new Vector3(transform.position.x, enemy.transform.position.y, transform.position.z), 0.1f); // 블랙홀 기능
        }
    }

    private void OnTriggerEnter(Collider enemy)
    {
        EnterEnemyInAoe(enemy);
    }

    private void EnterEnemyInAoe(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
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
        }
    }
    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(particle.startLifetime);
        PoolerManager.instance.InPool(GetType().ToString(), gameObject);

        if (AoeAttackDamage.GetInstance().isExplosion && target != null)
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
