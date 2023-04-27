using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Assault : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 7) {
            other.GetComponent<TPlayer>()?.OnDamage(gameObject, 12f);
        }
    }
}
