using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public SaveDataVO saveData { get; private set; }

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(this.gameObject);

        for(int i=0; i<40; i++) {
            print(GetNextExp(i));
        }
    }
    public void ExitGame() {
        Application.Quit();
    }
    public void SetSaveData(SaveDataVO saveData) {
        this.saveData = saveData;
    }
    public void SetActiveInput(bool active) {
        var inputSystem = FindObjectOfType<UnityEngine.InputSystem.PlayerInput>();
        if(inputSystem != null)
            inputSystem.enabled = active;
    }
	public void SaveAllData() {
        if(saveData == null)
            return;
        
        if(SSDNetworkManager.instance.isHost)
            SaveTPlayerSkillData();
        else
            SaveQPlayerSkillData();
	}
    public void SynchronizeData2Server() {
        SaveAllData();
        string json = JsonUtility.ToJson(saveData);
        StartCoroutine(ServerConnector.instance.SendIngameDataCoroutine(json));
    }

    #region Level & Exp
    public void IncreaseExp(int amount) {
        bool isTPlayer = SSDNetworkManager.instance.isHost;

        int level = isTPlayer ? saveData.tLevel : saveData.qLevel;
        int exp = isTPlayer ? saveData.tExp : saveData.qExp;

        exp += amount;
        while(exp >= GetNextExp(level)) {
            level ++;
        }
        
        if(isTPlayer) {
            saveData.tLevel = level;
            saveData.tExp = exp;
        } else {
            saveData.qLevel = level;
            saveData.qExp = exp;
        }
    }
    private int GetNextExp(int level) {
        return (int)(100 + 100f * Mathf.Pow(level*.7f, 2));
    }
    #endregion Level & Exp

    #region Skill
    public int GetRemainingSkillPoint() {
        bool isTPlayer = SSDNetworkManager.instance.isHost;

        string skillData = isTPlayer ? saveData.tSkillData : saveData.qSkillData;
        string[] attributes = skillData.Split(" ");

        int point = isTPlayer ? saveData.tLevel : saveData.qLevel;
        for(int i=0; i<attributes.Length; i++) {
            if(isTPlayer) {
                /* will be made. */
            } else {
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<0) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<1) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<2) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<3) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<4) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<5) > 0) point --;
                if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<6) > 0) point --;
            }
        }
        return point;
    }
    public bool[] GetTSkillData() {
        string[] attributes = saveData.tSkillData.Split(" ");
        bool[] result = new bool[18];
        for(int i=0; i<attributes.Length; i++) {
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<0) > 0) result[i*3 + 0] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<1) > 0) result[i*3 + 1] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<2) > 0) result[i*3 + 2] = true;
        }
        return result;
    }
    public bool[][] GetQSkillData() {
        string[] attributes = saveData.qSkillData.Split(" ");
        bool[][] result = new bool[7][];

        for(int i=0; i<result.Length; i++) {
            result[i] = new bool[8];
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<0) > 0) result[i][0] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<1) > 0) result[i][1] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<2) > 0) result[i][2] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<3) > 0) result[i][3] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<4) > 0) result[i][4] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<5) > 0) result[i][5] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<6) > 0) result[i][6] = true;
            if((int.Parse(attributes[i], System.Globalization.NumberStyles.HexNumber) & 1<<7) > 0) result[i][7] = true;
        }
        return result;
    }
	private void SaveTPlayerSkillData() {
		/* Make saving data. */
        /* temporary >> */
        saveData.tSkillData = "06 03 01 00 00 07";
        /* << temporary */
	}
	private void SaveQPlayerSkillData() {
		var unityBall = 		QPlayer.instance.skills[0] as QPlayerSkillUnityBall;
		var aoe = 				QPlayer.instance.skills[1] as QPlayerSkillAoe;
		var buffering = 		QPlayer.instance.skills[2] as QPlayerSkillBuffering;
		var shield = 			QPlayer.instance.skills[3] as QPlayerSkillShield;
		var flagit = 			QPlayer.instance.skills[4] as QPlayerSkillFlagit;
		var lightning = 		QPlayer.instance.skills[5] as QPlayerSkillLightning;
		var fightGhostFist = 	QPlayer.instance.skills[6] as QPlayerSkillFightGhostFist;

		int[] skillArr = new int[7];
		
		for(int i=0; i<unityBall.options.Length; i++) {
			skillArr[0] |= (unityBall.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<aoe.options.Length; i++) {
			skillArr[1] |= (aoe.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<buffering.options.Length; i++) {
			skillArr[2] |= (buffering.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<shield.options.Length; i++) {
			skillArr[3] |= (shield.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<flagit.options.Length; i++) {
			skillArr[4] |= (flagit.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<lightning.options.Length; i++) {
			skillArr[5] |= (lightning.options[i].active ? 1<<i : 0);
		}
		for(int i=0; i<fightGhostFist.options.Length; i++) {
			skillArr[6] |= (fightGhostFist.options[i].active ? 1<<i : 0);
		}

		string skillArrHex = "";
		for(int i=0; i<skillArr.Length; i++) {
			skillArrHex += skillArr[i].ToString("X2") + " ";
		}
		GameManager.instance.saveData.qSkillData = skillArrHex.Trim();
	}
    #endregion Skill
    
    /*  */
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Keypad0))
            print(saveData.token);
        if(Input.GetKeyDown(KeyCode.Keypad1))
            IncreaseExp(10);
        if(Input.GetKeyDown(KeyCode.Keypad2))
            IncreaseExp(100);
        if(Input.GetKeyDown(KeyCode.Keypad3))
            IncreaseExp(1000);
        if(Input.GetKeyDown(KeyCode.Keypad4))
            SynchronizeData2Server();
    }
    /*  */
}