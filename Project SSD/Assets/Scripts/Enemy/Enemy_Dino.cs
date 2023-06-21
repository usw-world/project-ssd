using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy_Dino : MovableEnemy {
    private CapsuleCollider dinoCollider;
    [SerializeField] private Transform modelBornRoot;
    
    #region Visual
    [SerializeField] private SkinnedMeshRenderer skinnedRenderer;
    private Material material;
    [SerializeField] private Effect_MotionTrail motionTrail;
    #endregion Visual

    #region States
    [Header("States")]
    State idleState = new State("Idle");
    State chaseState = new State("Chase");
    State jumpAttackState = new State("Jump Attack");
    State highJumpState = new State("High Jump");
    State breathState = new State("Breath");
    State readyAssaultState = new State("Ready Assault");
    State assaultState = new State("Assault");
    State summonState = new State("Summon");
    State stunnedState = new State("Stunned");
    State dieState = new State("Die");
    #endregion States

    #region Jump Attack
    [Header("JumpAttack")]
    private Coroutine jumpAttackCoroutine;
    [SerializeField] private const float JUMP_ATTACK_COOLTIME = 3f;
    private float currentJumpAttackCooltime = 3f;
    [SerializeField] private float jumpAttackDistance = 10f;
    [SerializeField] private float jumpAttackRangeRadius = 3f;
    [SerializeField] private Effect_DinoJumpAttack jumpAttackEffect;
    #endregion Jump Attack

    #region Assault
    [Header("Assault")]
    [SerializeField] private float assaultForceScalra = 100f;
    [SerializeField] private float assaultDamage = 100f;
    [SerializeField] private float assaultSpeed = 6f;
    [SerializeField] private float assaultRotateSpeed = 1f;
    [SerializeField] private ParticleSystem assaultParticle;
    [SerializeField] private CollisionEventHandler assaultCollisionHandler;

    private const int ASSAULT_MOTION_TRAIL_INTERVAL = 16;
    private int assaultMotionTrailCount = 0;
    #endregion Assault

    #region Breath
    [Header("Breath")]
    [SerializeField] private float breathPointMoveSpeed = 3f;
    private const float BREATH_PROBABILITY = .1f;
    [SerializeField] private Effect_DinoBeamHit dinoBeamHitEffect;
    [SerializeField] private UnityEngine.VFX.VisualEffect breathVfx;
    [SerializeField] private UnityEngine.VFX.VisualEffect breathHitVfx;
    [SerializeField] private Transform breathPoint;
    [SerializeField] private float breathDuration = 5f;
    private Coroutine breathCoroutine;
    [SerializeField] private UnityEngine.Animations.Rigging.Rig animationRig;
    #endregion Breath

    #region Summon
    private float currentSummonCooltime = 0;
    private const float SUMMON_COOLTIME = 30f;
    [Header("Summon")]
    [SerializeField] private Enemy_Pteranodon[] ownPteranodons;
    [SerializeField] private float minPteranodonCount = 2f;
    #endregion Summon

    #region High Jump
    [Header(" HighJump")]
    [SerializeField] private const float HIGH_JUMP_COOLTIME = 23f;
    // private float currentHighJumpCooltime = 15f;
    private float currentHighJumpCooltime = 0f;
    [SerializeField] private float highJumpRangeRadius = 5f;
    [SerializeField] private DecalProjector highJumpProjector;
    [SerializeField] private Effect_DinoJumpAttack highJumpEffect;
    private Coroutine highJumpCoroutine;

    [SerializeField] private Transform[] cactusPoints;
    #endregion High Jump

    #region Stunned
    [Header("Stunned")]
    [SerializeField] private ParticleSystem stunnedParticle;
    [SerializeField] private float currentStunnedTime = 0;
    #endregion Stunned

    #region Damage
    [Header("Damage")]
    private Coroutine damageCoroutine;
    #endregion Damage

    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource breathHitAudioSource;
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] assaultClips;
    [SerializeField] private AudioClip roarClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip highJumpLandingClip;
    [SerializeField] private AudioClip breathChargeClip;
    [SerializeField] private AudioClip breathHitClip;
    #endregion Audio

    #region Unity Events
    protected override void Awake() {
        base.Awake();
        dinoCollider = GetComponent<CapsuleCollider>();
        audioSource = audioSource ?? GetComponent<AudioSource>();
    }
    protected override void Start() {
        base.Start();
        InitializeState();
        InitializeEffects();
        skinnedRenderer.materials[0] = new Material(skinnedRenderer.materials[0]);
        material = skinnedRenderer.materials[0];
    }
    protected override void Update() {
        base.Update();
        Cooldown();
    }
    #endregion Unity Events

    private void InitializeState() {
        enemyStateMachine.SetIntialState(idleState);

        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(jumpAttackState.stateName, jumpAttackState);
        enemyStatesMap.Add(highJumpState.stateName, highJumpState);
        enemyStatesMap.Add(breathState.stateName, breathState);
        enemyStatesMap.Add(readyAssaultState.stateName, readyAssaultState);
        enemyStatesMap.Add(assaultState.stateName, assaultState);
        enemyStatesMap.Add(summonState.stateName, summonState);
        enemyStatesMap.Add(stunnedState.stateName, stunnedState);
        enemyStatesMap.Add(dieState.stateName, dieState);

        idleState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Idle", true);
        };
        idleState.onStay += () => {
            Vector3 yLessTargetPoint = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(yLessTargetPoint - transform.position), .1f);
        };
        idleState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Idle", false);
        };
        chaseState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Chase", true);
            if(onHost) {
                enemyMovement.MoveToPoint(targetPoint, moveSpeed);
            }
        };
        chaseState.onStay += () => {
            Vector3 yLessTargetPoint = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(yLessTargetPoint - transform.position), .1f);
        };
        chaseState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Chase", false);
            if(onHost) {
                enemyMovement.Stop();
            }
        };
        jumpAttackState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Jump Attack", true);
            Vector3 ylessTargetPoint = Vector3.Scale(new Vector3(1, 0, 1), targetPoint);
            transform.LookAt(ylessTargetPoint);
            if(jumpAttackCoroutine != null)
                StopCoroutine(jumpAttackCoroutine);
        };
        jumpAttackState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Jump Attack", false);
            if(jumpAttackCoroutine != null)
                StopCoroutine(jumpAttackCoroutine);
        };
        highJumpState.onActive += (State prevState) => {
            enemyAnimator.SetBool("High Jump", true);
        };
        highJumpState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("High Jump", false);
            if(highJumpCoroutine != null)
                StopCoroutine(highJumpCoroutine);
        };
        breathState.onActive += (State prevState) => {
            breathVfx.SetFloat("Duration", breathDuration);
            breathPoint.transform.position = transform.position + (transform.forward*5);
            
            breathHitVfx.SetFloat("Duration", breathDuration);

            enemyAnimator.SetBool("Breath", true);

            breathPoint.SetParent(null);
            animationRig.weight = 1;
            breathVfx.gameObject.SetActive(true);
            breathHitVfx.gameObject.SetActive(true);

            audioSource.volume = SoundManager.instance.GetEffectVolume();
            audioSource.PlayOneShot(breathChargeClip);
        };
        breathState.onInactive += (State nextState) => {
            dinoBeamHitEffect.isActive = false;
            enemyAnimator.SetBool("Breath", false);

            breathPoint.SetParent(this.transform);
            animationRig.weight = 0;
            breathVfx.gameObject.SetActive(false);
            breathHitVfx.gameObject.SetActive(false);
            
            if(breathCoroutine != null)
                StopCoroutine(breathCoroutine);
        };
        readyAssaultState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Assault", true);
        };
        readyAssaultState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Assault", false);
        };
        assaultState.onActive += (State prevState) => {
            assaultCollisionHandler.gameObject.SetActive(true);
            assaultParticle.Play();
        };
        assaultState.onStay += () => {
            if(onHost) {
                Vector3 ylessTargetPoint = new Vector3(targetPoint.x - transform.position.x, transform.position.y, targetPoint.z - transform.position.z);
                Quaternion nextRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ylessTargetPoint), Time.deltaTime * assaultRotateSpeed);
                transform.rotation = nextRotation;
                enemyMovement.MoveToward(transform.forward * assaultSpeed * Time.deltaTime, Space.World);
            }
            assaultMotionTrailCount ++;
            if(assaultMotionTrailCount >= ASSAULT_MOTION_TRAIL_INTERVAL) {
                motionTrail.GenerateTrail(new SkinnedMeshRenderer[]{ skinnedRenderer } );
                assaultMotionTrailCount = 0;
            }
        };
        assaultState.onInactive += (State nextState) => {
            assaultCollisionHandler.gameObject.SetActive(false);
            assaultParticle.Stop();
        };
        summonState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Summon", true);
        };
        summonState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Summon", false);
        };
        stunnedState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Stunned", true);
            stunnedParticle.Play();
        };
        stunnedState.onStay += () => {
            currentStunnedTime += Time.deltaTime;
            if(currentStunnedTime >= 8) {
                SendChangeState(idleState);
                currentStunnedTime = 0;
            }
        };
        stunnedState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Stunned", false);
            stunnedParticle.Stop();
            currentStunnedTime = 0;
        };
        dieState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Die", true);
        };
        dieState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Die", false);
        };
    }
    private void InitializeEffects() {
        PoolerManager.instance.InsertPooler(jumpAttackEffect.GetKey(), jumpAttackEffect.gameObject, true, 5, 2);
        PoolerManager.instance.InsertPooler(highJumpEffect.GetKey(), highJumpEffect.gameObject, true, 5, 2);

        assaultCollisionHandler.onTriggerEnter += OnHitAssault;
    }

    protected override void ChaseTarget(Vector3 point) {
        if(isDead
        || enemyStateMachine.Compare(dieState)
        || enemyStateMachine.Compare(jumpAttackState)
        || enemyStateMachine.Compare(highJumpState)
        || enemyStateMachine.Compare(breathState)
        || enemyStateMachine.Compare(readyAssaultState)
        || enemyStateMachine.Compare(assaultState)
        || enemyStateMachine.Compare(summonState)
        || enemyStateMachine.Compare(stunnedState))
            return;

        if(!DecideAction()) {
            if(IsArrive)
                SendChangeState(idleState, false);
            else {
                SendChangeState(chaseState, true);
            }
        }
    }

    private void Cooldown() {
        if(currentJumpAttackCooltime > 0)
            currentJumpAttackCooltime -= Time.deltaTime;
        if(currentHighJumpCooltime > 0)
            currentHighJumpCooltime -= Time.deltaTime;
        if(currentSummonCooltime > 0)
            currentSummonCooltime -= Time.deltaTime;
    }
    protected bool DecideAction() {
        if(TrySummon()) {
            return true;
        } else if(DistanceToTarget < 11) {
            return TryBreath() || TryJumpAttack();
        } else if(DistanceToTarget < 20) {
            return TryHighJump();
        }
        return false;
    }

    private bool TryJumpAttack() {
        // return false;
        if(currentJumpAttackCooltime <= 0
        && DistanceToTarget < jumpAttackDistance) {
            currentJumpAttackCooltime = JUMP_ATTACK_COOLTIME;
            SendChangeState(jumpAttackState);
            return true;
        }
        return false;
    }
    private IEnumerator JumpAttackCoroutine(Vector3 point) {
        Vector3 direction = transform.forward;
        float scalar = DistanceToTarget;

        float offset = 0;
        while(offset < 1) {
            float cofficient = Time.deltaTime;
            offset += cofficient;
            enemyMovement.MoveToward(direction * DistanceToTarget * cofficient, Space.World);
            dinoCollider.center = new Vector3(0, 3.5f+modelBornRoot.position.y, 0);
            yield return null;
        }
        dinoCollider.center = new Vector3(0, 3.5f, 0);
        SendChangeState(idleState, false);
    }
    private bool TryHighJump() {
        // return false;
        if(currentHighJumpCooltime <= 0
        /* && maxHp*.5f > hp */) {
            currentHighJumpCooltime = HIGH_JUMP_COOLTIME;
            SendChangeState(highJumpState);
            return true;
        }
        return false;
    }
    private IEnumerator HighJumpCoroutine() {
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime * .35f;
            Vector3 moveVector = (targetPoint - transform.position).normalized;
            moveVector = Vector3.Lerp(Vector3.zero, moveVector, (1-offset) * Time.deltaTime * 10f);
            if(DistanceToTarget > .1f)
                enemyMovement.MoveToward(moveVector, Space.World);

            Vector3 ylessTargetPoint = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
            transform.LookAt(ylessTargetPoint);
            yield return null;
        }
    }
    private bool TryBreath() {
        // return false;
        if(UnityEngine.Random.Range(0, 1f) < BREATH_PROBABILITY) {
            SendChangeState(breathState, false);
            return true;
        }
        return false;
    }
    private bool TrySummon() {
        if(currentSummonCooltime > 0) 
            return false;

        int livingCount = 0;
        for(int i=0; i<ownPteranodons.Length; i++) {
            if(!ownPteranodons[i].isDead)
                livingCount ++;
        }
        if(minPteranodonCount >= livingCount) {
            currentSummonCooltime = SUMMON_COOLTIME;
            SendChangeState(summonState, false);
            return true;
        }
        return false;
    }
    private void OnHitAssault(Collider target) {
        if(target.gameObject.layer == 7) {
            TPlayer player;
            if(target.TryGetComponent<TPlayer>(out player)) {
                Vector3 forceVector = (target.transform.position - transform.position).normalized * assaultForceScalra;
                player.OnDamage(new Damage(
                    assaultDamage,
                    1f,
                    forceVector,
                    Damage.DamageType.Down
                ));
                CameraManager.instance.MakeNoise(2.5f, 1f);
                audioSource.volume = SoundManager.instance.effectVolume;
                audioSource.PlayOneShot(landingClip);
                SendChangeState(jumpAttackState, false);
            }
        } else if(target.gameObject.layer == 8) {
            DestroyableObject cactus;
            if(target.TryGetComponent<DestroyableObject>(out cactus)) {
                cactus.OnDamage(new Damage(
                    100,
                    1,
                    (target.transform.position - transform.position).normalized * 10f,
                    Damage.DamageType.Normal
                ));
                CameraManager.instance.MakeNoise(4f, .5f);
                audioSource.volume = SoundManager.instance.effectVolume;
                audioSource.PlayOneShot(landingClip);
                SendChangeState(stunnedState, false);
            }
        }
    }

    protected override void OnLostTarget() {}
    /* Noting. Because he is boss monster. ^^ */

    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageCoroutine(damage));
    }
    private IEnumerator DamageCoroutine(Damage damage) {
        float hittingDuration = damage.hittingDuration * .2f;
        float offset = 0;

        SetAnimationSpeed(0);
        material.SetColor("_HighColor", new Color(.7f, .7f, .7f, 1));

        while(offset < hittingDuration) {
            offset += Time.deltaTime;
            float color = (1-offset) * .7f;
            material.SetColor("_HighColor", new Color(color, color, color, 1));
            yield return null;
        }
        SetAnimationSpeed(1);
        material.SetColor("_HighColor", new Color(0, 0, 0, 1));
    }
    private void SetAnimationSpeed(float coef) {
        enemyAnimator.SetFloat("Animation Speed", coef);
        enemyMovement.SetSpeed(moveSpeed * coef);
    }
    private IEnumerator BreathCoroutine() {
        dinoBeamHitEffect.isActive = true;
        breathHitAudioSource.volume = SoundManager.instance.GetEffectVolume();
        breathHitAudioSource.clip = breathHitClip;
        breathHitAudioSource.Play();

        breathVfx.Play();
        breathHitVfx.Play();
        float offset = 0;
        while(offset < breathDuration) {
            offset += Time.deltaTime;
            
            breathPoint.Translate((targetPoint - breathPoint.position).normalized * breathPointMoveSpeed * Time.deltaTime, Space.World);
            breathVfx.transform.LookAt(breathPoint);

            Vector3 ylessBreathPoint = new Vector3(breathPoint.position.x, transform.position.y, breathPoint.position.z);
            if(Vector3.Angle(transform.forward, ylessBreathPoint - transform.position) > 60) {
                Quaternion ylessNextRotation = Quaternion.LookRotation(ylessBreathPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, ylessNextRotation, Time.deltaTime * 7f);
            }

            RaycastHit hit;
            if( Physics.Raycast(
                breathVfx.transform.position,
                breathPoint.position - breathVfx.transform.position,
                out hit,
                50f,
                1<<6 | 1<<11
            )) {
                breathVfx.SetFloat("Distance", hit.distance*.25f);
            }
            yield return null;
        }
        dinoBeamHitEffect.isActive = false;
        SoundManager.instance.FadeOutVolume(breathHitAudioSource);
        enemyAnimator.SetBool("Breath", false);
    }

    protected override void OnDie() {
        base.OnDie();
        
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(roarClip);

        SendChangeState(dieState);
        StageManager.instance?.OnClearStage();
    }

    #region Animation Events
    public void AnimationEvent_WalkStep() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(walkClips[0]);
    }
    public void AnimationEvent_AssaultStep() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(assaultClips[UnityEngine.Random.Range(0, 3)]);
    }
    public void AnimationEvent_FlyJumpAttack() {
        if(jumpAttackCoroutine != null)
            StopCoroutine(jumpAttackCoroutine);
        jumpAttackCoroutine = StartCoroutine(JumpAttackCoroutine(targetPoint));
    }
    public void AnimationEvent_LandJumpAttack() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(landingClip);

        CameraManager.instance.MakeNoise(1, .5f);
        var effect = PoolerManager.instance.OutPool(jumpAttackEffect.GetKey()).GetComponent<Effect_DinoJumpAttack>();
        effect.AttackArea(transform.position);
    }
    public void AnimationEvent_HighJumpStart() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(landingClip);

        highJumpProjector.enabled = true;
        if(highJumpCoroutine != null)
            StopCoroutine(highJumpCoroutine);
        highJumpCoroutine = StartCoroutine(HighJumpCoroutine());
    }
    public void AnimationEvent_LandHighJump() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(landingClip);

        highJumpProjector.enabled = false;
        CameraManager.instance.MakeNoise(1, 1.5f);
        var effect = PoolerManager.instance.OutPool(highJumpEffect.GetKey()).GetComponent<Effect_DinoJumpAttack>();
        effect.AttackArea(transform.position);
        foreach(Transform point in cactusPoints) {
            var cactus = point.GetComponentInChildren<DestroyableObject>();
            cactus.Repair();
            var spawnParticle = point.GetComponentInChildren<ParticleSystem>();
            spawnParticle?.Play();
        }
    }
    public void AnimationEvent_EndHighJump() {
        SendChangeState(readyAssaultState, false);
    }
    public void AnimationEvent_SetBasicState() {
        SendChangeState(idleState);
    }
    public void AnimationEvent_StartAssault() {
        SendChangeState(assaultState, false);
    }
    public void AnimationEvent_BreathStart() {
        breathCoroutine = StartCoroutine(BreathCoroutine());
    }
    public void AnimationEvent_OnSummon() {
        audioSource.volume = SoundManager.instance.effectVolume;
        audioSource.PlayOneShot(roarClip);
        CameraManager.instance.MakeNoise(2, 2);
        for(int i=0; i<ownPteranodons.Length; i++) {
            var pteranodon = ownPteranodons[i];
            if(pteranodon.isDead) {
                pteranodon.Recovery();
            }
        }
    }
    #endregion Animation Events
}
