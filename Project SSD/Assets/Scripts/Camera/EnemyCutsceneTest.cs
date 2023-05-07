using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyCutsceneTest : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public GameObject target;
    // Start is called before the first frame update




    private void OnTriggerEnter(Collider other)
    {

        target.SetActive(true);
    }


}
