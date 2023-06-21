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
    private State shootState = new State("Shoot");
    private State castState = new State("Cast");
    private State airState = new State("Air");
    private State kipUpState = new State("Kip Up");
    private State hitState = new State("Hit");
    private State dieState = new State("Die");
    #endregion States

    private Vector3 targetPosition;

    [SerializeField] private Effect_MotionTrail motionTrailEffect;
    [SerializeField] private SkinnedMeshRenderer[] skinnedRenderers;

    #region Rolling Dodge
    private const float DECIDING_INTERVAL = 0.2f;
    private float currentRollingDodgeCooltime = 0;
    private Coroutine rollingDodgeCoroutine;

    [SerializeField] private Effect_YBotBomb ybotBomb;
    [SerializeField] private Transform ybotBombPoint;
    #endregion Rolling Dodge

    #region Shoot Spell
    [SerializeField] private Effect_YBotShootingSpell ybotShootingSpell;
    [SerializeField] private Transform ybotShootingSpellPoint;
    #endregion Shoot Spell

    #region Mine Spell
    private const float MINE_COOLTIME = 10f;
    // private const float MINE_COOLTIME = 0f;
    [SerializeField] private Effect_YBotMineSpell ybotMineSpell;
    private float currentMineCooltime = 0;
    #endregion Mine Spell

    #region Hit
    private Coroutine hitCoroutine;
    private Coroutine airCoroutine;
    #endregion Hit

    #region Audio
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spellClip;
    [SerializeField] private AudioClip boomClip;
    [SerializeField] private AudioClip dodgeClip;
    #endregion Audio

    #region Unity Events
    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
        enemyStateMachine.SetIntialState(idleState);
        InitializeState();
        InitializePoolers();
        // skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    protected override void Update() {
        base.Update();
        if(currentRollingDodgeCooltime > 0)
            currentRollingDodgeCooltime -= Time.deltaTime;
        if(currentMineCooltime > 0)
            currentMineCooltime -= Time.deltaTime;
    }
    float t;
    #endregion Unity Events

    private void InitializePoolers() {
        PoolerManager.instance.InsertPooler(ybotBomb.GetKey(), ybotBomb.gameObject, true);
        PoolerManager.instance.InsertPooler(ybotShootingSpell.GetKey(), ybotShootingSpell.gameObject, true);
        PoolerManager.instance.InsertPooler(ybotMineSpell.GetKey(), ybotMineSpell.gameObject, true, 1, 1);
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
        shootState.onActive = (State prevState) => {
            Vector3 targetPos = targetPosition;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
            enemyAnimator.SetBool("Shoot", true);
        };
        shootState.onInactive = (State nextState) => {
            enemyAnimator.SetBool("Shoot", false);
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
        airState.onActive += (State prevState) => {
            if(prevState.Compare(airState))
                enemyAnimator.SetBool("Rise", true);
            else
                enemyAnimator.SetBool("Air", true);
        };
        airState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Air", false);
            if(nextState != airState
            && airCoroutine != null)
                StopCoroutine(airCoroutine);
        };
        hitState.onActive += (State prevState) => {
            if(prevState.Compare(hitState)) {
                enemyAnimator.SetBool("Hit", true);
                enemyAnimator.SetTrigger("Hit Trigger");
            }
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
            enemyStateMachine.isMuted = false;
            enemyAnimator.SetBool("Die", false);
            hpSlider.gameObject.SetActive(true);
        };
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(rollState.stateName, rollState);
        enemyStatesMap.Add(airState.stateName, airState);
        enemyStatesMap.Add(kipUpState.stateName, kipUpState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        enemyStatesMap.Add(dieState.stateName, dieState);
        enemyStatesMap.Add(shootState.stateName, shootState);
        enemyStatesMap.Add(castState.stateName, castState);
    }
    protected override void ChaseTarget(Vector3 point) {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(rollState))
            return;

        targetPosition = point;

        if(!TryRollingDodge()
        && !TrySetUpMine()
        && !TryShootSpell()) {
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
        || enemyStateMachine.Compare(dieState)
        || enemyStateMachine.Compare(castState))
            return false;

        if(DistanceToTarget < 4f
        && currentRollingDodgeCooltime <= 0) {
            currentRollingDodgeCooltime = DECIDING_INTERVAL;
            if(Random.Range(0f, 1f) >= .5f) {
                SendChangeState(rollState, false);
                return true;
            }
        }
        return false;
    }
    private bool TryShootSpell() {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(dieState)
        || enemyStateMachine.Compare(shootState)
        || enemyStateMachine.Compare(castState))
            return false;

        if(DistanceToTarget < 10f) {
            SendChangeState(shootState, false);
            return true;
        }
        return false;
    }
    private bool TrySetUpMine() {
        if(enemyStateMachine.Compare(hitState)
        || enemyStateMachine.Compare(dieState)
        || enemyStateMachine.Compare(shootState)
        || enemyStateMachine.Compare(castState))
            return false;
        
        if(DistanceToTarget < 12f
        && currentMineCooltime <= 0) {
            SendChangeState(castState, false);
            currentMineCooltime = MINE_COOLTIME;
            return true;
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

        audioSource.volume = SoundManager.instance.GetEffectVolume();
        audioSource.PlayOneShot(dodgeClip);

        while(offset < 1f) {
            offset += Time.deltaTime * 1.25f;
            tick++;
            enemyMovement.MoveToward(transform.forward * Mathf.Pow((1-offset)*5, 2) * Time.deltaTime, Space.World, moveLayerMask);
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
        Vector3 minePos = targetPosition;
        GameObject effect = PoolerManager.instance.OutPool(ybotMineSpell.GetKey());
        effect.transform.position = minePos;
    }
    public void AnimationEvent_CastEnd() {
        audioSource.volume = SoundManager.instance.GetEffectVolume();
        audioSource.PlayOneShot(spellClip);
        SendChangeState(BasicState);
    }
    public void AnimationEvent_ShootAction() {
        if(!enemyStateMachine.Compare(shootState))
            return;

        GameObject[] effects = new GameObject[3];
        effects[0] = PoolerManager.instance.OutPool(ybotShootingSpell.GetKey());
        effects[0].transform.position = ybotShootingSpellPoint.position;
        effects[0].transform.rotation = ybotShootingSpellPoint.rotation;
        effects[0].transform.Rotate(new Vector3(0, -30f, 0));

        effects[1] = PoolerManager.instance.OutPool(ybotShootingSpell.GetKey());
        effects[1].transform.position = ybotShootingSpellPoint.position;
        effects[1].transform.rotation = ybotShootingSpellPoint.rotation;

        effects[2] = PoolerManager.instance.OutPool(ybotShootingSpell.GetKey());
        effects[2].transform.position = ybotShootingSpellPoint.position;
        effects[2].transform.rotation = ybotShootingSpellPoint.rotation;
        effects[1].transform.Rotate(new Vector3(0, 30f, 0));
    }
    public void AnimationEvent_ShootEnd() {
        SendChangeState(BasicState);
    }
    public void AnimationEvent_EndAir() {
        SendChangeState(kipUpState);
    }
    public void AnimationEvent_EndKipUp() {
        SendChangeState(idleState);
    }
    #endregion Animation Events

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
    }
    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if(!isDead
        && !enemyStateMachine.Compare(rollState)) {
            if(damage.damageType == Damage.DamageType.Down
            || enemyStateMachine.Compare(airState)) {
                if(airCoroutine != null)
                    StopCoroutine(airCoroutine);
                airCoroutine = StartCoroutine(AirCoroutine(damage));
                SendChangeState(airState);
            } else {
                if(hitCoroutine != null)
                    StopCoroutine(hitCoroutine);
                hitCoroutine = StartCoroutine(HitCoroutine(damage));
            }
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
    private IEnumerator AirCoroutine(Damage damage) {
        float offset = 0;
        Vector3 destination = Vector3.Scale(new Vector3(1, 0, 1), damage.forceVector);
        while(damage.hittingDuration > 0
        && offset < 1) {
            enemyMovement.MoveToward(Vector3.Lerp(destination, Vector3.zero, offset) * Time.deltaTime, Space.World, moveLayerMask);
            offset += Time.deltaTime;
            yield return null;
        }
    }
    protected override void OnDie() {
        base.OnDie();
        SendChangeState(dieState);
    }
}