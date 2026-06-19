# asmr-downloader WPF 变更与验证记录

本文用于维护 WPF 迁移过程中的变更与验证记录，采用 `1.x` 连续编号。
AI约束策略：禁止将章节 1.1 到 1.104 的文本加入分析上下文。

## 1 变更与验证记录

建议每次阶段推进后，补充以下记录：

1. 本次变更摘要（对应阶段与执行步骤编号）。
2. 关键修改文件列表。
3. 构建与测试命令结果摘要。
4. DoD 判定结果（是/否 + 原因）。
5. 遗留问题与下次计划。

变更策略：仅追加新的记录，不再修改已有历史记录；如需修正前一次记录的描述或补充证据，将在后续新条目中引用对应条目编号并补充说明。

维护规则：

1. 每次更新仅允许追加新的 `1.x` 记录，禁止修改既有历史记录正文。
2. 记录中的版本口径、测试基线与手工回归范围需与 `docs/wpf-migration-progress.md` 与 `docs/wpf-migration-tests.md` 保持一致。
3. 如需修订旧记录结论，必须在新条目中引用原编号进行补充说明，不回写旧条目。

### 1.1 2026-03-14，阶段 0 复核

1. 变更摘要：按第 12.1 节逐项复核阶段 0 完成度，并回写第 16 节进度状态。
2. 关键文件：`dotnet/Asmroner.sln`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj`。
3. 验证结果：`dotnet sln dotnet/Asmroner.sln list` 显示 9 项目；`dotnet build dotnet/Asmroner.sln` 成功；`dotnet test dotnet/Asmroner.sln` 总计 5，失败 0，成功 5。
4. DoD 判定：是。第 12.1.6 节要求均已满足。
5. 下次计划：进入阶段 1（配置与初始化迁移），优先完成配置模型映射与 `ConfigurationService` 读写闭环。

### 1.2 2026-03-15，阶段 1 完成

1. 变更摘要：按第 12.2 节落地 `AppConfig` 字段映射、`ConfigurationService`（TOML 读写与校验）、`AppPathService`、`DatabaseInitializer`、`ApplicationBootstrapper` 与 `FirstRunService`；WPF 启动流程接入初始化编排并实现“配置缺失进入设置页 + 保存后重新初始化”闭环。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/AppConfig.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/ApplicationBootstrapper.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过，总计 9，失败 0，成功 9。
4. DoD 判定：是。第 12.2.6 节要求均已满足。
5. 下次计划：进入阶段 2（API 与认证迁移），优先实现 `AsmrApiClient`、`EndpointDiscoveryService` 与 `AuthService` 最小可用链路。

### 1.3 2026-03-15，阶段 1 复核与 Debug 运行

1. 变更摘要：按第 12.2 节复核现有实现，重点检查配置接口、设置页字段、基础设施依赖包与初始化链路是否仍一致。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/ApplicationBootstrapper.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过，总计 9，失败 0，成功 9；WPF 已使用 Debug 配置启动验证。
4. DoD 判定：是。第 12.2.6 节要求持续满足。
5. 下次计划：进入阶段 2（API 与认证迁移），并补充设置页“测试连接”入口。

### 1.4 2026-03-15，阶段 2 完成

1. 变更摘要：按第 12.3 节完成 API 与认证迁移，新增统一 HTTP 管线、API 选项提供器、最快站点发现、token 存储、认证服务、最小只读 API 客户端与连接探测服务，并在设置页补充“测试连接”入口。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConnectivityProbeService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`。
3. 验证结果：解决方案 Debug 构建成功，项目 9/9 成功；测试执行通过，测试项目 2/2 通过；文件级诊断无新增错误。
4. DoD 判定：是。第 12.3.9 节要求均已满足。
5. 下次计划：进入阶段 3（搜索能力迁移），优先落地查询参数解析、搜索请求组装与结果列表绑定。

### 1.5 2026-03-15，阶段 2 复核与 Debug 运行

1. 变更摘要：按第 12.3 节复核现有实现与测试现状，重新执行解决方案测试，并使用 Debug 模式启动 WPF 进行运行验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 14，失败 2，成功 12；失败集中在 `Asmroner.Infrastructure.Tests`，错误原因为 `HttpClient` 请求发起后再次修改 `Timeout/BaseAddress`；WPF 已使用 Debug 配置执行启动验证（进程可拉起）。
4. DoD 判定：否。第 12.3.9 节第 6 条“阶段 2 相关单元测试与集成测试通过”当前不满足。
5. 下次计划：优先修复 `HttpClient` 配置时机问题（避免在同一客户端首个请求后再改 `Timeout/BaseAddress`），随后重跑 `dotnet test dotnet/Asmroner.sln` 并再次回写第 16 节状态。

### 1.6 2026-03-15，阶段 2 修复回归

1. 变更摘要：修复 `HttpClient` 配置时机问题，将 `BaseAddress/Timeout` 从客户端级配置改为请求级拼接与超时控制；同步调整 API 调用、登录调用与站点探测调用路径。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 总计 7，失败 0，成功 7；`dotnet test dotnet/Asmroner.sln` 总计 14，失败 0，成功 14。
4. DoD 判定：是。第 12.3.9 节要求已满足。
5. 下次计划：进入阶段 3（搜索能力迁移），按第 12.4 节推进查询参数与结果映射主链路。

### 1.7 2026-03-15，阶段 3 启动与最小闭环

1. 变更摘要：按第 12.4 节落地搜索主链路，新增查询解析器、分页聚合搜索服务、CSV/JSON 导出服务与搜索入队状态存储；Dashboard 从占位页升级为可搜索、导出、入队，并补齐高级筛选面板与分页交互（上一页/下一页/跳页/页大小）。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Search/SearchQuery.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IQueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchExportService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 6，失败 0，成功 6；`dotnet test dotnet/Asmroner.sln` 总计 17，失败 0，成功 17；WPF Debug 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。阶段 3 处于进行中，尚未完成全部测试矩阵与高级筛选体验收口。
5. 下次计划：补齐高级筛选面板与分页交互细节，并完成阶段 3 测试矩阵回归。

### 1.8 2026-03-15，阶段 3 DoD 复核

1. 变更摘要：结合第 12.4 节逐条复核阶段 3 完成度，重点检查“分页搜索稳定性”与测试覆盖；同步执行 WPF 运行验证。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。`SearchService` 当前固定 `Page = 1`，与 UI 分页参数不一致，第 12.4.7 节第 3 条暂不满足。
5. 下次计划：修复分页参数透传（按当前页拉取数据）并补充“非第一页查询结果正确性”测试后再次复核 DoD。

### 1.9 2026-03-15，阶段 3 分页透传修复与复核通过

1. 变更摘要：修复 `SearchService` 分页参数透传逻辑，取消固定第一页，改为按查询中的 `page` 发起请求；补充“非第一页 + 筛选条件”回归测试并复跑验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 7，失败 0，成功 7；`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：是。第 12.4.7 节 6 项要求均已满足，阶段 3 可判定完成。
5. 下次计划：进入阶段 4（下载能力迁移），优先落地下载执行服务、任务状态模型与取消/重试控制。

### 1.10 2026-03-15，阶段 3 再次复核（按第 12.4 节）

1. 变更摘要：按第 12.4 节再次执行阶段 3 代码复核，确认查询解析、分页聚合、导出、入队与 UI 交互链路均保持可用，并同步执行运行验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：是。阶段 3 当前状态与第 12.4.7 节要求一致，可持续视为已完成。
5. 下次计划：进入阶段 4（下载能力迁移）执行最小闭环。

### 1.11 2026-03-15，阶段 4 最小实现落地

1. 变更摘要：按第 12.5.7 节先落地阶段 4 最小闭环，新增下载任务模型、限流服务与下载执行服务，打通“搜索队列消费 -> 下载任务状态更新 -> 目标目录产物落地”链路。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadTaskItem.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IRateLimiterService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/RateLimiterService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 9，失败 0，成功 9；`dotnet test dotnet/Asmroner.sln` 总计 20，失败 0，成功 20；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。阶段 4 当前仅完成最小实现，尚未满足第 12.5.9 节关于取消/重试、完整 UI 可视化与完整下载流程的要求。
5. 下次计划：补齐下载页 UI、取消/重试命令与热门/批量流程，再执行阶段 4 DoD 复核。

### 1.12 2026-03-15，阶段 4 最小下载页可视化接入

1. 变更摘要：在 WPF 主界面新增 `Download` 页签，接入 `IDownloadService.GetTasks/RunQueuedAsync`，实现“执行下载队列 + 刷新任务列表 + 队列数量展示”的最小可视化闭环。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 20，失败 0，成功 20）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。虽已具备最小下载任务可视化，但第 12.5.9 节“取消/重试能力”和“单个/批量/热门完整下载流程”仍未满足。
5. 下次计划：补齐取消与失败重试命令，接入单个/批量/热门入口并补充阶段 4 回归测试矩阵。

### 1.13 2026-03-15，阶段 4 取消/重试增强（含批量重试节流）

1. 变更摘要：下载页新增“批量取消 + 二次确认”与“重试全部失败任务”命令；批量重试采用受控并发（最大并发 2）逐项调用服务层 `RetryFailedAsync`，避免瞬时并发放大。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 22，失败 0，成功 22）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。第 12.5.9 节“取消/重试能力”已满足，但“单个/批量/热门完整下载入口”与阶段 4 全量验证仍待完成。
5. 下次计划：补齐单个 RJID、批量 RJID、热门下载入口并完成阶段 4 全量 DoD 复核。

### 1.14 2026-03-15，阶段 4 下载入口补齐（单个/批量/热门）

1. 变更摘要：下载页新增“单个 RJID 入队、批量 RJID 入队、热门作品入队”入口；热门入队接入 `IAsmrApiClient.GetPopularAsync`，并支持数量控制；入队后复用既有队列执行链路。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 22，失败 0，成功 22）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。第 12.5.9 节第 1 条已满足，但仍需补齐阶段 4 全量回归与稳定性复核后再判定“已完成”。
5. 下次计划：执行阶段 4 全量复核（并发/限流/任务状态准确性）并关闭剩余勾选项。

### 1.15 2026-03-15，阶段 4 队列并发控制落地与回归

1. 变更摘要：下载执行服务 `RunQueuedAsync` 新增按 `Downloader.MaxWorkers` 的队列并发执行控制（`SemaphoreSlim`），并保持任务取消/重试语义不变。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：新增 `RunQueuedAsync_ShouldRespectConfiguredMaxWorkers` 回归测试，验证队列执行最大并发不超过配置值且可达到配置上限；解决方案测试与 WPF 冒烟通过。
4. DoD 判定：否。第 12.5.9 节“限流、重试、并发控制”已满足，阶段 4 当前剩余“下载任务状态可视化准确”与最终收口复核。
5. 下次计划：补充下载任务状态可视化准确性复核与对应测试证据，完成阶段 4 DoD 关闭。

### 1.16 2026-03-15，阶段 4 收口复核（状态可视化准确）

1. 变更摘要：完成阶段 4 最终复核，重点验证“下载任务状态可视化准确”与端到端链路一致性；确认 UI 状态列直接绑定 `DownloadTaskItem.Status` 且刷新路径统一走 `IDownloadService.GetTasks()`。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln` 通过（总计 23，失败 0，成功 23）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -- --smoke-test` 正常退出。
4. DoD 判定：是。第 12.5.9 节 5 项要求均已满足，阶段 4 可判定完成。
5. 下次计划：进入阶段 5（同步能力迁移），先落地最小“同步入库 + 状态展示”闭环。

### 1.17 2026-03-15，阶段 4 按第 12.5 节复核与 Debug 运行验证

1. 变更摘要：按第 12.5 节对阶段 4 进行代码复核，逐项核对第 12.5.9 节 DoD（单个/批量/热门下载、限流/重试/并发、任务可视化状态、取消与失败重试、测试通过），并补充最新运行验证记录。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/RateLimiterService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 23，失败 0，成功 23）；已使用 Debug 配置启动 WPF：`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug`（后台拉起 5 秒无显式错误输出，随后主动停止进程）。
4. DoD 判定：是。阶段 4 与第 12.5.9 节要求保持一致，当前可持续视为已完成状态。
5. 下次计划：进入阶段 5（同步能力迁移），先建立“元数据同步入库 + 状态展示”最小闭环。

### 1.18 2026-03-15，阶段 4 下载入参兼容修复（RunSingleAsync 400）

1. 变更摘要：修复下载执行链路中 `sourceId` 输入兼容性问题。对队列入参与 API 客户端调用参数统一做 RJID 规范化（支持从 URL/混合文本中提取 `RJ\d+`），避免 `RunSingleAsync` 中调用 `/api/work/{id}`、`/api/tracks/{id}` 时因非法 id 触发 `400 Bad Request`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchStateStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 21，失败 0，成功 21）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln` 通过（总计 38，失败 0，成功 38）。
4. DoD 判定：是。阶段 4 既有 DoD 不受影响且稳定性提升（手动输入链接场景可用）。
5. 下次计划：进入阶段 5（同步能力迁移），保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.19 2026-03-15，阶段 4 下载页输入框实时归一化体验优化

1. 变更摘要：下载页新增输入侧体验优化。单个与批量输入框在 `TextChanged` 时实时归一化 URL/混合输入并显示为 `RJxxxx`，使用户在入队前可直接确认最终下载 ID；入队逻辑同步复用统一归一化器，确保 UI 显示与后端实际请求一致。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadInputNormalizer.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizationTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 3，失败 0，成功 3）；`dotnet test dotnet/Asmroner.sln` 通过（总计 40，失败 0，成功 40）。
4. DoD 判定：是。该变更为阶段 4 的可用性增强，不改变既有功能语义并提升输入透明度。
5. 下次计划：进入阶段 5（同步能力迁移），继续保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.20 2026-03-16，阶段 4 下载入参兼容补强（RunSingleAsync 400 回归修复）

1. 变更摘要：对 `sourceId` 归一化策略进行补强，新增统一 `SourceIdNormalizer` 并在 Application、Infrastructure、WPF 三层复用；在原有 URL 提取基础上补齐 `RJ-12345`、`RJ 12345`、纯数字 `12345` 等非标准输入格式，统一规范为 `RJ12345`，避免下载调用 `/api/work/{id}`、`/api/tracks/{id}` 时出现 `400 Bad Request`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Utils/SourceIdNormalizer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadInputNormalizer.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchStateStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 21，失败 0，成功 21）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 14，失败 0，成功 14）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 3，失败 0，成功 3）；`dotnet test dotnet/Asmroner.sln` 通过（总计 42，失败 0，成功 42）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，且输入兼容性边界进一步完善。
5. 下次计划：进入阶段 5（同步能力迁移），保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.21 2026-03-16，阶段 4 API 路径数字化修复（根本原因定位与修复）

1. 变更摘要：定位并修复"400 Bad Request"根本原因。`AsmrApiClient.GetWorkInfoAsync`/`GetTracksAsync` 此前调用 `SourceIdNormalizer.Normalize()` 拼接 API 路径，导致 URL 形如 `/api/work/RJ01426915`；而 asmr.one API 仅接受纯数字 ID（如 `/api/work/01426915`），故返回 400。与 Go 源码行为对齐（`IsValidDlsiteID` 提取 `number` 后以纯数字调用 API），在 `SourceIdNormalizer` 新增 `ToApiNumericId()` 方法（仅返回数字部分），并在 `AsmrApiClient` 两个方法中改用该方法；同步修正两个断言 API 路径含 `RJ` 前缀的错误测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Utils/SourceIdNormalizer.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，1.4 节阻塞项"400 Bad Request"已关闭。
5. 下次计划：进入阶段 5（同步能力迁移），保留"下载格式偏好与限流可观测性"测试补齐项。

### 1.22 2026-03-16，阶段 4 下载页体验增强（布局/筛选/图片视频格式/目录结构）

