using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private SaveDataVO saveData;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public void ExitGame() {
        Application.Quit();
    }
    public void SetSaveData(SaveDataVO saveData) {
        this.saveData = saveData;
    }
}