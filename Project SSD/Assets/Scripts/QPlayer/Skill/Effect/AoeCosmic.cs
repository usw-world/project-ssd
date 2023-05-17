using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeCosmic : MonoBehaviour, IPoolableObject
{
    ParticleSystem particle;

    private float damageAmount = 50f;
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

        if (particle.particleCount <= 0f)
        {
            IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
            Damage damage = new Damage(gameObject, damageAmount, 1f, (enemy.transform.position - transform.position).normalized * 10f, Damage.DamageType.Normal);
            target.OnDamage(damage);
            return;
        }
    }

    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(3f);
        PoolerManager.instance.InPool(GetType().ToString(), gameObject);
    }
    public string GetKey()
    {
        return GetType().ToString();
    }
}
