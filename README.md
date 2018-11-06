# Welcome

这个项目来源于Unity Asset Store上的[3d-game-kit](https://assetstore.unity.com/packages/essentials/tutorial-projects/3d-game-kit-115747)。主要变更是为其增加了个后端服务器，使它变成了一个网络游戏，现阶段功能还不完整。

# How to Start

#### [安装Unity3d](https://store.unity.com/cn)

#### [安装Visual Studio](https://visualstudio.microsoft.com/)

#### [下载资源](https://share.weiyun.com/5nofemx)

密码：wkwk6g

资源文件中包括字体，3D模型，图片，音乐等资源

#### 目录结构
```
MMORPG
  |-- Backend                                            服务端
        |-- Backend.sln                                  服务端解决方案文件
        |-- Backend.csproj
        |-- ....
  |-- Frontend                                           客户端, Unity工程，可以用Unity Editor打开
        |-- Assets
            |-- 3DGamekit
                  |-- Art                                资源文件，包括字体，3D模型，图片，音乐等资源
                  |-- Scripts                            C#代码
                  |-- ....
            |-- BEAssets                                 从客户端导出的一些信息，供后端使用， 每个场景的阻档点，精灵出生点坐标等
            |-- ....
        |-- Library
        |-- obj                                          客户端解决方案文件
        |-- Packages
        |-- Frontend.sln                                 
        |-- Assembly-CSharp-Editor.csproj
        |-- Assembly-CSharp.csproj
        |-- NavMeshComponents.csproj
        |-- SimpleSFXRuntime.csproj
        |-- Skybox3DRuntime.csproj
        |-- WorldBuildingRuntime.csproj
        |-- ....
  |-- MMORPG.sln  
  |-- ....

```
#### 解压资源
解压资源文件Art.zip到 *MMORPG\Frontend\Assets\3DGamekit* 目录下


#### 使用Unity Editor导入项目

启动Unity Editor， PROJECT --> OPEN --> 选择项目目录MMORPG\Frontend  

Unity Editor会在MMORPG\Frontend文件夹下创建Library文件夹，存放依赖的库文件，也会重新导入资源文件

#### 安装依赖库

Backend用到了[GeometRi](https://github.com/RiSearcher/GeometRi.CSharp)，一个用于空间计算的library。进入项目目录:

```
dotnet add backend package GeometRi --version 1.3.5.3
```

#### 使用VS Build Backend

#### 启动Backend

- 更改配置文件backend.conf，<assetPath>改成自己的配置
（在MMORPG/Frontend/Assets/assets下，这个文件夹存的是从客户端导出的一些信息, 每个场景的阻档点，精灵出生点坐标等）
- 打开MMORPG.sln,发布bakcend项目（配置中将路径修改为自己的配置）
- 将backend设为启动项目，在VS中启动Backend.exe，命令行参数为配置文件路径
```
backend.exe [path_to_backend.conf]
```
backend.exe运行大致如下：
>Backend start up and waiting for a connection on port 7777...

#### 使用Unity3d Editor执行, 或build成APP执行
- 选择场景：打开scene-level1/...
- 点击上方小三角，进入调试

#### 人物可以wsad 移动，左键点击进行攻击


# __Roadmap__

##### __Asset导出__
  - [x] Navmesh
  - [x] 出生点坐标
  - [x] Backend配置文件格式

##### __网络__
  - [x] 通信接口
  - [x] 同屏全场景广播
  - [ ] GRID广播

##### __AI__
  - [x] A\*寻路
    - [x] 空间索引

##### __登录和注册__
  - [ ] UI
  - [ ] 登录
  - [ ] 注册

##### __商城__
  - [x] UI
  - [ ] 交易

##### __聊天__
  - [x] UI
  - [ ] 聊天逻辑

##### __主角__
  - [x] 走路
  - [x] 攻击精灵
  - [ ] 攻击其它人
  - [x] 跳跃
  - [ ] 受伤
  - [x] 出生
  - [ ] 死亡
  - [x] 装备
  - [ ] 背包
  
##### __精灵__
  - [ ] 走路
  - [ ] 攻击
  - [ ] 受伤
  - [ ] 出生
  - [ ] 死亡

##### __物品__
  - [ ] 可捡取物品
  - [ ] 可破坏物品
  - [ ] 机关
  - [ ] 装备

##### __退出游戏__
  - [ ] Elegent Exit
