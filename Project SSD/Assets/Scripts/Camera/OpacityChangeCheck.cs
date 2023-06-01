using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityChangeCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            var color = other.gameObject.GetComponent<MeshRenderer>().material.color;
            var editColor = new Color(color.r, color.g, color.b, 0.5f);
            other.gameObject.GetComponent<MeshRenderer>().material.color = editColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            var color = other.gameObject.GetComponent<MeshRenderer>().material.color;
            var editColor = new Color(color.r, color.g, color.b, 1);
            other.gameObject.GetComponent<MeshRenderer>().material.color = editColor;
            Debug.Log("Out");
        }
    }
}
