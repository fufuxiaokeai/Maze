using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
    public Camera cam;
    public float speed = 1f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.RotateAround(new Vector3(500, 0, 500), Vector3.up, speed);
    }
}
