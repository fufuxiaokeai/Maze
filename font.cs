using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class font : MonoBehaviour
{
    public Transform player;
    private Camera Cam;

    private void Start()
    {
        if (Cam == null)
        {
            Cam = GameObject.Find("Lumine FBX/Main Camera")?.GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (PublicVar.IsPass == 1)
        {
            var camera = GameObject.Find("Camera(Clone)")?.GetComponent<Camera>();
            if (camera!=null)
            {
                Cam = GameObject.Find("Camera(Clone)")?.GetComponent<Camera>();
                PublicVar.IsPass = 0;
            }
            else
            {
                print("Camera not found, using default camera.");
            }
        }
        this.transform.position = player.position + new Vector3(0, 2f, 0);
        this.transform.rotation = Cam.transform.rotation;
    }
}
