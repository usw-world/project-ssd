using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataVO {
    public string token = "";
    public string skillPoint = "";
    public int currentStage = 0;

    public int skill_unityBall = 0;
    public int skill_aoe = 0;

    public SaveDataVO() {}
    public SaveDataVO(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void Write(string saveData) {
        JsonUtility.FromJsonOverwrite(saveData, this);
    }
}
