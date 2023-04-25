using System.Collections;
using UnityEngine;

class Enemy_XBot : MovableEnemy {
    #region States
    private State idleState = new State("Idle");
    private State chaseState = new State("Chase");
    private State basicState {
        get {
            if(targetInRange)
                return chaseState;
            else
                return idleState;
        }
    }
    private State assaultState = new State("Assault");
    private State jumpAttackState = new State("Jump Attack");
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

    #region Unity Events
    protected override void Awake() {
        base.Awake();

        InitializeState();
        enemyStateMachine.SetIntialState(idleState);

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
            }
        );
    }
    protected override void Update() {
        if(currentJumpAttackCooltime > 0)
            currentJumpAttackCooltime -= Time.deltaTime;
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
                enemyStateMachine.ChangeState(idleState);
            }
        };
        chaseState.onInactive += (State prevState) => {
            enemyAnimator.SetBool("Chase", false);
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
        dieState.onActive += (State prevState) => {
            // enemyAnimator.SetBool("Die", true);
            enemyAnimator.enabled = false;
        };
        dieState.onInactive += (State prevState) => {
            // enemyAnimator.SetBool("Die", false);
            enemyAnimator.enabled = true;
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
    }
    protected override void ChaseTarget(Vector3 point) {
        targetPosition = point;

        TryAssault();
        TryJumpAttack();

        if((  enemyStateMachine.Compare(idleState)
           || enemyStateMachine.Compare(chaseState))
        && !IsArrive) {
            enemyStateMachine.ChangeState(chaseState, true);
        }
    }
    protected override void OnLostTarget(Vector3 lastTargetPoint) {
        enemyStateMachine.ChangeState(idleState);
    }
    private void TryJumpAttack() {
        if(DistanceToTarget > 3f
        && DistanceToTarget < 8f
        && !enemyStateMachine.Compare(jumpAttackState)
        && !enemyStateMachine.Compare(assaultState)
        && currentJumpAttackCooltime <= 0) {
            enemyStateMachine.ChangeState(jumpAttackState);
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
        && !enemyStateMachine.Compare(assaultState)) {
            enemyStateMachine.ChangeState(assaultState, false);
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
        enemyStateMachine.ChangeState(idleState);
    }

    #region Animation Events
    public void AnimationEvent_OnEndJumpAttack() {
        enemyStateMachine.ChangeState(basicState);
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

    public override void OnDamage(GameObject origin, float amount) {
        base.OnDamage(origin, amount);
    }
    protected override void OnDie() {
        enemyStateMachine.ChangeState(dieState);
        enemyStateMachine.isMuted = true;
    }
}