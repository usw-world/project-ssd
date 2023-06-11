using System.Collections;
using UnityEngine;

class Enemy_XBot : MovableEnemy {
    #region States
    private State idleState = new State("Idle");
    private State chaseState = new State("Chase");
    private State BasicState {
        get {
            if(targetInRange)
                return chaseState;
            else
                return idleState;
        }
    }
    private State assaultState = new State("Assault");
    private State jumpAttackState = new State("Jump Attack");
    private State stunnedState = new State("Stunned");
    private State hitState = new State("Hit");
    private State dieState = new State("Die");
    #endregion States

    
    private Vector3 targetPosition;

    #region Jump Attack
    private const float JUMP_ATTACK_COOLTIME = 7f;
    private float currentJumpAttackCooltime = 0f;
    private Coroutine jumpAttackCoroutine;
    [SerializeField] private Effect_XBotJumpAttack jumpAttackEffect;
    [SerializeField] private Transform jumpAttackEffectPoint;
    #endregion Jump Attack

    #region Assault Attack
    private Coroutine assaultCoroutine;
    [SerializeField] private GameObject assaultEffect;
    #endregion Assault Attack

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
    protected override void Update() {
        if(currentJumpAttackCooltime > 0)
            currentJumpAttackCooltime -= Time.deltaTime;
    }
    #endregion Unity Events

    private void InitializePoolers() {
        PoolerManager.instance.InsertPooler(this.jumpAttackEffect.GetKey(), this.jumpAttackEffect.gameObject, true);
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
        assaultState.onActive += (State prevState) => {
            enemyMovement.Stop();
            enemyAnimator.SetBool("Assault Crouch", true);
            transform.LookAt(Vector3.Scale(new Vector3(1,0,1), targetPosition));
		};
        assaultState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Assault Crouch", false);
            if(assaultCoroutine != null)
                StopCoroutine(assaultCoroutine);
            assaultEffect.SetActive(false);
        };
        jumpAttackState.onActive += (State prevState) => {
            enemyMovement.Stop();
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
            enemyAnimator.SetBool("Jump Attack", true);
            currentJumpAttackCooltime = JUMP_ATTACK_COOLTIME;
            jumpAttackCoroutine = StartCoroutine(JumpAttackCoroutine());
        };
        jumpAttackState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Jump Attack", false);
            StopCoroutine(jumpAttackCoroutine);
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
        enemyStatesMap.Add(assaultState.stateName, assaultState);
        enemyStatesMap.Add(jumpAttackState.stateName, jumpAttackState);
        enemyStatesMap.Add(stunnedState.stateName, stunnedState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        enemyStatesMap.Add(dieState.stateName, dieState);
    }
    protected override void ChaseTarget(Vector3 point) {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(dieState))
            return;

        targetPosition = point;

        if(!TryAssault()
        && !TryJumpAttack()) {
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
    private bool TryJumpAttack() {
        if(DistanceToTarget > 3f
        && DistanceToTarget < 8f
        && !enemyStateMachine.Compare(jumpAttackState)
        && !enemyStateMachine.Compare(assaultState)
        && !enemyStateMachine.Compare(hitState)
        && currentJumpAttackCooltime <= 0) {
            SendChangeState(jumpAttackState);
            return true;
        }
        return false;
    }
    private IEnumerator JumpAttackCoroutine() {
        Vector3 point1 = transform.position;
        Vector3 point2 = targetPosition;
        Vector3 distance = point2 - point1;
        distance.y = 0;
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            enemyMovement.MoveToward(distance * Time.deltaTime*.8f, Space.World, moveLayerMask);
            yield return null;
        }
    }
    private bool TryAssault() {
        if(DistanceToTarget < 5f
        && !enemyStateMachine.Compare(jumpAttackState)
        && !enemyStateMachine.Compare(assaultState)
        && !enemyStateMachine.Compare(hitState)) {
            SendChangeState(assaultState, false);
            return true;
        }
        return false;
    }
    private IEnumerator AssaultCoroutine() {
        float offset = 0;
        transform.LookAt(targetPosition);
        Vector3 dir = (targetPosition - transform.position).normalized;
        dir.y = 0;

        while(offset < 1) {
            enemyMovement.MoveToward(dir * 10f * Time.deltaTime, Space.World, moveLayerMask);
            offset += Time.deltaTime;
            yield return null;
        }
        SendChangeState(idleState);
    }

    #region Animation Events
    public void AnimationEvent_OnEndJumpAttack() {
        SendChangeState(BasicState);
    }
    public void AnimationEvent_OnJumpAttackAction() {
        /* temporary >> */
        GameObject effect = PoolerManager.instance.OutPool(this.jumpAttackEffect.GetKey());
        effect.GetComponent<Effect_XBotJumpAttack>()?.AttackArea(jumpAttackEffectPoint.position);
        /* << temporary */
    }
    public void AnimationEvent_OnEndCrouch() {
        if(enemyStateMachine.Compare(assaultState)) {
            enemyAnimator.SetBool("Assault Crouch", false);
            assaultEffect.SetActive(true);
            assaultCoroutine = StartCoroutine(AssaultCoroutine());
        }
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
        SendChangeState(BasicState);
    }
    protected override void OnDie() {
        base.OnDie();
        SendChangeState(dieState);
    }
}