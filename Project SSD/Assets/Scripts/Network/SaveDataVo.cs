using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataVO {
    public string userId = "";
    public string token = "";

    public int tExp = 0;
    public int qExp = 0;
    public int tLevel = 1;
    public int qLevel = 1;
    public int[] tSkillData = new int[6] { 0, 0, 0, 0, 0, 0 };
    public int[] qSkillData = new int[7] { 0, 0, 0, 0, 0, 0, 0 };

    public SaveDataVO() {}
    public SaveDataVO(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }
    public void Write(string saveData) {
        JsonUtility.FromJsonOverwrite(saveData, this);
    }
}

public class ErrorMessage {
    public string message;
}
