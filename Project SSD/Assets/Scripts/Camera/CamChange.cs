using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamChange : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera cutSceneCam;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("object Enter");
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("change");
            PlayerCamera.instance.ChangeCam(cutSceneCam);
            //other.gameObject.GetComponent<CameraTarget>().cameraManager.changeCam(cutSceneCam);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Object Exit");
        if (other.gameObject.tag.Equals("Player"))
        {
            //other.gameObject.GetComponent<CameraTarget>().cameraManager.RollbackCam();
            PlayerCamera.instance.RevertCam();
        }

    }
}
