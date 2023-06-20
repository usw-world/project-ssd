using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QPlayerSkillUi : MonoBehaviour
{
	[SerializeField] private List<QPlayerSkillUiSubTree> subTrees;
	[SerializeField] private List<QPlayerSkillUiInfo> skillInfos;
	[SerializeField] private List<RectTransform> mainBranchs;
	[SerializeField] private RectTransform mainBranchGroub;
	[SerializeField] private TextMeshProUGUI skillPoint;
	private Animator animator;
	private static bool isPlayingAction = false;
	private bool isPlayingAnimation = false;
	private bool isActive = false;
	private bool isSubTreeSeting = false;
	private int currSelectedMainBranch = -1;

	private void Awake()
	{
		animator = GetComponent<Animator>(); 
		float angle = 360f / mainBranchs.Count * Mathf.Deg2Rad;
		for (int i = 0; i < mainBranchs.Count; i++)
		{
			float x = 300f * Mathf.Sin(angle * i);
			float y = 300f * Mathf.Cos(angle * i);
			mainBranchs[i].localPosition = new Vector3(x, y);
		}
	}
	private void Start() {
		Refresh();
		AcceptSaveData();
	}
	private void Refresh() {
		var skillDatas = GameManager.instance.saveData.qSkillData;
		for(int i=0; i<skillDatas.Length; i++) {
			
		}
	}

	public bool OnActive()
	{
		if (!CanActive()) return true;
		if (isActive)
		{
			if (currSelectedMainBranch != -1)
			{
				ReturnToMain();
				return true;
			}
			animator.SetTrigger("Off");
			isActive = false;
			currSelectedMainBranch = -1;
			
			GameManager.instance.SynchronizeData2Server(); // usoock is good.
			return false;
		}
		else
		{
			if (!isSubTreeSeting) {
				for (int i = 0; i < subTrees.Count; i++){
					subTrees[i].Set(i);
				}
				isSubTreeSeting = true;
			}
			animator.SetTrigger("On");
			isActive = true;
			return true;
		}
	}
	private void AcceptSaveData() {
		bool[][] skillData = GameManager.instance.GetQSkillData();
		for(int i=0; i<(QPlayer.instance.skills[0] as QPlayerSkillUnityBall).options.Length; i++) {
			(QPlayer.instance.skills[0] as QPlayerSkillUnityBall).options[i].active = skillData[0][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[1] as QPlayerSkillAoe).options.Length; i++) {
			(QPlayer.instance.skills[1] as QPlayerSkillAoe).options[i].active = skillData[1][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[2] as QPlayerSkillBuffering).options.Length; i++) {
			(QPlayer.instance.skills[2] as QPlayerSkillBuffering).options[i].active = skillData[2][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[3] as QPlayerSkillShield).options.Length; i++) {
			(QPlayer.instance.skills[3] as QPlayerSkillShield).options[i].active = skillData[3][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[4] as QPlayerSkillFlagit).options.Length; i++) {
			(QPlayer.instance.skills[4] as QPlayerSkillFlagit).options[i].active = skillData[4][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[5] as QPlayerSkillLightning).options.Length; i++) {
			(QPlayer.instance.skills[5] as QPlayerSkillLightning).options[i].active = skillData[5][i];
		} 
		for(int i=0; i<(QPlayer.instance.skills[6] as QPlayerSkillFightGhostFist).options.Length; i++) {
			(QPlayer.instance.skills[6] as QPlayerSkillFightGhostFist).options[i].active = skillData[6][i];
		} 
	}
	public bool OnPressEscape()
	{
		if (!CanActive()) return true;
		if (currSelectedMainBranch == -1)
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
		if (!CanActive()) return;
		StartCoroutine(ReturnToMainCo());
	}
	public bool CanActive()
	{
		return !isPlayingAnimation && !isPlayingAction;
	}
	private IEnumerator ReturnToMainCo() 
	{
		isPlayingAction = true;
		QPlayerSkillUiSubTree subTree = subTrees[currSelectedMainBranch];
		QPlayerSkillUiInfo info = skillInfos[currSelectedMainBranch];
		StartCoroutine(info.FadeOut());
		yield return StartCoroutine(subTree.FadeOut());
		animator.SetTrigger("Un Select main");
		currSelectedMainBranch = -1;
		isPlayingAction = false;
	}
	private IEnumerator RotMainBranchCo(int number)
	{
		isPlayingAction = true;
		QPlayerSkillUiSubTree subTree;
		QPlayerSkillUiInfo info;
		float angle = 360f / mainBranchs.Count * number;
		float time = 0;
		Quaternion nextRot = Quaternion.Euler(0, 0, angle);
		if (currSelectedMainBranch == -1){ animator.SetTrigger("Select main"); }
		else{
			info = skillInfos[currSelectedMainBranch];
			subTree = subTrees[currSelectedMainBranch];
			StartCoroutine(info.FadeOut());
			yield return StartCoroutine(subTree.FadeOut());
		}
		while (time < 1f || isPlayingAnimation){
			Quaternion currRot = mainBranchGroub.rotation;
			mainBranchGroub.rotation = Quaternion.Lerp(currRot, nextRot, Time.deltaTime * 5f);
			time += Time.deltaTime;
			yield return null;
		}
		foreach (var item in subTrees){
			item.gameObject.SetActive(false);
		} 
		currSelectedMainBranch = number;
		info = skillInfos[currSelectedMainBranch];
		subTree = subTrees[currSelectedMainBranch];
		subTree.gameObject.SetActive(true);
		StartCoroutine(info.FadeIn());
		yield return StartCoroutine(subTree.FadeIn());
		isPlayingAction = false;
	}
	public void SelectMainBranch(int number)
	{
		if (isPlayingAction || isPlayingAnimation) return;
		if (number == currSelectedMainBranch) ReturnToMain();
		else StartCoroutine(RotMainBranchCo(number));
	}
	public void StartAnimation() => isPlayingAnimation = true; 
	public void EndAnimation() => isPlayingAnimation = false;
	public void ActiveOption(int number) 
	{
		if (!CanActive()) return;
		StartCoroutine(ActiveOptionCo(number));
	}
	private IEnumerator ActiveOptionCo(int number) 
	{
		isPlayingAction = true;
		// currSelectedMainBranch = 스킬 넘버
		// number = 특성 넘버
		int group = number / 2; // 그룹 넘버 0,1,2,3
		bool left = (number % 2 == 0) ? true : false; // true면 왼쪽꺼
		QPlayerSkillUiSubTree st = subTrees[currSelectedMainBranch];
		bool run = true;
		bool change = false;
		if (group != 0 && run){
			if (!st.optionGroups[group - 1].optionActive) run = false; 
		}
		if (group != 3 && run) {
			if (st.optionGroups[group + 1].optionActive && 
				st.optionGroups[group].currLeftActive == left)
				run = false;
			else change = true;
		}
		if (run){
			if (change)	{
				StartCoroutine(st.Refresh(group, left));
				yield return StartCoroutine(st.branchGroups[group + 1].Refresh());
			}
			else{
				yield return StartCoroutine(st.Refresh(group, left));
			}
		}
		isPlayingAction = false;
	}
}
