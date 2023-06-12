using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

// 플레이어 카메라 까지 사용할지는 모르겠는데 일단 환경 오브젝트 용으로 설계
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineVirtualCamera playerCam;
    [HideInInspector] public CinemachineVirtualCamera mainCam;
    private CinemachineVirtualCamera prevCam;
    private CinemachineBrain cinemachineBrain;

    
    private void Awake()
    {
        #region 싱글톤
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        #endregion
        Initialize();
    }

    public void Initialize()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        prevCam = null;
        mainCam = null;
    }

    public void SetCineMachineBrain(CinemachineBrain cinemachineBrain)
    {
        this.cinemachineBrain = cinemachineBrain;
    }

    public virtual void MakeNoise(float value, float time)
    {
        StartCoroutine(SetNoise(value, time));
    }
    IEnumerator SetNoise(float value, float time)
    {
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = value;
        yield return new WaitForSeconds(time);
        mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        StopCoroutine("SetNoise");
    }

    public void ShakeCamera(GameObject camera, float time, float power) 
    {
        StartCoroutine(ShakeCameraCo(camera, time, power));
    }
    private IEnumerator ShakeCameraCo(GameObject camera, float time, float power) 
    {
        Vector3 currPos = camera.transform.localPosition;
        for (float i = 0; i < time; i += Time.deltaTime)
		{
            float x = currPos.x + Random.Range(-power, power);
            float y = currPos.y + Random.Range(-power, power);
            float z = currPos.z + Random.Range(-power, power);
            camera.transform.localPosition = new Vector3(x, y, z);
            yield return null;
		}
        camera.transform.localPosition = currPos;
    }
    #region SwitchCam
    // 카메라 전환 커스텀 할 경우 CinemachineBrain에 커스텀 블랜드 항목 만들어서 사용
    // PostProcessing 적용시 카메라가 가진 CinemachineVolume도 전환시 똑같은 방식으로 블랜딩
    // 타임라인과 동시 적용시 타임라인 우선순위로 적용

    // 카메라 즉시 전환(CinemacineBrain 컴포넌트 defaultBlend 건드리는거)
    public void SwitchCamera_Cut(CinemachineVirtualCamera target)
    {
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        SwitchCameara(target);
    }

    // 카메라 선형적 전환
    public void SwitchCamera_Easy_InOut(CinemachineVirtualCamera target, float time)
    {
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        cinemachineBrain.m_DefaultBlend.m_Time = time;
        SwitchCameara(target);
    }

    // 카메라 기존 설정 사용해서 전환
    public void SwitchCameara(CinemachineVirtualCamera target)
    {
        if (mainCam != null)
            mainCam.m_Priority = 0;
        mainCam = target;
        mainCam.m_Priority = 12;
    }



    #endregion


    #region SetTarget
    // 가급적이면 카메라 돌려쓰지 말고 컷씬 마다 카메라 추가해서 사용
    public void SetLookTarget(Transform target)
    {
        mainCam.GetComponent<CinemachineVirtualCamera>().m_LookAt = target;
    }

    public void SetFollowTarget(Transform target)
    {
        mainCam.GetComponent<CinemachineVirtualCamera>().m_Follow = target;
    }

    public void SetPlayerCamera()
    {

        if (mainCam != null)
            mainCam.m_Priority = 0;
        mainCam = playerCam;
        mainCam.m_Priority = 11;
    }
    #endregion

    // 수정 예정

    public void SetLookTargetGroup(GameObject tartget)
    {

    }
    
}
