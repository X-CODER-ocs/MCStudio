# MC Studio 实现计划

## 概述

在 `/Users/cangcang/code/MCS` 中创建 **MC Studio** - 一个基于 .NET 10 + Avalonia UI 12 的跨平台 Minecraft 整合工具，包含四大模块：

1. **模组开发** - 重写 Open-craft-creator (CCS) 的 Java 版模组开发功能
2. **内置启动器** - 基于 SulfurBlockLauncher (你的项目)
3. **录屏功能** - 类似 OBS 的游戏录制
4. **MC 动画** - 基于 Blockbench 概念的 3D 模型/动画编辑

---

## 项目结构

```
/Users/cangcang/code/MCS/
  McStudio.sln
  Directory.Build.props              # 共享 SDK/版本属性 (net10.0, Avalonia 12)

  src/
    McStudio/                        # 主 Avalonia 12 UI 应用
      McStudio.csproj
      App.axaml / App.axaml.cs
      ViewModels/                    # ViewModelBase, 页面 VM
      Views/
        MainWindow.axaml             # 主窗口 (TabWindow)
        Pages/
          HomePage.axaml             # 简化主页
          ModDevelopmentPage.axaml   # 模组开发
          LauncherPage.axaml         # 启动器
          RecordingPage.axaml        # 录屏
          AnimationPage.axaml        # 动画
      Styles/                        # 主题
      Assets/                        # 图标, 字体

    McStudio.Core/                   # 核心服务
    McStudio.Desktop/                # 桌面入口 Program.cs
    McStudio.Localization/           # 本地化资源 (zh-CN, en-US)

  modules/                           # 功能模块 (垂直切片)
    ModDevelopment/
      McStudio.ModDevelopment.Core/  # 工作区、元素、代码生成
      McStudio.ModDevelopment.UI/    # 模组开发 UI
    Launcher/
      McStudio.Launcher.Core/        # 启动器核心 (引用 MinecraftLaunch)
      McStudio.Launcher.UI/          # 启动器 UI
    Recording/
      McStudio.Recording.Core/       # 捕获引擎
      McStudio.Recording.UI/         # 录屏 UI
    Animation/
      McStudio.Animation.Core/       # 3D 模型/动画引擎
      McStudio.Animation.UI/         # 动画 UI

  module/                            # 从 SulfurBlockLauncher 引用的模块
    MinecraftLaunch/                 # 核心启动库
    MinecraftLaunch.Base/            # 基础模型
    Iridium/                         # 插件框架
    Tio.Avalonia.Standard/           # 基础 UI 组件
    TioUi.Avalonia/                  # TioUi 控件
    LiteSkinViewer/                  # 3D 皮肤查看器
```

---

## 分阶段实现

### 阶段 1: 基础框架 (第1周)

**目标**: 创建 MC Studio 应用壳，搭建基本导航

1. 创建 `McStudio.sln` 解决方案文件
2. 创建 `Directory.Build.props` (net10.0, Avalonia 12, CommunityToolkit.Mvvm)
3. 从 SulfurBlockLauncher 复制所需模块到 `module/`
4. 创建 `McStudio.Core`, `McStudio.Desktop`, `McStudio.Localization` 项目
5. 创建 `McStudio` 主 UI 项目：
   - `App.axaml/.cs` - 应用入口
   - `MainWindow.axaml` - 使用 TioTabWindowBase 的主窗口
   - `HomePage.axaml` - 简化主页 (Nothing UI 设计)
   - 四个空模块页面占位
6. 验证构建和运行

### 阶段 2: 模组开发核心 (第2-4周)

**目标**: 实现工作区和代码生成核心管线

1. 创建 `McStudio.ModDevelopment.Core` 项目
2. 实现 `McWorkspace` 数据模型 (工作区文件夹结构、设置、文件管理)
3. 实现 `McModElement` 和 `McModElementType` 类型系统
4. 实现 `McGeneratableElement` 基类和首批元素类型：
   - `McBlockElement`, `McItemElement`, `McLivingEntityElement`, `McProcedureElement`
