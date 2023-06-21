using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaController : MonoBehaviour {
    [SerializeField] private Animator cinemaAnimator;
    [SerializeField] private AudioSource audioSource;

    [System.Serializable]
    public class CinemaEvent : UnityEngine.Events.UnityEvent{}
    [SerializeField] private CinemaEvent onCinemaEnd;

    private void Awake() {
        audioSource = audioSource ?? GetComponent<AudioSource>();
    }
    public void StartCinema() {
        GameManager.instance.SetActiveInput(false);
        UIManager.instance.SetActiveHud(false);
        SetAnimationTrigger("Play");
    }
    public void EndCinema() {
        GameManager.instance.SetActiveInput(true);
        UIManager.instance.SetActiveHud(true);
        SetAnimationTrigger("Stop");
        onCinemaEnd?.Invoke();
    }
    public void SetAnimationTrigger(string parameter) {
        cinemaAnimator.SetTrigger(parameter);
    }
    public void FadeIn() {
        UIManager.instance.FadeIn();
    }
    public void FadeOut() {
        UIManager.instance.FadeOut();
    }
    public void PlayAudioClip(AudioClip clip) {
        audioSource.volume = SoundManager.instance.GetEffectVolume();
        audioSource.PlayOneShot(clip);
    }
}
