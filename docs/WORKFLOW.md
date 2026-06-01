# 多仓库工作流

元仓库只维护 **config / tools / docs** 与 **packages/ Submodule 指针**；UPM 源码在子仓库。

目录说明：[STRUCTURE.md](STRUCTURE.md)

## 目录角色

| 路径 | 职责 |
|------|------|
| 根目录 | 入口 *.bat |
| config/registry.json | 包索引、installDefault、远程 URL |
| 	ools/ | 安装、同步子模块 |
| packages/ | Submodule（仅源码） |
| docs/ | 说明文档 |

## 日常流程

### 1. 克隆

`ash
git clone --recurse-submodules https://github.com/Airuxul/AirUnityPackage.git CustomPackages
cd CustomPackages
`

或 init-submodules.bat。

### 2. 安装到 Unity

`at
install-to-unity.bat C:\Project\GameDemo
`

将 
egistry.json 中 installDefault: true 的包写入 manifest。已存在的依赖不会覆盖或重复添加。

### 3. 改包并发布

在各 packages/<name> 子仓内 commit / tag / push，再更新元仓 submodule 指针。

本地开发用 install-to-unity.bat（ile:）。
