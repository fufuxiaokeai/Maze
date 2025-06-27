using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    private int coll = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            coll++;
            PublicVar.coll = coll;
        }
    }
}
