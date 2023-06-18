using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Enemy_Flower_throwObj : MonoBehaviour, IPoolableObject
{
    public Vector3 targetPos;
    public float speed = 2;
    private float timer = 0;
    public Transform parent;
    private Damage damage;
    public string GetKey()
    {
        return GetType().ToString();
    }

    private void Start()
    {
        // parent = GetComponentInParent<Transform>();
        damage = new Damage(5, .5f, transform.forward * 5, Damage.DamageType.Normal);
    }

    private void Update()
    {
        // transform.Translate(Vector3.forward * (10 * Time.deltaTime));
        transform.Translate(transform.forward * (10 * Time.deltaTime), Space.World);
        timer += Time.deltaTime;
        if(timer > 5)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(1 << 7))
        {
            other.GetComponent<IDamageable>().OnDamage(damage);
            Destroy(gameObject);
        }
        else if(other.gameObject.layer.Equals(1 << 6))
        {
            Destroy(gameObject);
        }
    }
}
