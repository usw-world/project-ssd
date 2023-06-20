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
        
        if(SSDNetworkManager.instance.isHost) {
            (TPlayerCamera.instance as TPlayerCamera)?.SetActiveMouseMove(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void Close() {
        settingUiGobj.SetActive(false);
        exitConfirmUiGobj.SetActive(false);
        this.gameObject.SetActive(false);
        
        if(SSDNetworkManager.instance.isHost) {
            (TPlayerCamera.instance as TPlayerCamera)?.SetActiveMouseMove(true);
            Cursor.lockState = CursorLockMode.Locked;
        }
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