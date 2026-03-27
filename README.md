
## 📖 项目简介

ASMRoner v0.4.1 — Windows 客户端（WPF）

当前版本：v0.4.1

本仓库当前主要面向 Windows 桌面应用（基于 WPF），提供 ASMR.one 内容的搜索、预览与下载功能。原始的 Go 命令行工具与本地 WebUI 实现仅作为历史参考并保留于仓库中，迁移工作以 WPF 为主线。

## 🚀 快速开始

开发调试（Visual Studio）
- 在 Visual Studio 中打开解决方案 [dotnet/Asmroner.sln](dotnet/Asmroner.sln)。
- 设置启动项目为 Asmroner.Wpf 并运行（F5）。

使用 dotnet CLI（开发调试）
```powershell
dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug
```

发布示例：
```powershell
dotnet publish dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Release -o ./publish
```

历史的 Go/CLI 使用示例移至：docs/legacy-go.md（历史参考）

## 📋 命令说明（历史 CLI 参考）

WPF 客户端以可视化操作为主，历史的 Go/CLI 命令参考请见 [docs/legacy-go.md](docs/legacy-go.md)。

## 📸 截图 (待修改)

|              配置               |             搜索              |
| :-----------------------------: | :---------------------------: |
|    ![配置](dist/config.png)     |   ![搜索](dist/search.png)    |
|            **下载**             |           **同步**            |
|   ![下载](dist/download.png)    |    ![同步](dist/sync.png)     |
|          **同步下载**           |           **统计**            |
| ![同步下载](dist/sync-down.png) | ![统计](dist/sync-report.png) |
|      **Web 界面（历史）**       |    **Web 界面 2（历史）**     |
|   ![Web界面](dist/listen.png)   | ![Web界面2](dist/listen2.png) |

<details>
<summary><b>✨ 功能特性</b></summary>

- **搜索**：单个/批量 RJID、高级搜索语法、结果导出 CSV/JSON
- **下载**：单个/批量/热门作品下载，自动限流、重试、指数退避
- **同步（历史参考）**：元数据同步、批量下载控制、状态跟踪、失败重试
- **Web 界面（历史参考）**：可视化浏览、浏览器内播放（历史实现）
- **配置**：首次启动向导与设置页，支持代理、限流、抖动等高级配置

</details>

<details>
<summary><b>⚙️ 配置文件说明</b></summary>

配置文件路径：`~/.asmroner/config.toml`（TOML 格式）

```toml
[user]
account = "guest"
password = "guest"

[downloader]
api_url = ""                # 留空自动获取最快站点
proxy_url = ""              # 支持 http / socks5
max_workers = 5
max_retries = 3
sync_data_folder = "./syncdata"
sync_wanted_size = "200MB"  # 同步容量限制
prefer_media = "all"        # all | mp3>wav>flac

[limit]
sync_qps = 2
sync_jitter_min = 100       # ms
sync_jitter_max = 500
download_qps = 0.2
download_jitter_min = 2000
download_jitter_max = 5000
```

</details>

<details>
<summary><b>📋 命令/功能速查（历史 CLI 参考）</b></summary>

| 命令              | 选项       | 说明                               |
| ----------------- | ---------- | ---------------------------------- |
| `search`          | `-c`       | 搜索结果数量（默认 10）            |
| `search download` | `-d`, `-s` | 下载目录、下载数量                 |
| `search export`   | `-f`, `-n` | 导出文件名（.csv/.json）、导出数量 |
| `download`        | `-d`, `-n` | 下载目录、hot100 数量              |
| `sync download`   | `-d`       | 下载目录                           |
| `sync retry`      | `-d`       | 失败文件所在目录                   |
| `sync export`     | `-s`, `-f` | 状态（failed/success）、导出文件   |
| `listen`          | `-p`       | 端口（默认 9999）                  |

</details>

## 🤝 贡献

欢迎提交 Pull Request！Fork → 新建分支 → 提交更改 → 开启 PR。

## 📄 许可证

本项目采用 MIT 许可证，详情请查看 [LICENSE](LICENSE)。

## 🙏 致谢

- 特别感谢 [go-asmr-spider](https://github.com/DiheChen/go-asmr-spider)
- 感谢所有贡献者和用户！

---

ASMRoner v0.4.1 — Windows 客户端（WPF）迁移计划中

*最后更新：2026 年 3 月 19 日*
