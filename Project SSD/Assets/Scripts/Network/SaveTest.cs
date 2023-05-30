using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveTest : MonoBehaviour
{
    public static SaveDataVO saveData = new SaveDataVO();
    // Start is called before the first frame update
    public Text[] data;
    public Text statusText;
    public InputField spInput;
    public InputField ubInput;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            saveData.Print();
            UpdateUi();
        }
    }

    public void UpdateUi()
    {
        data[2].text = saveData.token;
        data[3].text = saveData.skillPoint;
        data[4].text = saveData.skill_UnityBall+"";
    }

    public void ChangeData()
    {
        saveData.skillPoint = spInput.text;
        saveData.skill_UnityBall = int.Parse(ubInput.text);
        UpdateUi();
        statusText.text = "InGame Data Changed";
    }

    public void ResetData()
    {
        statusText.text = "Logout";
        saveData.token = "";
        saveData.skillPoint = "";
        saveData.skill_UnityBall = int.Parse("0");
        UpdateUi();
    }
}
