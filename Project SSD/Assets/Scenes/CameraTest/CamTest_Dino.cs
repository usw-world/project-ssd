using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTest_Dino : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public CinemachineVirtualCamera cam;
    private void Awake()
    {
        PlayerCamera.instance.ChangeCam(cam);
    }

    public void noise(float time)
    {
        PlayerCamera.instance.MakeNoise(10, time);
    }

    public void ChangeTarget()
    {
        PlayerCamera.instance.SetTarget(target.transform);
    }

    public void RevertCam()
    {
        PlayerCamera.instance.RevertCam();
        Debug.Log("revert");
    }

    public void DestroyCamEffect()
    {
        Destroy(this);
    }

}