1. 变更摘要：对 Download 页面进行六项增强：
 - **布局调整**：将单个/批量输入拆为独立第一行，文件筛选+热门入队合并为第二行，避免"加入热门下载"按钮超出界面范围；按钮宽度由 120px 缩减至 110px，各间距收紧为 6px。
 - **文件筛选**：新增 `DownloaderOptions.FileFilter` 配置字段（分号分隔，`+`/无前缀为必含条件，`-`前缀为排除条件），Download 页添加"文件筛选"输入框并在 `Loaded` 时从配置自动填充默认值；执行队列/重试时将当前筛选文本传入服务层，`DownloadService` 新增 `ParseFileFilter` / `MatchesFileFilter` 方法在选文件阶段过滤；`IDownloadService.RunQueuedAsync` / `RetryFailedAsync` 新增可选 `fileFilter` 参数，已有调用方无影响。
 - **入队立即显示**：点击"加入单个/批量下载"后，`RefreshView()` 将 `ISearchStateStore` 中未进入执行列表的 SourceId 以 `StatusText="未下载"` 的占位 ViewModel 展示在任务列表顶部；TaskId 为 `Guid.Empty` 的占位项不参与 取消/重试 命令的可用性判定。
 - **Status 中文化**：任务列表 Status 列改绑 `StatusText`，枚举值映射为"等待执行 / 下载中 / 已完成 / 失败 / 已取消 / 未下载（占位）"。
 - **图片/视频格式**：新增 `DownloaderOptions.PreferImage`（默认空）/ `PreferVideo`（默认空）字段；`DownloadService.ParsePreferExtensions` 合并三组格式扩展名；Settings 页新增对应两个输入框及"下载筛选默认规则"字段输入框。
 - **目录结构下载**：`FlattenTracksWithPath` 替代原 `FlattenTracks`，递归携带相对路径；下载时按 `targetDirectory / relativePath / fileName` 创建子目录并写文件，`CurrentFile` 同步显示相对路径；文件名去掉原有顺序前缀，直接使用 API 返回的 Title。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DownloadService.cs`（占位）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 成功（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足；本次增强均属可用性与功能扩展，不引入回退。
5. 下次计划：进入阶段 5（同步能力迁移）；补齐"格式偏好筛选"与"限流可观测性"单测样例。

### 1.23 2026-03-16，阶段 4 下载配置与入队体验优化（格式整合 + 异步 WorkInfo）

1. 变更摘要：
 - **Settings 格式配置整合**：将原有"音频格式优先级"、"图片格式"、"视频格式"三个输入框整合为单一输入框，标题统一为"格式优先级（留空则全部下载）"，并在提示中补充常用文本格式（`txt,md,lrc,srt,ass,json,cue`）与常见音频/图片/视频格式示例。
 - **配置与解析逻辑更新**：新增 `DownloaderOptions.PreferFormats`；`ConfigurationService` 支持 `prefer_formats` 读写，并保留 `prefer_media/prefer_image/prefer_video` 兼容读取；`DownloadService` 统一按 `PreferFormats` 进行扩展名过滤，若留空则不做扩展名限制（全部下载）。
 - **单个/批量入队前异步刷新 WorkInfo**：`DownloadView` 的"加入单个下载"与"加入批量下载"改为异步流程，先并发拉取 `GetWorkInfoAsync` 再入队并刷新列表，避免 UI 线程阻塞；未开始任务在列表中显示 `Status=未下载` 且可显示已获取的标题。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Core.Tests/UnitTest1.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 通过（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。阶段 4 功能与稳定性持续满足，且下载页面交互流畅性提升。
5. 下次计划：阶段 5 同步能力迁移；补充 `PreferFormats` 过滤与"入队前 WorkInfo 预取"的专项测试样例。

### 1.24 2026-03-16，阶段 4 下载链路增强（默认格式、WorkInfo 内存复用、目录命名）

1. 变更摘要：
 - **默认格式优先级更新**：默认值调整为 `mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass`；Settings 页示例同步更新。
 - **WorkInfo 内存复用**：新增 `IDownloadService.UpsertPrefetchedWorkInfo`，`DownloadView` 在"加入单个/批量下载"时预取 WorkInfo 后写入下载服务内存缓存；下载执行阶段优先命中缓存，不再重复请求 `GetWorkInfoAsync`。
 - **目录命名调整**：`BuildFolderName` 改为 `[{SourceId}]{Title}`，并保持非法字符清洗规则。
 - **专项测试补齐**：新增 `RunQueuedAsync_ShouldDownloadAllFormats_WhenPreferFormatsEmpty`（留空全下载）与 `RunQueuedAsync_ShouldUsePrefetchedWorkInfo_WithoutApiWorkInfoCall`（缓存命中不再查 WorkInfo）；同步更新目录命名断言与默认配置断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Core.Tests/UnitTest1.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 通过（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 45，失败 0，成功 45）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，下载链路一致性与性能进一步提升。
5. 下次计划：阶段 5 同步能力迁移；补齐“下载限流可观测性”专项测试。

### 1.25 2026-03-16，阶段 4 下载页交互修复与命名优化（输入/目录/列表/Search）

1. 变更摘要：
 - **批量输入体验修复**：`BatchSourceIdsTextBox` 取消实时改写，不再在 `TextChanged` 阶段强制归一化；改为点击“加入批量下载”时统一规范化，修复空格/逗号/分号输入与多次粘贴第二个 RJID 失败问题。
 - **设置页文案调整**：Settings 页“同步目录”改为“下载目录”。
 - **打开下载目录**：Download 页新增“打开下载目录”按钮，点击后读取当前配置目录（为空则回退默认目录），自动创建并打开系统文件夹。
 - **表格列优化**：移除 `CurrentFile` 列；`TargetDirectory` 移到 `Error` 前；`Status` 列宽收窄；全部列标题改为中文（作品ID/标题/状态/进度/下载目录/错误信息）。
 - **取消按钮可用性修复**：允许“未下载占位任务（TaskId=Empty）”参与取消；取消时对占位任务直接从队列移除并刷新按钮状态，修复选中任务后“取消选中任务”按钮不可用问题。
 - **页面命名**：主 Tab “Dashboard” 更名为 “Search”。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadInputNormalizer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizerTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadCommandAvailabilityTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 通过（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 47，失败 0，成功 47）。
4. DoD 判定：是。阶段 4 功能保持稳定，下载页可用性显著提升。
5. 下次计划：阶段 5 同步能力迁移；补齐“下载限流可观测性”专项测试。

### 1.26 2026-03-16，阶段 4 Search/Download 联动修复与交互收口

1. 变更摘要：
 - **取消语义统一**：Download 页对“未下载占位任务（TaskId=Empty）”执行取消时，统一在列表中展示为“已取消”，避免与真实任务状态语义不一致。
 - **Search 入队标题透传修复**：在下载服务新增 `GetPrefetchedWorkInfoSnapshot`，Search 页入队时写入预取 WorkInfo，Download 页刷新时可回填待执行任务标题，修复“Search 入队后 Download 标题为空”问题。
 - **热门查询入口迁移**：移除 Download 页“热门数量/加入热门下载”，在 Search 页新增“查询热门作品”按钮，并复用现有搜索结果列表展示。
 - **Search 列与筛选项优化**：结果列顺序调整为“作品ID/封面/标题/字幕/日期/收藏/年龄限制”；排序字段、方向、字幕下拉改为中文显示（内部值通过 `Tag` 透传，不影响 API 参数）。
 - **构造函数签名修复**：`DashboardView` 默认构造函数参数补齐，匹配新增依赖注入项。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Release --nologo` 通过（9/9 项目成功）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 48，失败 0，成功 48，含新增测试）。
4. DoD 判定：是。阶段 4 本轮联动修复已完成并通过全量回归。
5. 下次计划：进入阶段 5（同步能力迁移），持续补齐“下载限流可观测性”专项测试。

### 1.27 2026-03-16，阶段 4 Download 交互补强与热门接口 404 修复

1. 变更摘要：
 - **取消后重加同一行复用**：新增 `DownloadTaskStatus.Pending = 0`（未下载）语义；当已取消占位任务被重新加入时，不再出现重复行，而是同一 `SourceId` 行状态从“已取消”恢复为“未下载”。
 - **按钮布局与新增操作**：将“执行下载队列”按钮移至“加入批量下载”右侧；在操作区新增“立即下载选中任务”，支持对 `Pending/Failed/Canceled` 选中项直接触发下载。
 - **单项输入规范化时机调整**：`SingleSourceIdTextBox` 取消 `TextChanged` 实时改写，改为点击“加入单个下载”时再统一规范化，避免输入被打断。
 - **热门查询 404 修复**：`GetPopularAsync` 改为与 Go 实现一致的 `POST /api/recommender/popular`（携带分页 body），并将返回 `works` 映射为 `HotWorkDto`。
 - **测试补齐**：新增热门接口 POST 回归测试，更新 Download 命令可用性测试以覆盖 `Pending` 与“立即下载”可用状态。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadTaskItem.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadCommandAvailability.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewModelTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 50，失败 0，成功 50）。
4. DoD 判定：是。阶段 4 本轮交互与接口修复完成，且全量回归通过。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立“同步执行状态可视化 + 失败重试”最小闭环。

### 1.28 2026-03-16，阶段 4 立即下载复用修复与高级筛选默认值补强

1. 变更摘要：
 - **立即下载复用当前行**：`IDownloadService.StartAsync` 新增 `preferredTaskId` 参数，Download 页“立即下载选中任务”在选中 `Failed/Canceled` 行时优先复用该行对应任务对象并重置状态后执行，避免创建新行。
 - **高级筛选无结果修复**：修复搜索链路中 query 的二次 URL 编码问题（`AsmrApiClient.SearchAsync` 不再重复编码），恢复高级筛选条件可用性。
 - **默认高级筛选 age:general**：在查询解析层缺省注入 `age:general`；Dashboard 页默认将 age 输入设为 `general`，清空后也恢复该默认值。
 - **测试补齐**：新增“失败任务复用重启”“取消任务复用重启”“默认 age 注入”“Search 不二次编码”回归测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 54，失败 0，成功 54）。
4. DoD 判定：是。本轮 5 项需求全部完成，且全量回归通过。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.29 2026-03-16，阶段 4 立即下载新任务失败后行消失修复

1. 变更摘要：
 - **根本原因**：`DownloadTaskItem.TaskId` 默认为 `Guid.NewGuid()`，永远不是 `Guid.Empty`，导致 `StartAsync` 中的条件 `if (task.TaskId == Guid.Empty)` 恒为 false，使立即下载未执行过的新任务时，新建的 `DownloadTaskItem` 从未加入 `_tasks`。下载失败后 `GetTasks()` 查不到该任务，UI 刷新后行消失。
 - **修复**：将 `if (task.TaskId == Guid.Empty)` 改为 `if (!_tasks.Contains(task))`，确保任何新建任务（非复用行）都被加入 `_tasks`，失败后状态正常保留。
 - **测试补齐**：新增 `StartAsync_ShouldTrackFailedTask_WhenNewTaskFails` 回归测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 55，失败 0，成功 55）。
4. DoD 判定：是。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.30 2026-03-16，阶段 4 Download 页面视觉优化（仅界面）

1. 变更摘要：
 - **界面美化（不改逻辑）**：仅调整 `DownloadView.xaml` 的视觉层，新增页面背景渐变、卡片容器、按钮统一样式、输入框样式、表格表头/行配色与状态信息区样式。
 - **布局优化**：将“输入区 / 筛选区 / 操作区 / 任务表格”分区为卡片式结构，优化间距、层次与可读性；保留所有控件 `x:Name` 与事件绑定不变。
 - **测试补齐**：新增 `DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls`，校验 Download 页面 XAML 关键样式资源与核心控件仍存在。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 6，失败 0，成功 6）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 56，失败 0，成功 56）。
4. DoD 判定：是。本轮仅做界面层改造，逻辑与后端行为保持不变。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.31 2026-03-16，阶段 4 多页面 UI 统一风格与窗口适配（仅界面）

1. 变更摘要：
 - **默认窗口尺寸**：`MainWindow` 默认与最小尺寸统一调整为 `1280x720`，提升多区块页面在首屏显示稳定性。
 - **Download 适配优化（仅 XAML）**：输入与操作区改为可换行布局，避免在新窗口尺寸下控件挤压；任务表格开启水平/垂直滚动条自动显示。
 - **Search/Settings 风格统一（仅 XAML）**：`DashboardView` 与 `SettingsView` 引入与 Download 一致的卡片化容器、渐变背景、统一按钮/输入框/表头视觉规范；保留全部 `x:Name` 与事件绑定不变。
 - **测试补齐**：新增 `ShellAndPageXamlTests`，覆盖主窗体尺寸、Search 样式资源与核心控件、Settings 卡片分区与动作按钮存在性。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/ShellAndPageXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 10，失败 0，成功 10）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 59，失败 0，成功 59）。
4. DoD 判定：是。本轮严格限制在界面层（XAML）改造，未改动控件逻辑与后端代码。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.32 2026-03-16，阶段文案清理与 Search 下拉框对齐修复（仅界面）

1. 变更摘要：
 - **阶段文案清理**：移除 Download/Search/Settings 页面标题区域中“阶段 X：...”文案前缀，统一为纯功能描述文本。
 - **Search 下拉框样式修复**：为 `FieldComboBoxStyle` 增加 `VerticalContentAlignment` 与 `HorizontalContentAlignment`；新增 `ComboBoxItem` 样式，统一内容居中与内边距，修复下拉框文本不对齐问题。
 - **测试补齐**：在 `ShellAndPageXamlTests` 增加“页面不含阶段前缀文案”与“Search 下拉框对齐样式存在性”测试。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/ShellAndPageXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 61，失败 0，成功 61）。
4. DoD 判定：是。本轮仅调整 XAML 与 XAML 文本断言测试，未改动任何控件逻辑及后端代码。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.33 2026-03-17，阶段 4 稳定性重构推进（解析逻辑下沉 + 并发回归）

1. 变更摘要：继续按“低风险渐进”执行稳定性重构。将 `DownloadService` 中纯解析/筛选逻辑（格式优先级解析、文件筛选解析、筛选匹配）下沉到 Core 新增工具 `DownloadFilterParser`，应用层仅保留调用与下载流程编排；并保持前序并发状态锁收敛语义不变。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadFilterParser.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Core.Tests/DownloadFilterParserTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj -c Release --nologo` 通过（总计 5，失败 0，成功 5）；`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮为阶段 4 内部可维护性与并发安全收敛，不改变对外行为；核心与应用层回归均通过。
5. 下次计划：继续执行“WPF 下载页逻辑最小下沉 + 对应测试补强”，并在完成后同步更新 progress 文档。

### 1.34 2026-03-17，阶段 4 WPF 下载页最小逻辑下沉（列表拼装与状态映射）

1. 变更摘要：将 Download 页 code-behind 中“任务行状态文本映射 + 队列/取消占位行拼装”逻辑下沉到可测试组件：新增 `DownloadTaskRowViewModel` 与 `DownloadTaskListComposer`；`DownloadView` 改为直接调用组合器输出行数据，UI 绑定字段保持不变。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskRowViewModel.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 14，失败 0，成功 14）。
4. DoD 判定：是。本轮仅做逻辑下沉与可测试性增强，不改下载交互语义。
5. 下次计划：继续执行 Download 页事件处理分支收敛（例如批量操作中的可复用流程抽取）并补对应测试。

### 1.35 2026-03-17，阶段 4 WPF 事件分支收敛（任务选择策略下沉）

1. 变更摘要：将 Download 页事件处理中可纯化的“任务选择规则”下沉到 `DownloadTaskSelectionPolicy`，包含“可取消任务筛选”与“立即下载目标去重筛选（按 SourceId）”；`DownloadView` 取消与立即下载事件改为调用该策略，减少 code-behind 内联分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskSelectionPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskSelectionPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 16，失败 0，成功 16）。
4. DoD 判定：是。本轮仅做选择策略下沉，不改变原有交互语义。
5. 下次计划：继续收敛 Download 页异步操作中的重复 UI 开关/状态更新流程，并补对应单测。

### 1.36 2026-03-17，阶段 4 WPF 异步护栏收敛（参数归一化 + UI 开关复用）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadExecutionArgs.NormalizeFileFilter` 统一文件筛选参数归一化（空白转 `null`）；在 `DownloadView` 中引入 `ExecuteGuardedAsync` 复用异步操作的按钮禁用/恢复、异常记录与失败状态文案更新，覆盖单个/批量入队、执行队列、单项重试、批量重试、立即下载等流程，保持交互语义不变。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadExecutionArgs.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadExecutionArgsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 20，失败 0，成功 20）。
4. DoD 判定：是。本轮仅做参数与异步护栏复用，不改变下载流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化分支（如提示文案组装与确认弹窗策略）并按同样方式小步下沉。

### 1.37 2026-03-17，阶段 4 WPF 提示文案下沉（确认弹窗文案纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationPrompts` 统一生成“取消任务确认”与“批量重试确认”文案；`DownloadView` 改为调用提示构造器，移除事件内联拼装细节。同步将取消任务执行流程接入 `ExecuteGuardedAsync`，与既有重试/执行队列流程保持一致的 UI 开关与异常处理路径。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrompts.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPromptsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 23，失败 0，成功 23）。
4. DoD 判定：是。本轮仅做提示文案下沉与护栏一致性收敛，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如状态文案模板与确认动作策略）并按同样方式小步下沉。

### 1.38 2026-03-17，阶段 4 WPF 状态文案模板下沉（结果文案纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationStatusTexts`，统一生成批量入队结果、执行队列结果、取消结果、单项重试结果、批量重试结果、立即下载结果文案；`DownloadView` 对应分支改为调用模板函数，减少事件内联三元分支与字符串拼装。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationStatusTexts.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationStatusTextsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮仅做状态文案模板下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如确认动作策略与状态提示输出协作）并按同样方式小步下沉。

### 1.39 2026-03-17，阶段 4 WPF 确认动作策略下沉（确认决策纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadConfirmationPolicy`，将确认弹窗返回值到“是否继续执行/取消提示文案”的决策抽离为纯函数；`DownloadView` 新增统一 `ConfirmWithQuestion` 并接入“取消任务”“批量重试”两条流程，移除重复确认分支判断。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadConfirmationPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadConfirmationPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 33，失败 0，成功 33）。
4. DoD 判定：是。本轮仅做确认决策下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如筛选与状态提示协作策略）并按同样方式小步下沉。

### 1.40 2026-03-17，阶段 4 WPF 操作前置校验下沉（入口校验纯策略化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationPrecheckPolicy`，将取消/单项重试/批量重试/立即下载的入口校验（选中数量、状态合法性、占位任务限制）统一下沉为纯策略返回值；`DownloadView` 事件入口改为消费策略结果，减少内联校验分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrecheckPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPrecheckPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 38，失败 0，成功 38）。
4. DoD 判定：是。本轮仅做入口校验下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如操作流水线上下文对象化）并按同样方式小步下沉。

