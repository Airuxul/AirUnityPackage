# tools/

| 文件 | 说明 |
|------|------|
| `install-packages.ps1` | 按 `installDefault` 补全缺失的本地包到 manifest |
| `unity-compile-loop.ps1` | 固定使用 `editor` profile 执行一次 `compile(--timeout 20000)` + `console` 错误检查（可选包含 warning） |

由仓库根目录脚本调用：

- `install-to-unity.bat` → `install-packages.ps1`
- `init-submodules.bat` → `git submodule update`（在根目录）
