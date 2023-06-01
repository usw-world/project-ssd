using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOpacityChange : MonoBehaviour
{
    public GameObject rayPrefab;
    private GameObject ray;
    private float distance;
    private void Start()
    {
        ray = Instantiate(rayPrefab, transform);
        //direction = (playerPos - transform.position).normalized;
    }
    private void Update()
    {
        ray.transform.position = transform.position;
        ray.transform.rotation = transform.rotation;
        var rayCollider = ray.GetComponent<CapsuleCollider>();
        distance = CameraManager.instance.playerCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        rayCollider.height = distance;
        rayCollider.center = new Vector3(0, 0, distance / 2);






        //Debug.DrawRay(transform.position, transform.forward * distance,  Color.cyan);
        //RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, distance, 1 << 14);
        //foreach (RaycastHit hit in hits)
        //{
        //    var color = hit.transform.GetComponent<MeshRenderer>().material.color;
        //    color = new Color(color.r, color.g, color.b, 40);
        //    hit.transform.GetComponent<MeshRenderer>().material.color = color;
        //    Debug.Log("Color Change");
        //}
    }
}
