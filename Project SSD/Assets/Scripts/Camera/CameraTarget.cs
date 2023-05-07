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
    CinemachineFramingTransposer camBody;
    [SerializeField] private GameObject qPlayerCamera;
    private PlayerCamera playerCamera;
    public CameraManager cameraManager;

    private void Awake()
    {
        InitializeCamera();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region move
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward, Space.Self);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.forward * -10 * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -100 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 100 * Time.deltaTime, 0);
        }

        #endregion

        #region CamEffect
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCamera.MakeNoise(10, 3);
        }
        #endregion

        #region zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
            playerCamera.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        #endregion


    }

    /*
    IEnumerator SetNoise()
    {
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = noiseValue;
        yield return new WaitForSeconds(1f);
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        StopCoroutine("SetNoise");
    }
    */

    private void InitializeCamera()
    {
        if (PlayerCamera.instance == null)
        {
            GameObject camera = Instantiate(qPlayerCamera);
            playerCamera = camera.GetComponent<PlayerCamera>();
            playerCamera.SetTarget(this.transform);
            playerCamera.virtualCamera.m_Priority = 11;
            CameraManager.instance.playerCam = camera.GetComponent<CinemachineVirtualCamera>();
        }
    }
}
