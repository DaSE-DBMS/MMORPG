# Welcome

这个项目主要来源于Unity Asset Store上的[3d-game-kit](https://assetstore.unity.com/packages/essentials/tutorial-projects/3d-game-kit-115747)。主要变更是为其增加了个后端服务器，使它变成了一个网络游戏，现阶段功能还不完整。

# How to Start

#### 安装Unity3d

#### 安装Visual Studio

#### 安装依赖

Backend用到了GeometRi，一个用于空间计算的library。进入项目目录:

```
dotnet add backend package GeometRi --version 1.3.5.3
```
#### [下载资源文件](x)  

提取码
```
```

资源文件中包括字体，3D模型，图片，音乐等资源

解压资源文件到 *MMORPG\Assets\3DGamekit* 目录下

#### 使用VS Build Backend

#### 使用Unity3d Editor执行, 或build成APP执行


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
