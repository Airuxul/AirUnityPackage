
# tools/

| 文件 | 说明 |
|------|------|
| `install-packages.ps1` | 按 `installDefault` 补全缺失的本地包到 manifest（已配置则跳过） |

由仓库根目录脚本调用：

- `install-to-unity.bat` → 本脚本
- `init-submodules.bat` → `git submodule update`（在根目录）

维护：`fix-markdown-utf8.py` 将 GBK/损坏的 UTF-8 转为标准 UTF-8（文档乱码时运行）。
