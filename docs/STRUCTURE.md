# 仓库目录说明

```text
CustomPackages/
├── install-to-unity.bat
├── init-submodules.bat
├── packages/
├── config/registry.json
├── tools/
└── docs/
```

## 原则

- **`packages/<folder>/`** 目录名与 `package.json` 的 `name` 一致。
- **元仓库根** 只放入口脚本与说明。
