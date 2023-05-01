using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] float noiseValue;
    [SerializeField] float zoomSensitivity;
    public List<CinemachineVirtualCamera> camList;
    public CinemachineVirtualCamera mainCam;
    CinemachineFramingTransposer camBody;
    float camDistance;

    void Start()
    {
        camBody = mainCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        mainCam = GameObject.Find("MainCam").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        #region move
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Translate(transform.forward * 10 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
            transform.Translate(transform.forward * -10 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
            transform.Translate(transform.right * 10 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.Translate(transform.right * -10 * Time.deltaTime);
        }

        #endregion

        #region CamEffect
        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine("SetNoise");
        }
        #endregion

        #region zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
            camBody.m_CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        #endregion


    }

    IEnumerator SetNoise()
    {
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = noiseValue;
        yield return new WaitForSeconds(1f);
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        StopCoroutine("SetNoise");
    }
}
