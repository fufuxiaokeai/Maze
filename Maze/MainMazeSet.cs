using System.Collections;
using UnityEngine;

public class MainMazeSet : MonoBehaviour
{
    public Transform DFS;
    public Transform Prim;
    public Transform Kruskal;
    public Transform player; // 拖入场景中的玩家对象

    IEnumerator Start()
    {
        // 初始禁用所有生成器
        DFS.gameObject.SetActive(false);
        Prim.gameObject.SetActive(false);
        Kruskal.gameObject.SetActive(false);

        // 等待一帧确保组件状态重置
        yield return null;

        if (PublicVar.IsOnline)
        {
            Random.InitState(PublicVar.mazeSeed);
            Debug.Log($"使用种子生成迷宫: {PublicVar.mazeSeed}");
        }

        Transform activeGenerator = SelectGenerator();

        if (activeGenerator != null)
        {
            // 激活生成器并等待初始化
            activeGenerator.gameObject.SetActive(true);
            yield return StartCoroutine(WaitForMazeGeneration(activeGenerator));

            // 获取起点位置
            Vector3 startPos = GetStartPositionFromGenerator(activeGenerator);

            // 更新玩家位置
            if (player != null)
            {
                player.position = startPos;
                Debug.Log($"玩家已放置到：{startPos}");
            }
        }
    }

    Transform SelectGenerator()
    {
        switch (PublicVar.difficulty)
        {
            case "简单": 
                PublicVar.algorithm = "DFS";
                return DFS;
            case "普通": 
                PublicVar.algorithm = "Prim";
                return Prim;
            case "困难":
            case "地狱": 
                PublicVar.algorithm = "Kruskal";
                return Kruskal;
            default: 
                return DFS;
        }
    }

    IEnumerator WaitForMazeGeneration(Transform generator)
    {
        // 等待最多3秒（根据迷宫大小调整）
        float timeout = 3f;
        float startTime = Time.time;

        while (Time.time - startTime < timeout)
        {
            // 通过检查任意墙壁是否生成来判断初始化完成
            if (generator.childCount > 0)
                break;

            yield return null;
        }
    }

    Vector3 GetStartPositionFromGenerator(Transform generator)
    {
        // 通过组件类型调用对应方法
        if (generator.GetComponent<DFSMazeGenerator>() != null)
            return generator.GetComponent<DFSMazeGenerator>().GetStartPosition();

        if (generator.GetComponent<PrimMazeGenerator>() != null)
            return generator.GetComponent<PrimMazeGenerator>().GetStartPosition();

        if (generator.GetComponent<KruskalMazeGenerator>() != null)
            return generator.GetComponent<KruskalMazeGenerator>().GetStartPosition();

        Debug.LogError("未找到有效的迷宫生成器组件");
        return Vector3.zero;
    }
}