using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraObjectExample : MonoBehaviour
{
    // Start is called before the first frame update
    public List<CinemachineVirtualCamera> camList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera virtualCamera;
    public void SelectCam(int camIndex)
    {
        virtualCamera = camList[camIndex];
    }
    public void SwitchCam_Easy_InOut(float time)
    {
        CameraManager.instance.SwitchCamera_Easy_InOut(virtualCamera, time);
    }

    public void SwitchCam_Cut()
    {
        CameraManager.instance.SwitchCamera_Cut(virtualCamera);
    }

    public void SwitchCam()
    {
        CameraManager.instance.SwitchCameara(virtualCamera);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        GetComponent<Animator>().SetTrigger("Play");
    }

    public void SetCameraToPlayerCamera()
    {
        virtualCamera = CameraManager.instance.playerCam;
        CameraManager.instance.SetPlayerCamera();
    }
}
