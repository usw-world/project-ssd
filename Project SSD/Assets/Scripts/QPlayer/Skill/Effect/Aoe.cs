using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

public class Aoe : MonoBehaviour, IPoolableObject
{
    private ParticleSystem particle;
    private HashSet<Collider> attackedEnemies = new HashSet<Collider>();

	[Obsolete]
	private void Start()
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
         AoeAttackDamage.GetInstance().DoGeneralDamageAttack(enemy, transform);
    }

    private void EnterTPlayerInAoe()
    {
        AoeBuffer.GetInstance()?.onEnter?.Invoke();
    }

	[Obsolete]
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

public class AoeBuffer
{
    public Action onEnter;

    private static AoeBuffer Instance = null;

    public static AoeBuffer GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AoeBuffer();
        }
        return Instance;
    }
}