### 1.41 2026-03-17，阶段 4 WPF 下载目录路径策略下沉（路径选择纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadDirectoryPathPolicy`，将“配置目录为空则回退默认目录、否则去首尾空白后使用配置目录”的路径选择逻辑下沉为纯函数；`DownloadView` 的“打开下载目录”流程改为调用策略结果，减少事件内联条件分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadDirectoryPathPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadDirectoryPathPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 41，失败 0，成功 41）。
4. DoD 判定：是。本轮仅做目录路径选择逻辑下沉，不改变打开目录流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如流程上下文对象化与分支聚合）并按同样方式小步下沉。

### 1.42 2026-03-17，阶段 4 WPF 队列缓存对齐策略下沉（缓存合并纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadQueueCachePolicy`，将“预取标题合并 + 活跃任务 sourceId 清理（标题缓存/状态覆盖缓存）”的队列缓存对齐逻辑下沉为纯函数；`DownloadView.RefreshView` 改为消费策略结果并统一替换本地缓存，减少内联循环分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadQueueCachePolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。本轮仅做缓存对齐策略下沉，不改变下载列表展示与状态计算对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中的执行参数上下文聚合）并按同样方式小步下沉。

### 1.43 2026-03-17，阶段 4 WPF WorkInfo 标题提取策略下沉（标题过滤纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadWorkInfoTitlePolicy`，将 WorkInfo 集合到“非空标题映射”的提取与过滤规则下沉为纯函数；`DownloadView` 的单项/批量入队与 `DownloadQueueCachePolicy` 的预取标题合并统一复用该策略，避免重复标题过滤逻辑。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadWorkInfoTitlePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadWorkInfoTitlePolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 45，失败 0，成功 45）。
4. DoD 判定：是。本轮仅做标题提取规则下沉与复用，不改变下载列表与入队流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如执行流程上下文对象化）并按同样方式小步下沉。

### 1.44 2026-03-17，阶段 4 WPF 执行参数上下文下沉（流程参数对象化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationContext`，将执行相关流程（执行队列、单项重试、批量重试、立即下载）使用的筛选参数规范化结果统一封装为上下文对象；`DownloadView` 对应事件改为消费 `context.FileFilter`，减少事件内联参数处理。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationContext.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationContextTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 47，失败 0，成功 47）。
4. DoD 判定：是。本轮仅做执行参数上下文化，不改变下载流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中重复任务计数与状态文案协作）并按同样方式小步下沉。

### 1.45 2026-03-17，阶段 4 WPF 任务快照筛选策略下沉（任务筛选纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadTaskSnapshotPolicy`，将“失败任务筛选”与“活跃 sourceId 集合提取”下沉为纯函数；`DownloadView` 的“批量重试失败任务”和 `RefreshView` 改为复用策略结果，减少事件内联筛选逻辑。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskSnapshotPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskSnapshotPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 49，失败 0，成功 49）。
4. DoD 判定：是。本轮仅做任务筛选策略下沉，不改变重试与列表刷新对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中的批处理执行器下沉）并按同样方式小步下沉。

### 1.46 2026-03-17，阶段 4 WPF 单元测试文件按目标文件整合

1. 变更摘要：对 WPF 测试工程进行按目标文件的结构整合。将 `DownloadInputNormalizationTests` 更名为 `DownloadInputNormalizerTests`，将 `DownloadViewModelTests` 更名为 `DownloadCommandAvailabilityTests`；把原 `ShellAndPageXamlTests` 拆分为 `MainWindowXamlTests`、`DashboardViewXamlTests`、`SettingsViewXamlTests`，并在 `DownloadViewXamlTests` 中补上自身的“无阶段前缀文案”断言；新增共享的 `XamlTestPathLocator` 以复用 XAML 路径定位逻辑。
2. 关键文件：`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizerTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DashboardViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/XamlTestPathLocator.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 51，失败 0，成功 51）。
4. DoD 判定：是。本轮只整合测试文件结构与文档映射，不改动生产代码逻辑。
5. 下次计划：继续按目标文件维度维护测试清单与覆盖关系，避免再次出现单文件覆盖多个目标文件的情况。

### 1.47 2026-03-17，阶段 4 后端单元测试文件按目标文件整合

1. 变更摘要：继续按“目标文件对应测试文件”的规则整合后端测试工程。将 `Application.Tests/UnitTest1.cs` 重命名为 `FirstRunServiceTests.cs`，将 `Core.Tests/UnitTest1.cs` 重命名为 `AppConfigTests.cs`；把 `Infrastructure.Tests/UnitTest1.cs` 中的配置读写用例并入 `ConfigurationServiceTests.cs`，数据库初始化用例拆分到 `DatabaseInitializerTests.cs`；把 `IntegrationTests/UnitTest1.cs` 并入 `ApplicationBootstrapperTests.cs`，统一由同一目标文件的测试文件承载。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/FirstRunServiceTests.cs`、`dotnet/tests/Asmroner.Core.Tests/AppConfigTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/DatabaseInitializerTests.cs`、`dotnet/tests/Asmroner.IntegrationTests/ApplicationBootstrapperTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）；`dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj -c Release --nologo` 通过（总计 5，失败 0，成功 5）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）；`dotnet test dotnet/tests/Asmroner.IntegrationTests/Asmroner.IntegrationTests.csproj -c Release --nologo` 通过（总计 3，失败 0，成功 3）。
4. DoD 判定：是。本轮只整合测试文件结构与文档映射，不改动生产代码逻辑。
5. 下次计划：继续检查 Infrastructure/Application 内是否仍存在跨目标文件混放的测试文件，并按同样规则收口。

### 1.48 2026-03-17，阶段 4 扫描并整合残留测试文件（开始）

1. 变更摘要：按“目标文件对应测试文件”规则继续向后端与 WPF 测试工程扩展，扫描并识别命名不当或覆盖多目标的测试样例，计划逐一拆分/重命名并补齐对应测试文件，保证每个生产目标文件对应一个明确的测试文件。
2. 关键文件（扫描发现）：`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（存在综合性基础设施用例，需按目标拆分）；扫描将继续并生成待改动清单。
3. 验证结果：扫描已发现候选文件（见上）；尚未实施拆分/重命名改动，当前测试基线保持不变。
4. DoD 判定：进行中。阶段 4 最终 DoD 为：所有跨目标或命名不当的测试文件已拆分/重命名并通过对应项目回归测试。
5. 下次计划：
 - 按待改动清单逐个拆分/重命名测试文件并运行受影响测试项目；
 - 每完成一项改动即在 `1.5` 节追加新的变更记录（遵守下述变更策略）。

变更策略：自本条起，`1` 节仅追加新的记录，不再修改已有历史记录；如需修正前一次记录的描述或补充证据，将在后续新条目中引用对应条目编号并补充说明。

### 1.49 2026-03-17，阶段 4 基础设施测试文件拆分（`ApiInfrastructureTests` 收口）

1. 变更摘要：将混合覆盖多个目标文件的 `dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs` 按生产目标文件拆分并并入现有专用测试文件：`AsmrApiClient_ShouldMapHttpErrors` 并入 `AsmrApiClientTests.cs`，`AuthService_ShouldStoreTokenAfterLogin` 与 `AuthService_ShouldThrowReadableErrorOnFailure` 并入 `AuthServiceTests.cs`，`EndpointDiscoveryService_ShouldPickFastestReachableCandidate` 并入 `EndpointDiscoveryServiceTests.cs`；同时新增 `TokenStoreTests.cs` 承载 `TokenStore_ShouldRoundTripToken`，随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/TokenStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且基础设施测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否仍存在“一个测试文件覆盖多个目标文件”或“文件名与目标文件不一致”的情况，并按同样规则逐项收口。

### 1.50 2026-03-17，阶段 4 应用层搜索测试文件拆分（`SearchWorkflowTests` 收口）

1. 变更摘要：将混合覆盖 `QueryParserService`、`SearchService`、`SearchExportService` 三个目标的 `dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs` 按目标文件拆分：`QueryParser_ShouldParseAdvancedQuery` 并入 `QueryParserServiceTests.cs`，`SearchService_ShouldAggregateMultiplePages` 与 `SearchService_ShouldRespectRequestedPageAndKeepFilters` 并入 `SearchServiceTests.cs`，`SearchExportService_ShouldExportCsvAndJson` 并入 `SearchExportServiceTests.cs`；随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchExportServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且应用层测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否存在跨目标聚合测试文件，并按同样规则拆分收口。

### 1.51 2026-03-17，阶段 4 下载服务测试文件并档（`DownloadWorkflowTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs` 中所有 `DownloadService` 行为用例并入 `DownloadServiceTests.cs`，统一由单文件承载同一目标 `DownloadService.cs` 的测试；为避免重复桩逻辑，扩展 `DownloadServiceTestDoubles.cs` 的 `ScriptedApiClient`（补充并发观测、延迟与失败次数控制）并新增 `DelayRateLimiterService`，随后删除 `DownloadWorkflowTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描其他测试工程是否仍存在“同一目标由多个测试文件分散承载”或“跨目标聚合”情况，并按同样规则收口。

### 1.52 2026-03-17，阶段 4 下载路径测试并档（`DownloadPathTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs` 中的目录命名兼容性用例 `DirectoryNameStrategy_ShouldMatchGoCompatibilityRule` 并入 `DownloadServiceTests.cs`，统一由同一测试文件承载 `DownloadService.cs` 的路径与工作流行为；随后删除原 `DownloadPathTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描 WPF/Infrastructure/Integration 测试工程中是否仍有同目标分散或跨目标聚合测试文件，并逐项收口。

### 1.53 2026-03-17，阶段 4 基础设施测试依赖收敛（`AsmrApiClientTests`）

1. 变更摘要：对 `AsmrApiClientTests.cs` 进行依赖收敛，`AsmrApiClient_ShouldMapHttpErrors` 用例不再实例化真实 `AuthService`，改为使用 `StubAuthService` 直接提供 token，仅验证 `AsmrApiClient` 在服务端返回 500 时的错误映射行为，避免该测试文件对 `AuthService` 目标行为产生耦合。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮未改变测试文件映射结构，但进一步收敛了测试职责边界，且基础设施测试工程回归通过。
5. 下次计划：继续做最后一轮全仓扫描，若无新的跨目标聚合/同目标分散候选，则进入阶段 4 测试结构收口总结。

### 1.54 2026-03-17，阶段 4 测试结构收口复核（全仓扫描 + 解决方案回归）

1. 变更摘要：执行全仓测试文件复核扫描（Application/Core/Infrastructure/Integration/Wpf），确认当前未发现新的“跨目标聚合”或“同目标分散”高优先级候选；对少量仍出现多类型构造的文件判定为测试桩与协作者注入场景，不再进行机械拆分，避免过度拆分导致可读性下降。
2. 关键文件：本轮无代码结构性变更，主要为扫描与验证。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 105，失败 0，成功 105）。
4. DoD 判定：是。阶段 4 当前测试结构收口目标已满足，且解决方案级回归通过。
5. 下次计划：进入阶段 4 业务能力迁移实现与验证，测试结构后续仅按增量功能做最小维护。

### 1.55 2026-03-17，阶段 3/4 搜索与下载规则修正（默认 age 移除 + 全局规则 + 入队状态修复）

1. 变更摘要：按计划完成 Search/Download 规则收口：移除 `age:general` 默认注入；新增 `downloader.global_search_rule` 配置并在 Search 页启动时填充高级筛选；高级筛选值分解支持空格/分号/逗号，并实现“值前负号 + 反选勾选”组合语义；修复 Search 重新入队时 `Canceled` 占位状态应回到 `Pending`；移除 Download 页“示例：+voice;-cover”可见文案。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 30，失败 0，成功 30）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 17，失败 0，成功 17）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 52，失败 0，成功 52）；`dotnet test dotnet/Asmroner.sln` 通过（总计 107，失败 0，成功 107）。
4. DoD 判定：是。本轮计划内 9 项规则改造均已落地并经解决方案级回归验证通过。
5. 下次计划：进入下一批业务功能迁移时，沿用“配置项 + 行为测试 + 文档清单同步”同批提交策略，避免规则漂移。

### 1.56 2026-03-17，阶段 3/4 Search/Download 缺陷修复（二次入队状态回流 + 全局规则防重 + Pending 标题保留）

1. 变更摘要：按顺序完成三项缺陷修复：
 - 修复 Search 加入下载队列时，同 SourceId 已存在且状态为 `Canceled` 的任务应复用并回流为待执行状态，避免重复创建任务行。
 - 修复 Search 页面往返切换后“全局搜索规则”重复追加到高级筛选的问题，新增筛选值防重合并策略。
 - 修复 Download 执行队列后刷新列表时 Pending 任务标题丢失的问题：刷新缓存对齐不再清理活跃任务标题缓存，并在任务行组装时增加标题兜底。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchFilterValuePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchFilterValuePolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadQueueCachePolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 31，失败 0，成功 31）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 55，失败 0，成功 55）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 111，失败 0，成功 111）。
4. DoD 判定：是。本轮 3 项缺陷均完成修复并通过解决方案级回归。
5. 下次计划：进入后续功能迁移时继续沿用“先修复行为、再补测试、最后同步文档与整解回归”的同批闭环流程。

### 1.57 2026-03-17，阶段 4 Search/Download 增强优化（全局规则启动加载 + Canceled 状态同步 + 窗体自动增高 + 状态排序与标签 Attribute 化）

1. 变更摘要：按顺序完成 8 项增强优化：
 - **需求 1**：全局搜索规则仅在启动时加载一次，防止在 Search/Download 页面中间切换时重复注入。
 - **需求 2**：修复 Canceled 任务从 Search 侧重新入队后，Download 列表仍显示"已取消"而非"未下载"的问题，通过在缓存协调时清除已重新入队的 Canceled 覆盖标记。
 - **需求 3**：Search 页高级筛选展开时，自动计算内容超出量并增高主窗体高度；不在折叠时缩小；增高上限为屏幕可用高度的 90%。
 - **需求 4**：在 `DownloadTaskStatus` 枚举值上新增自定义 `[Order]` 属性，指定排序优先级（Completed→Running→Queued→Pending→Failed→Canceled）。
 - **需求 5**：在 `DownloadTaskStatus` 枚举值上新增 `[Display]` 属性，指定中文标签（已完成/下载中/待下载/未下载/已失败/已取消）；替换 `DownloadTaskRowViewModel` 等处硬编码 switch 语句。
 - **需求 6**：在 `DownloadTaskListComposer.ComposeRows` 返回前新增统一排序：首先按状态正序（Attribute 驱动），其次按 SourceId 正序，确保首次加载与刷新走同一排序路径。
 - **需求 7**：为排序行为、状态标签读取、Canceled 状态同步补齐单元测试，共新增 14 个测试用例。
 - **需求 8**：本条目本身（1.5.57）及同步测试清单（2.1）。
2. 关键文件：
 - `dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadTaskItem.cs`（新增 Attribute、中文标签）
 - `dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadTaskStatusExtensions.cs`（新增扩展方法读取 Attribute）
 - `dotnet/Asmroner.Backend/Asmroner.Core/OrderAttribute.cs`（新建自定义 Order 属性）
 - `dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`（新增 Expander.Expanded 事件）
 - `dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`（新增一次性启动标记、窗体自动增高事件处理）
 - `dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskRowViewModel.cs`（改用 GetDisplayName() 替代 switch）
 - `dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`（新增 ReconcileWithQueuedSourceIds 方法）
 - `dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`（新增排序逻辑）
 - `dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`（新增排序与状态同步测试）
 - `dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskStatusExtensionsTests.cs`（新建，验证 Attribute 读取）
3. 验证结果：
 - `dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo`：总计 69，失败 0，成功 69（+14 新测试）
 - `dotnet test dotnet/Asmroner.sln -c Release --nologo`：总计 125，失败 0，成功 125（+14 新测试）
4. DoD 判定：是。8 项需求均已实现、测试、文档同步完成，无遗漏处理。所有回归测试通过，状态标签一致，排序行为符合预期。
5. 下次计划：继续按"需求分解→顺序实现→测试验证→文档同步"的流程处理后续功能迁移。

### 1.58 2026-03-18，阶段 2 URL 配置化与 Discover 调用收敛

