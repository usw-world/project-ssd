using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCamera : MonoBehaviour
{
    public virtual void EEEEEEEEEndCutScene()
    {
        print("dddd");
        CameraManager.CM.EndCutScene(gameObject);
    }
}
