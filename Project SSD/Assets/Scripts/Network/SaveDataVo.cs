using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataVo
{

    public string user_id;
    public string user_pw;
    [SerializeField] public int token;
    public string skillPoint;
    [SerializeField]public int skill_UnityBall;

    public void load(string saveData)
    {
        JsonUtility.FromJsonOverwrite(saveData, this);
    }

    public void print()
    {
        Debug.Log(user_id);
        Debug.Log(user_pw);
        Debug.Log(token);
        Debug.Log(skillPoint);
        Debug.Log(skill_UnityBall);
    }
}
