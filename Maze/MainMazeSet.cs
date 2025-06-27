using System.Collections;
using UnityEngine;

public class MainMazeSet : MonoBehaviour
{
    public Transform DFS;
    public Transform Prim;
    public Transform Kruskal;
    public Transform player; // ���볡���е���Ҷ���

    IEnumerator Start()
    {
        // ��ʼ��������������
        DFS.gameObject.SetActive(false);
        Prim.gameObject.SetActive(false);
        Kruskal.gameObject.SetActive(false);

        // �ȴ�һ֡ȷ�����״̬����
        yield return null;

        if (PublicVar.IsOnline)
        {
            Random.InitState(PublicVar.mazeSeed);
            Debug.Log($"ʹ�����������Թ�: {PublicVar.mazeSeed}");
        }

        Transform activeGenerator = SelectGenerator();

        if (activeGenerator != null)
        {
            // �������������ȴ���ʼ��
            activeGenerator.gameObject.SetActive(true);
            yield return StartCoroutine(WaitForMazeGeneration(activeGenerator));

            // ��ȡ���λ��
            Vector3 startPos = GetStartPositionFromGenerator(activeGenerator);

            // �������λ��
            if (player != null)
            {
                player.position = startPos;
                Debug.Log($"����ѷ��õ���{startPos}");
            }
        }
    }

    Transform SelectGenerator()
    {
        switch (PublicVar.difficulty)
        {
            case "��": 
                PublicVar.algorithm = "DFS";
                return DFS;
            case "��ͨ": 
                PublicVar.algorithm = "Prim";
                return Prim;
            case "����":
            case "����": 
                PublicVar.algorithm = "Kruskal";
                return Kruskal;
            default: 
                return DFS;
        }
    }

    IEnumerator WaitForMazeGeneration(Transform generator)
    {
        // �ȴ����3�루�����Թ���С������
        float timeout = 3f;
        float startTime = Time.time;

        while (Time.time - startTime < timeout)
        {
            // ͨ���������ǽ���Ƿ��������жϳ�ʼ�����
            if (generator.childCount > 0)
                break;

            yield return null;
        }
    }

    Vector3 GetStartPositionFromGenerator(Transform generator)
    {
        // ͨ��������͵��ö�Ӧ����
        if (generator.GetComponent<DFSMazeGenerator>() != null)
            return generator.GetComponent<DFSMazeGenerator>().GetStartPosition();

        if (generator.GetComponent<PrimMazeGenerator>() != null)
            return generator.GetComponent<PrimMazeGenerator>().GetStartPosition();

        if (generator.GetComponent<KruskalMazeGenerator>() != null)
            return generator.GetComponent<KruskalMazeGenerator>().GetStartPosition();

        Debug.LogError("δ�ҵ���Ч���Թ����������");
        return Vector3.zero;
    }
}