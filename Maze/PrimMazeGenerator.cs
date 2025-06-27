using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class PrimMazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public Transform playerTransform;
    public Transform exitPrefab;

    [Header("Maze Settings")]
    private int width = PublicVar.Wmaze;
    private int height = PublicVar.Hmaze;

    public float cellSize = 3f;
    public float wallHeight = 5f;
    public float groundOffset = -0.1f;
    [Header("位置设置")]
    public Vector3 mazeOffset = Vector3.zero;

    [Header("解锁道具")]
    public GameObject unlockSpherePrefab;

    [Header("音乐处理")]
    public AudioSource audioSource;
    public AudioClip OpenDoorMusic;//开门音乐

    private Cell[,] maze;
    private Vector2Int startPos = new Vector2Int(5, 5);
    private Vector2Int exitPos;

    private float duration = 0.9f;
    private float elapsedTime = 0f;

    private class Cell
    {
        public bool visited;
        public bool[] walls = new bool[4]; // 0:North, 1:East, 2:South, 3:West
    }

    private class Frontier
    {
        public Vector2Int cell;
        public Vector2Int parent;

        public Frontier(Vector2Int c, Vector2Int p)
        {
            cell = c;
            parent = p;
        }
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
        PositionPlayer();

        exitPrefab.position = new Vector3(706.8735f, 0, 2576);

        Vector3 spherePos = new Vector3(
            (width / 2) * cellSize,
            1f,
            (height / 2) * cellSize
        ) + mazeOffset;

        GameObject sphere = Instantiate(unlockSpherePrefab, spherePos, Quaternion.identity);
        sphere.GetComponent<UnlockSphere>().mazeGenerator = this;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (PublicVar.IsDelect_Prim)
        {
            SpawnExit();
            audioSource.clip = OpenDoorMusic;
            audioSource.Play();
            while (elapsedTime >= duration) break;
            PublicVar.IsDelect_Prim = false;
        }
    }

    // 新增解锁方法
    public void UnlockExit()
    {
        // 更新迷宫数据
        maze[exitPos.x, exitPos.y].walls[0] = false;
        maze[exitPos.x, exitPos.y].walls[1] = false;

        // 销毁对应的墙壁
        foreach (WallInfo wall in GetComponentsInChildren<WallInfo>())
        {
            if (wall.cellX == exitPos.x && wall.cellY == exitPos.y)
            {
                if (wall.direction == 0 || wall.direction == 1)
                {
                    Destroy(wall.gameObject);
                }
            }
        }

        // 重新打开出口（如果需要可以添加额外效果）
        Debug.Log("出口已解锁！");
    }

    void ValidateParameters()
    {
        width = Mathf.Clamp(width, 5, 100);
        height = Mathf.Clamp(height, 5, 100);
        startPos = new Vector2Int(0, 0);
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
        List<Frontier> frontiers = new List<Frontier>();

        // 随机选择起始点
        Vector2Int startCell = startPos;

        maze[startCell.x, startCell.y].visited = true;
        AddFrontiers(startCell, frontiers);

        while (frontiers.Count > 0)
        {
            // 随机选择边缘单元格
            int index = Random.Range(0, frontiers.Count);
            Frontier selected = frontiers[index];
            frontiers.RemoveAt(index);

            if (maze[selected.cell.x, selected.cell.y].visited) continue;

            // 连接单元格
            maze[selected.cell.x, selected.cell.y].visited = true;
            RemoveWalls(selected.parent, selected.cell);

            // 添加新边缘
            AddFrontiers(selected.cell, frontiers);
        }
    }

    void AddFrontiers(Vector2Int cell, List<Frontier> frontiers)
    {
        CheckFrontier(cell, new Vector2Int(cell.x, cell.y + 1), frontiers); // North
        CheckFrontier(cell, new Vector2Int(cell.x + 1, cell.y), frontiers); // East
        CheckFrontier(cell, new Vector2Int(cell.x, cell.y - 1), frontiers); // South
        CheckFrontier(cell, new Vector2Int(cell.x - 1, cell.y), frontiers); // West
    }

    void CheckFrontier(Vector2Int parent, Vector2Int checkCell, List<Frontier> frontiers)
    {
        if (IsValidCell(checkCell) && !maze[checkCell.x, checkCell.y].visited)
        {
            if (!frontiers.Exists(f => f.cell == checkCell))
            {
                frontiers.Add(new Frontier(checkCell, parent));
            }
        }
    }

    bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    void RemoveWalls(Vector2Int a, Vector2Int b)
    {
        // North/South
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
        // East/West
        else
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
        Vector3 groundPosition = new Vector3(
            (width * cellSize) / 2f,
            groundOffset,
            (height * cellSize) / 2f
        ) + mazeOffset;

        ground.transform.position = groundPosition;
        ground.transform.localScale = new Vector3(
            width * cellSize * 1.25f,
            1f,
            height * cellSize * 1.25f
        );
    }

    void CreateWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 basePos = new Vector3(x * cellSize, 0, y * cellSize) + mazeOffset;

                // North Wall
                if (maze[x, y].walls[0])
                    CreateWall(basePos + new Vector3(0, wallHeight / 2, cellSize / 2),
                             new Vector3(cellSize, wallHeight, 0.1f), x, y, 0);

                // East Wall
                if (maze[x, y].walls[1])
                    CreateWall(basePos + new Vector3(cellSize / 2, wallHeight / 2, 0),
                             new Vector3(0.1f, wallHeight, cellSize), x, y, 1);

                // South Wall
                if (maze[x, y].walls[2])
                    CreateWall(basePos + new Vector3(0, wallHeight / 2, -cellSize / 2),
                             new Vector3(cellSize, wallHeight, 0.1f), x, y, 2);

                // West Wall
                if (maze[x, y].walls[3])
                    CreateWall(basePos + new Vector3(-cellSize / 2, wallHeight / 2, 0),
                             new Vector3(0.1f, wallHeight, cellSize), x, y, 3);
            }
        }
    }

    void CreateWall(Vector3 position, Vector3 scale, int x, int y, int dir)
    {
        GameObject wall = Instantiate(wallPrefab, transform);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        // 添加墙壁信息组件
        WallInfo info = wall.AddComponent<WallInfo>();
        info.cellX = x;
        info.cellY = y;
        info.direction = dir;
    }

    void PositionPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 spawnPos = new Vector3(
                startPos.x * cellSize + cellSize / 2,
                groundOffset + 0.5f,
                startPos.y * cellSize + cellSize / 2
            ) + mazeOffset;

            playerTransform.position = spawnPos;
            playerTransform.rotation = Quaternion.Euler(0, 45f, 0);
        }
    }

    void SpawnExit()
    {
        Vector3 exitPosition = new Vector3(
            exitPos.x * cellSize + cellSize / 2,
            groundOffset + 0.5f,
            exitPos.y * cellSize + cellSize / 2
        ) + mazeOffset;

        exitPrefab.position = exitPosition + new Vector3(0, 0.1f, -2f);
        if (exitPrefab.TryGetComponent(out Collider col))
        {
            col.isTrigger = true;
            Debug.Log("成为触发器了！");
        }
    }

    public Vector3 GetStartPosition()
    {
        Vector3 startCell = new Vector3(
            startPos.x * cellSize + cellSize / 2f,
            groundOffset + 0.5f,
            startPos.y * cellSize + cellSize / 2f
        );
        return startCell + mazeOffset; // 包含全局偏移
    }
}