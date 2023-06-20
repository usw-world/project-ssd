using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonUI : MonoBehaviour
{
    [SerializeField] private RectTransform cutSceneBannerTop;
    [SerializeField] private RectTransform cutSceneBannerBottom;
    [SerializeField] private float CinemaMoveSpeed = 120f;
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
    }
}
