using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager UIM;

    private float cinemaMoveSpeed = 200f;      // 배너가 움직이는 속도

	[SerializeField] private RectTransform cutSceneBannerTop;           // 상단 배너
    [SerializeField] private RectTransform cutSceneBannerBottom;        // 하단 배너

    private UIManager() { }
    private void Awake() => UIM = this;
    public void StartCutScene() => StartCoroutine(OnBanner()); 
    public void EndCutScene() => StartCoroutine(OffBanner()); 
    IEnumerator OnBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y >= -65f)
        {
            cutSceneBannerTop.anchoredPosition -= new Vector2(0, cinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition += new Vector2(0, cinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, -65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, 65f);
    }
    IEnumerator OffBanner()
    {
        while (cutSceneBannerTop.anchoredPosition.y <= 65f)
        {
            cutSceneBannerTop.anchoredPosition += new Vector2(0, cinemaMoveSpeed * Time.deltaTime);
            cutSceneBannerBottom.anchoredPosition -= new Vector2(0, cinemaMoveSpeed * Time.deltaTime);
            yield return null;
        }
        cutSceneBannerTop.anchoredPosition = new Vector2(0, 65f);
        cutSceneBannerBottom.anchoredPosition = new Vector2(0, -65f);
    }
}
