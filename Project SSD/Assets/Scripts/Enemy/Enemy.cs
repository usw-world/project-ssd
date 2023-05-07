using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public abstract class Enemy : MonoBehaviour, IDamageable {
    public int networkId = -1;

    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float detectInterval = .05f;

    protected StateMachine enemyStateMachine;
    protected Dictionary<string, State> enemyStatesMap = new Dictionary<string, State>();
    [SerializeField] protected Animator enemyAnimator;

    [SerializeField] protected float hp;
    protected bool isDead;

    protected bool targetInRange;

    protected GameObject target;

    protected Action updateTargetEvent;
    protected Action lostTargetEvent;

    private bool onHost = false;

    protected virtual void Awake() {
        enemyStateMachine = GetComponent<StateMachine>();
        enemyAnimator = enemyAnimator==null ? GetComponent<Animator>() : enemyAnimator;
        target = FindObjectOfType<TPlayer>()?.gameObject;
    }
    protected virtual void Start() {
        /* temporary : Initialize function must be called on instantiated by other object. 
                       reason : Pooling Issue >> */
        /* << temporary */
        if(SSDNetworkManager.instance.isHost)
            onHost = true;
    }
    protected virtual void OnEnable() {
        Initialize();
    }
    protected virtual void Update() {}
    public void Initialize() {
        StartCoroutine(UpdateTargetCoroutine());
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
    protected void SendChangeState(State state, bool intoSelf=true) {
        if(onHost) {
            NetworkClient.Send(new C2SMessage.ChangeStateMessage(networkId, state.stateName));
        }
    }
    public void ChangeState(string stateName) {
        State nextState;
        if(enemyStatesMap.TryGetValue(stateName, out nextState)) {
            enemyStateMachine.ChangeState(nextState);
        } else {
            Debug.LogWarning("Wrong state is called.");
        }
    }
    public virtual void OnDamage(Damage damage) {
        hp -= damage.amount;
        if(hp <= 0) {
            OnDie();
        }
    }
    private void SendDamageMessage() {
        var message = new S2CMessage.DamageMessage();
        NetworkClient.Send(message);
    }
}