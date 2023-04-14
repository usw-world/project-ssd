using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCamera : MonoBehaviour
{
    public virtual void EndCutScene()
    {
        CameraManager.CM.EndCutScene(gameObject);
    }
}
