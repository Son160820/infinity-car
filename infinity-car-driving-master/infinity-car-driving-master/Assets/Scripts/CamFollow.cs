using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;
    public Camera cam;


    void LateUpdate()
    {
        cam.transform.position = player.transform.position + new Vector3(0f, 5f, -4.5f);
    }
}
