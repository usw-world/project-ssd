using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable {
    
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float detectInterval = .25f;

    private float hp;
    private bool isDead;

    GameObject target;
    Vector3 targetPoint;

    private void Awake() {
        target = FindObjectOfType<TPlayer>().gameObject;

        
    }
    private void Update() {
        
    }

    public void OnDamage(GameObject origin, float amount) {
        
    }

    private bool TryDetectTarget() {
        if(Vector3.Distance(transform.position, target.transform.position) < detectRange) {
            return true;
        } else {
            return false;
        }
    }
}