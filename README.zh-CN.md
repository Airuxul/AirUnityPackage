# Air Unity Packages（用户文档）

[English](README.md)

**最后更新：** 2026-06-04 · **范围：** 用户文档（中文）

Unity UPM **元仓库**：在 `packages/` 维护子模块指针，提供安装工具，Agent 规范见 `docs/`。

## 目录结构

```text
CustomPackages/
├── packages/              # Git 子模块（UPM 源码）
├── config/                # 注册表与 Tag（工具用；见 docs/AGENTS.md）
├── tools/                 # 安装、文档校验、Unity 编译辅助脚本
├── docs/                  # Agent 文档（英文）
├── TODO.zh-CN.md          # 跨包优化待办（中文）
├── init-submodules.bat
├── install-to-unity.bat
├── README.md              # 英文用户文档
└── README.zh-CN.md        # 本文件（中文）
```

## 快速开始

```bat
cd C:\Project\GameDemo\CustomPackages
init-submodules.bat
install-to-unity.bat C:\Project\GameDemo
```

`install-to-unity.bat` 会根据 `config/registry.json` 中 `installDefault` 的包，向 Unity 工程 `Packages/manifest.json` 写入 `file:` 依赖；已存在的项不会被覆盖。

## 常用命令

| 命令 | 作用 |
|------|------|
| `init-submodules.bat` | 对 `packages/` 执行 `git submodule sync` 与 `update --init --recursive` |
| `install-to-unity.bat [Unity工程路径]` | 将默认包安装到 Unity manifest |

## 手动编辑 manifest

若需手写 Unity 依赖，可参考 [config/manifest.example.json](config/manifest.example.json) 中的 `file:` 路径示例（请按本机路径调整）。

## 工具脚本（贡献者）

| 脚本 | 作用 |
|------|------|
| [tools/install-packages.ps1](tools/install-packages.ps1) | `install-to-unity.bat` 调用的安装逻辑 |
| [tools/validate-docs.ps1](tools/validate-docs.ps1) | 提交前文档一致性检查 |
| [tools/install-git-hooks.ps1](tools/install-git-hooks.ps1) | 一次性启用 `.githooks/pre-commit` |
| [tools/unity-compile-loop.ps1](tools/unity-compile-loop.ps1) | 通过 `unity-cmd` 做编译与控制台检查 |

说明见 [docs/TOOLS.md](docs/TOOLS.md)。

## 常见问题

| 现象 | 处理建议 |
|------|----------|
| Submodule 更新失败 | 重跑 `init-submodules.bat`；检查 `.gitmodules`、网络与 Git 凭据 |
| 包目录不存在 | 子模块未初始化 — 先执行 `init-submodules.bat` |
| `install-to-unity.bat` 失败 | 传入正确的 Unity 工程根目录（含 `Assets/` 与 `Packages/manifest.json`） |
| 提交被 doc hook 拒绝 | 改用户文档时需同时更新 `README.md` 与 `README.zh-CN.md`；见 [docs/DOC_GOVERNANCE.md](docs/DOC_GOVERNANCE.md) |

## 优化待办

跨包后续优化（仅现有功能）：[TODO.zh-CN.md](TODO.zh-CN.md)。Agent 统筹（英文）：[docs/TODO_ROADMAP.md](docs/TODO_ROADMAP.md)。各子模块另有 `TODO.zh-CN.md` / `docs/TODO.md`。

## Agent 文档

面向 Agent 的规则与技术说明在 [docs/](docs/)（英文），入口：[docs/AGENTS.md](docs/AGENTS.md)。

## 用户文档语言策略

- 英文入口：`README.md`
- 中文入口：本文件 `README.zh-CN.md`
- 任何面向用户的文档变更必须**中英文同步**更新
