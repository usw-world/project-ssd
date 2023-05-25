using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;


public class Skill_wheel : MonoBehaviour, IPoolableObject
{
    #region parameter

    public GameObject wheelPrefab;
    public GameObject wheelOption07Prefab;

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
    public bool option7Active = false;

    private GameObject[] wheel;
    private float degree;
    [SerializeField] private bool option06;
    private string wheelPrefabKey;
    private string wheelOption07PrefabKey;
    #endregion

    public void OnActive()
    {
        wheelPrefabKey = wheelPrefab.GetComponent<IPoolableObject>().GetKey();
        wheelOption07PrefabKey = wheelOption07Prefab.GetComponent<IPoolableObject>().GetKey();
        // 같은 스크립트라서 풀러에서 뽑을 때 모양 똑같이 나오는거 수정 예정
        PoolerManager.instance.InsertPooler(wheelPrefabKey, wheelPrefab, false);
        PoolerManager.instance.InsertPooler(wheelOption07PrefabKey, wheelOption07Prefab, false);
        CreateWheel();
        if(option7Active)
            StartCoroutine(DestroySelf(durationTime));
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
        speed = 300f;
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
        degree += Time.deltaTime * speed;
        if (option06)
            radius += Time.deltaTime * 2;
        if (option7Active)
        {
            index = 0;
            foreach (GameObject temp in wheel)
            {
                var rad = Mathf.Deg2Rad * (degree + (++index * (360f / wheel.Length)));
                var x = distance * Mathf.Sin(rad);
                var z = distance * Mathf.Cos(rad);
                temp.transform.position = target.transform.position + new Vector3(x, 0, z) * radius;
                temp.transform.position += new Vector3(0, 1.0f);
            }
        }
        else
        {
            if (degree < maxDegree)
            {
                index = 0;
                foreach (GameObject temp in wheel)
                {
                    var rad = Mathf.Deg2Rad * (degree + (++index * (360f / wheel.Length)));
                    var x = distance * Mathf.Sin(rad);
                    var z = distance * Mathf.Cos(rad);
                    temp.transform.position = target.transform.position + new Vector3(x, 0, z) * radius;
                    temp.transform.position += new Vector3(0, 1.0f);
                }
            }
            else
            {
                StartCoroutine(DestroySelf(0));
            }
        }
        
    }

    public void Active_rolling()
    {
        option06 = true;
        scale = 2f;
        quantity += 3;
        speed = 1000f;
        maxDegree = 960;
        distance = 3f;
    }

    public void CreateWheel()
    {
        wheel = new GameObject[quantity];
        target = TPlayer.instance.transform;
        for (int i = 0; i < wheel.Length; i++)
        {
            if (option7Active)
            {
                Debug.Log(option7Active + " : option7");
                var temp = PoolerManager.instance.OutPool(wheelOption07PrefabKey);
                wheel[i] = temp;
                temp.GetComponent<Wheel>().skill_Wheel = this;
            }
            else
                wheel[i] = PoolerManager.instance.OutPool(wheelPrefabKey);

            wheel[i].transform.localScale = new Vector3(scale, 0.1f, scale);
            wheel[i].GetComponent<Wheel>().strength = strength;
        }
        Debug.Log("start");
        usingSkill = true;
    }


    public void Active_stack()
    {
        // stack value
        degree = 0f;
        strength = 150f;
        scale = 1f;
        durationTime = 5.0f;
        speed = 300f;
        quantity = 9;
        distance = 2f;
        radius = 1f;
        usingSkill = false;
        maxDegree = 360;
        option7Active = true;
    }


    public string GetKey()
    {
        return GetType().ToString();
    }

    IEnumerator DestroySelf(float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        foreach (GameObject temp in wheel)
        {
            Destroy(temp.gameObject);
        }
        Destroy(this.gameObject);
    }
}
