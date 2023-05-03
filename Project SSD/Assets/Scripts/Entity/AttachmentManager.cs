using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttachmentManager : MonoBehaviour
{
	public List<Attachment> attachments = new List<Attachment>();
	public void AddAttachment(Attachment attachment) {

		attachments.Add(attachment);
		print("33 attachment : " + attachment);
		attachment.coroutine = StartCoroutine(Run(attachment));
	}
	public void Clear() {
		for (int i = 0; i < attachments.Count; i++)	{
			StopCoroutine(attachments[i].coroutine);
		}
		attachments.Clear();
	}
	IEnumerator Run(Attachment attachment) {
		print("44");
		attachment.onAction?.Invoke();
		print("55");
		for (float i = attachment.time; i > 0; i -= attachment.interval)
		{
			attachment.onStay?.Invoke();
			yield return new WaitForSeconds(attachment.interval);
		}
		attachment.onInactive?.Invoke();
	}
}
public class Attachment
{
	public float time;
	public float interval;
	public Coroutine coroutine;
	public Sprite image;
	public Action onAction;
	public Action onStay;
	public Action onInactive;
	public Attachment(float time, float interval, Sprite image) {
		this.time = time;
		this.interval = interval;
		this.image = image;
	}
}