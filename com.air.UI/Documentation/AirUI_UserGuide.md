# AirUI 用户指南

## 概述

AirUI是一个Unity UI代码生成器框架，旨在简化Unity UI开发流程。本指南将帮助您快速上手使用AirUI框架。

## 安装

### 方法1: 通过Git URL安装
1. 在Unity中打开 `Window > Package Manager`
2. 点击 `+` 按钮，选择 `Add package from git URL`
3. 输入: `https://github.com/yourusername/airui.git`

### 方法2: 本地安装
1. 将 `com.airui` 文件夹复制到项目的 `Packages` 目录
2. Unity会自动识别并加载Package

## 快速开始

### 1. 创建UI GameObject
在Unity中创建一个UI GameObject，添加必要的UI组件：
- Text
- Image
- Button
- 其他UI组件

### 2. 生成UI脚本
1. 选择要生成脚本的GameObject
2. 右键选择 "Generate UI Script"
3. 系统自动生成对应的C#脚本

### 3. 使用生成的脚本
脚本编译完成后，系统会自动：
- 挂载脚本到GameObject
- 绑定UI组件字段
- 设置序列化字段

## 核心概念

### UIComponent
所有UI组件的基类，提供：
- Unity生命周期管理
- UI生命周期管理
- 工具方法

### 自动字段绑定
系统会自动查找并绑定标记为 `[SerializeField]` 的字段：
1. 精确名称匹配
2. 驼峰命名规则匹配
3. 包含名称匹配

### 模板系统
支持自定义代码生成模板：
- 逻辑脚本模板
- 设计器脚本模板
- 可扩展的占位符系统

## 高级功能

### 自定义模板
1. 修改 `Editor/Templates/` 目录下的模板文件
2. 支持 `#CLASSNAME#` 和 `#FIELDS#` 占位符
3. 重新生成脚本应用新模板

### 扩展UI组件类型
在 `UIComponentTypes.cs` 中添加新的基础UI组件类型。

### 自定义代码生成逻辑
继承或修改 `UIScriptGenerator` 类实现自定义逻辑。

## 最佳实践

1. **命名规范**: 使用清晰的GameObject名称
2. **组件组织**: 合理组织UI组件层级结构
3. **模板管理**: 根据项目需求定制模板
4. **版本控制**: 将生成的脚本纳入版本控制

## 故障排除

### 常见问题
1. **脚本未生成**: 检查GameObject名称和权限
2. **字段未绑定**: 确认字段标记为 `[SerializeField]`
3. **编译错误**: 检查模板语法和命名空间

### 调试技巧
1. 查看Console日志输出
2. 使用 "Bind Fields" 按钮手动绑定
3. 检查EditorPrefs中的待处理任务

## 技术支持

如有问题或建议，请：
1. 查看示例项目
2. 阅读源代码注释
3. 提交Issue或Pull Request
