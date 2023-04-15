using System;
using UnityEngine;

public class DestroyableObject: MonoBehaviour, IDamageable {
    bool isDestroyed = false;
    Rigidbody[] childrenRigidbodies;

    public void Awake() {
        childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    public void OnDamage(GameObject origin, float amount) {
        if(!isDestroyed
        && amount != 0) {
            Vector3 originPos = origin.transform.position;
            foreach(Rigidbody rb in childrenRigidbodies) {
                rb.isKinematic = false;
                rb.AddForce((rb.transform.position - originPos) * amount, ForceMode.VelocityChange);
                isDestroyed = true;
            }
        }
    }
    /* temporary >> */
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            OnDamage(other.gameObject, 5);
        }
    }
    /* << temporary */
}