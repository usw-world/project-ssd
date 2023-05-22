using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveTest : MonoBehaviour
{
    public static SaveDataVo saveData = new SaveDataVo();
    // Start is called before the first frame update
    public Text[] data;
    public Text Status;
    public InputField SpInput;
    public InputField UBInput;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            saveData.print();
            UpdateUi();
        }
    }

    public void UpdateUi()
    {
        data[0].text = saveData.user_id;
        data[1].text = saveData.user_pw;
        data[2].text = saveData.token;
        data[3].text = saveData.skillPoint;
        data[4].text = saveData.skill_UnityBall+"";
    }

    public void ChangeData()
    {
        saveData.skillPoint = SpInput.text;
        saveData.skill_UnityBall = int.Parse(UBInput.text);
        UpdateUi();
        Status.text = "InGame Data Changed";
    }

    public void ResetData()
    {
        Status.text = "Logout";
        saveData.user_id = "";
        saveData.user_pw = "";
        saveData.token = "";
        saveData.skillPoint = "";
        saveData.skill_UnityBall = int.Parse("0");
        UpdateUi();
    }
}
