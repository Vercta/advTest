# advTest 用户文档 Ver. 0.1
你来到了“advTest”的发布页面：**advTest** 旨在实现[Unity中文课堂《勇士传说》教程](https://learn.u3d.cn/tutorial/2DAdventure)中的所有功能，最终用于提交“Hollow Knight Demo”的第二次工程任务(8.6)

## 关于本项目
这是一款基于 Unity 2D 的平台跳跃冒险游戏（类银河恶魔城），目前内容并未实现完整，支持键鼠&手柄控制，欢迎体验，欢迎批评！

### 构建工具
- Unity 23.2.20
- VScode2022 with Extensions:C# Dev Kit, Unity

## 开始体验
您可以`git clone`到本地，并使用Unity 23.2 进行测试体验，这样您也可以看到全部的项目细节与相关设计；您同样可以下载之后会进行发布的alpha发行版，会首先支持Windows平台。

### 克隆至本地
```sh
git clone https://github.com/Vercta/advTest.git
```
### 控制
> 支持键盘&手柄操作

| 键位 | 功能 |
| :---: | :---: |
| <kbd>AD<kbd> 或 左摇杆（水平方向）| 向左/右移动（跑步）|
|<kbd>S<kbd> 或左摇杆| 触发时下蹲 |
| <kbd>Space<kbd> 或 Button South (A) |  跳跃  |
| <kbd>J<kbd> 或 Button West (X)|  攻击  |
| <kbd>Shift<kbd> 或 Shoulder (肩键) | 长按同时移动触发步行 |
| <kbd>K<kbd> 或 Left Shoulder (左肩键) | 滑铲（带来无敌效果） |

## 功能开发进度
- [x] 素材切割导入与基本地图绘制
- [x] 支持键鼠&手柄控制
- [x] 玩家控制脚本的逻辑构建
- [x] 玩家生命周期内的完整动画
- [x] 基本的敌人逻辑实现（状态机）
- [x] 基础敌人的动画部分
- [x] HUD（玩家状态栏）
- [x] 玩家特殊动作
- [x] 摄像机跟随
- [x] 摄像机高级跟随
- [ ] 音效管理系统
- [ ] 场景切换与管理
- [ ] 数据保存
- [ ] 数据加载
- [ ] 游戏结束逻辑
- [ ] 游戏的多端适配

## Interacts
若您有任何建议，请fork本项目并创建pull request, 或是直接开启一个 issue （推荐）

## 致谢
感谢所有点击进入浏览本项目；提出过宝贵意见的你们。