using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable {
    
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float detectInterval = .25f;

    private float hp;
    private bool isDead;
    private bool targetInRange;

    GameObject target;
    Vector3 targetPoint;


    void Awake() {
        target = FindObjectOfType<TPlayer>().gameObject;
    }
    void OnEnable() {
        
    }
    void Update() {
        
    }

    public void OnDamage(GameObject origin, float amount) {
        
    }

    private IEnumerator DetectTargetCoroutine() {
        while(!isDead) {
            if(Vector3.Distance(transform.position, target.transform.position) < detectRange)
                targetInRange = true;
            else
                targetInRange = false;
            yield return detectInterval;
        }
    }
}