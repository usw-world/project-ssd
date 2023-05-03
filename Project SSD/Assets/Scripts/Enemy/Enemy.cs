using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable {
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float detectInterval = .05f;

    protected StateMachine enemyStateMachine;
    [SerializeField] protected Animator enemyAnimator;

    [SerializeField] protected float hp;
    protected bool isDead;

    protected bool targetInRange;

    protected GameObject target;

    protected Action updateTargetEvent;
    protected Action lostTargetEvent;

    protected virtual void Awake() {
        enemyStateMachine = GetComponent<StateMachine>();
        enemyAnimator = enemyAnimator==null ? GetComponent<Animator>() : enemyAnimator;
        target = FindObjectOfType<TPlayer>()?.gameObject;
    }
    protected virtual void Start() {
        /* temporary : Initialize function must be called on instantiated by other object. 
                       reason : Pooling Issue >> */
        Initialize();
        /* << temporary */
    }
    protected virtual void Update() {}
    public void Initialize() {
        StartCoroutine(UpdateTargetCoroutine());
    }
    public virtual void OnDamage(GameObject origin, float amount) {
        hp -= amount;
        if(hp <= 0) {
            OnDie();
        }
    }
    private IEnumerator UpdateTargetCoroutine() {
        while(!isDead) {
            Collider[] inners = Physics.OverlapSphere(transform.position, detectRange, 1<<7);
            if(inners.Length > 0)
                target = inners[0].gameObject;

            if(target != null
            && Vector3.Distance(transform.position, target.transform.position) < detectRange) {
                targetInRange = true;
                updateTargetEvent?.Invoke();
            } else {
                targetInRange = false;
                lostTargetEvent?.Invoke();
            }
            yield return new WaitForSeconds(detectInterval);
        }
    }
    protected virtual void OnDie() {
        isDead = true;
        gameObject.layer = 9;
    }
}