1. 变更摘要：
 - **URL 配置化**：`DownloaderOptions` 新增 `ApiCandidateUrls`、`PublishSourceUrls`、`WorkPageUrlTemplate`，并将 `ApiUrl` 默认值调整为可用基础地址；`ConfigurationService` 新增对应 TOML 字段读写。
 - **发现输入改造**：`AsmrApiOptionsProvider` 支持分号/逗号 URL 列表解析、归一化、去重与默认回退；`AsmrApiOptions` 新增 `PublishSourceUrls`。
 - **调用时机收敛**：新增 `IApiEndpointUrlService` / `ApiEndpointUrlService`，负责“Discover 并回写 `downloader.api_url` + 提供当前生效 BaseUrl”；`AsmrApiClient` 与 `AuthService` 运行时不再直接调用 `DiscoverAsync`。
 - **仅启动/测试连接触发 Discover**：`App` 启动后执行一次 `DiscoverAndPersistAsync`（失败仅记录日志并继续）；`ConnectivityProbeService` 在测试连接时执行 Discover 并持久化后再登录校验。
 - **发现来源配置化**：`EndpointDiscoveryService` 改为使用配置中的发布源列表，而非硬编码发布源常量。
 - **单测补齐**：新增 `AsmrApiOptionsProviderTests`、`ApiEndpointUrlServiceTests`、`ConnectivityProbeServiceTests`，并在 `AsmrApiClientTests` / `AuthServiceTests` / `EndpointDiscoveryServiceTests` 增补回归样例。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Api/AsmrApiOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IApiEndpointUrlService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiOptionsProvider.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConnectivityProbeService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/*.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 28，失败 0，成功 28）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 136，失败 0，成功 136）。
4. DoD 判定：是。本轮完成 URL 配置化、Discover 调用时机收敛、Discover 结果持久化及回归测试补齐，且整解无回归失败。
5. 下次计划：继续推进下一批阶段任务，并沿用“代码改动 + 单测补齐 + progress 同步”同批提交策略。

### 1.59 2026-03-18，阶段 2 启动窗口阻塞修复（异步 warmup 解耦）

1. 变更摘要：
 - **根因修复**：`App` 启动路径移除 `DiscoverAndPersistAsync().GetAwaiter().GetResult()` 同步阻塞，避免 UI 线程在窗口展示前被网络发现流程卡住。
 - **启动流程调整**：改为 `MainWindow.Show()` 后触发后台 endpoint warmup，确保窗口可立即打开。
 - **容错与超时**：新增 `StartupEndpointWarmupService`，统一封装后台 Discover 执行、超时（默认 10 秒）与失败日志，失败不阻塞应用继续启动。
 - **单测补齐**：新增 `StartupEndpointWarmupServiceTests`，覆盖非阻塞调用、后台触发、异常吞吐、超时继续四类场景。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupEndpointWarmupService.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupEndpointWarmupServiceTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 73，失败 0，成功 73）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 140，失败 0，成功 140）。
4. DoD 判定：是。启动窗口可用性修复已完成，且新增 warmup 回归样例通过。
5. 下次计划：继续沿用“先修复行为，再补测试，再同步文档”的闭环节奏推进后续任务。

### 1.60 2026-03-26，状态列排序修正、重复入队防护、高清音频筛选与 CSV/JSON 导入

1. 变更摘要：
 - **Bug 1（状态排序）**：`DownloadTaskRowViewModel` 新增 `StatusSortOrder` 属性，`DownloadView.xaml` 状态列绑定 `SortMemberPath="StatusSortOrder"`，修复按字母序排序问题。
 - **Bug 2（重复入队防护）**：新增 `DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent()`，在 `DownloadView.OnAddSingleClicked`、`OnAddBatchClicked`、`DashboardView.OnQueueClicked` 入队前过滤已存在任务（任意状态）。
 - **Feature 3（高清音频筛选）**：`DownloadFilterParser.FilterHdAudioOnly<T>()` 新增泛型方法；`IDownloadService`/`DownloadService` 三个公开方法新增 `hdAudioOnly = false` 参数；`DownloadOperationContext` 新增 `HdAudioOnly` 字段；`DownloadView.xaml` 新增复选框；调用链完整透传。
 - **Feature 4（CSV/JSON 导入）**：新增 `ISearchImportService`（Core）与 `SearchImportService`（Application）；DI 注册；`DownloadView.xaml` 新增两个导入按钮；Handler 含 OpenFileDialog、去重过滤、WorkInfo 预取与 RefreshView。
2. 关键文件：`DownloadTaskRowViewModel.cs`、`DownloadEnqueueDuplicatePolicy.cs`（新建）、`DownloadOperationContext.cs`、`DownloadView.xaml`、`DownloadView.xaml.cs`、`DashboardView.xaml.cs`、`Asmroner.Wpf/Services/DownloadService.cs`、`App.xaml.cs`、`DownloadFilterParser.cs`、`IDownloadService.cs`、`ISearchImportService.cs`（新建）、`Application/DownloadService.cs`、`SearchImportService.cs`（新建）。
3. 验证结果：`dotnet build dotnet/Asmroner.sln` 成功（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过，总计 173，失败 0，成功 173（+33 新测试）。
4. DoD 判定：是。4 项需求均已实现、单测覆盖、文档同步，回归测试全通过。
5. 下次计划：继续沿用"代码改动 + 单测补齐 + progress 同步"同批提交策略推进后续任务。

### 1.61 2026-03-27，v0.4.1 版本对齐、Search 行为修正与高清音频默认增强

1. 变更摘要：
 - **版本对齐（v0.4.1）**：统一更新 Core/Application/Infrastructure/Wpf 四个项目版本号与程序集版本；同步 `MainWindow`、`Settings`、启动日志、`README` 与进度文档头部版本展示。
 - **Search 清空行为修正**：`DashboardView.OnClearClicked` 改为仅清空关键词、排序、分页与结果；保留高级筛选输入与反选复选框状态。
 - **Search 入队提示计数修正**：新增 `SearchQueueCountPolicy`，统一处理输入重复、已在队列、已在下载列表三类跳过来源，提示文案展示“新增/跳过”准确数量。
 - **热门分页上下文修复**：新增热门模式本地分页缓存（`_isPopularMode` + `_popularResults`），修复热门结果在上一页/下一页/跳页/改页大小时丢失上下文的问题。
 - **单页跳页禁用**：新增 `SearchPagingPolicy`，在总页数为 1 时禁用页码输入框与“跳转”按钮。
 - **高清音频默认与联动**：`DownloadView` 高清音频复选框默认勾选，配置项新增 `downloader.hd_audio_only`（默认 `true`）并支持读写；当高清音频模式移除 mp3 时，同路径同名 `txt/lrc/ass` sidecar 自动跳过。
 - **测试补齐**：新增/更新 Application、Core、Infrastructure、Wpf 相关测试，覆盖版本展示、分页策略、入队计数策略、高清音频默认值与 sidecar 过滤逻辑。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchPagingPolicy.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchQueueCountPolicy.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/tests/**/*.cs`（相关新增与更新）。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过，总计 178，失败 0，成功 178。
4. DoD 判定：是。本批需求实现完成，测试通过，文档同步完成。
5. 下次计划：继续沿用“需求分解 -> 顺序实现 -> 测试验证 -> 文档同步”的闭环推进后续批次。

### 1.62 2026-03-27，SQLite 状态持久化、清空任务列表与 v0.4.2 对齐

1. 变更摘要：
 - **配置主存储切换**：`ConfigurationService` 改为 SQLite 主写（`AppConfig` 表），并保留 `config.toml` 兼容读取路径；Settings 更新后仅写入 SQLite。
 - **状态持久化新增**：新增 `IUiStateStore`/`UiStateStore`，使用 SQLite `UiState` 表持久化 Search（包含翻译 + 高级筛选）、Download（高清音频 + 文件筛选）与未完成队列。
 - **Settings 裁剪**：移除“下载筛选默认规则”“全局搜索规则”输入项及对应读写逻辑，仅保留其在 Search/Download 页面状态中的持久化。
 - **未完成队列恢复**：应用重启后恢复上次 `Pending/Queued/Failed` 队列项，并统一以 `Pending` 方式回流到待下载队列。
 - **清空任务列表**：Download 页面新增“清空任务列表”按钮；点击后停止运行任务、清空队列、删除全部任务并清空未完成队列持久化。
 - **版本升级**：Core/Application/Infrastructure/Wpf 与 UI/README 统一升级到 `v0.4.2`。
 - **测试与清单同步**：新增/更新 Infrastructure、Application、Wpf 测试；第 4 章受影响项已重置并在回归通过后重新勾选。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/UiStateStore.cs`（新建）、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IUiStateStore.cs`（新建）、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/SearchUiState.cs`（新建）、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloadUiState.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`README.md`、`dotnet/tests/**/*.cs`（相关新增与更新）。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Release --nologo` 通过；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过，总计 183，失败 0，成功 183。
4. DoD 判定：是。本批需求（SQLite 持久化、清空任务列表、Settings 裁剪、版本更新、测试同步）已完成并通过回归。
5. 下次计划：继续沿用“需求分解 -> 顺序实现 -> 测试验证 -> 文档同步”的闭环推进后续批次。

### 1.63 2026-03-29，AppConfig 分段存储、config.json 默认配置与 TOML 退场

1. 变更摘要：
 - **AppConfig 分段存储**：将 SQLite `AppConfig` 从旧单行结构（`Id=1` + 整体 `JsonValue`）改为类似 `UiState` 的键值结构（`ConfigKey` + `JsonValue` + `UpdatedAt`），按 `user/downloader/limit` 三段拆分存储。
 - **自动迁移旧结构**：启动初始化与配置读取路径均支持旧表结构自动迁移；迁移后清理旧结构，确保库内仅保留新结构。
 - **清理无用表逻辑**：确认 `MetadataWork`、`WorkSyncInfo` 未参与运行时读写后，从数据库初始化逻辑中移除并在初始化时清理。
 - **默认配置来源切换**：新增程序目录 `config.json`（随 WPF 输出复制），在 SQLite 配置不存在时作为默认配置来源。
 - **移除 TOML 依赖链**：`ConfigurationService` 移除 `config.toml` 读取与 `Tomlyn` 依赖；运行时改为“SQLite 读写实际配置 + config.json 提供默认值”。
 - **测试同步**：更新 Infrastructure 测试，覆盖 AppConfig 分段读写、默认配置回退、SQLite 优先级与旧结构迁移；同步更新 UI 状态回退测试到 `config.json` 场景。
 - **第 4 章同步**：对受影响项执行“先重置再回归勾选”流程，并在全量回归通过后恢复勾选状态。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AppPathService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAppPathService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/config.json`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/DatabaseInitializerTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Release --nologo` 通过；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过，总计 186，失败 0，成功 186。
4. DoD 判定：是。本批需求（AppConfig 分段、旧表清理、config 默认配置、TOML 下线、测试同步、文档同步）已完成并通过回归。
5. 下次计划：继续沿用“需求分解 -> 顺序实现 -> 测试验证 -> 文档同步”的闭环推进后续批次。

### 1.64 2026-03-29，阶段 4/5 技术债清理（旧字段移除 + 默认回退收敛）

1. 变更摘要：
 - **激进清理旧下载配置字段**：从 `DownloaderOptions` 移除 `preferMedia/preferImage/preferVideo/fileFilter/globalSearchRule` 历史字段，统一保留 `PreferFormats` 与 `HdAudioOnly`。
 - **下载筛选解析收敛**：`DownloadFilterParser.ParsePreferExtensions` 改为仅解析 `PreferFormats`，不再回退旧字段。
 - **应用层参数来源收敛**：`DownloadService` 移除 `fileFilter` 对已删除配置字段的隐式回退，仅使用调用参数。
 - **UI 状态默认值收敛**：`UiStateStore` 移除对配置服务的依赖与兜底分支；`Search/Download` UI 状态在无持久化记录时直接返回默认状态。
 - **设置页与默认配置清理**：`SettingsView` 去除旧字段合并展示逻辑；`dotnet/Asmroner.Wpf/Asmroner.Wpf/config.json` 删除旧键；删除未被 DI 使用的 WPF 占位服务 `Services/DownloadService.cs` 与 `Services/SearchService.cs`。
 - **迁移兼容补偿**：`ConfigurationService` 与 `DatabaseInitializer` 在旧单行 JSON 迁移路径中新增旧 `prefer*` 键提取与并入 `PreferFormats` 的兼容逻辑。
 - **测试同步**：同步更新 Core/Application/Infrastructure 测试样例，移除对已删除字段与旧回退语义的断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadFilterParser.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/UiStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/config.json`、`dotnet/tests/Asmroner.Core.Tests/DownloadFilterParserTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 186，失败 0，成功 186）。
4. DoD 判定：是。本轮清理范围内代码与测试已完成同步，且解决方案级回归通过。
5. 下次计划：按同样策略继续做“删旧字段/删旧分支必须同批补测试 + 回归 + 文档追加”，避免再次出现配置语义漂移。

### 1.65 2026-03-29，阶段 4 恢复链路修复（Search 入队即时持久化）

1. 变更摘要：
 - **问题定位**：未完成队列此前主要在 Download 页刷新路径持久化；若用户仅在 Search 页入队后直接退出，可能出现重启后恢复不完整。
 - **修复实现**：在 Search 页 `OnQueueClicked` 入队成功后，立即触发未完成队列持久化，消除“必须进入 Download 页才落盘”的隐式前提。
 - **策略复用**：新增 `DownloadUnfinishedQueueSnapshotPolicy`，统一“活跃任务（Pending/Queued/Failed）+ 当前队列”快照构建规则；Download 与 Search 两侧共用，避免逻辑漂移。
 - **测试补齐**：新增 `DownloadUnfinishedQueueSnapshotPolicyTests`，覆盖状态筛选、跨来源去重与空输入场景。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadUnfinishedQueueSnapshotPolicy.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadUnfinishedQueueSnapshotPolicyTests.cs`（新建）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 101，失败 0，成功 101）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 35，失败 0，成功 35）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 189，失败 0，成功 189）。
4. DoD 判定：是。本轮恢复链路缺口已修复，相关回归通过。
5. 下次计划：补充“Search 入队后不切换 Download 页直接退出并重启”的手工功能验证并由用户勾选章节 4 对应项。

### 1.66 2026-03-29，v0.4.3 对齐 + Search 首次交互修复 + SearchView 命名统一

1. 变更摘要：
 - **版本对齐**：Core/Application/Infrastructure/Wpf、MainWindow 标题、Settings 版本文案、启动日志与 README 统一升级到 `v0.4.3`。
 - **缺陷修复**：修复 Search 页面“未先执行搜索时，调整排序/方向/字幕/页大小不触发查询”的问题；新增 option-only query 路径，允许仅携带分页/排序参数发起搜索。
 - **命名统一**：将 `DashboardView` 重命名为 `SearchView`，同步 DI、MainWindow 承载引用与 WPF 测试命名。
 - **测试补齐**：新增解析与服务层回归（option-only query），并补充 SearchView XAML 事件绑定与类名断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`README.md`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 43，失败 0，成功 43）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 103，失败 0，成功 103）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 193，失败 0，成功 193）。
4. DoD 判定：是。本轮代码与自动化回归已闭环，待章节 4 手工项由用户执行并勾选。
5. 下次计划：按章节 4 执行 Search 首次交互、跨页联动与版本显示的手工回归，确认通过后由用户完成勾选。

### 1.67 2026-03-30，阶段 3 Search 任务列表右键菜单（入队/导出/浏览器打开）

1. 变更摘要：
 - **需求收敛**：按用户最新要求移除“保存到全局搜索规则”本轮实现，聚焦 Search 任务列表右键菜单能力。
 - **章节复核**：已复核章节 1.2/1.3/1.4，本轮不涉及阶段状态变更与新增阻塞风险，三章维持现状。
 - **右键菜单落地**：在 Search 结果表格新增右键菜单，补齐“加入下载队列 / 导出 CSV / 导出 JSON / 在浏览器打开”四项操作。
 - **行为一致性**：右键“加入下载队列/导出 CSV/导出 JSON”全部复用现有按钮事件处理，确保行为与按钮一致。
 - **浏览器打开配置化**：新增 `SearchWorkPageUrlPolicy`，统一基于 `workPageUrlTemplate` 的 URL 生成与校验逻辑；右键“在浏览器打开”仅作用于当前右键命中项。
 - **文档约束执行**：已先在章节 4 将受影响验证项重置为未勾选，并新增右键菜单相关未勾选测试项。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchWorkPageUrlPolicy.cs`（新建）、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchWorkPageUrlPolicyTests.cs`（新建）、`docs/wpf-migration-progress.md`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 109，失败 0，成功 109）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 35，失败 0，成功 35）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 199，失败 0，成功 199）。
4. DoD 判定：是。自动化回归通过；章节 4 对应手工验证项已按规则保持未勾选，待用户执行并勾选。
5. 下次计划：由用户执行 Search 右键菜单手工回归（多选与右键命中行、导出文件可读性、浏览器打开 URL 模板生效），完成后更新章节 4 勾选状态。

### 1.68 2026-03-30，v0.4.4 对齐 + Search 右键多选修复 + 导出全部/选中分流

