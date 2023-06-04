using UnityEngine;

public class AlertUI : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Text alertMessage;
    [SerializeField] private UnityEngine.UI.Button confirmButton;
    [SerializeField] public UnityEngine.Events.UnityEvent onClickConfirmButton;
    private void OnEnable() {
        confirmButton.Select();
    }
    private void Start() {
        onClickConfirmButton.AddListener(() => { Destroy(this.gameObject); });
        confirmButton.onClick.AddListener(() => { onClickConfirmButton?.Invoke(); });
    }
    public void SetMessage(string message) {
        alertMessage.text = message;
    }
}
