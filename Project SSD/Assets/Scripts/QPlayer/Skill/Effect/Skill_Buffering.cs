using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Skill_Buffering : MonoBehaviour, IPoolableObject
{
    #region parameter

    [FormerlySerializedAs("bufferList")] public List<GameObject> bufferingList = new List<GameObject>();
    public GameObject explosionEffect;
    public QPlayerSkillBuffering parent;
    public float speed;
    public float scale;
    public float strength;
    public float durationTime;
    public Transform target;
    public float maxDegree;
    public float distance;
    public float radius;
    public int quantity;
    public bool usingSkill = false;
    public bool throwable = false;
    public float degree;
    private bool wheelDestroyed = false;
    private bool isSpin = true;
    private bool lastSpin = true;
    private float timer;
    private string explosionEffectPrefabKey;
    #endregion

    public void OnActive()
    {
        
        explosionEffectPrefabKey = explosionEffect.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(explosionEffectPrefabKey, explosionEffect, false);
        ActiveBuffering();
        //if(option7Active)
        //    StartCoroutine(DestroySelf(durationTime));
        //StartCoroutine(DestroySelf(durationTime));
    }

    private void Awake()
    {
        initalize();
    }

    public void BoostDamage(Attachment attachment)
    {
        TPlayer.instance.AddAttachment(attachment);
    }
    public void IncreaseDistance()
    {
        foreach (var obj in bufferingList)
        {
            obj.transform.localPosition *= 1.5f;
        }
    }

    public void AddHitShield()
    {
        foreach (var obj in bufferingList)
        {
            obj.GetComponent<Buffering>().addShield = true;
        }
    }
    private void initalize()
    {
        // default value
        degree = 0f;
        strength = 5f;
        scale = 1f;
        speed = 5f;
        quantity = 1;
        distance = 4f;
        usingSkill = false;
        maxDegree = 720;
    }
    
    

    void Update()
    {
        if (!usingSkill)
            return;
        degree += Time.deltaTime + speed;
        if (!throwable)
        {
            transform.position = target.transform.position + new Vector3(0, 1, 0);
            transform.Rotate(new Vector3(0, Time.deltaTime + speed, 0));
        }
        else
            foreach (var buffering in bufferingList)
            {
                Debug.Log(buffering.transform.forward);
                buffering.transform.position += buffering.transform.forward * 3 * (speed * Time.deltaTime);
            }
        if(degree > maxDegree)
            StartCoroutine(DestroySelf(0));
    }



    public void ActiveBuffering()
    {
        transform.position = target.position + new Vector3(0, 1, 0);
        transform.rotation = target.rotation;
        degree -= 30;
        if(quantity == 1)
            bufferingList[0].SetActive(true);
        else
            foreach (var obj in bufferingList)
            {
                obj.SetActive(true);
            }

        foreach (var obj in bufferingList)
        {
            obj.transform.localScale = new Vector3(scale, scale, scale);
        }

        Debug.Log(quantity+" : quantity");
        usingSkill = true;
    }


    public string GetKey()
    {
        return GetType().ToString();
    }

    IEnumerator DestroySelf(float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        parent.canUseChain = false;
        Destroy(this.gameObject);
    }


}
