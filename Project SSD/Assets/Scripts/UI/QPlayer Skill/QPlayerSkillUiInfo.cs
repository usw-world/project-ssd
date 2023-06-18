using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QPlayerSkillUiInfo : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI skillName;
    [SerializeField] public TextMeshProUGUI skillInfo;
    [SerializeField] private Image branch;
	private void Start()
	{
		branch.fillAmount = 0;
		skillName.color = new Color(1f, 1f, 1f, 0);
		skillInfo.color = new Color(1f, 1f, 1f, 0);
	}
	public IEnumerator FadeIn() 
	{
		branch.color = Color.white;
		float value = skillName.color.a;
		Color color;
		while (value < 1f)
		{
			value += Time.deltaTime * 2f;
			color = skillName.color;
			color.a = value;
			skillName.color = color;
			color = skillInfo.color;
			color.a = value;
			skillInfo.color = color;
			branch.fillAmount = value * 2f;
			yield return null;
		}
	}
	public IEnumerator FadeOut()
	{
		float value = skillName.color.a;
		Color color;
		while (value > 0)
		{
			value -= Time.deltaTime * 2f;
			color = skillName.color;
			color.a = value;
			skillName.color = color;
			color = skillInfo.color;
			color.a = value;
			skillInfo.color = color;
			color = branch.color;
			color.a = value;
			branch.color = color;
			yield return null;
		}
	}
}
