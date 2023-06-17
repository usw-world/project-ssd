using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TPlayerSkillBtnEvent : MonoBehaviour
{
	[SerializeField] private Image branchImg;
	[SerializeField] private TextMeshProUGUI skillNameTxt;
	[SerializeField] private TextMeshProUGUI skillInfoTxt;
	[SerializeField] private Image subBranchImg;
	private EventTrigger eventTrigger;
	private Coroutine fadeIn;
	private Coroutine fadeOut;
	private bool isLock = false;
	private void Awake()
	{
		eventTrigger = GetComponent<EventTrigger>();
		EventTrigger.Entry entey = new EventTrigger.Entry();
		entey.eventID = EventTriggerType.PointerEnter;
		entey.callback.AddListener((eventData) => { MouseEnter();});
		eventTrigger.triggers.Add(entey);
		entey = new EventTrigger.Entry();
		entey.eventID = EventTriggerType.PointerExit;
		entey.callback.AddListener((eventData) => { MouseExit(); });
		eventTrigger.triggers.Add(entey);
	}
	public void MouseEnter()
	{
		if (isLock) return;
		if (fadeOut != null) StopCoroutine(fadeOut);
		fadeIn = StartCoroutine(FadeIn());
	}
	public void MouseExit()
	{
		if (isLock) return;
		if (fadeIn != null) StopCoroutine(fadeIn);
		fadeOut = StartCoroutine(FadeOut());
	}
	public void Lock() 
	{
		isLock = true;
	}
	public void UnLock()
	{
		isLock = false;
	}
	private IEnumerator FadeIn()
	{
		float val = skillNameTxt.color.a;
		while (val < 1f)
		{
			branchImg.fillAmount += Time.deltaTime * 4f;
			val += Time.deltaTime * 2f;
			Color color = skillNameTxt.color;
			color.a = val;
			skillNameTxt.color = color;
			color = skillInfoTxt.color;
			color.a = val;
			skillInfoTxt.color = color;
			yield return null;
		}
		branchImg.fillAmount = 1f;
	}
	private IEnumerator FadeOut()
	{
		float val = skillNameTxt.color.a;
		while (val > 0.01f)
		{
			branchImg.fillAmount -= Time.deltaTime * 4f;
			val -= Time.deltaTime * 2f;
			Color color = skillNameTxt.color;
			color.a = val;
			skillNameTxt.color = color;
			color = skillInfoTxt.color;
			color.a = val;
			skillInfoTxt.color = color;
			yield return null;
		}
		branchImg.fillAmount = 0;
	}
}
