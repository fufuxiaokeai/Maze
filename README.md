# Maze
这个迷宫游戏是我在大一时期自主完成的迷宫游戏，可以支持联机，但是只限局域网（由于文件大小受限，所以本次只上传代码文件）
# 文件夹说明
Camre：用于Unity相机的控制
Maze：用于迷宫的生成与控制
online：用于聊天的代码
online mode：联机模式的处理
Player：玩家代码（包括其他玩家）
UI：UI主键的控制

# 对应代码文件说明

## Camre
### BigMap.cs
管理大地图的相机代码，使用时确保该相机是玩家的子物体
### CameraController.cs
用于控制主相机移动逻辑的代码
### SetCamera.cs
用于处理开始画面的运镜代码
### VPCC.cs
用于处理联机模式下胜利后的观战模式的相机处理

## Maze
### DFSMazeGenerator.cs
由DFS算法进行的迷宫生成代码
### KruskalMazeGenerator.cs
由Kruskal算法进行的迷宫生成代码
### PrimMazeGenerator.cs
由Rrim算法进行的迷宫生成代码
### MainMazeSet.cs
用于控制上述的3个迷宫代码。将上述3个迷宫代码分别放在3个不同的空物体，再将这个代码（MainMazeSet.cs）放在新的空物体上，并将那3个空物体拖到新的物体上。
### Unlock_Kru.cs
对Kruskal算法所生成的迷宫进行开门处理。

