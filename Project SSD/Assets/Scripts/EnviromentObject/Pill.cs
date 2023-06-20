using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public float healingAmount;

    private void Update()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.y += Time.deltaTime * 15;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(7))
        {
            other.gameObject.GetComponent<TPlayer>().ChangeHp(healingAmount);
            Destroy(gameObject);
        }
    }
}
