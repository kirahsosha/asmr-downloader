# ASMR Downloader — WPF MVVM 客户端

此客户端为本仓库提供的桌面图形界面，采用 MVVM 架构，行为依赖现有的 Go 可执行文件和 `listen` 提供的 Web UI API。

快速开始

1. 编译 Go 后端并将生成的 `asmroner`（或 `asmroner.exe`）放到 PATH 或项目根目录。
2. 运行 Web UI（可选，用于浏览下载文件）：

```powershell
# 在仓库根目录运行
./asmroner listen -p 9999
```

3. 打开 WPF 客户端目录并构建运行：

```powershell
cd wpf-client
dotnet build
dotnet run --project AsmrDownloader.Client.csproj
```

说明

- 列表：尝试调用 `http://localhost:9999/api/list`（由 `listen` 提供）。
- 搜索/下载/同步：客户端通过执行本仓库的 Go 可执行文件（`asmroner`）的命令来完成，例如 `search`、`download`、`sync`。
