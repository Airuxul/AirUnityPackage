# 仓库目录说明

```text
CustomPackages/
├── install-to-unity.bat
├── init-submodules.bat
├── packages/              # Git Submodule（UPM 源码）
├── config/registry.json
├── tools/
├── docs/
└── .cursor/rules/         # Cursor 项目规则（含 C# 规范）
```

## 原则

- **`packages/<folder>/`** 目录名与 `package.json` 的 `name` 一致。
- **元仓库根** 只放入口脚本、配置与说明；UPM 实现均在子仓库。

## 文档

| 文档 | 用途 |
|------|------|
| [C_SHARP_STANDARDS.md](C_SHARP_STANDARDS.md) | C# 规范与 `packages/` 目录结构（**改 C# 必读**） |
| [CONSTRAINTS.md](CONSTRAINTS.md) | 包职责与 v2 API |
| [ARCHITECTURE.md](ARCHITECTURE.md) | 依赖与归属 |

Markdown 使用 **UTF-8（无 BOM）**。编辑规范见 Cursor User Rule `docs-utf8` 及 `.editorconfig`、`.vscode/settings.json`。