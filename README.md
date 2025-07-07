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
### UnlockSphere.cs
对Prim算法所生成的迷宫进行开门处理。
### WallInfo.cs
存储迷宫墙壁信息的类。

## online
### ChatClient.cs
用于处理聊天系统的代码文件。

## online mode
### OnlineUISet.cs
用于处理聊天时的UI界面逻辑。
### RoomSelect.cs
用于处理房间逻辑的代码。

## Player
### Click.cs
处理玩家点击触发点击音效。
### collsion.cs
累计玩家碰墙多少次。
### GenshinCharacter.cs
这是我模仿《原神》的玩家移动逻辑代码。
### MouseControl.cs
这是用于隐藏鼠标和显示鼠标的代码。
### moving.cs
用于播放玩家移动时的动画代码
### Other_players.cs
这是一个处理其他玩家的方位和动画代码
### PlayerManager.cs
这是联机时，玩家与玩家之间的处理。
### SharedVariables.cs
这是一个存储玩家体力的静态类。