5. 实现 `McTemplateEngine` 使用 Scriban 模板引擎 (替代 FreeMarker)
6. 实现 `McCodeGenerator` 生成协调器
7. 实现 `McGradleRunner` (Gradle 进程管理)
8. 创建 `McStudio.ModDevelopment.UI` 项目：
   - 工作区视图、元素树面板、属性编辑器、Gradle 控制台、代码编辑器

### 阶段 3: 启动器模块 (第5-6周)

**目标**: 集成 MinecraftLaunch 启动库

1. 创建 `McStudio.Launcher.Core` 服务层
2. 引用 `MinecraftLaunch` 和 `MinecraftLaunch.Base`
3. 封装服务接口：账户、版本、实例、下载
4. 创建启动器 UI：游戏列表、版本下载、账户管理、设置

### 阶段 4: 录屏模块 (第7-8周)

**目标**: 实现屏幕录制功能

1. 创建 `McStudio.Recording.Core` 捕获引擎
2. 实现 `IPlatformCaptureService` 平台抽象接口
3. macOS 使用 CGDisplayStream 捕获 (P/Invoke)
4. 使用 FFmpeg 进程封装实现视频编码
5. 创建录屏 UI：控制面板、源选择器、叠加层

### 阶段 5: 动画模块 (第9-12周)

**目标**: 实现 3D 模型和动画编辑

1. 创建 `McStudio.Animation.Core` 项目
2. 实现核心数据模型：`McModel`, `McCube`, `McBone`, `McTexture`, `McAnimation`, `McKeyframe`
3. 使用 SkiaSharp 实现 `ModelRenderer` (正交投影、立方体渲染、UV 贴图)
4. 实现模型格式解析器：Java Edition JSON、Bedrock geometry JSON、Blockbench .bbmodel
5. 创建动画 UI：3D 视口、时间轴、关键帧曲线编辑器、像素纹理编辑器

### 阶段 6: 整合与完善 (第13-16周)

1. 集成 Iridium 插件框架
2. 补全所有模块的本地化 (zh-CN, en-US)
3. 主题支持 (Dark/Light)
4. 跨平台测试 (macOS, Windows, Linux)
5. 构建和打包脚本

---

## 关键技术决策

| 决策 | 方案 | 理由 |
|---|---|---|
| UI 框架 | Avalonia 12 | 与 SulfurBlockLauncher 一致，跨平台 |
| 模板引擎 | Scriban (替代 FreeMarker) | .NET 原生，功能强大，易移植 |
| 3D 渲染 | SkiaSharp 手动投影 | 立方体模型足够，避免额外依赖 |
| 录屏 | 平台 API + FFmpeg | 跨平台捕获，FFmpeg 编码成熟 |
| 代码编辑器 | AvaloniaEdit | 与 SulfurBlockLauncher 一致 |
| JSON 序列化 | System.Text.Json | .NET 内置，高性能 |
| 本地化 | .resx 文件 | 与 SulfurBlockLauncher 一致模式 |

---

## 验证方法

1. **构建**: `dotnet build src/McStudio.Desktop/McStudio.Desktop.csproj -c Debug`
2. **运行**: `dotnet run --project src/McStudio.Desktop/McStudio.Desktop.csproj`
3. **模组开发**: 创建新工作区 -> 添加 Block 元素 -> 设置属性 -> 生成代码 -> 验证输出 Java 文件
4. **启动器**: 下载 Minecraft 版本 -> 登录账户 -> 启动游戏
5. **录屏**: 选择捕获源 -> 开始录制 -> 停止 -> 验证输出视频文件
6. **动画**: 创建新模型 -> 添加立方体 -> 编辑 UV -> 创建骨骼动画 -> 导出模型 JSON