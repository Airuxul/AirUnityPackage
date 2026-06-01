# Air Unity Packages

Unity UPM 元仓库：**配置 / 工具 / 文档** 与 **`packages/` 子模块** 分离。

**作者：** [airuxul](https://github.com/airuxul)

## 目录一览

```text
CustomPackages/
├── install-to-unity.bat      # 写入 Unity manifest
├── init-submodules.bat         # 拉取 packages/ 下 Submodule
├── packages/                   # 仅子模块（UPM 源码）
├── config/registry.json        # 包注册表
├── tools/                      # 脚本实现
└── docs/                       # 说明文档
```

详见 [docs/STRUCTURE.md](docs/STRUCTURE.md)。

## 快速开始

```bat
cd C:\Project\GameDemo\CustomPackages
init-submodules.bat
install-to-unity.bat C:\Project\GameDemo
```

| 命令 | 作用 |
|------|------|
| `init-submodules.bat` | `git submodule update --init --recursive` |
| `install-to-unity.bat [工程路径]` | 将 registry 中 `installDefault` 的包写入 manifest（已存在则跳过） |

## 包列表

[docs/PACKAGES.md](docs/PACKAGES.md) · 架构约束：[docs/CONSTRAINTS.md](docs/CONSTRAINTS.md)

## 手动 manifest

[config/manifest.example.json](config/manifest.example.json)

## 更多

- 工作流：[docs/WORKFLOW.md](docs/WORKFLOW.md)
- Submodule：[docs/SUBMODULE-SETUP.md](docs/SUBMODULE-SETUP.md)
