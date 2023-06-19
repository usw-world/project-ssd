using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillUiSubTree : MonoBehaviour
{
	[SerializeField] public List<QPlayerSkillUiBranchGroup> branchGroups;
	[SerializeField] public List<QPlayerSkillUiOptionGroup> optionGroups;
	public IEnumerator FadeIn()
	{
		for (int i = 0; i < 4; i++){
			QPlayerSkillUiBranchGroup bg = branchGroups[i];
			QPlayerSkillUiOptionGroup og = optionGroups[i];
			yield return StartCoroutine(bg.FadeIn());
			yield return StartCoroutine(og.FadeIn());
		}
		yield return new WaitForSeconds(.4f);
	}
	public IEnumerator FadeOut()
	{
		for (int i = 0; i < 4; i++){
			StartCoroutine(branchGroups[i].FadeOut());
			StartCoroutine(optionGroups[i].FadeOut());
		}
		yield return new WaitForSeconds(0.5f);
	}
	public void Set(int skillIdx) 
	{
		QPlayerSkillUnityBall ub;
		QPlayerSkillAoe aoe;
		QPlayerSkillBuffering bf;
		QPlayerSkillShield sd;
		QPlayerSkillFlagit fg;
		QPlayerSkillLightning lt;
		QPlayerSkillFightGhostFist fgf;
		SkillOptionInformation[] options = null;

		switch (skillIdx)
		{
			case 0: ub  = QPlayer.instance.skills[skillIdx] as QPlayerSkillUnityBall;		
				options = ub.options;
				break;
			case 1: aoe = QPlayer.instance.skills[skillIdx] as QPlayerSkillAoe;				
				options = aoe.options;
				break;
			case 2: bf  = QPlayer.instance.skills[skillIdx] as QPlayerSkillBuffering;		
				options = bf.options;
				break;
			case 3: sd  = QPlayer.instance.skills[skillIdx] as QPlayerSkillShield;			
				options = sd.options;
				break;
			case 4: fg  = QPlayer.instance.skills[skillIdx] as QPlayerSkillFlagit;			
				options = fg.options;
				break;
			case 5: lt  = QPlayer.instance.skills[skillIdx] as QPlayerSkillLightning;		
				options = lt.options;
				break;
			case 6: fgf = QPlayer.instance.skills[skillIdx] as QPlayerSkillFightGhostFist;
				options = fgf.options;
				break;
		}
		for (int i = 0; i < 4; i++)
		{
			int idx = i * 2;
			optionGroups[i].Set(options[idx], options[idx + 1]);
			branchGroups[i].Set(i, optionGroups);
		}
	}
	public IEnumerator Refresh(int groupIdx, bool left) 
	{
		optionGroups[groupIdx].SelectOption(left);
		yield return StartCoroutine(branchGroups[groupIdx].Refresh());
	}
}
