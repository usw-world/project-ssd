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
    private State hitState = new State("Hit");
    private State dieState = new State("Die");
    #endregion States

    private Vector3 targetPosition;

    #region Jump Attack
    private const float JUMP_ATTACK_COOLTIME = 5f;
    private float currentJumpAttackCooltime = 0f;
    private Coroutine jumpAttackCoroutine;
    [SerializeField] private GameObject jumpAttackParticle;
    [SerializeField] private Transform jumpAttackParticlePoint;
    private ObjectPooler jumpAttackParticlePooler;
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

        jumpAttackParticlePooler = new ObjectPooler(
            jumpAttackParticle,
            (GameObject go) => {
                go.SetActive(false);
            },
            (GameObject go) => {
                IEnumerator OutPoolParticle() {
                    yield return new WaitForSeconds(3f);
                    jumpAttackParticlePooler.InPool(go);
                }
                StartCoroutine(OutPoolParticle());
                go.SetActive(true);
            }, transform, 2, 1
        );
    }
    protected override void Start() {
        base.Start();
        InitializeState();
        enemyStateMachine.SetIntialState(idleState);
    }
    protected override void Update() {
        if(currentJumpAttackCooltime > 0)
            currentJumpAttackCooltime -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Return) && SSDNetworkManager.instance.isHost)
            Mirror.NetworkClient.Send(new C2SMessage.ChangeStateMessage(networkId, jumpAttackState.stateName));
    }
    #endregion Unity Events


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
        hitState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Hit", true);
        };
        hitState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Hit", false);
        };
        dieState.onActive += (State prevState) => {
            enemyAnimator.enabled = false;
        };
        dieState.onInactive += (State prevState) => {
            enemyAnimator.enabled = true;
        };
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(assaultState.stateName, assaultState);
        enemyStatesMap.Add(jumpAttackState.stateName, jumpAttackState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        enemyStatesMap.Add(dieState.stateName, dieState);
    }
    protected override void ChaseTarget(Vector3 point) {
        if(enemyStateMachine.Compare(hitState))
            return;

        targetPosition = point;

        TryAssault();
        TryJumpAttack();

        if((  enemyStateMachine.Compare(idleState)
           || enemyStateMachine.Compare(chaseState))
        && !IsArrive) {
            SendChangeState(chaseState, true);
        }
    }
    protected override void OnLostTarget() {
        SendChangeState(idleState);
    }
    private void TryJumpAttack() {
        if(DistanceToTarget > 3f
        && DistanceToTarget < 8f
        && !enemyStateMachine.Compare(jumpAttackState)
        && !enemyStateMachine.Compare(assaultState)
        && !enemyStateMachine.Compare(hitState)
        && currentJumpAttackCooltime <= 0) {
            SendChangeState(jumpAttackState);
        }
    }
    private IEnumerator JumpAttackCoroutine() {
        Vector3 point1 = transform.position;
        Vector3 point2 = targetPosition;
        Vector3 distance = point2 - point1;
        distance.y = 0;
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            enemyMovement.MoveToward(distance * Time.deltaTime*.8f, Space.World, 5<<6);
            yield return null;
        }
    }
    private void TryAssault() {
        if(DistanceToTarget < 5f
        && !enemyStateMachine.Compare(jumpAttackState)
        && !enemyStateMachine.Compare(assaultState)
        && !enemyStateMachine.Compare(hitState)) {
            SendChangeState(assaultState, false);
        }
    }
    private IEnumerator AssaultCoroutine() {
        float offset = 0;
        transform.LookAt(targetPosition);
        Vector3 dir = (targetPosition - transform.position).normalized;
        dir.y = 0;

        while(offset < 1) {
            offset += Time.deltaTime;
            
            enemyMovement.MoveToward(dir * 10f * Time.deltaTime, Space.World, 5<<6);
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
        jumpAttackParticlePooler.OutPool(jumpAttackParticlePoint.position, Quaternion.identity, null);
        /* << temporary */
    }
    public void AnimationEvent_OnEndCrouch() {
        enemyAnimator.SetBool("Assault Crouch", false);
        assaultEffect.SetActive(true);
        assaultCoroutine = StartCoroutine(AssaultCoroutine());
    }
    #endregion Animation Events

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
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
        enemyStateMachine.isMuted = true;
    }
}