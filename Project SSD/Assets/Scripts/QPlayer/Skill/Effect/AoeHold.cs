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

    private ParticleSystem particle;
    private HashSet<Collider> attackedEnemies = new HashSet<Collider>();

    public GameObject explosion;

    [Obsolete]
    public void Run()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DestoryTimer());
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

        holdCollider.isTrigger = false;
    }

    private void EnterTPlayerInAoe()
    {
        AoeBuffer.GetInstance().onEnter.Invoke();
    }

	[Obsolete]
	IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(particle.startLifetime - 0.2f);
        if (AoeAttackDamage.GetInstance().isExplosion)
        {
			CameraManager.instance.MakeNoise(2f, 0.3f);
			GameObject obj = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
            AoeAttackDamage.GetInstance().DoExplosionDamageAttack(attackedEnemies, transform);
            Destroy(obj, obj.GetComponent<ParticleSystem>().startLifetime);
        }
		PoolerManager.instance.InPool(GetType().ToString(), gameObject);
	}

    public string GetKey()
    {
        return GetType().ToString();
    }

}
