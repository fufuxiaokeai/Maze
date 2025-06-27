// UnlockSphere.cs
using UnityEngine;

public class UnlockSphere : MonoBehaviour
{
    public PrimMazeGenerator mazeGenerator;
    public float rotateSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PublicVar.IsDelect_Prim = true;
            mazeGenerator.UnlockExit();
            Destroy(gameObject);
        }
    }
}