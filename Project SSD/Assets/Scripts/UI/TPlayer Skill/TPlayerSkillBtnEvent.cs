using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkillBtnEvent : MonoBehaviour
{
	[SerializeField] private Color branchColor;
	[SerializeField] private Color textColor;
	private Coroutine fadeIn;
	private Coroutine fadeOut;
	private void OnMouseEnter()
	{
		if (fadeOut != null) StopCoroutine(fadeOut);
		fadeIn = StartCoroutine(FadeIn());
	}
	private void OnMouseExit()
	{
		if (fadeIn != null) StopCoroutine(fadeIn);
		fadeOut = StartCoroutine(FadeOut());
	}
	private IEnumerator FadeIn()
	{
		float time = 0;
		while (time < 1)
		{
			time += Time.deltaTime;
			yield return null;
		}
	}
	private IEnumerator FadeOut()
	{
		float time = 0;
		while (time < 1)
		{
			time += Time.deltaTime;
			yield return null;
		}
	}
}
