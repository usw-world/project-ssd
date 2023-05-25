using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttachmentManager : MonoBehaviour
{
	public List<Attachment> attachments = new List<Attachment>();
	public void AddAttachment(Attachment attachment) {

		attachments.Add(attachment);
		attachment.coroutine = StartCoroutine(Run(attachment));
	}
	public void Clear() {
		for (int i = 0; i < attachments.Count; i++)	{
			StopCoroutine(attachments[i].coroutine);
		}
		attachments.Clear();
	}
	IEnumerator Run(Attachment attachment) {
		attachment.onAction?.Invoke(gameObject);
		for (float i = attachment.time; i > 0; i -= attachment.interval)
		{
			attachment.onStay?.Invoke(gameObject);
			yield return new WaitForSeconds(attachment.interval);
		}
		attachment.onInactive?.Invoke(gameObject);
	}
}
public class Attachment
{
	public float time;
	public float interval;
	public EAttachmentType type;
	public Coroutine coroutine;
	public Sprite image;
	public Action<GameObject> onAction;
	public Action<GameObject> onStay;
	public Action<GameObject> onInactive;
	public Attachment(float time, float interval, Sprite image, EAttachmentType type) {
		this.time = time;
		this.interval = interval;
		this.image = image;
		this.type = type;
	}
}
public enum EAttachmentType
{
	damage,		// 데미지
	slow,		// 슬로우
	inability,	// 기절
	healing,    // 힐
	shield,     // 쉴드
	boost       // 공격력 상승
}