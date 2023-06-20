using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

	public TPlayerUI tPlayerHUD;
	[SerializeField] private TPlayerSkillUi tPlayerSkill;

	public QPlayerUI qPlayerHUD;
	[SerializeField] private QPlayerSkillUi qPlayerSkill;

	[SerializeField] private Canvas commonHudCanvas;
	[SerializeField] private EscapeMenu escapeMenu;

 	[SerializeField] private GameObject alertUIPrefab;
	[SerializeField] private Animator fadeInOutAnimator;
	private Coroutine fadeInOutCoroutine;
	private GameObject activatedUi = null;

	private GameObject ownHud;

	#region Unity Events
    private void Awake() {
		if(instance == null)
			instance = this;
		else
			Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
	}
	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape))
			UIManager.instance.OnPressEscape();
		// if(Input.GetKeyDown(KeyCode.K))
		// 	SSDNetworkManager.instance.isHost
	}
	#endregion Unity Events

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
					tPlayerHUD.gameObject.SetActive(true);
					activatedUi = null;
				}
			}
			else if (activatedUi == qPlayerSkill.gameObject)
			{
				if (!qPlayerSkill.OnPressEscape())
				{
					qPlayerHUD.gameObject.SetActive(true);
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
			{
				activatedUi = tPlayerSkill.gameObject;
				tPlayerHUD.gameObject.SetActive(false);
			}
			else {
				activatedUi = null; 
				tPlayerHUD.gameObject.SetActive(true);
			}
		}
	}
	public void OnPressQPlayerSkill()
	{
		if (qPlayerSkill.CanActive())
		{
			if (qPlayerSkill.OnActive())
			{
				activatedUi = qPlayerSkill.gameObject;
				qPlayerHUD.gameObject.SetActive(false);
			}
			else { 
				activatedUi = null; 
				qPlayerHUD.gameObject.SetActive(true); 
			}
		}
	}

	public void SetActiveHud(bool active) {
		if (!SSDNetworkManager.instance.isHost) {
			tPlayerHUD.gameObject.SetActive(active);
		}
		else {
			qPlayerHUD.gameObject.SetActive(active);
		}
		commonHudCanvas.gameObject.SetActive(active);
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