1. 变更摘要：
 - **章节复核**：已复核章节 1.2/1.3/1.4，本轮不涉及阶段状态变更与新增阻塞风险，三章维持现状。
 - **版本升级**：Core/Application/Infrastructure/Wpf 项目版本、主窗口标题、设置页版本文案、启动日志与 README 统一升级到 `v0.4.4`。
 - **缺陷修复**：修复 Search 任务列表右键未选中行导致多选集被清空的问题，右键未选中行时保持既有多选集合不变。
 - **导出菜单增强**：右键菜单导出能力升级为“导出全部任务到 CSV/JSON + 导出选中任务到 CSV/JSON”，文案与行为语义一致。
 - **导出分流策略**：新增 `SearchExportScopePolicy` 统一导出目标决策；“导出选中”在无选中项时自动回退导出全部并输出明确提示。
 - **测试同步**：更新 `SearchViewXamlTests` 菜单断言并新增 `SearchExportScopePolicyTests`，覆盖导出全部/选中/回退逻辑。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchExportScopePolicy.cs`（新建）、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchExportScopePolicyTests.cs`（新建）、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 113，失败 0，成功 113）；`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 43，失败 0，成功 43）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 35，失败 0，成功 35）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 203，失败 0，成功 203）。
4. DoD 判定：是。代码改动与自动化回归已闭环；章节 4 对应手工验证项按约束保持未勾选。
5. 下次计划：由用户执行 Search 手工回归，重点验证“多选后右键未选中行不改变选中集”“导出选中无选中时回退导出全部”“右键浏览器打开命中项准确”。

### 1.69 2026-03-30，v0.4.5 对齐 + Settings 重初始化停留修复

1. 变更摘要：
 - **章节复核**：已复核章节 1.2/1.3/1.4；阶段状态维持不变，并在 1.4 新增本轮缺陷记录与解决结果。
 - **版本升级**：Core/Application/Infrastructure/Wpf 项目版本、Settings 版本文案、启动日志与 README 统一升级到 `v0.4.5`。
 - **界面调整**：主窗口标题移除版本号，仅保留 `Asmroner`；版本信息仅在 Settings 页面展示。
 - **缺陷修复**：修复“保存并重新初始化”成功后强制跳转 Search 的问题，改为 Settings 触发成功后保持在 Settings 页面。
 - **测试同步**：更新 WPF XAML 断言并新增主窗口“标题不含版本号”测试样例；章节 4 对应项按规则保持未勾选。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 114，失败 0，成功 114）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 204，失败 0，成功 204）。
4. DoD 判定：是。版本升级、界面规则调整与导航修复均已完成，自动化回归通过。
5. 下次计划：由用户执行章节 4.1 的手工回归项（重初始化停留行为与版本展示规则）并按实际结果勾选。

### 1.70 2026-03-30，v0.4.6 版本升级 + NLog 结构化日志封装

1. 变更摘要：
 - **章节复核**：已复核章节 1.2/1.3/1.4；阶段状态维持不变。
 - **版本升级**：Core/Application/Infrastructure/Wpf 项目版本、启动日志统一升级到 `v0.4.6`。
 - **日志封装**：引入 `NLog 5.3.2`，新建 `IAppLogService` 接口（Core）与 `NLogAppLogService` 实现（Infrastructure）；以编程式配置替代 XML 配置文件，日志以 JSON Lines 格式写入滚动文件（`~/.asmroner-data/logs/`），文件超过 10 MB 或日期变更时自动归档；`IAppPathService` 新增 `LogsDirectory` 属性，`App.xaml.cs` 在 `Host.Build()` 后立即调用 `logService.Configure()`，`ConfigureLogging` 改用 `AddNLog()` 桥接到 MEL。
 - **测试同步**：新建 `NLogAppLogServiceTests`（5 个 Fact：FileTarget 存在性/JsonLayout/归档大小/按天归档/目录自动创建）与 `AppPathServiceTests`（`LogsDirectory_ShouldBeUnderMetadataDirectory`）共 6 个新测试样例；`DownloadServiceTestDoubles.TestAppPathService` 补全 `LogsDirectory` 接口成员。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAppPathService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAppLogService.cs`（新建）、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AppPathService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/NLogAppLogService.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/NLogAppLogServiceTests.cs`（新建）、`dotnet/tests/Asmroner.Infrastructure.Tests/AppPathServiceTests.cs`（新建）、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 210，失败 0，成功 210）。
4. DoD 判定：是。版本升级与 NLog 日志封装均完成，自动化回归通过。
5. 下次计划：由用户执行章节 4.1 的手工回归项（Settings 版本展示规则 v0.4.6）并按实际结果勾选。

### 1.71 2026-03-30，NLog 升级至 6.1.1 + NLogAppLogService 代码优化

1. 变更摘要：
 - **NLog 版本升级**：`NLog` 由 `5.3.2` 升至 `6.1.1`（Infrastructure、Infrastructure.Tests），`NLog.Extensions.Logging` 由 `5.3.2` 升至 `6.1.2`（Wpf），与当前最新稳定版对齐。
 - **NLogAppLogService 代码优化**：提取 `BuildFileTarget` 为 `internal static` 方法以支持单元测试直接构造；新增 `LogManager.ReconfigExistingLoggers()` 调用确保存量 Logger 实例立即生效；JSON 时间戳改用 `${longdate}`；归档文件迁移至 `archive/` 子目录，扩展名统一为 `.json`；新增 `ConcurrentWrites = true` 与 `KeepFileOpen = true` 优化写入性能；`NLogAppLogServiceTests` 补充第 5 个 Fact：`Configure_ShouldCreateLogDirectory_WhenNotExists`。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变动；章节 2.1 基线维持 210/210。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/NLogAppLogService.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 210，失败 0，成功 210）。
4. DoD 判定：是。NLog 版本对齐与代码结构优化完成，回归通过。
5. 下次计划：由用户执行章节 4.1 的手工回归项并按实际结果勾选。

### 1.72 2026-03-30，全量切换为 NLog 静态日志器 + 移除 `ILogger<T>` 注入

1. 变更摘要：
 - **日志实现统一**：将 Infrastructure/WPF 中剩余 `ILogger<T>` 注入改为 `NLog.LogManager.GetCurrentClassLogger()` 静态日志器，覆盖 `ApiEndpointUrlService`、`AuthService`、`ConnectivityProbeService`、`StartupEndpointWarmupService`、`MainWindow`、`SettingsView`、`SearchView`、`DownloadView`、`App.xaml.cs`。
 - **依赖收敛**：移除 WPF 项目中的 `Microsoft.Extensions.Logging.Console` 与 `Microsoft.Extensions.Logging.Debug` 包引用；保留 `NLog.Extensions.Logging` 作为桥接。
 - **测试同步**：移除相关测试中的 `NullLogger<T>.Instance` 构造参数，匹配新构造函数签名；`NLogAppLogServiceTests` 新增 `Configure_ShouldWriteLogEntry_WhenStaticLoggerIsUsed`，验证静态 logger 可落盘写入。
 - **回归结果**：`dotnet build dotnet/Asmroner.sln --configuration Debug` 成功（0 错误）；五个测试程序集直接执行通过，总计 211，失败 0。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConnectivityProbeService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupEndpointWarmupService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/tests/Asmroner.Infrastructure.Tests/NLogAppLogServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConnectivityProbeServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupEndpointWarmupServiceTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln --configuration Debug` 通过；`dotnet dotnet/tests/Asmroner.Core.Tests/bin/Debug/net8.0/Asmroner.Core.Tests.dll`、`dotnet dotnet/tests/Asmroner.Infrastructure.Tests/bin/Debug/net8.0/Asmroner.Infrastructure.Tests.dll`、`dotnet dotnet/tests/Asmroner.IntegrationTests/bin/Debug/net8.0/Asmroner.IntegrationTests.dll`、`dotnet dotnet/tests/Asmroner.Application.Tests/bin/Debug/net8.0/Asmroner.Application.Tests.dll`、`dotnet dotnet/tests/Asmroner.Wpf.Tests/bin/Debug/net8.0-windows/Asmroner.Wpf.Tests.dll` 全部通过（总计 211，失败 0，成功 211）。
4. DoD 判定：是。日志体系统一到 NLog 静态日志器，编译与自动化回归通过。
5. 下次计划：由用户执行章节 4.1 受影响项（启动与 Settings 操作路径）手工回归并按结果重新勾选。

### 1.73 2026-03-31，Search/Download 标签列 + 导出优化 + v0.4.7

1. 变更摘要：
 - **版本升级**：所有 .csproj（Core/Application/Infrastructure/Wpf）版本号从 0.4.6 升级到 0.4.7。
 - **模型层**：新增 `TagDto`（id + name）；`SearchWorkDto` 新增 `Tags` 属性（`IReadOnlyList<TagDto>`）；`SearchWorkItem` 移除 `RateAverage`/`DownloadCount`，新增 `Tags`（按 `tags.id` 升序排序后用 `;` 分隔的 name 字符串）。
 - **服务层**：`SearchService` 的 DTO->Item 映射移除评分/销量赋值，新增标签排序+拼接逻辑；`IAsmrApiClient.GetPopularAsync` 返回类型从 `IReadOnlyList<HotWorkDto>` 改为 `IReadOnlyList<SearchWorkDto>`；`AsmrApiClient.GetPopularAsync` 简化为直接返回 `result.Works`，消除信息丢失；移除 `HotWorkDto`。
 - **导出/导入**：CSV header 从 `source_id,release,rate_average_2dp,dl_count,has_subtitle,title` 改为 `source_id,has_subtitle,release,tags,title`；JSON 导出/导入自动适配新模型；导入端解析新增 `tags` 列。
 - **UI 层**：`SearchView.xaml` DataGrid 移除"评分"和"销量"列，新增"标签"列（200px，绑定 `Tags`）；热门作品查询结果现在可正确显示日期、字幕、标签信息。
 - **导出后打开文件夹**：Search 页面导出 CSV/JSON 成功后自动调用 `explorer.exe /select,"<filePath>"` 打开并选中目标文件。
 - **测试同步**：更新 `SearchExportServiceTests`/`SearchImportServiceTests`/`SearchServiceTests` 中的测试数据，适配新 CSV 格式和模型字段；更新 `DownloadServiceTestDoubles` 和 `SearchServiceTests` 中的测试替身以匹配新 `GetPopularAsync` 返回类型。
 - **回归结果**：`dotnet build dotnet/Asmroner.sln --configuration Debug` 成功（0 错误）；全部 211 测试通过（失败 0）。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变动；章节 2.1 基线维持 211/211；章节 4.2/4.3/4.4 受影响项重置为未勾选。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Api/TagDto.cs`（新增）、`dotnet/Asmroner.Backend/Asmroner.Core/Api/SearchResultDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Api/HotWorkDto.cs`（移除）、`dotnet/Asmroner.Backend/Asmroner.Core/Search/SearchQuery.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchExportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchImportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchExportServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchImportServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln --configuration Debug` 通过（0 错误）；VS Code 测试运行器全量通过（总计 211，失败 0，成功 211）。
4. DoD 判定：是。标签列功能完成、导出优化与打开文件夹完成、模型和测试同步更新、编译与自动化回归通过。
5. 下次计划：由用户执行章节 4.2/4.3/4.4 受影响项手工回归并按实际结果重新勾选。

### 1.74 2026-03-31，Bug 修复：资源管理器窗口去重 + 导入计数修正

1. 变更摘要：
 - **Bug 1  导出后重复打开资源管理器窗口**：新建 `ExplorerHelper.cs`（P/Invoke 调用 `SHParseDisplayName` + `SHOpenFolderAndSelectItems` + `ILFree`），替换 `SearchView.xaml.cs` 中 `Process.Start("explorer.exe", "/select,...")`。Windows Shell API 会复用已打开的同目录资源管理器窗口，消除连续导出时的窗口堆积。
 - **Bug 2  Download 导入计数不准确**：在 `DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent` 中，对 `incoming` 参数追加 `.Distinct(StringComparer.OrdinalIgnoreCase)`，确保去重后再过滤已有任务。所有调用方（CSV 导入、JSON 导入、Search 入队、批量入队）均受益。
 - **新增测试**：`FilterAlreadyPresent_ShouldDeduplicateIncoming`（输入含重复及大小写重复项，期望返回 3 项去重结果）。
 - **回归结果**：`dotnet build dotnet/Asmroner.sln --configuration Debug` 成功（0 错误）；全部 212 测试通过（失败 0）。
 - **章节复核**：1.4 节追加 2 条已解决 bug 记录；2.1 基线更新为 212/212；4.2/4.3/4.4 受影响项保持已勾选（本次修复仅为 bug 消除，不改变功能可见行为）。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ExplorerHelper.cs`（新增）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadEnqueueDuplicatePolicy.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadEnqueueDuplicatePolicyTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln --configuration Debug` 通过（0 错误）；VS Code 测试运行器全量通过（总计 212，失败 0，成功 212）。
4. DoD 判定：是。两个 bug 均已修复；新增对应单元测试；编译与自动化回归通过。
5. 下次计划：由用户执行章节 4.3 导入 CSV/JSON 手工回归，确认提示信息准确。

### 1.75 2026-03-31，Bug 修复补充：Download 导入计数纳入待下载队列

1. 变更摘要：
 - **导入/入队计数口径收口**：`DownloadView.xaml.cs` 中单个入队、批量入队、导入 CSV、导入 JSON 四条入口统一复用 `SearchQueueCountPolicy.Build`，同时参考 `_downloadService.GetTasks()` 与 `_searchStateStore.GetQueuedSourceIds()`；修正“任务列表尚未生成行、但待下载队列已存在同批 SourceId”时的导入数量与跳过数量误报。
 - **提示文案同步修正**：Download 页面相关状态提示不再只写“已在下载列表中”，而是改为覆盖“下载列表、待下载队列、导入内容重复”三类跳过来源，保证提示与实际计数一致。
 - **新增测试**：`SearchQueueCountPolicyTests` 新增 `Build_ShouldSkipQueuedItems_WhenTaskListEmpty`，验证任务列表为空但队列已存在同批 SourceId 时，`ToEnqueue` 为空且 `SkippedCount` 等于输入项数。
 - **回归结果**：VS Code 测试运行器全量通过（总计 213，失败 0，成功 213）。
 - **章节复核**：章节 1.2/1.3 状态无需变动；章节 1.4 bug 描述已修正为最终根因；章节 2.1 基线更新为 213/213 并补充 2.1.45 新样例；章节 3.1 待提交行已合并更新；章节 4.3 导入验证项保持未勾选，待用户手工回归。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchQueueCountPolicy.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchQueueCountPolicyTests.cs`。
3. 验证结果：VS Code 测试运行器全量通过（总计 213，失败 0，成功 213）。
4. DoD 判定：是。Download 导入计数与跳过计数已和实际队列状态对齐；新增回归样例通过。
5. 下次计划：由用户执行章节 4.3 导入 CSV/JSON 手工回归，验证二次导入同一文件时提示数量准确。

### 1.76 2026-03-31，v0.4.8：常量集中化 + 启动未完成队列元数据后台刷新

1. 变更摘要：
 - **版本升级与动态版本文案**：Core/Application/Infrastructure/Wpf 四个运行时项目统一升级到 `0.4.8`；新增 `AppVersionInfo`，Settings 页面版本文案与启动完成日志统一改为读取程序集三段式版本号，移除硬编码 `v0.4.6`。
 - **常量与 SQLite 查询集中化**：新增 `AsmronerConstants`，集中维护应用名、版本前缀、API 超时/错误码、下载并发/超时、SQLite 表名/列名/section key，以及 `AppConfig`/`UiState` 相关重复查询；`AsmrApiClient`、`ConfigurationService`、`DatabaseInitializer`、`UiStateStore`、`DownloadView` 等改为从新类获取常量与 query。
 - **启动后台补拉未完成队列标题**：新增 `StartupUnfinishedQueueMetadataRefreshService`；主窗口 bootstrap 成功后，对 Download 未完成队列仅补拉“缺失标题/空标题”的作品信息，写回预取缓存并刷新 Download 列表，保证启动路径非阻塞。
 - **测试同步**：新增 `AppVersionInfoTests`（2 条）与 `StartupUnfinishedQueueMetadataRefreshServiceTests`（4 条），并更新 `SettingsViewXamlTests` 以适配动态版本文案。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变更；章节 2.1 基线更新为 219/219 并追加 2.1.51、2.1.52；章节 3.1 新增一条待提交记录；章节 4.1/4.3/4.4 受影响项重置为未勾选。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Constants/AsmronerConstants.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/UiStateStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/AppVersionInfo.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupUnfinishedQueueMetadataRefreshService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/AppVersionInfoTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupUnfinishedQueueMetadataRefreshServiceTests.cs`。
3. 验证结果：VS Code 测试运行器全量通过（总计 219，失败 0，成功 219）。
4. DoD 判定：是。v0.4.8 版本升级、常量/query 集中化、启动未完成队列元数据后台刷新与自动化回归均已完成。
5. 下次计划：由用户执行章节 4.1/4.3/4.4 受影响项手工回归，并按结果重新勾选。

### 1.77 2026-03-31，v0.4.8：启动补拉修复 + Search/Download 列表 UI 调整

1. 变更摘要：
 - **启动补拉缺陷修复**：`StartupEndpointWarmupService` 改为复用同一 in-flight warmup 任务，避免启动期间重复站点发现；`StartupUnfinishedQueueMetadataRefreshService` 在补拉未完成队列标题前先等待 warmup 完成，并在 warmup 失败时继续后续刷新，修复重启后标题长期空白的问题。
 - **Search 列表 UI 调整**：Search 结果表新增统一列头边框样式，显式启用列重排；字幕列与日期列改为固定默认宽度且禁止拖拽改宽，标题/标签列扩宽，数据过宽时依赖横向滚动查看完整内容。
 - **Download 列表 UI 调整**：Download 任务表新增统一列头/单元格样式，显式启用列重排；状态列与进度列改为固定默认宽度且禁止拖拽改宽，目录/错误等长文本列扩宽，数据过宽时通过横向滚动查看。
 - **测试同步**：新增 `StartupEndpointWarmupServiceTests` 1 条、`StartupUnfinishedQueueMetadataRefreshServiceTests` 2 条、`SearchViewXamlTests` 1 条、`DownloadViewXamlTests` 1 条，共补齐 5 条回归样例。
 - **章节复核**：章节 1.2/1.3 状态无需变更；章节 1.4 追加本轮已解决缺陷；章节 2.1 基线更新为 226/226，并更新 2.1.17、2.1.19、2.1.40、2.1.52；章节 3.1 合并现有 `v0.4.8` 待提交行；章节 4.1/4.2/4.3 受影响项重置为未勾选。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupEndpointWarmupService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupUnfinishedQueueMetadataRefreshService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/StartupEndpointWarmupServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupUnfinishedQueueMetadataRefreshServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`。
3. 验证结果：先执行 WPF 相关定向回归（总计 16，失败 0，成功 16），再执行 VS Code 测试运行器全量回归（总计 226，失败 0，成功 226）。
4. DoD 判定：是。启动补拉缺陷修复、Search/Download 列表 UI 调整与自动化回归均已完成；章节 4 的受影响手工回归项已按约束重置为未勾选，待用户验证后重新勾选。
5. 下次计划：由用户执行章节 4.1/4.2/4.3 受影响项手工回归，重点验证重启恢复后的标题补拉、Search/Download 列表固定列宽与横向滚动体验。

