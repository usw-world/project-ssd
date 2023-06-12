using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Skill_wheel : MonoBehaviour, IPoolableObject
{
    #region parameter

    public GameObject wheelPrefab;
    public GameObject wheelOption07Prefab;
    public GameObject explosionEffect;
    public float speed;
    public float scale;
    public float strength;
    public Transform target;
    public float durationTime;
    public float maxDegree;
    public float distance;
    public float radius;
    public int index;
    public int quantity;
    public bool usingSkill = false;
    public bool option07IsActive = false;

    private GameObject[] wheelArray;
    private float degree;
    private bool wheelDestroyed = false;
    private bool isSpin = true;
    private bool lastSpin = true;
    [SerializeField] private bool option06IsActive;
    private string wheelPrefabKey;
    private string wheelOption07PrefabKey;
    private string explosionEffectPrefabKey;
    #endregion

    public void OnActive()
    {
        
        wheelPrefabKey = wheelPrefab.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(wheelPrefabKey, wheelPrefab, false);
        explosionEffectPrefabKey = explosionEffect.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InsertPooler(explosionEffectPrefabKey, explosionEffect, false);
        wheelOption07PrefabKey = wheelOption07Prefab.GetComponent<IPoolableObject>().GetKey()+"_option07";
        PoolerManager.instance.InsertPooler(wheelOption07PrefabKey, wheelOption07Prefab, false);
        CreateWheel();
        //if(option7Active)
        //    StartCoroutine(DestroySelf(durationTime));
        //StartCoroutine(DestroySelf(durationTime));
    }

    private void Awake()
    {
        initalize();
    }

    public void initalize()
    {
        // default value
        degree = 0f;
        strength = 5f;
        scale = 1f;
        durationTime = 50.0f;
        speed = 10f;
        quantity = 1;
        distance = 2f;
        radius = 1f;
        usingSkill = false;
        maxDegree = 360;
    }


    void Update()
    {
        if (!usingSkill)
            return;
        
        transform.position = target.transform.position + new Vector3(0, 1, 0);
        var rot = transform.rotation.eulerAngles;
        rot.y = degree;

        if (option07IsActive)
        {
            degree += Time.deltaTime * speed * 10;
            if (degree < maxDegree)
                transform.rotation = Quaternion.Euler(rot);
            else if (isSpin)
            {
                StartCoroutine(ThrowWheel(2));
                transform.rotation = Quaternion.Euler(rot);
            }
            else
            {
                if (lastSpin)
                {
                    transform.rotation = Quaternion.Euler(rot);
                    degree -= Time.deltaTime * speed * 8;
                }
                foreach (var wheel in wheelArray)
                {
                    if(!wheelDestroyed) wheel.transform.localPosition += wheel.transform.forward * Time.deltaTime * speed/3;
                }
            }
        }
        else
        {
            degree += Time.deltaTime * speed * 10;
            if (degree < maxDegree)
            {
                transform.rotation = Quaternion.Euler(rot);
            }
            else
            {
                StartCoroutine(DestroySelf(0));
            }
            if (option06IsActive)
            {
                foreach (var wheel in wheelArray)
                {
                    wheel.transform.position += wheel.transform.forward * (Time.deltaTime * speed) / 10;
                }
            }
        }
        
    }

    public void Active_rolling()
    {
        option06IsActive = true;
        scale = 2f;
        quantity = 15;
        speed = 150f;
        maxDegree = 1080;
        distance = 3f;
    }

    public void CreateWheel()
    {
        wheelArray = new GameObject[quantity];
        try
        {
            target = TPlayer.instance.transform;
        }
        catch (Exception e)
        {
            Debug.Log("Tplayer is null set QPlayer instead " + e.Message);
            target = QPlayer.instance.transform;
        }
        
        transform.position = target.position + new Vector3(0, 1, 0);
        transform.rotation = target.rotation;
        for (int i = 0; i < wheelArray.Length; i++)
        {
            if (option07IsActive)
            {
                var temp = PoolerManager.instance.OutPool(wheelOption07PrefabKey);
                wheelArray[i] = temp;
                temp.GetComponent<Wheel>().skill_Wheel = this;
            }
            else
                wheelArray[i] = PoolerManager.instance.OutPool(wheelPrefabKey);
            wheelArray[i].transform.SetParent(transform);
            wheelArray[i].transform.localScale = new Vector3(scale, 0.1f, scale);
            wheelArray[i].GetComponent<Wheel>().strength = strength;
            var rot = wheelArray[i].transform.rotation.eulerAngles;
            rot.y = (degree + (i * (360 / wheelArray.Length)));
            wheelArray[i].transform.rotation = Quaternion.Euler(rot);
            var rad = Mathf.Deg2Rad * (degree+(i*(360/wheelArray.Length)));
            var x = distance * Mathf.Sin(rad);
            var y = distance * Mathf.Cos(rad);
            wheelArray[i].transform.position = transform.position + new Vector3(x, 0, y);
            
        }
        

        index = 0;

        usingSkill = true;
    }


    public void Active_stack()
    {
        // stack value
        degree = 0f;
        strength = 150f;
        scale = 3f;
        durationTime = 5.0f;
        speed = 30f;
        quantity = 4;
        distance = 1.5f;
        radius = 2f;
        usingSkill = false;
        maxDegree = 360;
        option07IsActive = true;
    }


    public string GetKey()
    {
        return GetType().ToString();
    }

    IEnumerator DestroySelf(float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        foreach (GameObject temp in wheelArray)
        {
            Destroy(temp.gameObject);
        }
        Destroy(this.gameObject);
    }

    IEnumerator ThrowWheel(float time)
    {
        if (!isSpin)
            yield break;
        isSpin = false;
        yield return new WaitForSeconds(time);
        lastSpin = false;
        yield return new WaitForSeconds(time);
        StartCoroutine(Boom(time));
    }

    IEnumerator Boom(float time)
    {
        wheelDestroyed = true;
        var boomList = new List<GameObject>();
        foreach (var wheel in wheelArray)
        {
            var boom = PoolerManager.instance.OutPool(explosionEffectPrefabKey);
            boomList.Add(boom);
            boom.transform.SetParent(wheel.transform);
            boom.transform.localPosition = Vector3.zero;
            boom.SetActive(true);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var boom in boomList)
        {
            Destroy(boom);
        }
        Destroy(this.gameObject);
    }
}
