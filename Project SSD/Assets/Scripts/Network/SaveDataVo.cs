using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataVO {
    public string token = "";
    public string skillPoint = "";
    [SerializeField]public int skill_UnityBall = 0;

    public void Load(string saveData) {
        JsonUtility.FromJsonOverwrite(saveData, this);
    }

    public void Print()
    {
        Debug.Log(token);
        Debug.Log(skillPoint);
        Debug.Log(skill_UnityBall);
    }
}
