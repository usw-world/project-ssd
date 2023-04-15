using System;
using UnityEngine;

public class DestroyableObject: MonoBehaviour, IDamageable {
    Rigidbody[] childrenRigidbodies;

    public void Awake() {
        childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    public void OnDamage(GameObject origin, float amount) {
        if(amount != 0) {
            Vector3 originPos = origin.transform.position;
            foreach(Rigidbody rb in childrenRigidbodies) {
                rb.isKinematic = false;
                rb.AddForce((rb.transform.position - originPos) * amount, ForceMode.VelocityChange);
            }
        }
    }
    /* temporary >> */
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
            OnDamage(other.gameObject, 5);
    }
    /* << temporary */
}