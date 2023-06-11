using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy_Dino : MovableEnemy {
    #region States
    State idleState = new State("Idle");
    State chaseState = new State("Chase");
    State jumpAttackState = new State("Jump Attack");
    State highJumpState = new State("High Jump");
    State breathState = new State("Breath");
    State assaultState = new State("Assault");
    State summonState = new State("Summon");
    #endregion States

    #region Jump Attack
    [SerializeField] private const float JUMP_ATTACK_COOLTIME = 5f;
    private float currentJumpAttackCooltime = 0f;
    [SerializeField] private float jumpAttackDistance = 10f;
    [SerializeField] private float jumpAttackRangeRadius = 3f;
    [SerializeField] private DecalProjector jumpAttackProjector;
    #endregion Jump Attack

    #region High Jump
    [SerializeField] private const float HIGH_JUMP_COOLTIME = 10f;
    private float currentHighJumpCooltim = 0f;
    [SerializeField] private float highJumpRangeRadius = 5f;
    [SerializeField] private DecalProjector highJumpPfojector;
    #endregion High Jump

    #region Unity Events
    protected override void Start() {
        base.Start();
        InitializeState();
    }
    #endregion Unity Events

    private void InitializeState() {
        enemyStateMachine.SetIntialState(idleState);

        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(jumpAttackState.stateName, jumpAttackState);
        enemyStatesMap.Add(highJumpState.stateName, highJumpState);
        enemyStatesMap.Add(breathState.stateName, breathState);
        enemyStatesMap.Add(assaultState.stateName, assaultState);
        enemyStatesMap.Add(summonState.stateName, summonState);

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
        };
        jumpAttackState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Jump Attack", false);
        };
        highJumpState.onActive += (State prevState) => {
            enemyAnimator.SetBool("High Jump", true);
        };
        highJumpState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("High Jump", false);
        };
        breathState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Breath", true);
        };
        breathState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Breath", false);
        };
        assaultState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Assault", true);
        };
        assaultState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Assault", false);
        };
        summonState.onActive += (State prevState) => {
            enemyAnimator.SetBool("Summon", true);
        };
        summonState.onInactive += (State nextState) => {
            enemyAnimator.SetBool("Summon", false);
        };
    }

    protected override void ChaseTarget(Vector3 point) {
        if(isDead
        || enemyStateMachine.Compare(jumpAttackState)
        || enemyStateMachine.Compare(highJumpState)
        || enemyStateMachine.Compare(breathState)
        || enemyStateMachine.Compare(assaultState)
        || enemyStateMachine.Compare(summonState))
            return;

        if(!TryJumpAttack()
        && !TryHighJump()
        && !TryBreath()
        && !TryAssault()
        && !TrySummon()) {
            if(IsArrive)
                SendChangeState(idleState, false);
            else {
                SendChangeState(chaseState, true);
            }
        }
    }

    private bool TryJumpAttack() {
        if(currentJumpAttackCooltime > 0)
            currentJumpAttackCooltime -= Time.deltaTime;

        if(DistanceToTarget < jumpAttackDistance) {
            currentJumpAttackCooltime = JUMP_ATTACK_COOLTIME;
            SendChangeState(jumpAttackState);
            return true;
        }
        return false;
    }
    private IEnumerator JumpAttackCoroutine() {
        throw new NotImplementedException();
        yield return null;
    }
    private bool TryHighJump() {
        return false;
    }
    private bool TryBreath() {
        return false;
    }
    private bool TryAssault() {
        return false;
    }
    private bool TrySummon() {
        return false;
    }

    protected override void OnLostTarget() {}
    /* Noting. Because he is boss monster. ^^ */
}
