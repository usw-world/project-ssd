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
    public string tSkillData = "00 00 00 00 00 00";
    public string qSkillData = "00 00 00 00 00 00 00";

    public SaveDataVO() {}
    public SaveDataVO(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }
    public void Write(string saveData) {
        JsonUtility.FromJsonOverwrite(saveData, this);
    }
    public override string ToString() {
        string str = "";
        str += "Token : " + token;
        str += "T Player Exp : " + tExp;
        str += "\nQ Player Exp : " + qExp;
        str += "\nT Player Level : " + tLevel;
        str += "\nQ Player Level : " + qLevel;
        str += "\nT Player SkillData : " + tSkillData;
        str += "\nQ Player SkillData : " + qSkillData;
        return str;
    }
}

public class ErrorMessage {
    public string message;
}