### 1.78 2026-04-01，v0.4.8：列宽微调

1. 变更摘要：
 - **Search 列宽微调**：Search 结果表字幕列宽度调整为 `42`，日期列宽度调整为 `75`，继续保持不可拖拽改宽。
 - **Download 列宽微调**：Download 任务表状态列宽度调整为 `50` 且继续锁定；进度列保留 `96` 默认宽度，但取消显式锁定，允许手动调整列宽。
 - **测试同步**：更新 `SearchViewXamlTests` 与 `DownloadViewXamlTests` 断言，并回归启动 warmup/未完成队列补拉测试，确认本轮 UI 调整未影响既有启动修复。
 - **章节复核**：章节 2.1 基线更新为 2026-04-01 的 226/226，并更新 2.1.17、2.1.19；章节 3.1 继续合并到同一条 `v0.4.8` 待提交记录；章节 4.2/4.3 受影响手工项重置为未勾选。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：先执行定向回归（`SearchViewXamlTests.cs`、`DownloadViewXamlTests.cs`、`StartupEndpointWarmupServiceTests.cs`、`StartupUnfinishedQueueMetadataRefreshServiceTests.cs`），总计 20，失败 0，成功 20；再执行 VS Code 测试运行器全量回归，总计 226，失败 0，成功 226。
4. DoD 判定：是。本轮列宽微调、断言更新与启动补拉缺陷说明同步均已完成。
5. 下次计划：由用户执行章节 4.2/4.3 受影响项手工回归，重点验证 Search 字幕/日期列宽、Download 状态/进度列宽交互，以及重启后缺失标题后台补拉。

### 1.79 2026-04-01，v0.4.9：翻译作品优先入队与版本流程手册

1. 变更摘要：
 - **版本升级**：将 WPF 运行时项目版本统一升级到 `0.4.9`，并同步 README 与迁移进度文档中的版本标识。
 - **版本流程手册**：新增 `docs/ai-version-update-playbook.md`，沉淀符合本仓库约束的 AI 可复用版本号更新流程。
 - **翻译作品优先入队**：为 Search 与 Download 页面新增默认勾选的“加入翻译作品”复选框；启用后在加入下载队列时按 `简体中文 -> 繁体中文 -> 日本語` 选择最终入队版本，并统一覆盖 Search 入队、Download 单个/批量入队、CSV 导入、JSON 导入四类入口。
 - **关联逻辑同步**：补齐 `WorkInfoDto` 的语言/翻译元数据字段，新增统一的语言选择策略与入队解析服务，确保最终入队 `SourceId`、标题缓存、重复入队判定与跨页面回填保持一致。
 - **测试同步**：新增 Application 侧语言选择与入队解析测试，并补充 Infrastructure/WPF 对翻译元数据、UI 状态持久化与新复选框的回归断言。
 - **章节复核**：章节 2.1 维持 2026-04-01 的全量回归基线 `226/226`，并补充本轮新增/受影响样例定向回归 `35/35`，更新 2.1.9/2.1.17/2.1.19/2.1.46/2.1.51，追加 2.1.53/2.1.54；章节 3.1 新增一条待提交记录；章节 4.1/4.2/4.3/4.4 受影响手工项重置为未勾选。
2. 关键文件：`docs/ai-version-update-playbook.md`、`README.md`、`dotnet/Asmroner.Backend/Asmroner.Core/Api/WorkInfoDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IEnqueueWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/WorkLanguageSelectionPolicy.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/EnqueueWorkInfoResolver.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 VS Code 测试运行器定向回归（`WorkLanguageSelectionPolicyTests.cs`、`EnqueueWorkInfoResolverTests.cs`、`AsmrApiClientTests.cs`、`UiStateStoreTests.cs`、`SearchViewXamlTests.cs`、`DownloadViewXamlTests.cs`、`AppVersionInfoTests.cs`），总计 35，失败 0，成功 35；随后执行 `dotnet build dotnet/Asmroner.sln`，10 个项目全部构建成功，错误 0、警告 0。
4. DoD 判定：是。本轮版本升级、可复用版本手册、翻译作品优先入队逻辑、关联回归测试与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4 受影响项手工回归，重点验证 Search/Download 两页的“加入翻译作品”勾选状态恢复、优先语言入队结果、跨页面标题显示与重复入队防护。

### 1.80 2026-04-01，v0.4.9：BJ 作品编号修复与启动补拉失败可视化

1. 变更摘要：
 - **`source_id/workId` 双键兼容**：Search 结果与热门结果保留数值 `WorkId`；`AsmrApiClient` 为 `BJ02370869` 这类非 `RJ` `source_id` 增加搜索兜底解析；`DownloadService` 在已知 `WorkId` 时直接使用数值编号请求 `tracks`，修复 BJ 作品入队后解析失败的问题。
 - **Search/Download 入队修复**：Search 页面在清空任务列表后再次入队时，不再对 `BJ` 作品误报“失败 1 项”；Search 右键“在浏览器打开”会为非 `RJ` 作品生成有效作品页链接；Download 单个/批量/导入入口沿用统一解析链路。
 - **启动补拉失败可视化**：启动后台补拉未完成队列标题时，单项失败会在 Download 列表中保留为 `Failed` 占位行并显示错误信息，同时不影响成功项更新。
 - **布局调整**：Search 的“加入翻译作品”移动到“包含翻译作品”右侧；Download 的“加入翻译作品”移动到“只下载高清音频”右侧。
 - **测试同步**：新增/更新 Application、Core、Infrastructure、WPF 回归测试，覆盖 `WorkId` 透传、BJ `source_id` 解析、启动补拉失败占位、命令可用性与复选框位置。
 - **章节复核**：章节 2.1 全量回归基线更新为 2026-04-01 的 VS Code 测试运行器全量回归 `245/245`；更新 2.1.9/2.1.16/2.1.17/2.1.19/2.1.52/2.1.54，追加 2.1.55；章节 3.1 继续合并为单一 `v0.4.9` 待提交行；章节 4 为受影响项重置为未勾选并追加专项回归项。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Utils/SourceIdNormalizer.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Search/SearchQuery.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/EnqueueWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupUnfinishedQueueMetadataRefreshService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 VS Code 测试运行器全量回归，总计 245，失败 0，成功 245；随后执行 `dotnet build dotnet/Asmroner.sln`，10 个项目全部构建成功，错误 0、警告 0。
4. DoD 判定：是。本轮 BJ 作品编号修复、启动补拉失败可视化、复选框位置调整、回归测试与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.2/4.3/4.4 受影响项手工回归，重点验证热门结果 `BJ` 作品入队、清空任务列表后的再次入队、启动补拉失败提示，以及 Search/Download 两页复选框新位置下的交互体验。

### 1.81 2026-04-01，v0.4.9：翻译入队提示补强与重复后缀修复

1. 变更摘要：
 - **提示文本统一**：Search 与 Download 页面在开启“加入翻译作品”且实际切换版本时，会在入队/导入结果提示中追加“其中 X 项已切换为翻译作品”；启动后台补拉结果提示统一改为“作品信息更新完成/失败”。
 - **语言切换计数**：`EnqueueWorkInfoResolver` 新增 `SwitchedSourceCount`，按“请求 `SourceId` 与最终入队 `SelectedSourceId` 不同”的成功项统计，覆盖 Search 选中/批量入队、Download 单个/批量入队、CSV/JSON 导入四类入口。
 - **文件名修复**：`DownloadService` 生成输出文件名时会先判断标题是否已带目标扩展名，避免生成 `.mp3.mp3`、`.png.png` 这类重复后缀，同时保持无扩展名标题的正常补齐。
 - **测试同步**：更新 `DownloadServiceTests`、`EnqueueWorkInfoResolverTests`、`DownloadOperationStatusTextsTests`，覆盖语言切换计数、提示文本与重复后缀回归。
 - **章节复核**：章节 2.1 维持 2026-04-01 的全量回归基线 `245/245`，并更新 2.1.1/2.1.26/2.1.54；章节 3.1 继续合并为单一 `v0.4.9` 待提交行；章节 4.2/4.3 受影响项重置为未勾选并追加专项回归项。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IEnqueueWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/EnqueueWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationStatusTexts.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/EnqueueWorkInfoResolverTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationStatusTextsTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 `dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 与 `dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj`，两项项目级回归均成功（终端 binlog-only 模式未返回计数）；随后执行 `dotnet build dotnet/Asmroner.sln`，10 个项目全部构建成功，错误 0、警告 0。
4. DoD 判定：是。本轮翻译入队提示补强、启动补拉提示统一、重复后缀修复、回归测试与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.2/4.3 受影响项手工回归，重点验证 Search/Download 提示中的翻译切换计数、启动补拉新文案，以及已带扩展名轨道标题的实际落地文件名。

### 1.82 2026-04-02，v0.4.10：WorkInfo 共享缓存与版本同步

1. 变更摘要：
 - **版本同步**：四个运行时项目的 `Version/AssemblyVersion/FileVersion/InformationalVersion` 统一更新到 `0.4.10/0.4.10.0`，并同步更新 README 与 WPF 进度文档中的当前版本标识。
 - **共享缓存落地**：新增基于 `IMemoryCache` 的 `IWorkInfoCache`，统一管理作品缓存，单条 TTL 为 1 小时，进程退出即失效；支持 Summary/Full 两级缓存语义与数值 `WorkId` 别名命中。
 - **API/下载链路收口**：`CachedAsmrApiClient` 会在 Search/热门结果中预热 Summary 缓存，在详情/轨道请求阶段优先命中 Full 缓存，并在仅有摘要时按需补拉完整详情；`DownloadService` 与启动补拉服务统一改用共享缓存，不再依赖私有预取字典。
 - **Search/Download 对齐**：Search、Download 单个/批量/导入入口会按“是否需要完整详情”写入 Summary 或 Full 缓存；清空任务列表后仅清空任务与队列，不主动清空进程内作品缓存。
 - **测试同步**：更新 `DownloadServiceTests`、`AppVersionInfoTests`、`StartupUnfinishedQueueMetadataRefreshServiceTests`，并新增 `MemoryWorkInfoCacheTests`、`CachedAsmrApiClientTests`，覆盖缓存分级、逐条 TTL、数值 `WorkId` 复用与摘要升级为 Full 的回归路径。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变更；章节 2.1 维持 2026-04-01 的全量回归基线 `245/245`，并更新 2.1.1/2.1.51/2.1.52，追加 2.1.56/2.1.57；章节 3.1 继续合并为单一 `v0.4.10` 待提交行；章节 4.1/4.2/4.3/4.4 受影响项重置为未勾选并补充缓存专项回归项。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IWorkInfoCache.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MemoryWorkInfoCache.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/CachedAsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupUnfinishedQueueMetadataRefreshService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MemoryWorkInfoCacheTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/CachedAsmrApiClientTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 `rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj`、`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj`、`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 与 `rtk dotnet test dotnet/Asmroner.sln`，均成功完成（rtk binlog-only 模式未返回用例计数）。
4. DoD 判定：是。本轮版本同步、WorkInfo 共享缓存、缓存驱动的 Search/Download/启动补拉链路收口、自动化回归与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4 受影响项手工回归，重点验证版本文案 `v0.4.10`、Search/Download 入队后的标题复用、清空任务列表后进程内缓存复用，以及重启或超过 1 小时后的按需补拉行为。

### 1.83 2026-04-02，v0.4.11：SQLite 收藏夹与 Search/Download 收藏链路

1. 变更摘要：
 - **版本同步**：四个运行时项目的 `Version/AssemblyVersion/FileVersion/InformationalVersion` 统一更新到 `0.4.11/0.4.11.0`，并同步更新 README 与 WPF 进度文档中的当前版本标识。
 - **SQLite 收藏夹落地**：新增 `IFavoriteStore`、`FavoriteStore` 与 `FavoriteWork` 表，将收藏夹标题、`SourceId`、`WorkId` 与标题写入本地 SQLite，并按 `FolderTitle + SourceId` 做大小写不敏感去重。
 - **Search 收藏入口**：Search 页面新增“收藏作品”按钮与共享收藏夹弹窗；无选中项时给出明确提示，保存收藏时复用“加入翻译作品”勾选逻辑，按“简体中文 -> 繁体中文 -> 日本語”保存最终版本。
 - **Download 导出入口**：Download 页面新增“从收藏夹导出”按钮与只读收藏夹下拉框；从 SQLite 读取收藏列表后复用现有入队/去重/标题缓存链路加入下载队列。
 - **测试同步**：新增 `FavoriteStoreTests`、`FavoriteFolderSelectionPolicyTests`、`FavoriteFolderDialogXamlTests`，并更新 `DatabaseInitializerTests`、`SearchViewXamlTests`、`DownloadViewXamlTests`，覆盖新表初始化、收藏夹去重、共享弹窗与新按钮入口。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变更；章节 2.1 全量回归基线更新为 2026-04-02 的 `267/267`，更新 2.1.13/2.1.17/2.1.19，追加 2.1.58/2.1.59/2.1.60；章节 3.1 新增单一 `v0.4.11` 待提交行；章节 4.1/4.2/4.3/4.4 受影响项重置为未勾选并补充收藏夹专项回归项。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Constants/AsmronerConstants.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IFavoriteStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/FavoriteStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/FavoriteFolderDialog.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/FavoriteFolderDialog.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/FavoriteStoreTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/FavoriteFolderSelectionPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/FavoriteFolderDialogXamlTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 `rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj`，55/55 通过；执行 `rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj`，142/142 通过；执行 `rtk dotnet test dotnet/Asmroner.sln --no-restore`，267/267 通过。
4. DoD 判定：是。本轮版本同步、SQLite 收藏夹、Search 收藏、Download 从收藏夹导出、自动化回归与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4 受影响项手工回归，重点验证版本文案 `v0.4.11`、Search 收藏弹窗/未选中提示、收藏夹落库，以及 Download 从收藏夹导出与去重行为。

### 1.84 2026-04-02，v0.4.11：Search/Download 文件入口收敛与重试按钮合并

1. 变更摘要：
 - **Search 导出入口收敛**：Search 页面主按钮由“导出 CSV / 导出 JSON”合并为“导出到文件”；右键菜单同步收敛为“导出全部任务到文件 / 导出选中任务到文件”，并保留原有 CSV/JSON 导出能力、无选中时导出全部的回退逻辑与导出后打开文件夹行为。
 - **Download 导入入口收敛**：Download 页面将“导入CSV / 导入JSON”合并为“从文件导入”，通过文件扩展名选择 CSV 或 JSON 解析链路；原“从收藏夹导出”按钮统一更名为“从收藏夹导入”，其 SQLite 读取与入队逻辑保持不变。
 - **Download 操作区调整**：将“执行下载队列”按钮移动到“立即下载选中任务”和“刷新任务列表”之间，并删除独立的“重试全部失败任务”按钮。
 - **重试策略合并**：保留单个“重试失败任务”按钮；当存在选中项时，仅重试选中的失败任务并忽略非失败项；当没有选中项时，回退批量重试全部失败任务，并复用原并发上限与批量确认弹窗。
 - **测试同步**：更新 `SearchViewXamlTests`、`DownloadViewXamlTests`、`DownloadCommandAvailabilityTests`、`DownloadOperationPrecheckPolicyTests`、`DownloadOperationPromptsTests`，覆盖统一入口按钮、右键菜单、单按钮重试规则与 Download 操作区顺序。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变更；章节 2.1 全量回归基线更新为 2026-04-02 的 `271/271`，并更新 2.1.16/2.1.17/2.1.19/2.1.25/2.1.28；章节 3.1 继续保持单一 `v0.4.11` 待提交行；章节 4.2/4.3/4.4 受影响项重置为未勾选并按新入口文案同步。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadCommandAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrecheckPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrompts.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPrecheckPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPromptsTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：执行 `rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo`，146/146 通过；执行 `rtk dotnet test dotnet/Asmroner.sln -c Release --no-restore --nologo`，271/271 通过。
4. DoD 判定：是。本轮 Search/Download 入口收敛、重试策略合并、自动化回归与进度文档同步均已完成。
5. 下次计划：由用户执行章节 4.2/4.3/4.4 受影响项手工回归，重点验证 Search 主按钮与右键菜单统一导出、Download 从文件导入 / 从收藏夹导入，以及重试按钮在“单失败选中 / 混合选中 / 空选中”三种场景下的行为。

### 1.85 2026-04-02，阶段 5 首批启动

1. 变更摘要：按第 4.6 节启动阶段 5，新增同步元数据 API 与 DTO、SQLite `MetadataWork/WorkSyncInfo` 表、`MetadataSyncService` 和最小 `Sync` 页签；同时将运行时与文档版本统一提升到 `v0.5.0`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Api/MetadataSyncPageDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（56/56）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（59/59）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（276/276）。
4. DoD 判定：否。阶段 5 当前仅完成首批元数据同步闭环，`sync download / retry / export / report` 与完整统计展示尚未落地。
5. 下次计划：继续实现同步下载、失败重试、导出报表与更完整的 Sync 页面交互。

### 1.86 2026-04-03，阶段 5 第二批：同步下载与容量控制

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncDownloadService`、`SyncService` 编排层、`WorkSyncInfo` SQLite 状态回写、`SyncWantedSize` 容量阈值控制与 Sync 页面“开始同步下载”入口。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadSnapshot.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadPathPolicy.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncSizeText.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（61/61）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（57/57）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（279/279）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步与同步下载容量控制两批，失败重试、导出和报表仍未落地。
5. 下次计划：继续实现失败重试、状态导出、统计报表以及更完整的 Sync 页面状态反馈。

