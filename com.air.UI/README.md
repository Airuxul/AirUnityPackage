# AirUI - Unity UI代码生成器

## 概述

AirUI是一个强大的Unity UI代码生成器框架，旨在简化Unity UI开发流程。通过自动分析GameObject结构，自动生成对应的C#脚本代码，并提供智能的组件字段绑定功能。

## 特性

- 🚀 **自动代码生成**: 根据GameObject结构自动生成UI脚本
- 🔗 **智能字段绑定**: 自动绑定UI组件字段，无需手动拖拽
- 📝 **模板化系统**: 支持自定义代码生成模板
- 🎯 **类型识别**: 智能识别基础UI组件和自定义组件
- 🛠️ **编辑器集成**: 无缝集成到Unity编辑器中
- 📚 **完整文档**: 提供详细的使用说明和示例
- 🧠 **智能重生成**: 自动检测已存在脚本，仅重新生成必要的部分
- 🔄 **组件管理**: 自动处理已挂载的UIComponent，避免冲突

## 安装

### 方法1: 通过Git URL安装
1. 在Unity中打开 `Window > Package Manager`
2. 点击 `+` 按钮，选择 `Add package from git URL`
3. 输入: `https://github.com/yourusername/airui.git`

### 方法2: 本地安装
1. 将 `com.airui` 文件夹复制到项目的 `Packages` 目录
2. Unity会自动识别并加载Package

## 快速开始

### 方法1: 使用Editor窗口
1. 在Unity中打开 `Window > AirUI > UI Script Generator`
2. 拖拽UI GameObject到拖拽区域
3. 设置类名和输出文件夹
4. 点击"生成UI脚本"按钮

### 方法2: 使用右键菜单
1. 在Hierarchy中选择UI GameObject
2. 右键选择 `AirUI > Generate UI Script`
3. 在打开的窗口中配置参数并生成

## API接口

### 基本用法
```csharp
// 指定类名和输出路径
UIScriptGenerator.GenerateUIScript(gameObject, "MyUIPanel", "Assets/Scripts/UI");
```

## 使用方法

### 1. 创建UI GameObject
在Unity中创建一个UI GameObject，添加必要的UI组件：
- Text
- Image
- Button
- 其他UI组件

### 2. 生成UI脚本
选择上述任一方法生成UI脚本，系统会自动：
- 分析GameObject结构
- 检查是否已存在同名脚本
- 智能决定生成策略：
  - 如果脚本不存在：生成完整的逻辑脚本和设计器脚本
  - 如果脚本已存在：仅重新生成设计器脚本，保留用户逻辑
- 处理已挂载的UIComponent，避免冲突
- 挂载脚本到GameObject
- 自动绑定UI组件字段

### 3. 使用生成的脚本
脚本编译完成后，系统会自动：
- 挂载脚本到GameObject
- 绑定UI组件字段
- 设置序列化字段

## 智能生成逻辑

### 脚本存在性检查
- 系统会自动检查输出目录中是否已存在同名脚本
- 如果逻辑脚本（`.cs`）和设计器脚本（`.Designer.cs`）都已存在，则仅重新生成设计器脚本
- 这样可以保护用户编写的业务逻辑代码不被覆盖

### 组件冲突处理
- 在生成脚本前，系统会自动检查GameObject上是否已挂载UIComponent
- 如果发现已挂载的UIComponent，会自动移除它们
- 对于同类型的组件，会记录日志提示将被替换
- 对于不同类型的组件，会记录日志并移除

### 重新生成策略
- **首次生成**: 生成完整的逻辑脚本和设计器脚本
- **重新生成**: 仅重新生成设计器脚本，保留逻辑脚本中的用户代码
- **增量更新**: 根据GameObject结构变化，智能更新字段绑定

### Parent字段自动绑定
- 系统会自动为每个UIComponent生成对应的Parent字段
- Parent字段命名规则：`{组件名}_Parent`
- 自动绑定父级UIComponent，建立组件间的层级关系
- 支持多层嵌套的UIComponent结构
- 在运行时自动查找并赋值父组件引用

## 核心组件

### UIComponent
所有UI组件的基类，提供生命周期管理和工具方法。

### UIScriptGenerator
负责根据GameObject结构自动生成UI脚本代码，包含智能生成逻辑。

### UISerializer
自动绑定UI组件字段，支持多种匹配策略。

## 示例

查看 `Samples` 目录中的示例项目，了解如何使用AirUI框架。

## 文档

详细文档请查看 `Documentation` 目录。

## 许可证

MIT License

## 支持

如有问题或建议，请提交Issue或Pull Request。
