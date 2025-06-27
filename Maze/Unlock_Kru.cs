using UnityEngine;

public class Unlock_Kru : MonoBehaviour
{
    public KruskalMazeGenerator mazeGenerator; // 修改类型
    public float rotateSpeed = 180f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PublicVar.IsDelect_Kru = true;
            mazeGenerator.UnlockExit();
            Destroy(gameObject);
        }
    }
}