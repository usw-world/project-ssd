using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TPlayerSkillUi : MonoBehaviour
{
	[SerializeField] private List<TPlayerSkillBtnEvent> mainBranch;
	[SerializeField] private List<GameObject> cantClickZone;
	[SerializeField] private TextMeshProUGUI skillPoint;
	private Animator animator;
	private bool isActive = false;
	private bool isPlayingAnimation = false;
	private int currSubBranch = -1;
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	public bool OnActive()
	{
		if (isPlayingAnimation) return true;
		if (isActive)
		{
			if (currSubBranch != -1) {
				ReturnToMain();
				return true;
			}
			animator.SetTrigger("Off");
			foreach (var item in mainBranch){
				item.MouseExit();
			}
			isActive = false;
			Cursor.lockState = CursorLockMode.Locked;
			currSubBranch = -1;
			return false;
		}
		else
		{
			foreach (var item in mainBranch){
				item.MouseExit();
			}
			foreach (var item in cantClickZone)	{
				item.gameObject.SetActive(false);
			}
			cantClickZone[6].SetActive(true);
			animator.SetTrigger("On");
			isActive = true;
			Cursor.lockState = CursorLockMode.None;
			return true;
		}
	}
	public bool OnPressEscape() 
	{
		if (isPlayingAnimation) return true;
		if (currSubBranch == -1)
		{
			OnActive();
			return false;
		}
		else
		{
			ReturnToMain();
			return true;
		}
	}
	public void ReturnToMain() 
	{
		if (isPlayingAnimation) return;
		foreach (var item in cantClickZone){
			item.gameObject.SetActive(false);
		}
		cantClickZone[6].SetActive(true);
		string animationTrigger = currSubBranch + " to main";
		animator.SetTrigger(animationTrigger);
		currSubBranch = -1;
	}
	public void SelectSubBranch(int idx)
	{
		if (isPlayingAnimation) return;
		if (currSubBranch != -1) {
			ReturnToMain();
			return; 
		}
		string animationTrigger = "main to " + idx;
		animator.SetTrigger(animationTrigger);
		foreach (var item in mainBranch){
			item.MouseExit();
			item.Lock();
		}
		foreach (var item in cantClickZone){
			item.gameObject.SetActive(false);
		}
		cantClickZone[idx].SetActive(true);
		mainBranch[idx].UnLock();
		mainBranch[idx].MouseEnter();
		mainBranch[idx].Lock();
		currSubBranch = idx;
	}
	public bool CanActive() 
	{
		return !isPlayingAnimation;
	}
	public void ResetMainBranch() 
	{
		foreach (var item in mainBranch)
		{
			item.UnLock();
			item.MouseExit();
		}
	}
	public void StartAnimation() { isPlayingAnimation = true; }
	public void EndAnimation() { isPlayingAnimation = false; }
	public void ActiveOption(int number)
	{
		if (isPlayingAnimation) return;
		print(number + " 클릭");
	}
}
