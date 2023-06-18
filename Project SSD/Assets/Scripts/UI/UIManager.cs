using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

	[SerializeField] private TPlayerUI tPlayerUI;
	[SerializeField] private QPlayerUI qPlayerUI;

	[SerializeField] private Canvas commonHudCanvas;
 	[SerializeField] private GameObject alertUIPrefab;
	[SerializeField] private EscapeMenu escapeMenu;
	[SerializeField] private TPlayerSkillUi tPlayerSkill;
	[SerializeField] private QPlayerSkillUi qPlayerSkill;

	[SerializeField] private Animator fadeInOutAnimator;
	private Coroutine fadeInOutCoroutine;
	private GameObject activatedUi = null;

    private void Awake() {
		if(instance == null)
			instance = this;
		else
			Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
	}
	public void FadeIn(float delay=0f, float duration=2f, System.Action callback=null) {
		if(fadeInOutCoroutine != null)
			StopCoroutine(fadeInOutCoroutine);
		fadeInOutCoroutine = StartCoroutine(FadeInOutCoroutine(true, delay, duration, callback));
	}
	public void FadeOut(float delay=0f, float duration=2f, System.Action callback=null) {
		if(fadeInOutCoroutine != null)
			StopCoroutine(fadeInOutCoroutine);
		fadeInOutCoroutine = StartCoroutine(FadeInOutCoroutine(false, delay, duration, callback));
	}
	public IEnumerator FadeInOutCoroutine(bool fadeIn, float delay, float duration, System.Action callback) {
		fadeInOutAnimator.SetFloat("Fade Speed", 0);
		if(fadeIn)
			fadeInOutAnimator.SetTrigger("Fade In");
		else
			fadeInOutAnimator.SetTrigger("Fade Out");
		yield return new WaitForSeconds(delay);
		fadeInOutAnimator.SetFloat("Fade Speed", 1f/duration);

		yield return new WaitForSeconds(duration);
		callback?.Invoke();
	}
	public void OnPressEscape() {
		if (activatedUi == null)
		{
			escapeMenu.OnPressEscape();
		}
		else
		{
			if (activatedUi == tPlayerSkill.gameObject)
			{
				if (!tPlayerSkill.OnPressEscape())
				{
					activatedUi = null;
				}
			}
			else if (activatedUi == qPlayerSkill.gameObject)
			{
				if (!qPlayerSkill.OnPressEscape())
				{
					activatedUi = null;
				}
			}
		}
	}
	public AlertUI AlertMessage(string message) {
		GameObject alertUIGobj = Instantiate(alertUIPrefab, commonHudCanvas.transform);
		AlertUI alertUI = alertUIGobj.GetComponent<AlertUI>();
		if(alertUI != null) {
			alertUI.SetMessage(message);
		} else {
			Debug.LogError("Message box for alert is loaded but it hasn't 'AlertUI' Component.");
		}
		return alertUI;
	}
	public void OnPressTPlayerSkill()
	{
		if (tPlayerSkill.CanActive())
		{
			if (tPlayerSkill.OnActive())
				activatedUi = tPlayerSkill.gameObject;
			else activatedUi = null;
		}
	}
	public void OnPressQPlayerSkill()
	{
		if (qPlayerSkill.CanActive())
		{
			if (qPlayerSkill.OnActive())
				activatedUi = qPlayerSkill.gameObject;
			else activatedUi = null;
			return;
		}
	}
	/*
	public void StartCutScene()
    {
        StartCoroutine(OnBanner());
    }
    public void EndCutScene()
    {
        StartCoroutine(OffBanner());
    }
    IEnumerator OnBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y >= -65f)
        {
            cutSceneBannerTop.anchoredPosition -= new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition += new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, -65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, 65f);
    }
    IEnumerator OffBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y <= 65f)
        {
            cutSceneBannerTop.anchoredPosition += new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition -= new Vector2(0, CinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, 65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, -65f);
    }*/
}
