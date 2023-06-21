using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mirror;
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour, IDamageable {
    public int networkId = -1;

    public Vector3 nextPosition;
    public Quaternion nextRotation;

    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float detectInterval = .05f;

    protected StateMachine enemyStateMachine;
    protected Dictionary<string, State> enemyStatesMap = new Dictionary<string, State>();
    [SerializeField] protected Animator enemyAnimator;
    protected AttachmentManager attachmentManager; // 버프/디버프 메니저

    #region Status
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    public bool isDead { get; protected set; }

    [SerializeField] protected Slider hpSlider;
    #endregion Status

    protected bool targetInRange;

    protected GameObject target;

    protected Action updateTargetEvent;
    protected Action lostTargetEvent;

    protected bool onHost = false;

    protected virtual void Awake() {
        enemyStateMachine = GetComponent<StateMachine>();
        enemyAnimator = enemyAnimator==null ? GetComponent<Animator>() : enemyAnimator;
        target = FindObjectOfType<TPlayer>()?.gameObject;
        attachmentManager = GetComponent<AttachmentManager>(); // 버프/디버프 메니저
		if (attachmentManager == null) {
            attachmentManager = gameObject.AddComponent<AttachmentManager>();
        }
    }
    protected virtual void Start() {
        if(SSDNetworkManager.instance.isHost)
            onHost = true;

        hp = maxHp;
        // RefreshStatusUI();
        RefreshHPSlider();
    }
    protected virtual void OnEnable() {
        Initialize();
        target = target ?? FindObjectOfType<TPlayer>()?.gameObject;
    }
    protected virtual void Update() {
        InterpolateTransform();
    }
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
    protected virtual void Initialize() {
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
            Debug.LogWarning($"Wrong state is called. Called state is '{stateName}'");
        }
    }
    public virtual void OnDamage(Damage damage) {
        if(SSDNetworkManager.instance.isHost) {
            var message = new S2CMessage.DamageMessage(this.networkId, damage);
            NetworkServer.SendToAll(message);
        }
    }
    public virtual void TakeDamage(Damage damage) {
        hp -= damage.amount;
        RefreshHPSlider();
        if(hp <= 0) {
            OnDie();
        }
        Vector3 ylessForceVector = Vector3.Scale(new Vector3(1, 0, 1), damage.forceVector);
        transform.LookAt(transform.position - ylessForceVector);
    }
    public virtual void AddAttachment(Attachment attachment) {
        // 버프/디버프 추가하는 함수!
        attachmentManager.AddAttachment(attachment);
    }
    private void InterpolateTransform() {
        if(!onHost) {
            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * 16);
            transform.rotation = nextRotation;
        }
    }

    #region Status UI Controll
    // protected virtual void RefreshStatusUI() {
    //     SetHPSlider(hp / maxHp);
    // }
    protected virtual void RefreshHPSlider() {
        if(hpSlider != null)
            hpSlider.value = hp / maxHp;
    }
    #endregion Status UI Controll
}