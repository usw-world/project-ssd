using System.Collections;
using UnityEngine;

class Enemy_Pteradonon : MovableEnemy {
    #region States
    private State idleState = new State("Idle");
    private State chaseState = new State("Chase");
    private State attackState = new State("Attack");
    private State stunnedState = new State("Stunned");
    private State hitState = new State("Hit");
    private State dieState = new State("Die");
    #endregion States

    private Vector3 targetPosition;

    #region Attack
    [SerializeField] private Effect_PteradononAttack attackEffect;
    #endregion Attack

    #region Hit
    private Coroutine hitCoroutine;
    #endregion Hit

    #region Unity Events
    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
        enemyStateMachine.SetIntialState(idleState);
        InitializeState();
        InitializePoolers();
    }
    #endregion Unity Events

    private void InitializePoolers() {
        PoolerManager.instance.InsertPooler(this.attackEffect.GetKey(), this.attackEffect.gameObject, true);
    }
    private void InitializeState() {
        idleState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Idle", true);
            enemyMovement.Stop();
        };
        idleState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Idle", false);
        };
        chaseState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Chase", true);
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
            enemyMovement.MoveToPoint(targetPosition, moveSpeed, moveLayerMask);
        };
        chaseState.onStay += () => {
            if(IsArrive) {
                SendChangeState(idleState);
            }
        };
        chaseState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Chase", false);
            enemyMovement.Stop();
        };
        attackState.onActive += (State prevState) => {
            enemyMovement.Stop();
            enemyAnimator.SetBool("Attack", true);
            transform.LookAt(Vector3.Scale(new Vector3(1,0,1), targetPosition));
		};
        attackState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Attack", false);
        };
        stunnedState.onActive = (State prevState) => {
            enemyAnimator.SetBool("Stunned", false);
        };
        stunnedState.onInactive = (State nextState) => {
            enemyAnimator.SetBool("Stunned", false);
        };
        hitState.onActive += (State prevState) => {
            if(prevState.Compare(hitState))
                enemyAnimator.SetTrigger("Hit Trigger");
            else
                enemyAnimator.SetBool("Hit", true);
        };
        hitState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Hit", false);
            if(hitCoroutine != null
            && !nextState.Compare(hitState))
                StopCoroutine(hitCoroutine);
        };
        dieState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Die", true);
            enemyStateMachine.isMuted = true;
            hpSlider.gameObject.SetActive(false);
        };
        dieState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Die", false);
            enemyStateMachine.isMuted = false;
            hpSlider.gameObject.SetActive(true);
        };
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(attackState.stateName, attackState);
        enemyStatesMap.Add(stunnedState.stateName, stunnedState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        enemyStatesMap.Add(dieState.stateName, dieState);
    }
    protected override void ChaseTarget(Vector3 point) {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(dieState)
        || enemyStateMachine.Compare(attackState))
            return;

        targetPosition = point;

        if(!TryAttack()) {
            if((   enemyStateMachine.Compare(idleState)
                || enemyStateMachine.Compare(chaseState))
            && !IsArrive) {
                SendChangeState(chaseState, true);
            }
        }
    }
    protected override void OnLostTarget() {
        SendChangeState(idleState);
    }
    private bool TryAttack() {
        if(DistanceToTarget < 5f
        && !enemyStateMachine.Compare(attackState)) {
            SendChangeState(attackState);
            return true;
        }
        return false;
    }

    #region Animation Events
    public void AnimationEvent_OnAttackAction() {
        var effect = PoolerManager.instance.OutPool(attackEffect.GetKey());
        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
    }
    public void AnimationEvent_OnEndAttack() {
        SendChangeState(idleState);
    }
    #endregion Animation Events

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
    }
    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if(!isDead) {
            if(hitCoroutine != null)
                StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(damage));
        }
    }
    private IEnumerator HitCoroutine(Damage damage) {
        float offset = 0;
        float pushedOffset = 0;
        Vector3 pushedDestination = Vector3.Scale(new Vector3(1, 0, 1), damage.forceVector);
        if(damage.hittingDuration > 0)
            SendChangeState(hitState);
        while(offset < damage.hittingDuration) {
            enemyMovement.MoveToward(Vector3.Lerp(pushedDestination, Vector3.zero, pushedOffset) * Time.deltaTime, Space.World, moveLayerMask);
            pushedOffset += Time.deltaTime * 2;
            offset += Time.deltaTime;
            yield return null;
        }
        SendChangeState(idleState);
    }
    protected override void OnDie() {
        base.OnDie();
        SendChangeState(dieState);
    }
}