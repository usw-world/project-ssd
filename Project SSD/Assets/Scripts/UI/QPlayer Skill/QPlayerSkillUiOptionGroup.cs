using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QPlayerSkillUiOptionGroup : MonoBehaviour
{
    [SerializeField] private List<QPlayerSkillUiInfo> infos;
    [SerializeField] private List<Image> options;
	private SkillOptionInformation left;
	private SkillOptionInformation rigth;
	public bool optionActive = false;
	public bool currLeftActive = false;
	private void Start()
	{
		Color color = new Color(1f, 1f, 1f, 0);
		foreach (var item in options){
			item.color = color;
		}
	}
	public IEnumerator FadeIn()
	{
		Color color = Color.white;
		float value = 0;
		while (value < 1f){
			value += Time.deltaTime * 8f;
			color.a = value;
			options[0].color = color;
			options[1].color = color;
			yield return null;
		}
		foreach (var item in infos){
			StartCoroutine(item.FadeIn());
		}
	}
	public IEnumerator FadeOut()
	{
		Color color = Color.white;
		float value = 1;
		foreach (var item in infos){
			StartCoroutine(item.FadeOut());
		}
		while (value > 0f){
			value -= Time.deltaTime * 2f;
			color.a = value;
			options[0].color = color;
			options[1].color = color;
			yield return null;
		}
	}
	public void Set(SkillOptionInformation left, SkillOptionInformation rigth) 
	{
		this.left = left;
		this.rigth = rigth;
		infos[0].skillName.text = left.name;
		infos[0].skillInfo.text = left.info;
		infos[1].skillName.text = rigth.name;
		infos[1].skillInfo.text = rigth.info;

		if (left.active || rigth.active){
			if (left.active) SelectOption(true);
			else SelectOption(false);
		}
	}
	public void SelectOption(bool leftActive)
	{
		if (optionActive)
		{
			if (currLeftActive)	// 왼쪽은 이미 활성화 상태
			{
				if (leftActive) // 왼쪽 취소 하기
				{
					left.active = false;
					optionActive = false;
				}
				else			// 오른쪽랑 교체
				{
					left.active = false;
					rigth.active = true;
					currLeftActive = false;
				}
			}
			else				// 오른쪽 이미 활성화
			{
				if (leftActive) // 왼쪽이랑 교체
				{
					left.active = true;
					rigth.active = false;
					currLeftActive = true;
				}
				else            // 오른쪽 취소
				{
					rigth.active = false;
					optionActive = false;
				}
			}
		}
		else
		{
			optionActive = true;
			left.active = false;
			rigth.active = false;
			currLeftActive = leftActive;
			if (leftActive) left.active = true; 
			else rigth.active = true; 
		}
	}
}
