using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeBlackHole : MonoBehaviour, IPoolableObject
{
    private ParticleSystem particle;
    private HashSet<Collider> attackedEnemies = new HashSet<Collider>();

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

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (!attackedEnemies.Contains(coll))
            {
                EnterEnemyInAoe(coll);
                attackedEnemies.Add(coll);
            }
        }

        if (coll.gameObject.CompareTag("TPlayer"))
        {
            EnterTPlayerInAoe();
        }
    }

    private void EnterEnemyInAoe(Collider enemy)
    {
        if (AoeAttackDamage.GetInstance().isMultipleAttack)
        {
            StartCoroutine(AoeAttackDamage.GetInstance().DoMultipleAttack(enemy, transform));
        }
        else
        {
            AoeAttackDamage.GetInstance().DoGeneralDamageAttack(enemy, transform);
        }
    }
    private void EnterTPlayerInAoe()
    {
        AoeBuffer.GetInstance().onEnter.Invoke();
    }
    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(particle.startLifetime);
        PoolerManager.instance.InPool(GetType().ToString(), gameObject);

        if (AoeAttackDamage.GetInstance().isExplosion)
        {
            GameObject obj = (GameObject) Instantiate(explosion, transform.position, transform.rotation);
            AoeAttackDamage.GetInstance().DoExplosionDamageAttack(attackedEnemies, transform);
            Destroy(obj, obj.GetComponent<ParticleSystem>().startLifetime);
        }
    }
    public string GetKey()
    {
        return GetType().ToString();
    }
}