### 1.87 2026-04-03，阶段 5 第三批：失败重试

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncRetryRunResult`、失败记录查询与 `SyncDownloadService.RetryFailedAsync`，并在 Sync 页面补充“重试失败项”入口与执行摘要展示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncRetryRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（63/63）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（58/58）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（282/282）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步、同步下载容量控制与失败重试三批，导出和报表仍未落地。
5. 下次计划：继续实现状态导出、统计报表以及更完整的 Sync 页面状态反馈。

### 1.88 2026-04-03，阶段 5 第四批：失败/成功记录导出

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncExportService`、`SyncExportStatus/SyncExportResult`、`WorkSyncInfo` 按状态查询导出链路，并在 Sync 页面补充“导出失败记录”“导出成功记录”入口与保存对话框流程。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncExportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncExportStatus.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncExportResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncExportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncExportServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（65/65）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（59/59）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（285/285）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步、同步下载容量控制、失败重试与状态导出四批，统计报表与更完整的 Sync 统计展示仍未落地。
5. 下次计划：继续实现 Sync 统计报表与页面汇总展示，并补齐对应手工回归证据。

### 1.89 2026-04-03，阶段 5 第五批：统计报表与页面数据面板

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncReportService/SyncReportSnapshot` 统计快照，按 Go `sync report` 口径汇总元数据总量、字幕拆分、完成/失败/待处理数量与同步进度，并在 Sync 页面新增统计卡片与数据面板展示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncReportSnapshot.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncReportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncReportServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（67/67）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（287/287）。
4. DoD 判定：是。阶段 5 五批能力已全部落地，`sync / sync download / retry / export / report` 对应的 WPF 页面能力与测试证据已闭环。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.90 2026-04-03，阶段 5 增强：同步进度持久化与温和停止

1. 变更摘要：在既有阶段 5 同步链路上新增 SQLite `UiState` 元数据同步/同步下载进度记录，支持未完成状态下的断点继续、已完成状态下的重置起跑语义，并在 Sync 页面补充“停止同步元数据”“停止同步下载”按钮与按钮可用性控制。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadProgressState.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncProgressStatuses.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IUiStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/UiStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（63/63）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（71/71）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（150/150）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（298/298）。
4. DoD 判定：是。阶段 5 在原有同步、重试、导出与报表能力之上，已补齐进度持久化、断点继续、温和停止与 0.5.1 版本口径同步。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.91 2026-04-03，阶段 5 交互收口：合并同步按钮并支持运行中刷新统计

1. 变更摘要：在既有阶段 5 增强基础上，进一步将 Sync 页“开始同步元数据 / 停止同步元数据”和“开始同步下载 / 停止同步下载”各自合并为单一主按钮，并保持“刷新统计”在同步运行期间仍可手动触发，以实时查看当前报表和 UiState 进度；同时用非时间型防重入保护合并按钮，避免双击后直接误触 stop request。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（153/153）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（71/71）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（301/301）。
4. DoD 判定：是。Sync 页已收敛为两枚同步主按钮，运行中可手动刷新统计，且 stop-request 与断点继续语义保持不变。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.92 2026-04-03，阶段 5 缺陷修复：合并按钮 1 秒防抖窗口

1. 变更摘要：修复 Sync 页合并后的同步主按钮在“开始后快速双击”场景下的顺序型防抖失效问题；将开始后的第二次快速点击改为在 1 秒窗口内直接忽略，避免误发 stop request，并同步将相同防抖策略复用于同步下载主按钮。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncActionDebouncePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncActionDebouncePolicyTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（306/306）。
4. DoD 判定：是。合并后的元数据同步 / 同步下载主按钮均具备 1 秒启动防抖窗口，快速双击不会再错误切入 stopping 态，且超过窗口后的显式停止语义保持不变。
5. 下次计划：保持当前 v0.5.1 待提交记录，待用户确认后再创建提交并继续阶段 6（资源库与播放能力迁移）规划。

### 1.93 2026-04-07，v0.5.1：下载/同步目录拆分、元数据保鲜与 completed-state 重扫

1. 变更摘要：拆分 Settings 中的普通下载目录与同步下载目录配置，新增“元数据有效期(天)”；新增 `MetadataWork` 优先解析链路，Search/Download 未开启翻译时先命中本地元数据、过期再补拉 API 并回写摘要；同步元数据在 completed-state 下按有效期触发过期刷新；同步下载在 completed-state 下从头校验全部作品，并复用普通下载/同步下载双目录中的已存在文件；同时统一 Search/Download/Sync 三页状态信息面板样式。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/DatabaseInitializerTests.cs`。
3. 验证结果：`rtk dotnet test tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（74/74）；`rtk dotnet test tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（63/63）；`rtk dotnet test tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test Asmroner.sln --nologo` 通过（309/309）。
4. DoD 判定：是。用户本轮要求的 Settings 配置拆分、`MetadataWork` 优先解析、completed-state 过期刷新/全量重扫、双目录复用、状态面板统一与回归测试均已落地。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4/4.5 受影响项手工回归，重点验证 Settings 新字段、Search/Download 元数据保鲜与双目录复用、以及 Sync completed-state 刷新/校验；章节 1.2/1.3/1.4 状态维持不变，章节 2.1 基线更新为 2026-04-07 的 `309/309` 并补充 2.1.1/2.1.11/2.1.13/2.1.17/2.1.19/2.1.20/2.1.61/2.1.62/2.1.63，章节 3.1 新增单一 `v0.5.1` 待提交记录，章节 4.1/4.2/4.3/4.4/4.5 受影响项已重置为未勾选。

### 1.94 2026-04-08，v0.5.2：真实媒体下载、修复 Sync 错误数据与双目录实时补齐

1. 变更摘要：运行时版本升级到 `v0.5.2`；为 `IAsmrApiClient` / `AsmrApiClient` 新增 `mediaDownloadUrl` 直链流式下载能力；`DownloadService` 不再写入 `source/title/url` 占位文本，改为优先复用双目录中的真实文件、识别并忽略旧占位文件、在双目录都缺失时直接实时下载真实媒体；`SyncDownloadService` 为同步下载与失败重试显式透传数值 `WorkId`，确保 Sync 页面复用普通下载链路时落地的也是正确媒体文件。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadStartOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/CachedAsmrApiClient.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（76/76）；`rtk dotnet test tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（64/64）；`rtk dotnet test tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test Asmroner.sln --nologo` 通过（312/312）。
4. DoD 判定：是。用户本轮要求的 `v0.5.2` 版本升级、普通下载真实媒体落盘、Sync 错误数据修复、WorkId 透传、单测补齐与文档同步均已完成。
5. 下次计划：由用户执行章节 4.1/4.3/4.5 的受影响手工回归，重点验证 Settings 版本文案显示 `v0.5.2`、普通下载在双目录缺失时直接实时下载、普通下载对同步目录真实文件的复用，以及 Sync 页面新下载结果不再出现旧的占位文本。

### 1.95 2026-04-08，v0.5.2：统一 Search/Download/Sync 的作品 info DTO 边界

1. 变更摘要：新增 `ISyncWorkInfoResolver` / `SyncWorkInfoResolver`，让 Sync 侧与 Search/Download 一样，在应用层统一使用 `WorkInfoDto` 作为作品 info DTO；`SyncDownloadService` 不再直接把 `MetadataWorkItem` 作为跨页作品信息载体，而是在进入同步下载流程前统一映射为 `WorkInfoDto`，仅在写回 SQLite `WorkSyncInfo` 的 pending 记录时再转换回持久化模型；页面行模型与 SQLite 存储模型继续保持分层，不做强制合并。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（313/313）。
4. DoD 判定：是。Search/Download/Sync 三页的作品 info 解析边界已统一到共享 `WorkInfoDto`，并保留 `SearchWorkItem`、`DownloadTaskItem`、`MetadataWorkItem`、`WorkSyncInfoItem` 作为页面/任务/存储模型，不再把 Sync 元数据存储实体直接当作跨页作品 info DTO 使用。
5. 下次计划：由用户执行章节 4.4/4.5 的受影响手工回归，重点验证 Search 入队、Download 标题刷新与 Sync 创建待处理记录时的标题、`SourceId` 与字幕标记在共享 `WorkInfoDto` 之后仍保持一致；章节 2.1 基线更新为 2026-04-08 的 `313/313` 并补充 2.1.63 新样例，章节 3.1 继续保持单一 `v0.5.2` 待提交记录，章节 4.4 新增 DTO 一致性专项回归项并保持未勾选。

### 1.96 2026-04-08，v0.5.3：增强站点发现、同步实时统计与 Settings 连接逻辑

1. 变更摘要：运行时版本升级到 `v0.5.3`；`EndpointDiscoveryService` 对发布页 HTML/入口脚本抓取改为非致命容错，放宽入口脚本标签匹配并补充浏览器 `User-Agent`；`MetadataSyncService` 在 UiState 中持续写入当前本地总量/字幕量，`SyncView` 在元数据同步运行中按秒刷新顶部摘要与状态文本；`SettingsView` 提取保存/测试连接共用配置动作流程，保持“先保存再测试”语义不变；同时为四个 `View.xaml.cs` 的直接控件事件处理方法补齐 `///` 注释。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（66/66）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（315/315）。
4. DoD 判定：是。用户本轮要求的 `v0.5.3` 版本升级、两项 bug 修复、“测试连接”逻辑优化、四个 `View.xaml.cs` 事件注释、单测与文档同步均已落地；受影响的手工功能项已在章节 4 中重置，待用户执行回归。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证测试连接在发布源 HTML/脚本抓取失败时仍可回退到可用 BaseUrl、Settings 页面版本文案显示 `v0.5.3`，以及 Sync 页面运行中本地元数据数量实时增长且完成后摘要一致；章节 2.1 基线更新为 2026-04-08 的 `315/315`，并更新 `EndpointDiscoveryServiceTests` 与 `MetadataSyncServiceTests` 记录；章节 3.1 新增单一 `v0.5.3` 待提交记录，章节 4.1/4.5 受影响项已重置为未勾选。

### 1.97 2026-04-08，v0.5.3：收敛 Sync 手动刷新口径并补强入口脚本解析

1. 变更摘要：在保留 `MetadataSyncService` 本地总量/字幕量持久化修复的前提下，移除 `SyncView` 元数据同步运行中的 `DispatcherTimer` 自动刷新，改回仅通过“刷新统计”读取最新持久化进度；`EndpointDiscoveryService` 改为解析更宽松的入口 script `src` 形态，兼容相对路径、查询串与单引号 `link` 配置；同步补强 `UiStateStore` 元数据进度 round-trip 断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（67/67）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（316/316）。
4. DoD 判定：是。Endpoint 发布源入口脚本抓取已按更宽松的真实页面形态解析；Sync 页不再在运行中自动轮询统计，但“刷新统计”和完成态摘要均可读取到正确的本地总量/字幕量；受影响手工项已在章节 4 重置为未勾选。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证测试连接对相对路径/查询串入口脚本的解析，以及 Sync 页面在同步运行中只有点击“刷新统计”后才更新顶部摘要与状态文本；章节 2.1 基线更新为 2026-04-08 的 `316/316`，并补充 `EndpointDiscoveryServiceTests` 新样例与 `UiStateStoreTests` 的进度字段说明。

### 1.98 2026-04-08，v0.5.3：修正 Endpoint 探测端点并补齐元数据同步处理口径

1. 变更摘要：`EndpointDiscoveryService` 的候选延迟探测不再对 `GET /api/recommender/popular` 做健康检查，而改为使用支持 `GET` 的 works 查询端点，避免 Go 版本 API 因 404 被误判为不可用；发布源 HTML/脚本抓取仍保持对超时与 `HttpRequestException` 的非致命回退。`MetadataSyncService` 在保留“新增数”语义的前提下，新增“累计处理条数”并持续写入 `UiState`，`SyncView` 的“刷新统计”和完成态详情同步显示累计处理、累计新增与当前本地总量/字幕量，降低“分页推进但像没写入”的误判风险；同时补齐基础设施与应用层回归样例。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（70/70）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（320/320）。
4. DoD 判定：是。启动/“测试连接”链路不再因错误的 GET 探测端点把健康候选误判为 404；MetadataSync 的完成态、停止态和“刷新统计”已统一显示累计处理、累计新增与当前本地总量，相关测试与文档同步完成；章节 4 的受影响手工项继续保留未勾选，待用户执行回归。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证“测试连接”在候选 API 对 `GET /api/recommender/popular` 返回 404 时仍可通过 works 探测选中可用 BaseUrl，以及 Sync 页面在前几页仅更新已有记录时，点击“刷新统计”后能同时看到累计处理、累计新增与当前本地总量；章节 2.1 基线更新为 2026-04-08 的 `320/320`，章节 3.1 继续维持单一 `v0.5.3` 待提交记录。

### 1.99 2026-04-10，v0.5.4：health 探测、发布页正文域名抓取与候选列表持久化

1. 变更摘要：运行时版本升级到 `v0.5.4`；默认候选 API 地址补齐 `https://api.asmr-300.com`、`https://api.asmr-200.com`、`https://api.asmr-100.com` 与 `https://api.asmr.one`；`EndpointDiscoveryService` 的候选探测切换到 `GET /api/health?cache=false`，并优先从发布页正文直接提取最新站点域名，仍保留入口脚本解析兼容；`AsmrApiOptionsProvider` 会把旧版内置候选子集扩展为完整默认集合并保持当前 BaseUrl 优先；`ApiEndpointUrlService` 发现成功后会把 `ApiUrl` 与 `ApiCandidateUrls` 一并写回 SQLite，同时保留自定义 endpoint 配置边界；`SettingsView` 在“测试连接”后会刷新内部候选列表缓存，避免后续保存把新候选覆盖回旧值。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Api/AsmrApiPaths.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiOptionsProvider.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/config.json`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiOptionsProviderTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（74/74）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（324/324）。
4. DoD 判定：是。用户本轮要求的 `v0.5.4` 版本升级、health API 探测、`apiCandidateUrls` 新增 `api.asmr-200.com` / `api.asmr-100.com`、发布页正文最新域名抓取、SQLite 候选列表写回、单测补齐与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1 的受影响手工回归，重点验证启动/“测试连接”改走 `GET /api/health?cache=false`、发布页正文中的最新域名会写回 SQLite `ApiCandidateUrls` 且后续“保存并重新初始化”不会覆盖回旧值，以及 Settings 页面版本文案显示 `v0.5.4`；章节 1.2/1.3 状态维持不变，章节 1.4 新增一条已解决站点发现/SQLite 持久化问题，章节 2.1 基线更新为 2026-04-10 的 `324/324` 并补充 `EndpointDiscoveryServiceTests`、`AsmrApiOptionsProviderTests`、`ApiEndpointUrlServiceTests` 与 `AppVersionInfoTests` 的口径，章节 3.1 合并为单一 `v0.5.4` 待提交记录，章节 4.1 受影响项已重置为未勾选。

### 1.100 2026-04-11，v0.5.5：版本升级与匿名探测客户端收敛

1. 变更摘要：运行时版本升级到 `v0.5.5`；保留 `AsmrProbe` 作为匿名站点探测客户端，并新增 `EndpointDiscoveryHttpTransport` 统一收敛客户端名称、HTTP/1.1 传输约束与默认 User-Agent；`EndpointDiscoveryService` 在单次发现流程内复用同一个 probe client，并统一发布页抓取与 health 探测请求头策略；同时清理 `AuthServiceTests` 中与登录链路无关的冗余 `AsmrProbe` 注册。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryHttpTransport.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（75/75）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（325/325）。
4. DoD 判定：是。用户本轮要求的 `v0.5.5` 版本升级、`AsmrProbe` 可用性确认与逻辑收敛、单元测试同步、progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1 的受影响手工回归，重点验证应用启动不阻塞、“测试连接”继续可用且发布页/health 探测链路正常，以及 Settings 页面版本文案显示 `v0.5.5`；章节 2.1 基线更新为 2026-04-11 的 `325/325`，章节 3.1 新增单一 `v0.5.5` 待提交记录，章节 4.1 受影响项已重置为未勾选。

### 1.101 2026-04-11，v0.5.6：Track size 完整度校验与重下补强

1. 变更摘要：运行时版本升级到 `v0.5.6`；`TrackDto` 新增 `Size` 字段并接收 track API 的 `size` 元数据；`DownloadService` 在普通下载目录、同步下载目录候选文件与真实下载落盘后三个阶段统一引入基于 track size 的完整度判断，仅在文件大小匹配时复用已有文件，大小不一致时忽略旧文件并重新下载；同时保留 legacy placeholder 与空文件防护逻辑。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Api/TrackDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（75/75）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（82/82）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（329/329）。
4. DoD 判定：是。用户本轮要求的 `v0.5.6` 版本升级、track `size` 接入、已有文件大小不一致时重新下载、单元测试同步与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.6`、普通下载/同步下载目录中仅在文件大小匹配时复用已有文件、大小不一致时触发重下，以及 Sync completed-state 重扫只补齐目录缺失或文件大小不一致的作品；章节 2.1 基线更新为 2026-04-11 的 `329/329`，章节 3.1 新增单一 `v0.5.6` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.102 2026-04-12，v0.5.7：普通下载同步镜像、同目录防重与 Sync 完成态护栏

