using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class KruskalMazeGenerator : MonoBehaviour
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

    // 并查集数据结构
    private class UnionFind
    {
        private int[] parent;
        private int[] rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
                parent[i] = i;
        }

        public int Find(int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent[x]);
            return parent[x];
        }

        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX != rootY)
            {
                if (rank[rootX] > rank[rootY])
                    parent[rootY] = rootX;
                else
                {
                    parent[rootX] = rootY;
                    if (rank[rootX] == rank[rootY])
                        rank[rootY]++;
                }
            }
        }
    }

    private class Edge
    {
        public Vector2Int cellA;
        public Vector2Int cellB;
        public int wallDirection; // 0:North, 1:East

        public Edge(Vector2Int a, Vector2Int b, int dir)
        {
            cellA = a;
            cellB = b;
            wallDirection = dir;
        }
    }

    private class Cell
    {
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
        PositionPlayer();

        exitPrefab.position = new Vector3(706.8735f, 0, 2576);

        Vector3 spherePos = new Vector3(
            (width * cellSize) / 2f,
            1f,
            (height * cellSize) / 2f
        ) + mazeOffset;

        GameObject sphere = Instantiate(unlockSpherePrefab, spherePos, Quaternion.identity);
        sphere.GetComponent<Unlock_Kru>().mazeGenerator = this;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (PublicVar.IsDelect_Kru)
        {
            SpawnExit();
            audioSource.clip = OpenDoorMusic;
            audioSource.Play();
            while (elapsedTime >= duration) break;
            PublicVar.IsDelect_Kru = false;
        }
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
                maze[x, y] = new Cell { walls = new[] { true, true, true, true } };
    }

    void GenerateMaze()
    {
        List<Edge> edges = new List<Edge>();

        // 生成所有可能的边
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y < height - 1) // North
                    edges.Add(new Edge(new Vector2Int(x, y), new Vector2Int(x, y + 1), 0));
                if (x < width - 1) // East
                    edges.Add(new Edge(new Vector2Int(x, y), new Vector2Int(x + 1, y), 1));
            }
        }

        // 随机排序边
        edges = edges.OrderBy(e => Random.value).ToList();

        UnionFind uf = new UnionFind(width * height);

        foreach (Edge edge in edges)
        {
            int indexA = edge.cellA.x + edge.cellA.y * width;
            int indexB = edge.cellB.x + edge.cellB.y * width;

            if (uf.Find(indexA) != uf.Find(indexB))
            {
                uf.Union(indexA, indexB);
                RemoveWall(edge);
            }
        }
    }

    void RemoveWall(Edge edge)
    {
        Vector2Int a = edge.cellA;
        Vector2Int b = edge.cellB;

        switch (edge.wallDirection)
        {
            case 0: // North
                maze[a.x, a.y].walls[0] = false;
                maze[b.x, b.y].walls[2] = false;
                break;
            case 1: // East
                maze[a.x, a.y].walls[1] = false;
                maze[b.x, b.y].walls[3] = false;
                break;
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

    // 新增解锁方法
    public void UnlockExit()
    {
        // 更新迷宫数据
        maze[exitPos.x, exitPos.y].walls[0] = false;
        maze[exitPos.x, exitPos.y].walls[1] = false;

        // 查找并销毁对应的墙壁
        foreach (Transform child in transform)
        {
            WallInfo info = child.GetComponent<WallInfo>();
            if (info != null && info.cellX == exitPos.x && info.cellY == exitPos.y)
            {
                if (info.direction == 0 || info.direction == 1)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        Debug.Log("出口已解锁！");
    }

    void CreateWall(Vector3 position, Vector3 scale, int x, int y, int dir)
    {
        GameObject wall = Instantiate(wallPrefab, transform);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        WallInfo info = wall.AddComponent<WallInfo>();
        info.cellX = x;
        info.cellY = y;
        info.direction = dir;
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
        }
    }
}