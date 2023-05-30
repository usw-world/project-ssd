using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public SaveDataVO saveData { get; private set; }

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