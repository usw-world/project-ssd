using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Aoe : MonoBehaviour, IPoolableObject
{

    private float damageAmount = 50f;

    private void Start()
    {

        StartCoroutine(DestoryTimer());
    }


    private void OnTriggerStay(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

            //IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
            //Damage damage = new Damage(gameObject, damageAmount, 1f, (enemy.transform.position - transform.position).normalized * 10f, Damage.DamageType.Normal);
            //target.OnDamage(damage);
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
