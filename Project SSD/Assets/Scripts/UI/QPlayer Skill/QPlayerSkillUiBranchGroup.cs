using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QPlayerSkillUiBranchGroup : MonoBehaviour
{
    [SerializeField] private List<Image> grayBranchs;
    [SerializeField] private List<Image> whiteBranchs;
	private List<QPlayerSkillUiOptionGroup> options;
	private bool isActiveWhiteBranch;
	private bool currLeftActive;
	private bool currLeftActiveTop;
	private int myIdx;
	private void Start()
	{
		foreach (var item in grayBranchs){
			item.color = Color.gray;
			item.fillAmount = 0;
		}
		foreach (var item in whiteBranchs){
			item.color = Color.white;
			item.fillAmount = 0;
		}
	}
	public IEnumerator FadeIn()
	{
		foreach (var item in grayBranchs){
			item.color = Color.gray;
			item.fillAmount = 0;
		}
		foreach (var item in whiteBranchs)
		{
			item.color = Color.white;
			item.fillAmount = 0;
		}
		StartCoroutine(FadeInWhite());
		float value = 0;
		while (value < 1f) {
			value += Time.deltaTime * 10f;
			grayBranchs[0].fillAmount = value;
			grayBranchs[1].fillAmount = value;
			yield return null;
		}
		if (grayBranchs.Count > 2){
			value = 0;
			while (value < 1f)	{
				value += Time.deltaTime * 10f;
				grayBranchs[2].fillAmount = value;
				grayBranchs[3].fillAmount = value;
				yield return null;
			}
		}
	}
	public IEnumerator FadeOut()
	{
		isActiveWhiteBranch = false;
		float value = 1f;
		Color colorG = Color.gray;
		Color colorW = Color.white;
		while (value > 0){
			value -= Time.deltaTime * 2f;
			colorG.a = value;
			colorW.a = value;
			foreach (var item in grayBranchs){
				item.color = colorG;
			}
			foreach (var item in whiteBranchs){
				item.color = colorW;
			}
			yield return null;
		}
	}
	public IEnumerator FadeInWhite()
	{
		if (!isActiveWhiteBranch){
			float value;
			if (myIdx == 0){
				if (options[myIdx].optionActive){
					value = 0;
					while (value < 1f)	{
						value += Time.deltaTime * 10f;
						if (options[myIdx].currLeftActive)
							whiteBranchs[0].fillAmount = value;
						else whiteBranchs[1].fillAmount = value;
						yield return null;
					}
					currLeftActive = options[myIdx].currLeftActive;
					isActiveWhiteBranch = true;
				}
			}
			else{
				if (options[myIdx - 1].optionActive && options[myIdx].optionActive)	{
					value = 0;
					while (value < 1f)		{
						value += Time.deltaTime * 10f;
						if (options[myIdx - 1].currLeftActive)
							whiteBranchs[0].fillAmount = value;
						else whiteBranchs[1].fillAmount = value;
						yield return null;
					}
					value = 0;
					while (value < 1f)		{
						value += Time.deltaTime * 10f;
						if (options[myIdx].currLeftActive)
							whiteBranchs[2].fillAmount = value;
						else whiteBranchs[3].fillAmount = value;
						yield return null;
					}
					currLeftActive = options[myIdx - 1].currLeftActive;
					currLeftActiveTop = options[myIdx].currLeftActive;
					isActiveWhiteBranch = true;
				}
			}
		}
		yield return null;
	}
	public void Set(int myIdx, List<QPlayerSkillUiOptionGroup> options) 
	{
		this.options = options;
		this.myIdx = myIdx;
	}
	public IEnumerator Refresh() 
	{
		float value;
		if (isActiveWhiteBranch)
		{
			isActiveWhiteBranch = false;
			value = 1f;
			Color colorW = Color.white;
			while (value > 0)
			{
				value -= Time.deltaTime * 10f;
				colorW.a = value;
				foreach (var item in whiteBranchs)
				{
					item.color = colorW;
				}
				yield return null;
			}
			foreach (var item in whiteBranchs)
			{
				item.color = Color.white;
				item.fillAmount = 0;
			}
		}
		if (myIdx == 0)
		{
			if (options[myIdx].optionActive)
			{
				value = 0;
				while (value < 1f)
				{
					value += Time.deltaTime * 10f;
					if (options[myIdx].currLeftActive)
						whiteBranchs[0].fillAmount = value;
					else whiteBranchs[1].fillAmount = value;
					yield return null;
				}
				currLeftActive = options[myIdx].currLeftActive;
				isActiveWhiteBranch = true;
			}
		}
		else
		{
			if (options[myIdx - 1].optionActive && options[myIdx].optionActive)
			{
				value = 0;
				while (value < 1f)
				{
					value += Time.deltaTime * 10f;
					if (options[myIdx - 1].currLeftActive)
						whiteBranchs[0].fillAmount = value;
					else whiteBranchs[1].fillAmount = value;
					yield return null;
				}
				value = 0;
				while (value < 1f)
				{
					value += Time.deltaTime * 10f;
					if (options[myIdx].currLeftActive)
						whiteBranchs[2].fillAmount = value;
					else whiteBranchs[3].fillAmount = value;
					yield return null;
				}
				currLeftActive = options[myIdx - 1].currLeftActive;
				currLeftActiveTop = options[myIdx].currLeftActive;
				isActiveWhiteBranch = true;
			}
		}
		yield return null;
	}
}
