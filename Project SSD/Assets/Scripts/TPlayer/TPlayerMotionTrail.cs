using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerMotionTrail : MonoBehaviour, IPoolableObject
{
    private Animator animator;
    private string poolerKey;

	private void Awake()
	{
        animator = GetComponent<Animator>();
    }
	public void SetAction(int attackCount, string poolerKey) 
    {
        this.poolerKey = poolerKey;
        switch (attackCount)
        {
            case 0: StartCoroutine(SetAnimatorTrigger(0)); break;
            case 1: StartCoroutine(SetAnimatorTrigger(1)); break;
            case 2: StartCoroutine(SetAnimatorTrigger(2)); break;
            case 3: StartCoroutine(SetAnimatorTrigger(3)); break;
        }
    }
    public void AnimationEvent_StartAttack() { }
    public void CheckAttackZone() 
    {
        GameObject effect = PoolerManager.instance.OutPool(poolerKey);
        effect.GetComponent<TPlayerAttackEffect>().OnActiveMotionTrail(TPlayer.instance.GetAp() * 0.5f ,transform);
    }
    public void ResetState() 
    {
        PoolerManager.instance.InPool(GetKey(), gameObject);
    }
	public string GetKey()
	{
        return GetType().ToString();
	}
    IEnumerator SetAnimatorTrigger(int count)
    {
        animator.SetTrigger("Basic Attack");
        for (int i = 0; i < count; i++)
        {
            animator.SetTrigger("Buffered Input Basic Attack");
            yield return null;
        }
    }
}
