using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    public AudioSource audioSource;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioSource.Play();
        }
    }

}
