using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TPlayerSkillUi : MonoBehaviour
{
	private Animator animator;
	private bool isActive = false;
	private bool isPlayingAnimation = false;
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	public void OnActive()
	{
		if (isPlayingAnimation) return;
		if (isActive)
		{
			animator.SetTrigger("Off");
			isActive = false;
		}
		else
		{
			animator.SetTrigger("On");
			isActive = true;
		}
	}
	public void StartAnimation() { isPlayingAnimation = true; }
	public void EndAnimation() { isPlayingAnimation = false; }
}