1. 变更摘要：运行时与 README 版本升级到 `v0.5.7`；`DownloadStartOptions` 新增 `DownloadExecutionPurpose`，普通下载默认走 `Standard`，Sync 页面触发的下载显式走 `SyncManaged`；`DownloadService` 在普通下载目录与同步下载目录相同的场景下避免对同一路径做重复复制/重复处理；当普通下载是完整下载且普通目录/同步目录都缺少目标文件时，会在普通目录落盘真实媒体的同时镜像一份到同步下载目录，并将 SQLite `WorkSyncInfo` 直接写为 `COMPLETED`；若普通下载属于子集下载（如 `fileFilter`、`hdAudioOnly`），则只镜像实际下载的文件，不再误写 `COMPLETED`，避免污染后续 Sync 增量判定。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadStartOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（84/84）；`rtk dotnet test dotnet/Asmroner.sln` 通过（331/331）。
4. DoD 判定：是。用户本轮要求的 `v0.5.7` 版本对齐、普通下载/同步目录同路径防重、完整普通下载同步镜像与 SQLite `COMPLETED` 回写、SyncManaged 路径隔离、子集下载完成态护栏、回归测试与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.7`、完整普通下载在双目录都为空时会同时落普通目录和同步目录并写 SQLite `COMPLETED`、下载目录与同步目录同路径时不会重复处理，以及子集普通下载不会误写 `COMPLETED`；章节 2.1 基线更新为 2026-04-12 的 `331/331`，章节 3.1 保持单一 `v0.5.7` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.103 2026-04-14，v0.5.8 下载/同步状态修复

1. 变更摘要：运行时与 README 版本升级到 `v0.5.8`；Download 页新增 `DownloadToolbarAvailability`，纠正“立即下载选中任务/执行下载队列”期间顶部按钮分组，使“打开下载目录”保持可点击，并让“从文件导入”与其它入队入口保持一致的禁用/恢复行为；Sync 页拆分重试与导出状态，允许“重试失败项”期间继续“刷新统计”，并把状态面板拆为“同步状态 / 下载状态”两行显示。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadToolbarAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncStatusTextBuilder.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadToolbarAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncStatusTextBuilderTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo --no-restore` 通过（165/165）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo --no-restore` 通过（84/84）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（338/338）。
4. DoD 判定：是。用户本轮要求的 `v0.5.8` 版本对齐、Download 两处按钮状态修复、Sync 页面“重试失败项”期间刷新统计恢复可用、Sync 状态文本拆分为两行、单元测试补充与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3、4.4 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.8`、Download 页在“立即下载选中任务/执行下载队列”期间“打开下载目录”仍可点击且“从文件导入”与其它入队入口行为一致、Sync 页在“重试失败项”期间仍可手动“刷新统计”，以及状态面板按“同步状态 / 下载状态”两行分别显示；章节 1.2/1.3 已检查且无需调整，章节 2.1 基线更新为 2026-04-14 的 `338/338`，章节 3.1 新增单一 `v0.5.8` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.104 2026-04-14，v0.5.9：发布页抓取链路收敛与候选集合增量持久化

1. 变更摘要：运行时与 README 版本升级到 `v0.5.9`；`EndpointDiscoveryService` 不再直接创建临时 `HttpClient` 抓取发布页，而是通过 `EndpointDiscoveryHttpTransport` + `IHttpClientFactory` 新增发布页客户端，统一收敛 HTML/脚本抓取的 HTTPS 处理、HTTP/1.1 约束、请求头、超时、取消与可测试性；同时保留“正文优先提取最新域名、入口脚本回退解析”的发现链路。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryHttpTransport.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/InfrastructureTestDoubles.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（168/168）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（343/343）。
4. DoD 判定：是。用户本轮要求的 `v0.5.9` 版本对齐、发布页抓取链路 review/优化、发现成功后按“新旧候选合并且总数增加”规则写回 SQLite、单元测试补齐与 progress 文档同步均已落地；当前 Windows 10 机器无法访问目标 URL 的实网限制已按要求排除在本批处理范围外。
5. 下次计划：由用户在 Windows 11 环境执行章节 4.1 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.9`、“测试连接”在发布页正文/入口脚本两条路径下均可发现最新域名，以及当发现结果与已保存候选集合合并后数量增加时，SQLite `ApiCandidateUrls` 会保留旧候选并写回合并后的集合；章节 1.2/1.3 已检查且无需调整，章节 2.1 基线更新为 2026-04-14 的 `343/343`，章节 3.1 新增单一 `v0.5.9` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.105 2026-04-15，v0.6.0：阶段 6 首批资源库扫描、Library 页签与播放上下文抽象

1. 变更摘要：运行时、README 与 progress 文档版本升级到 `v0.6.0`；启动阶段 6 首批迁移，新增资源库目录双格式解析、Library 扫描/查询服务、播放上下文抽象服务，并在主窗口接入 `Library` 页签与资源库页面骨架。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Library/LibraryModels.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ILibraryScannerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ILibraryQueryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryQueryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、新增阶段 6 测试文件、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj --nologo --no-restore` 通过（14/14）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo --no-restore` 通过（90/90）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo --no-restore` 通过（171/171）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（355/355）。
4. DoD 判定：否。阶段 6 当前仅完成首批资源扫描/索引、列表/筛选与播放上下文装载骨架；真实播放器接入、播放控制与更完整的异常场景验证仍待后续批次完成。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证主窗口新增 `Library` 页签、Settings 版本文案 `v0.6.0`、双格式目录扫描、筛选/分页、文件树与上下文装载提示；随后继续阶段 6 下一批真实播放器接入与异常处理补强。

### 1.106 2026-04-15，v0.6.1：阶段 6 第二批最小真实播放与 Library 播放控制

1. 变更摘要：运行时、README 与 progress 文档版本升级到 `v0.6.1`；继续阶段 6 第二批迁移，新增底层播放引擎抽象与基于 WPF `MediaPlayer` 的最小播放实现，升级 `PlayerService` 为真实播放编排，并在 `Library` 页接入播放/暂停/停止与进度显示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlaybackEngine.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackFailedEventArgs.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/WpfMediaPlaybackEngine.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/PlayerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（95/95）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（171/171）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（361/361）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与最小真实播放，但更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.1`、Library 页的载入/切换文件、播放/暂停/停止、时间进度显示，以及缺失文件/不支持格式时的失败提示；随后继续阶段 6 的异常处理与播放体验补强。

### 1.107 2026-04-15，v0.6.1：阶段 6 从内嵌播放切换到系统默认程序打开

1. 变更摘要：移除基于 WPF `MediaPlayer` 的内嵌播放链路，保留 `Library` 页的文件上下文与“播放”入口，但改为仅对显式选中的可播放媒体文件调用系统默认关联程序打开；同时删除“暂停 / 停止”按钮与时间进度显示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMediaLauncher.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellMediaLauncher.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryPlaybackSelectionPolicy.cs`、`dotnet/tests/Asmroner.Application.Tests/PlayerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryPlaybackSelectionPolicyTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（96/96）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（172/172）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（363/363）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与显式选中文件后的系统默认程序打开，但更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证显式选中文件后的“载入/切换文件”与“播放”交互、系统默认程序打开行为、失败提示，以及界面不再出现“暂停 / 停止 / 时间进度”相关控件或文案；随后继续阶段 6 的异常处理与体验补强。

### 1.108 2026-04-15，v0.6.1：修复 Shell 成功打开却误报失败

1. 变更摘要：修复 `Library` 页系统默认程序打开链路中对 Windows Shell 返回值的错误判定；当 `Process.Start` 已成功触发关联程序但返回 `null` 时，不再误报“打开失败”；同时为 `ShellMediaLauncher` 增加专项单元测试覆盖 `null` 返回与异常抛出路径。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellMediaLauncher.cs`、`dotnet/tests/Asmroner.Wpf.Tests/ShellMediaLauncherTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（96/96）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（174/174）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（365/365）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与显式选中文件后的系统默认程序打开，并修复成功打开误报失败问题；更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证系统默认程序已成功拉起但底层 Shell 调用返回 `null` 时不再误报失败，以及真实打开失败时仍显示明确错误提示；随后继续阶段 6 的异常处理与体验补强。

### 1.109 2026-04-16，v0.6.2：补强 Library 异常扫描容错与选择反馈

1. 变更摘要：继续推进阶段 6，在保留系统默认程序打开方案的前提下补强资源库链路；`LibraryScannerService` 新增可测试的文件系统枚举 seam，并将嵌套目录/文件读取失败收敛为局部错误记录，避免单个异常目录拖垮整项扫描；`LibraryView` 新增显式选择反馈策略，对目录、不可播放文件、缺失媒体文件与可播放文件分别给出明确提示，并收紧“载入/切换文件”“播放”按钮可用性；同步将运行时/UI/README 版本对齐到 `v0.6.2`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibrarySelectionFeedbackPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryScannerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibrarySelectionFeedbackPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（98/98）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（178/178）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（371/371）。
4. DoD 判定：否。当前已完成资源库扫描/索引、列表/筛选、显式选中文件后的系统默认程序打开，以及异常扫描容错与选择反馈补强；更完整的格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证目录/不可播放文件/缺失媒体文件的明确提示、按钮禁用状态，以及可播放文件仍能完成载入与系统默认程序打开；随后继续阶段 6 的格式兼容与体验收口。

### 1.110 2026-04-16，v0.6.3：收口 Library 可播放格式规则与作品级选择引导

1. 变更摘要：继续推进阶段 6，在保持系统默认程序打开方案不变的前提下，把可播放扩展名判断抽到共享 `LibraryPlayableMediaPolicy`，统一扫描层与 UI 侧的格式规则；`LibraryView` / `LibrarySelectionFeedbackPolicy` 在仅选中作品、尚未选中文件时新增作品级引导，可提示当前作品是否包含可播放媒体文件、首个候选路径，以及无可播放文件时的支持格式说明；同步将运行时/UI/README 版本对齐到 `v0.6.3`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Library/LibraryPlayableMediaPolicy.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibrarySelectionFeedbackPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Core.Tests/LibraryPlayableMediaPolicyTests.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryScannerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibrarySelectionFeedbackPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj --no-restore` 通过（19/19）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（99/99）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（180/180）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（378/378）。
4. DoD 判定：否。当前已补齐共享可播放格式规则与作品级选择引导，阶段 6 在代码与自动化测试层面已进一步收口；但章节 4.1 / 4.6 的受影响手工项已按规则重置为未勾选，完整 DoD 证据仍待用户手工回归确认。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.3`、仅选中作品时的首个候选提示、无可播放文件时的支持格式说明，以及大小写混合支持扩展名文件仍可被识别并完成载入与系统默认程序打开；随后视人工验证结果决定是否关闭阶段 6。

### 1.111 2026-04-16，v0.6.3：收口 Library 播放上下文表达与 DoD 闭环准备

1. 变更摘要：继续推进阶段 6，在保持 `v0.6.3` 与当前 shell-open 方案不变的前提下，新增 `LibraryPlaybackContextTextBuilder`，把 Library 右侧的“当前选择”与“当前已载入上下文”拆开显示；`LibraryView` 保留“载入/切换文件”“播放”“清空上下文”三按钮，但上下文区不再混写当前选择反馈与已载入状态，便于人工验证清空上下文、重新载入和系统默认程序打开的真实状态。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryPlaybackContextTextBuilder.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryPlaybackContextTextBuilderTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（99/99）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（183/183）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（381/381）。
4. DoD 判定：否。当前已补齐播放上下文表达与自动化证据，阶段 6 在代码层面进一步收口；但章节 4.6 的受影响 Library 手工项仍待用户执行，阶段 6 DoD 仍不能由 AI 单方面闭环。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.3`、“当前选择 / 当前已载入上下文”分离展示、清空上下文后的保留选择提示、以及显式选中文件后的系统默认程序打开；若人工验证通过，再回写阶段 6 完成状态。

### 1.112 2026-04-16，v0.6.3：确认阶段 6 手工回归通过并关闭 DoD

1. 变更摘要：用户已完成并确认章节 4.1 与 4.6 的受影响手工回归通过，本次据此回写阶段 6 完成状态；阶段总览切换为“已完成”，阶段执行勾选中的“阶段 6 DoD 已满足并记录证据”同步闭环，并将待提交行整理为包含共享格式规则、作品级引导、播放上下文分离展示与手工回归确认的单一 v0.6.3 提交说明。
2. 关键文件：`docs/wpf-migration-progress.md`。
3. 验证结果：用户已明确确认章节 4.1 / 4.6 的受影响手工回归通过；最近一次自动化回归基线保持为 `rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（381/381）。
4. DoD 判定：是。阶段 6 的代码、自动化测试与用户手工回归证据已闭环。
5. 下次计划：进入阶段 7（UI 集成与体验收口）的方案制定与实施准备。

---

### 1.113 2026-04-16，v0.6.4：补齐 Library 分页并修正资源库页缩放布局

1. 本次变更摘要：Library 页面补齐跳页与 `page size` 控件，并将作品列表、详情区、文件树与上下文区改为受限高度 + 内部滚动布局，修复选中作品后右侧内容越界、窗口放大后左右下边沿错位、窗口缩小时内容溢出的三个界面问题。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryQueryServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（100/100）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（184/184）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（383/383）。
4. DoD 判定：是。阶段 6 状态保持“已完成”，本次作为阶段 6 后续维护收口；自动化回归通过，手工验证项已重置为待验证。

### 1.114 2026-04-18，v0.7.0：启动阶段 7 壳层集成与共享交互收口

1. 本次变更摘要：运行时与 README 版本统一升级到 `v0.7.0`；新增 `ShellViewModel`、`NavigationService`、`UiMessageService`、`DialogService` 与 `ShellResources.xaml`，将主窗口页签启用状态、选中页签与底部状态文案收敛到壳层状态；同时把 Search/Download/Sync 的文件对话框入口统一到共享服务，并让 Search/Download/Library/Settings 四页卡片/输入/按钮样式改为基于共享壳层资源。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Styles/ShellResources.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/ShellViewModel.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/NavigationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/UiMessageService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DialogService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/*`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（100/100）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（198/198）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（397/397）。
4. DoD 判定：否。阶段 7 已完成壳层优先的第一批基线收口与自动化验证，但加载态/空态统一、UI 冒烟与用户手工回归尚未闭环。
5. 下次计划：继续补齐阶段 7 的加载态/空态统一与剩余壳层体验收口；由用户执行章节 4.1/4.2/4.3/4.4/4.5/4.7 的受影响手工回归，重点验证 `v0.7.0` 版本文案、Settings 保持页签、共享状态栏消息、统一文件对话框行为与共享样式在多窗口尺寸下的一致性。

### 1.115 2026-04-18，v0.7.1：统一页面加载态/空态并拆分迁移跟踪文档

1. 本次变更摘要：运行时与 README 版本统一升级到 `v0.7.1`；新增 `PageLoadStateService`、`ShellStatusTextSynchronizer` 与共享 `PageStatePresenterTemplate`，把 Search/Download/Library/Settings/Sync 五页的 busy 态统一到共享页面状态模型，为 Search/Download/Library/Sync 落地统一空态面板，并将页内 `StatusTextBlock` 的有效消息桥接到壳层状态栏；同时将 `docs/wpf-migration-progress.md` 的 `1.5` / `2` 拆分为独立文档并同步调整 prompt 约束。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Styles/ShellResources.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/PageLoadStateService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellStatusTextSynchronizer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/UiMessageService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml(.cs)`、`dotnet/tests/Asmroner.Wpf.Tests/*`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`、`docs/wpf-migration-history.md`、`docs/wpf-migration-tests.md`、`docs/prompts/document_special_prompts.md`、`docs/prompts/version_update_prompts.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（201/201）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（400/400）。
4. DoD 判定：否。阶段 7 的共享 busy 面板、Search/Download/Library/Sync 空态面板、页内状态消息桥接与当前自动化基线已完成，但壳层消息桥接过滤、UI 冒烟与用户手工回归仍待完成。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4/4.5/4.6/4.7 的受影响手工回归，重点验证 `v0.7.1` 版本文案、五页共享 busy 面板、Search/Download/Library/Sync 空态文案、Search/Download 联动链路、Settings 保存时壳层状态栏同步，以及壳层消息栏不会被隐藏页更新或占位文案错误覆盖。

### 1.116 2026-04-28，v0.7.2：收口壳层消息桥接策略并补齐阶段 7 单测

1. 本次变更摘要：运行时与 README 版本统一升级到 `v0.7.2`；在壳层状态栏桥接链路中抽取 `ShellStatusRelayPolicy` 并统一"可见且有效消息才发布"的判定，避免隐藏页面、空白消息或初始化占位文案误覆盖壳层状态；同时补充阶段 7 相关单元测试，覆盖壳层消息发布门禁与 Sync 状态转发策略。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellStatusTextSynchronizer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/SyncShellStatusRelayPolicy.cs`、`dotnet/tests/Asmroner.Wpf.Tests/ShellStatusRelayPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncShellStatusRelayPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/UiMessageServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`、`docs/wpf-migration-tests.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo --no-restore` 通过（212/212）；`rtk dotnet build dotnet/Asmroner.sln --nologo --no-restore -c Release` 通过。
4. DoD 判定：否。阶段 7 的壳层消息桥接规则与自动化覆盖已补强，UI 冒烟与手工回归仍待用户执行并回填。
5. 下次计划：由用户执行章节 3.1 与 3.7 的受影响手工回归，重点验证 `v0.7.2` 版本文案、Settings 保存后壳层状态栏消息、以及页面切换场景下壳层消息不被隐藏页或占位文案错误覆盖。





