using UnityEngine;

public class EscapeMenu : MonoBehaviour {
    [SerializeField] private GameObject menuButtonWrapGobj;
    [SerializeField] private GameObject exitConfirmUiGobj;
    [SerializeField] private GameObject settingUiGobj;
    public void OnPressEscape() {
        if(settingUiGobj.activeInHierarchy) {
            CloseSettingMenu();
        } else if(exitConfirmUiGobj.activeInHierarchy) {
            exitConfirmUiGobj.SetActive(false);
        } else {
            if(this.gameObject.activeInHierarchy)
                Close();
            else
                Open();
        }
    }
    public void Open() {
        this.gameObject.SetActive(true);
    }
    public void Close() {
        settingUiGobj.SetActive(false);
        exitConfirmUiGobj.SetActive(false);
        this.gameObject.SetActive(false);

    }
    public void OpenSettingMenu() {
        menuButtonWrapGobj.SetActive(false);
        settingUiGobj.SetActive(true);
    }
    public void CloseSettingMenu() {
        menuButtonWrapGobj.SetActive(true);
        settingUiGobj.SetActive(false);
    }
	public void ExitGame() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else 
		GameManager.instance.ExitGame();
		#endif
	}
}