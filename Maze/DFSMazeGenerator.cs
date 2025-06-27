using UnityEngine;
using System.Collections.Generic;

public class DFSMazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public Transform playerTransform; // 拖拽场景中的玩家对象到此
    public Transform exitPrefab;

    [Header("Maze Settings")]
    private int width = PublicVar.Wmaze; // 迷宫宽度
    private int height = PublicVar.Hmaze; // 迷宫高度

    public float cellSize = 2f;
    public float wallHeight = 3f;
    public float groundOffset = -0.1f;

    [Header("位置设置")]
    public Vector3 mazeOffset = Vector3.zero; // 新增全局偏移参数

    private Cell[,] maze;
    private Vector2Int startPos = new Vector2Int(5, 5);
    private Vector2Int exitPos;
    public Vector3 playerSpawnOffset = new Vector3(0, 0.5f, 0); // 新增玩家位置微调

    private class Cell
    {
        public bool visited;
        public bool[] walls = new bool[4]; // 0:North, 1:East, 2:South, 3:West
    }

    void Start()
    {
        if (PublicVar.IsOnline)
        {
            Random.InitState(PublicVar.mazeSeed);
        }
        ValidateParameters();
        InitializeMaze();
        GenerateMaze();
        CreateGround();
        CreateWalls();
        CreateOpenings();
        SpawnPlayerAndExit();
        PositionPlayer(); // 新增玩家定位方法
    }

    void ValidateParameters()
    {
        width = Mathf.Clamp(width, 5, 50);
        height = Mathf.Clamp(height, 5, 50);
        exitPos = new Vector2Int(width - 1, height - 2);
    }

    void InitializeMaze()
    {
        maze = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = new Cell { visited = false, walls = new[] { true, true, true, true } };
    }

    void GenerateMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = startPos;
        maze[current.x, current.y].visited = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();
            var neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWalls(current, chosen);
                maze[chosen.x, chosen.y].visited = true;
                stack.Push(chosen);
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (cell.y < height - 1 && !maze[cell.x, cell.y + 1].visited) neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        if (cell.x < width - 1 && !maze[cell.x + 1, cell.y].visited) neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0 && !maze[cell.x, cell.y - 1].visited) neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.x > 0 && !maze[cell.x - 1, cell.y].visited) neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        return neighbors;
    }

    void RemoveWalls(Vector2Int a, Vector2Int b)
    {
        // Horizontal neighbors
        if (a.x == b.x)
        {
            if (a.y < b.y) // North
            {
                maze[a.x, a.y].walls[0] = false;
                maze[b.x, b.y].walls[2] = false;
            }
            else // South
            {
                maze[a.x, a.y].walls[2] = false;
                maze[b.x, b.y].walls[0] = false;
            }
        }
        else // Vertical neighbors
        {
            if (a.x < b.x) // East
            {
                maze[a.x, a.y].walls[1] = false;
                maze[b.x, b.y].walls[3] = false;
            }
            else // West
            {
                maze[a.x, a.y].walls[3] = false;
                maze[b.x, b.y].walls[1] = false;
            }
        }
    }

    void CreateGround()
    {
        GameObject ground = Instantiate(groundPrefab);

        // 正确计算迷宫中心点
        float mazeWidth = width * cellSize;
        float mazeHeight = height * cellSize;

        Vector3 groundPosition = new Vector3(
            mazeWidth / 2f,
            groundOffset,
            mazeHeight / 2f
        )+ mazeOffset;

        // 正确计算平面缩放（Unity平面默认1单位=10米）
        Vector3 groundScale = new Vector3(
            mazeWidth*1.25f,
            1f,
            mazeHeight*1.25f 
        );

        ground.transform.position = groundPosition;
        ground.transform.localScale = groundScale;

        // 调试用可视化（可选）
        Debug.Log($"地面尺寸：{mazeWidth}x{mazeHeight}米，位置：{groundPosition}");
    }

    void CreateWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 basePos = new Vector3(x * cellSize, 0, y * cellSize);

                // North Wall (跳过出口北墙)
                if (maze[x, y].walls[0] && !IsExitCell(x, y))
                    CreateWall(basePos + new Vector3(0, wallHeight / 2, cellSize / 2),
                             new Vector3(cellSize, wallHeight, 0.1f));

                // East Wall (跳过出口东墙)
                if (maze[x, y].walls[1] && !IsExitCell(x, y))
                    CreateWall(basePos + new Vector3(cellSize / 2, wallHeight / 2, 0),
                             new Vector3(0.1f, wallHeight, cellSize));

                // South Wall (跳过起点南墙)
                if (maze[x, y].walls[2] && !IsStartCell(x, y))
                    CreateWall(basePos + new Vector3(0, wallHeight / 2, -cellSize / 2),
                             new Vector3(cellSize, wallHeight, 0.1f));

                // West Wall (跳过起点西墙)
                if (maze[x, y].walls[3] && !IsStartCell(x, y))
                    CreateWall(basePos + new Vector3(-cellSize / 2, wallHeight / 2, 0),
                             new Vector3(0.1f, wallHeight, cellSize));
            }
        }
    }

    void CreateOpenings()
    {
        // 强制打开起点出口
        maze[startPos.x, startPos.y].walls[2] = false; // 打开起点南墙
        maze[startPos.x, startPos.y].walls[3] = false; // 打开起点西墙

        // 强制打开终点出口
        maze[exitPos.x, exitPos.y].walls[0] = false; // 打开终点北墙
        maze[exitPos.x, exitPos.y].walls[1] = false; // 打开终点东墙
    }

    bool IsStartCell(int x, int y) => x == startPos.x && y == startPos.y;
    bool IsExitCell(int x, int y) => x == exitPos.x && y == exitPos.y;

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, transform);
        wall.transform.position = position + mazeOffset;
        wall.transform.localScale = scale;
    }

    void PositionPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 spawnPos = new Vector3(60, 0, 60) + playerSpawnOffset;
            playerTransform.position = spawnPos;

            // 重置玩家旋转（可选）
            playerTransform.rotation = Quaternion.identity;

            // 确保玩家启用
            playerTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("未指定玩家对象！请拖拽场景中的玩家到Player Transform字段");
        }
    }

    Vector3 CalculateCellCenter(Vector2Int cell)
    {
        return new Vector3(
            cell.x * cellSize + cellSize / 2,
            groundOffset,
            cell.y * cellSize + cellSize / 2
        ) + mazeOffset;
    }

    public Vector3 GetStartPosition()
    {
        Vector3 startCell = new Vector3(60, 0, 60) + playerSpawnOffset;
        return startCell;
    }

    void SpawnPlayerAndExit()
    {
        Vector3 exitPosition = CalculateCellCenter(exitPos) + new Vector3(0, 0.56f, -2f);
        exitPrefab.position = exitPosition;

        Collider exitCollider = exitPrefab.GetComponent<Collider>();
        if (exitCollider != null)
        {
            exitCollider.isTrigger = true;
            Debug.Log("出口预制体已设置为触发器！");
        }
        else
        {
            Debug.LogWarning("出口预制体缺少碰撞体组件");
        }
    }
}