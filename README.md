# 今日待办 (Today Todo)

一款简洁的 Windows 桌面待办事项应用，基于 WPF + .NET 10 构建，采用 Ink & Paper 日式文具风格设计。

## 功能

- **待办管理** — 添加、完成、删除待办事项
- **完成标记** — 复选框勾选后文字显示删除线并变灰
- **数据持久化** — 自动保存到本地 JSON 文件，重启不丢失
- **日期切换** — 自定义日历弹窗，查看和管理不同日期的待办
- **进度统计** — 实时显示待办总数和完成百分比

## 运行环境

- Windows 10/11
- .NET 10 SDK

## 快速开始

```bash
# 克隆仓库
git clone git@github.com:fan-xii/today_todoList_wpf.git
cd today_todoList_wpf

# 运行
dotnet run

# 发布为独立可执行文件
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 项目结构

```
├── App.xaml / App.xaml.cs              # 应用入口
├── MainWindow.xaml / MainWindow.xaml.cs # 主窗口界面与交互
├── Controls/
│   └── CalendarPopup.xaml(.cs)         # 自定义日历弹窗组件
├── Converters/
│   └── ProgressWidthConverter.cs       # 进度条宽度转换器
├── Models/
│   ├── TodoItem.cs                     # 待办数据模型
│   └── TodoStore.cs                    # JSON 读写持久化
├── ViewModels/
│   └── MainViewModel.cs               # MVVM 视图模型
└── app.ico                             # 应用图标
```

## 数据存储

待办数据保存在可执行文件同目录下的 `todos.json`，按日期组织：

```json
{
  "2026-05-21": [
    { "id": "...", "text": "阅读一小时", "isCompleted": false },
    { "id": "...", "text": "完成周报", "isCompleted": true }
  ]
}
```

## 截图

<p align="center">
  <img src="Sreenshot.png" alt="今日待办截图" width="480"/>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Win10%2F11-blue" alt="Platform">
  <img src="https://img.shields.io/badge/.NET-10-purple" alt=".NET">
  <img src="https://img.shields.io/badge/UI-WPF-green" alt="WPF">
</p>
