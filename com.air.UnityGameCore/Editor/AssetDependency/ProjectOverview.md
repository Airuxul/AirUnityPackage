# 资源依赖查询工具 - 项目概览

## 📦 项目结构

```
AssetDependency/
├── AssetDependencyAnalyzer.cs          # 核心分析逻辑
├── AssetDependencyWindow.cs            # UIToolkit主窗口
├── AssetDependencyTreeWindow.cs        # 树形视图窗口
├── AssetDependencyBatchAnalyzer.cs     # 批量分析工具
├── AssetDependencyContextMenu.cs       # 右键菜单扩展
├── README.md                            # 完整功能说明
├── QuickStart.md                        # 快速入门指南
└── ProjectOverview.md                   # 本文件
```

## 🎯 核心功能模块

### 1. AssetDependencyAnalyzer (核心引擎)
**文件**: `AssetDependencyAnalyzer.cs`  
**职责**: 提供所有依赖分析的核心算法

**主要API**:
- `GetDependencies()` - 获取资源依赖（递归/非递归）
- `GetDirectDependencies()` - 获取直接依赖
- `GetReverseDependencies()` - 获取反向依赖
- `BuildDependencyTree()` - 构建依赖树结构
- `GetAssetInfo()` - 获取资源详细信息
- `FormatFileSize()` - 格式化文件大小

**数据结构**:
- `DependencyNode` - 依赖树节点
- `AssetInfo` - 资源信息

### 2. AssetDependencyWindow (主窗口)
**文件**: `AssetDependencyWindow.cs`  
**技术**: UIToolkit  
**菜单**: `Tools > 资源依赖查询工具`

**功能特性**:
- ✅ 现代化UIToolkit界面
- ✅ 资源依赖查询
- ✅ 反向依赖查询
- ✅ 递归/非递归切换
- ✅ 按类型分组显示
- ✅ 快速选择和定位资源
- ✅ 实时显示资源信息

**UI组件**:
- ObjectField - 资源选择
- ScrollView - 结果展示
- ToggleButton - 递归选项
- 自定义结果项 - 带图标和操作按钮

### 3. AssetDependencyTreeWindow (树形视图)
**文件**: `AssetDependencyTreeWindow.cs`  
**技术**: TreeView + IMGUI  
**菜单**: `Tools > 资源依赖树形视图`

**功能特性**:
- ✅ 层级树形展示
- ✅ 深度控制（1-10层）
- ✅ 循环依赖检测和高亮
- ✅ 文件大小显示
- ✅ 搜索功能
- ✅ 展开/折叠控制
- ✅ 导出为文本文件
- ✅ 双击定位资源
- ✅ 右键菜单操作

**高级功能**:
- 在新窗口中分析节点
- 复制资源路径
- 树形结构可视化

### 4. AssetDependencyBatchAnalyzer (批量分析)
**文件**: `AssetDependencyBatchAnalyzer.cs`  
**技术**: IMGUI  
**菜单**: `Tools > 资源依赖批量分析`

**功能特性**:
- ✅ 批量添加资源
- ✅ 拖放添加
- ✅ 文件夹批量导入
- ✅ 统计摘要
- ✅ 多种排序方式
- ✅ 导出CSV报告
- ✅ 导出详细文本报告
- ✅ 进度条显示

**排序选项**:
- 按名称
- 按依赖数
- 按被依赖数
- 按文件大小

**导出格式**:
- CSV格式（适合Excel分析）
- 详细文本格式（包含完整依赖列表）

### 5. AssetDependencyContextMenu (快捷菜单)
**文件**: `AssetDependencyContextMenu.cs`  
**位置**: Project窗口右键菜单

**菜单项**:
- `资源依赖 > 查看依赖关系` - 打开主窗口
- `资源依赖 > 打印依赖到控制台` - 快速查看依赖
- `资源依赖 > 打印反向依赖到控制台` - 快速查看反向依赖

## 🏗️ 技术架构

### 设计模式
- **单一职责原则**: 每个类专注于特定功能
- **分离关注点**: 分析逻辑与UI展示分离
- **策略模式**: 支持不同的分析和显示策略

### 技术栈
- **Unity Editor API**: AssetDatabase, EditorUtility
- **UIToolkit**: 现代化UI框架（主窗口）
- **IMGUI**: 传统编辑器UI（树形视图和批量分析）
- **TreeView**: Unity内置树形控件
- **C# 特性**: LINQ, 泛型, 扩展方法

