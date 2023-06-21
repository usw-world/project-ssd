using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class TPlayerSkillUi : MonoBehaviour
{
	[SerializeField] private List<TPlayerSkillBtnEvent> mainBranch;
	[SerializeField] private List<Image> optionBtnImg;
	[SerializeField] private List<GameObject> cantClickZone;
	[SerializeField] private List<TextMeshProUGUI> optionNames;
	[SerializeField] private List<TextMeshProUGUI> optionInfo;
	[SerializeField] private Color offColor;
	[SerializeField] private Color onColor;
	private Color offHideColor;
	private Color onHideColor;
	private Animator animator;
	private bool isActive = false;
	private bool isPlayingAnimation = false;
	private int currSubBranch = -1;

	public int skillPoint = 0;
	[SerializeField] private TextMeshProUGUI skillPointText;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		foreach (var item in mainBranch)
		{
			item.GetComponent<Image>().color = onColor;
		}
		offHideColor = offColor;
		offHideColor.a = 0;
		onHideColor = onColor;
		onHideColor.a = 0;
		foreach (var item in optionBtnImg)
		{
			item.color = offHideColor;
		}
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
			GameManager.instance.SetActiveInput(true);
			UIManager.instance.tPlayerHUD.Initialize();
			currSubBranch = -1;
			DisableEvent();
			return false;
		}
		else
		{
			print(GameManager.instance.saveData);
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
			GameManager.instance.SetActiveInput(false);

			SetTree();
			return true;
		}
	}
	public void AcceptSaveData() {
		bool[] skillData = GameManager.instance.GetTSkillData();
		if (skillData == null) return;
		for(int i=0; i<TPlayer.instance.options.Length; i++) {
			TPlayer.instance.options[i].active = skillData[i];
		}
		SetSkillPoint(GameManager.instance.GetRemainingSkillPoint());
	}
	private void SetSkillPoint(int point) {
		skillPoint = point;
		skillPointText.text = skillPoint.ToString();
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
		foreach (var item in optionBtnImg)
		{
			item.gameObject.SetActive(false);
		}
		optionBtnImg[idx * 3].gameObject.SetActive(true);
		optionBtnImg[idx * 3 + 1].gameObject.SetActive(true);
		optionBtnImg[idx * 3 + 2].gameObject.SetActive(true);
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
		if (isPlayingAnimation)
			return;
		SkillOptionInformation option = TPlayer.instance.options[number];
		if(!option.active && skillPoint <= 0)
			return;
			
		if(option.active)
			SetSkillPoint(skillPoint + 1);
		else
			SetSkillPoint(skillPoint - 1);

		option.active = (option.active) ? false : true;
		if (option.active)
		{
			optionBtnImg[number].color = onColor;
		}
		else
		{
			optionBtnImg[number].color = offColor;
		}
	}
	private void SetTree()
	{
		SkillOptionInformation[] options = TPlayer.instance.options;
		for (int i = 0; i < options.Length; i++){
			optionNames[i].text = options[i].name;
			optionInfo[i].text = options[i].info;
			if (options[i].active)
			{
				optionBtnImg[i].color = onColor;
			}
			else
			{
				optionBtnImg[i].color = offColor;
			}
		}
	}
	private void DisableEvent() {
		
		GameManager.instance?.SynchronizeData2Server();
	}
}
