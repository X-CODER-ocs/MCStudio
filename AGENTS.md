# AGENTS.md

## build
```bash
dotnet build src/McStudio.Desktop/McStudio.Desktop.csproj -c Debug
dotnet run --project src/McStudio.Desktop/McStudio.Desktop.csproj
```

## localization
所有用户可见文本必须使用本地化资源，不要在 XAML、C# 或其他 UI 代码中写死界面文案。

## ui design
- Nothing UI 设计语言：禁止使用 emoji，图标用字符或线条符号表示
- 简化主页，最少的元素，核心功能优先
- 深色主题为主

## modules
- `src/McStudio` - 主 UI 应用（Avalonia 12）
- `src/McStudio.Core` - 核心服务
- `src/McStudio.Desktop` - 桌面入口
- `src/McStudio.Localization` - 本地化资源
- `modules/ModDevelopment` - 模组开发模块
- `modules/Launcher` - 启动器模块
- `modules/Recording` - 录屏模块
- `modules/Animation` - 动画模块