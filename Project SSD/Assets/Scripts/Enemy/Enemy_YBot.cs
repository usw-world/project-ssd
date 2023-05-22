using System.Collections;
using UnityEngine;

class Enemy_YBot : MovableEnemy {
    #region States
    private State idleState = new State("Idle");
    private State chaseState = new State("Chase");
    private State BasicState {
        get {
            if(!IsArrive && targetInRange)
                return chaseState;
            else
                return idleState;
        }
    }
    private State rollState = new State("Roll");
    private State shotState = new State("Shot");
    private State castState = new State("Cast");
    private State hitState = new State("Hit");
    private State dieState = new State("Die");
    #endregion States

    private Vector3 targetPosition;

    [SerializeField] private Effect_MotionTrail motionTrailEffect;
    private SkinnedMeshRenderer[] skinnedRenderers;

    #region Rolling Dodge
    private const float DECIDING_INTERVAL = 2f;
    private float currentRollingDodgeCooltime = 0;
    private Coroutine rollingDodgeCoroutine;

    [SerializeField] private Effect_YBotBomb ybotBomb;
    [SerializeField] private Transform ybotBombPoint;
    #endregion Rolling Dodge

    #region Shot Spell

    #endregion Shot Spell

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
        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    protected override void Update() {
        if(currentRollingDodgeCooltime > 0)
            currentRollingDodgeCooltime -= Time.deltaTime;
    }
    #endregion Unity Events

    private void InitializePoolers() {
        PoolerManager.instance.InsertPooler(ybotBomb.GetKey(), ybotBomb.gameObject, true);
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
            enemyMovement.MoveToPoint(targetPosition, moveSpeed, 5<<6);
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
        shotState.onActive = (State prevState) => {
            enemyAnimator.SetBool("Shot", true);
        };
        shotState.onInactive = (State nextState) => {
            enemyAnimator.SetBool("Shot", false);
        };
        castState.onActive = (State prevState) => {
            enemyAnimator.SetBool("Cast", true);
        };
        castState.onInactive = (State nextState) => {
            enemyAnimator.SetBool("Cast", false);
        };
        rollState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Roll", true);
            if(rollingDodgeCoroutine != null)
                StopCoroutine(rollingDodgeCoroutine);
            rollingDodgeCoroutine = StartCoroutine(RollingDodgeCoroutine());

            GameObject bomb = PoolerManager.instance.OutPool(ybotBomb.GetKey());
            bomb.transform.position = ybotBombPoint.position;
        };
        rollState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Roll", false);
            if(rollingDodgeCoroutine != null)
                StopCoroutine(rollingDodgeCoroutine);
        };
        hitState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Hit", true);
            enemyAnimator.SetTrigger("Hit Trigger");
        };
        hitState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Hit", false);
            if(hitCoroutine != null
            && !nextState.Compare(hitState))
                StopCoroutine(hitCoroutine);
        };
        dieState.onActive += (State prevState) => {
            enemyAnimator.enabled = false;
            enemyStateMachine.isMuted = true;
        };
        dieState.onInactive += (State prevState) => {
            enemyAnimator.enabled = true;
        };
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(rollState.stateName, rollState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        enemyStatesMap.Add(dieState.stateName, dieState);
        enemyStatesMap.Add(shotState.stateName, shotState);
        enemyStatesMap.Add(castState.stateName, castState);
    }
    protected override void ChaseTarget(Vector3 point) {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(rollState))
            return;

        targetPosition = point;

        if(!TryRollingDodge()) {
            if((  enemyStateMachine.Compare(idleState)
               || enemyStateMachine.Compare(chaseState))
            && !IsArrive) {
                SendChangeState(chaseState, true);
            }
        }
    }
    protected override void OnLostTarget() {
        SendChangeState(idleState);
    }
    private bool TryRollingDodge() {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(dieState))
            return false;

        if(DistanceToTarget < 3f
        && currentRollingDodgeCooltime <= 0) {
            currentRollingDodgeCooltime = DECIDING_INTERVAL;
            if(Random.Range(0f, 1f) >= .5f) {
                SendChangeState(rollState, false);
                return true;
            }
        }
        return false;
    }
    private IEnumerator RollingDodgeCoroutine() {
        motionTrailEffect.GenerateTrail(skinnedRenderers);
        Vector3 targetYlessDir = Vector3.Scale(new Vector3(1, 0, 1), targetPosition - transform.position);
        float angle = Random.Range(150, 211);
        Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * targetYlessDir;

        transform.LookAt(transform.position + direction);
        float offset = 0;
        int tick = 0;
        const int motionTrailInterval = 7;
        while(offset < 1f) {
            offset += Time.deltaTime * 1.25f;
            tick++;
            enemyMovement.MoveToward(transform.forward * Mathf.Pow((1-offset)*5, 2) * Time.deltaTime, Space.World);
            if(offset < .3f && tick >= motionTrailInterval) {
                tick= 0;
                motionTrailEffect.GenerateTrail(skinnedRenderers);
            }
            yield return null;
        }
        SendChangeState(idleState);
        transform.LookAt(transform.position + targetYlessDir);
    }

    #region Animation Events
    public void AnimationEvent_CastAction() {

    }
    public void AnimationEvent_CastEnd() {

    }
    public void AnimationEvent_ShotAction() {

    }
    public void AnimationEvent_ShotEnd() {

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
        SendChangeState(hitState);
        while(offset < damage.hittingDuration) {
            enemyMovement.MoveToward(Vector3.Lerp(pushedDestination, Vector3.zero, pushedOffset) * Time.deltaTime, Space.World);
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