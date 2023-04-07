using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkQPlayer : NetworkBehaviour
{
    float speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKey(KeyCode.W))
            transform.Translate(transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(-transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(transform.right * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(-transform.right * speed * Time.deltaTime);
    }
}
