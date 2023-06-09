using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public float masterVolume = 1f;
    public float effectVolume = 1f;
    public float voiceVolume = 1f;
    public float bgmVolume = 1f;
	public TPlayerAudioClipPlayer tPlayer;
	public QPlayerAudioClipPlayer qPlayer;
	public SoundBGM bgm;
	private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
	public void PlayDelay(IEnumerator co) 
	{
		StartCoroutine(co);
	}
	public float GetEffectVolume() { return masterVolume * effectVolume; }
	public float GetVoiceVolume() { return voiceVolume * effectVolume; }
	public float GetBgmVolume() { return bgmVolume * effectVolume; }
	public void FadeOutVolume(AudioSource source) {
		StartCoroutine(FadeOutVolumeCoroutine(source));
	}
	private IEnumerator FadeOutVolumeCoroutine(AudioSource source) {
		float origin = source.volume;
		float offset = 1;
		while(offset > 0) {
			offset -= Time.deltaTime;
			source.volume = origin * offset;
			yield return null;
		}
	}
}
[Serializable]
public class SoundBGM
{
	public PlayerAudioClip test_01;
}
[Serializable]
public class PlayerAudioClip
{
	[SerializeField] private AudioClip clip;
	public void Play(AudioSource audioSource, ESoundType type)
	{
		SetVolume(audioSource, type);
		audioSource.clip = clip;
		audioSource.Play();
	}
	public void PlayOneShot(AudioSource audioSource, ESoundType type)
	{
		SetVolume(audioSource, type);
		audioSource.PlayOneShot(clip);
	}
	public void PlayDelay(AudioSource audioSource, ESoundType type, float time)
	{
		SetVolume(audioSource, type);
		SoundManager.instance.PlayDelay(PlayDelayCo(audioSource, time));
	}
	private IEnumerator PlayDelayCo(AudioSource audioSource, float time)
	{
		yield return new WaitForSeconds(time);
		audioSource.clip = clip;
		audioSource.Play();
	}
	public void PlayOneShotDelay(AudioSource audioSource, ESoundType type, float time)
	{
		SetVolume(audioSource, type);
		SoundManager.instance.PlayDelay(PlayOneShotDelayCo(audioSource, time));
	}
	private IEnumerator PlayOneShotDelayCo(AudioSource audioSource, float time) 
	{
		yield return new WaitForSeconds(time);
		audioSource.PlayOneShot(clip);
	}
	private void SetVolume(AudioSource audioSource, ESoundType type) 
	{
		switch (type)
		{
			case ESoundType.effect: audioSource.volume = SoundManager.instance.GetEffectVolume(); break;
			case ESoundType.voice: audioSource.volume = SoundManager.instance.GetEffectVolume(); break;
			case ESoundType.bgm: audioSource.volume = SoundManager.instance.GetEffectVolume(); break;
		}
	}
}
[Serializable]
public class TPlayerSoundEffect
{
	public PlayerAudioClip footStep;
	public PlayerAudioClip drawAttackSpecial_start;
	public PlayerAudioClip drawAttackSpecial_stay;
	public PlayerAudioClip drawAttackSpecial_end;
	public PlayerAudioClip slash_01;
	public PlayerAudioClip slash_02;
	public PlayerAudioClip slash_03;
	public PlayerAudioClip dodge;
}
[Serializable]
public class TPlayerSoundVoice
{
	public PlayerAudioClip[] attack;
	public PlayerAudioClip[] hit;
	public PlayerAudioClip down;
	public PlayerAudioClip attack_combo_6;
	public PlayerAudioClip attack_combo_7;
	public PlayerAudioClip drawAttackSpecialEnd;
	public PlayerAudioClip drawAttackSpecialEnd_Miss;
	public PlayerAudioClip drawAttackSpecialReady;
	public PlayerAudioClip drawAttackSpecialStart;
	public PlayerAudioClip drawAttackSpecialStart2;
	public PlayerAudioClip powerUp;
	private bool isPlayAttack = false;
	private bool isPlayHit = false;
	private int playedAttackIdx = 0;
	private int playedHitIdx = 0;
	public void AttackRandom(AudioSource audioSource, float probability) 
	{
		if (isPlayAttack)
		{
			if (UnityEngine.Random.Range(1f, 101f) <= probability)
			{
				int nextPlayAttackIdx = 0;
				while (playedAttackIdx == nextPlayAttackIdx)
					nextPlayAttackIdx = UnityEngine.Random.Range(0, attack.Length);
				attack[nextPlayAttackIdx].Play(audioSource, ESoundType.voice);
				playedAttackIdx = nextPlayAttackIdx;
				isPlayAttack = false;
			}
		}
		else
		{
			isPlayAttack = true;
		}
	}
	public void HitRandom(AudioSource audioSource, float probability)
	{
		if (isPlayHit)
		{
			if (UnityEngine.Random.Range(1f, 101f) <= probability)
			{
				int index = 0;
				while (playedHitIdx == index)
					index = UnityEngine.Random.Range(0, hit.Length);
				hit[index].Play(audioSource, ESoundType.voice);
				playedHitIdx = index;
				isPlayHit = false;
			}
		}
		else
		{
			isPlayHit = true;
		}
	}
}
[Serializable]
public class TPlayerAudioClipPlayer
{
	public TPlayerSoundEffect effect;
	public TPlayerSoundVoice voice;
}
[Serializable]
public class QPlayerSoundEffect
{
	public PlayerAudioClip finishEffectBlue;
	public PlayerAudioClip finishEffectRed;
	public PlayerAudioClip finishLaser;
	public PlayerAudioClip finishExplosion;
	public PlayerAudioClip Q_meteo;
	public PlayerAudioClip flame;
	public PlayerAudioClip fireBall;
	public PlayerAudioClip storm;
	public PlayerAudioClip storm2;
}
[Serializable]
public class QPlayerSoundVoice
{
}
[Serializable]
public class QPlayerAudioClipPlayer
{
	public QPlayerSoundEffect effect;
	public QPlayerSoundVoice voice;
}
public enum ESoundType 
{
	effect, voice, bgm
}