### 性能优化
- ✅ 过滤内置资源
- ✅ 避免重复分析
- ✅ 进度条反馈
- ✅ 结果缓存
- ✅ 按需加载

## 📊 使用场景对照表

| 需求 | 推荐工具 | 原因 |
|-----|---------|------|
| 快速查看单个资源的依赖 | 右键菜单 | 最快捷 |
| 详细分析单个资源 | 主窗口 (UIToolkit) | 界面友好，功能完整 |
| 查看依赖层级关系 | 树形视图 | 可视化层级结构 |
| 检测循环依赖 | 树形视图 | 自动检测并高亮 |
| 对比多个资源 | 批量分析 | 统计和对比功能 |
| 生成分析报告 | 批量分析 | 支持CSV和文本导出 |
| 清理未使用资源 | 批量分析 | 显示被依赖数为0的资源 |

## 🎨 UI设计特点

### UIToolkit主窗口
- 现代化设计语言
- 响应式布局
- 悬停效果
- 圆角和阴影
- 分组和层次感

### 色彩系统
- **主色**: 深灰色背景
- **强调色**: 蓝色按钮和链接
- **警告色**: 黄色（循环依赖、无被依赖）
- **辅助色**: 浅灰色文本

### 交互设计
- 拖放支持
- 双击快捷操作
- 右键上下文菜单
- 悬停反馈
- 搜索过滤

## 📈 扩展性

### 易于扩展的点
1. **新的分析指标**: 在`AssetDependencyAnalyzer`中添加新方法
2. **新的UI视图**: 创建新的EditorWindow
3. **新的导出格式**: 在批量分析器中添加导出方法
4. **新的菜单项**: 在ContextMenu中添加新的MenuItem
5. **自定义过滤器**: 添加资源过滤逻辑

### 建议的扩展方向
- [ ] 资源依赖图可视化（Graph View）
- [ ] AssetBundle依赖分析
- [ ] 资源冗余检测
- [ ] 依赖优化建议
- [ ] 实时监控资源变化
- [ ] 与版本控制集成
- [ ] Web端查看报告

## 🔒 代码规范遵循

### 命名规范 ✅
- 类名使用名词（PascalCase）
- 方法名使用动词（PascalCase）
- 私有字段使用下划线前缀（_camelCase）
- 语义清晰，避免缩写

### 注释规范 ✅
- 所有公共API都有XML文档注释
- 复杂逻辑有内联注释
- 注释解释"为什么"而非"是什么"

### 异常处理 ✅
- 合理的空值检查
- try-finally确保资源清理
- 用户友好的错误提示
- 进度条支持取消

### 函数设计 ✅
- 单一职责
- 函数长度适中
- 参数数量合理
- 返回类型明确

## 📚 文档完整性

- ✅ README.md - 完整功能说明
- ✅ QuickStart.md - 快速入门指南
- ✅ ProjectOverview.md - 项目概览（本文件）
- ✅ 代码内XML注释 - API文档
- ✅ 使用场景示例

## 🚀 性能指标

| 操作 | 时间复杂度 | 适用规模 |
|-----|----------|---------|
| 单资源依赖查询 | O(n) | 任意 |
| 单资源反向依赖 | O(n*m) | 中小型项目 |
| 树形结构构建 | O(n*d) | 深度限制 |
| 批量分析 | O(k*n) | k<100 |

说明: 
- n: 项目资源总数
- m: 平均依赖数
- d: 树深度
- k: 批量分析资源数

## 🎓 学习价值

### 对于Unity开发者
- AssetDatabase API使用
- Editor Window开发
- UIToolkit实战
- TreeView控件使用

### 对于软件工程
- 图遍历算法
- 递归数据结构
- 批量处理模式
- 报告生成系统

## 📝 版本信息

- **初始版本**: 1.0.0
- **Unity版本要求**: 2020.3+
- **依赖**: 无外部依赖
- **许可**: 项目内部使用

## 🤝 贡献指南

如需扩展此工具，建议：
1. 保持现有代码风格
2. 遵循命名规范
3. 添加XML注释
4. 更新相关文档
5. 确保向后兼容

---

**总结**: 这是一个功能完整、设计良好、易于扩展的Unity编辑器工具套件，适合任何规模的Unity项目使用。


