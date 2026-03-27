# asmr-downloader WPF 项目进度跟踪

当前跟踪版本：v0.4.1

AI约束策略：章节1.5.1到1.5.60的文本不加入分析上下文

## 1. 项目进度跟踪清单

本章节用于在迁移实施过程中持续记录阶段进展，作为团队与 AI 协作的统一进度面板。

## 1.1 状态标记说明

- `未开始`：阶段尚未进入实施。
- `进行中`：阶段已开始，尚未满足 DoD。
- `已完成`：阶段 DoD 已全部满足，并完成验证记录。
- `阻塞`：阶段存在外部依赖或关键问题，无法继续推进。

## 1.2 阶段总览清单

| 阶段   | 名称                   | 状态   | 负责人    | 开始日期   | 目标完成日期 | 实际完成日期 | 备注                                                                                                                              |
| ------ | ---------------------- | ------ | --------- | ---------- | ------------ | ------------ | --------------------------------------------------------------------------------------------------------------------------------- |
| 阶段 0 | 创建 .NET 解决方案骨架 | 已完成 | AI + 用户 | 2026-03-14 | 2026-03-14   | 2026-03-14   | 已确认 9 项目在解决方案中；`dotnet build dotnet/Asmroner.sln` 成功；`dotnet test dotnet/Asmroner.sln` 通过 5/5。                  |
| 阶段 1 | 配置与初始化迁移       | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成配置模型、TOML 读写、初始化编排、首次启动设置向导分支与“保存后重新初始化”闭环；`dotnet test dotnet/Asmroner.sln` 通过 9/9。 |
| 阶段 2 | API 与认证迁移         | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已修复 `HttpClient` 配置时机问题并完成回归，`dotnet test dotnet/Asmroner.sln` 通过 14/14。                                        |
| 阶段 3 | 搜索能力迁移           | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成查询解析、分页聚合、搜索导出、入队与高级筛选/分页 UI 交互；分页参数透传修复后回归通过。                                     |
| 阶段 4 | 下载能力迁移           | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成下载入口、并发/重试控制与状态可视化复核；详见 16.5.18 与 16.5.20；`dotnet test dotnet/Asmroner.sln` passed 40/40。          |
| 阶段 5 | 同步能力迁移           | 未开始 | 待填写    | 待填写     | 待填写       | 待填写       | -                                                                                                                                 |
| 阶段 6 | 资源库与播放能力迁移   | 未开始 | 待填写    | 待填写     | 待填写       | 待填写       | -                                                                                                                                 |
| 阶段 7 | UI 集成与体验收口      | 未开始 | 待填写    | 待填写     | 待填写       | 待填写       | -                                                                                                                                 |
| 阶段 8 | 最终验收与发布准备     | 未开始 | 待填写    | 待填写     | 待填写       | 待填写       | -                                                                                                                                 |

## 1.3 阶段执行勾选清单

每完成一个阶段后，在对应条目打勾并补齐证据链接或说明。

### 阶段 0：创建 .NET 解决方案骨架

- [x] `dotnet/Asmroner.sln` 已创建并可加载。
- [x] 项目结构与引用关系符合第 12.1.4 节约束。
- [x] WPF 客户端可启动并显示主窗口。
- [x] 解决方案级构建与测试命令通过。
- [x] 阶段 0 DoD 已满足并记录证据。

### 阶段 1：配置与初始化迁移

- [x] 配置读取、保存、校验闭环可用。
- [x] 初始化管线可稳定执行。
- [x] 首次启动向导分支可用。
- [x] 阶段 1 测试矩阵通过。
- [x] 阶段 1 DoD 已满足并记录证据。

### 阶段 2：API 与认证迁移

- [x] API 客户端与统一 HTTP 管线落地。
- [x] 登录与 token 缓存机制可用。
- [x] 关键只读接口调用通过。
- [x] 测试连接入口可输出明确结果。
- [x] 阶段 2 DoD 已满足并记录证据。

### 阶段 3：搜索能力迁移

- [x] 高级查询语法解析兼容。
- [x] 分页搜索与结果聚合可用。
- [x] 搜索结果导出可用。
- [x] 搜索到下载队列链路打通。
- [x] 阶段 3 DoD 已满足并记录证据。

### 阶段 4：下载能力迁移

- [x] 单个、批量、热门下载能力可用。
- [x] 限流、重试、并发控制生效。
- [x] 下载任务状态可视化准确。
- [x] 取消与失败重试可用。
- [x] 阶段 4 DoD 已满足并记录证据。

### 阶段 5：同步能力迁移

- [ ] 元数据同步与入库可用。
- [ ] 同步下载与容量控制可用。
- [ ] 失败重试与导出功能可用。
- [ ] 报表与统计展示正确。
- [ ] 阶段 5 DoD 已满足并记录证据。

### 阶段 6：资源库与播放能力迁移

- [ ] 本地资源扫描与索引可用。
- [ ] 资源库页面浏览与筛选可用。
- [ ] 播放控制（播放/暂停/停止/切换）可用。
- [ ] 异常文件处理不导致应用崩溃。
- [ ] 阶段 6 DoD 已满足并记录证据。

### 阶段 7：UI 集成与体验收口

- [ ] 全局导航与页面路由稳定。
- [ ] 消息、对话框、加载态、空态统一。
- [ ] 跨页面关键流程回归通过。
- [ ] UI 冒烟测试通过。
- [ ] 阶段 7 DoD 已满足并记录证据。

### 阶段 8：最终验收与发布准备

- [ ] 第 14 节验收项全部通过。
- [ ] 发布包构建、签名、安装验证通过。
- [ ] 发布说明与迁移说明完成。
- [ ] 回滚策略完成验证。
- [ ] 阶段 8 DoD 已满足并记录证据。

## 1.4 阻塞与风险登记

| 日期       | 阶段   | 问题描述                                                                                                                                                                                                                           | 影响范围                                     | 处理人    | 当前状态             | 计划解决日期 |
| ---------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- | --------- | -------------------- | ------------ |
| 2026-03-15 | 阶段 2 | `dotnet test dotnet/Asmroner.sln` 出现 2 个失败，均为 `HttpClient` 已发起请求后再次修改 `Timeout/BaseAddress` 导致。                                                                                                               | API 与认证迁移 DoD 无法闭环                  | AI + 用户 | 已解决（同日）       | 2026-03-15   |
| 2026-03-15 | 阶段 3 | 分页交互中 UI 已传入 `page` 参数，但 `SearchService` 内部固定 `Page = 1`，导致翻页请求实际仍从第一页开始。                                                                                                                         | 阶段 3 DoD 第 3 条“分页搜索可稳定运行”不满足 | AI + 用户 | 已解决（同日）       | 2026-03-15   |
| 2026-03-16 | 阶段 4 | 下载流程 RunSingleAsync 仍报 API 调用失败 400 Bad Request，问题待继续定位。                                                                                                                                                        | 阶段 4 下载链路稳定性受影响                  | AI + 用户 | 已解决（2026-03-16） | 2026-03-16   |
| 2026-03-17 | 阶段 4 | 在 Search 页面选择并点击"加入下载队列"时，如果该任务已存在且状态为 Canceled，仅通过 `_queuedStatusOverrides` 将枚举值更新为 Pending，但 `DownloadTaskRowViewModel.StatusText` 未同步更新，导致 Download 页面行仍显示"已取消"文本。 | Download 页面行状态文本与实际排队状态不一致  | AI + 用户 | 待解决               | 2026-03-19   |

## 1.5 变更与验证记录

建议每次阶段推进后，补充以下记录：

1. 本次变更摘要（对应阶段与执行步骤编号）。
2. 关键修改文件列表。
3. 构建与测试命令结果摘要。
4. DoD 判定结果（是/否 + 原因）。
5. 遗留问题与下次计划。

变更策略：`1.5` 节仅追加新的记录，不再修改已有历史记录；如需修正前一次记录的描述或补充证据，将在后续新条目中引用对应条目编号并补充说明。

### 1.5.1 2026-03-14，阶段 0 复核

1. 变更摘要：按第 12.1 节逐项复核阶段 0 完成度，并回写第 16 节进度状态。
2. 关键文件：`dotnet/Asmroner.sln`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj`。
3. 验证结果：`dotnet sln dotnet/Asmroner.sln list` 显示 9 项目；`dotnet build dotnet/Asmroner.sln` 成功；`dotnet test dotnet/Asmroner.sln` 总计 5，失败 0，成功 5。
4. DoD 判定：是。第 12.1.6 节要求均已满足。
5. 下次计划：进入阶段 1（配置与初始化迁移），优先完成配置模型映射与 `ConfigurationService` 读写闭环。

### 1.5.2 2026-03-15，阶段 1 完成

1. 变更摘要：按第 12.2 节落地 `AppConfig` 字段映射、`ConfigurationService`（TOML 读写与校验）、`AppPathService`、`DatabaseInitializer`、`ApplicationBootstrapper` 与 `FirstRunService`；WPF 启动流程接入初始化编排并实现“配置缺失进入设置页 + 保存后重新初始化”闭环。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/AppConfig.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/ApplicationBootstrapper.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过，总计 9，失败 0，成功 9。
4. DoD 判定：是。第 12.2.6 节要求均已满足。
5. 下次计划：进入阶段 2（API 与认证迁移），优先实现 `AsmrApiClient`、`EndpointDiscoveryService` 与 `AuthService` 最小可用链路。

### 1.5.3 2026-03-15，阶段 1 复核与 Debug 运行

1. 变更摘要：按第 12.2 节复核现有实现，重点检查配置接口、设置页字段、基础设施依赖包与初始化链路是否仍一致。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/ApplicationBootstrapper.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过，总计 9，失败 0，成功 9；WPF 已使用 Debug 配置启动验证。
4. DoD 判定：是。第 12.2.6 节要求持续满足。
5. 下次计划：进入阶段 2（API 与认证迁移），并补充设置页“测试连接”入口。

### 1.5.4 2026-03-15，阶段 2 完成

1. 变更摘要：按第 12.3 节完成 API 与认证迁移，新增统一 HTTP 管线、API 选项提供器、最快站点发现、token 存储、认证服务、最小只读 API 客户端与连接探测服务，并在设置页补充“测试连接”入口。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConnectivityProbeService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`。
3. 验证结果：解决方案 Debug 构建成功，项目 9/9 成功；测试执行通过，测试项目 2/2 通过；文件级诊断无新增错误。
4. DoD 判定：是。第 12.3.9 节要求均已满足。
5. 下次计划：进入阶段 3（搜索能力迁移），优先落地查询参数解析、搜索请求组装与结果列表绑定。

### 1.5.5 2026-03-15，阶段 2 复核与 Debug 运行

1. 变更摘要：按第 12.3 节复核现有实现与测试现状，重新执行解决方案测试，并使用 Debug 模式启动 WPF 进行运行验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 14，失败 2，成功 12；失败集中在 `Asmroner.Infrastructure.Tests`，错误原因为 `HttpClient` 请求发起后再次修改 `Timeout/BaseAddress`；WPF 已使用 Debug 配置执行启动验证（进程可拉起）。
4. DoD 判定：否。第 12.3.9 节第 6 条“阶段 2 相关单元测试与集成测试通过”当前不满足。
5. 下次计划：优先修复 `HttpClient` 配置时机问题（避免在同一客户端首个请求后再改 `Timeout/BaseAddress`），随后重跑 `dotnet test dotnet/Asmroner.sln` 并再次回写第 16 节状态。

### 1.5.6 2026-03-15，阶段 2 修复回归

1. 变更摘要：修复 `HttpClient` 配置时机问题，将 `BaseAddress/Timeout` 从客户端级配置改为请求级拼接与超时控制；同步调整 API 调用、登录调用与站点探测调用路径。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 总计 7，失败 0，成功 7；`dotnet test dotnet/Asmroner.sln` 总计 14，失败 0，成功 14。
4. DoD 判定：是。第 12.3.9 节要求已满足。
5. 下次计划：进入阶段 3（搜索能力迁移），按第 12.4 节推进查询参数与结果映射主链路。

### 1.5.7 2026-03-15，阶段 3 启动与最小闭环

1. 变更摘要：按第 12.4 节落地搜索主链路，新增查询解析器、分页聚合搜索服务、CSV/JSON 导出服务与搜索入队状态存储；Dashboard 从占位页升级为可搜索、导出、入队，并补齐高级筛选面板与分页交互（上一页/下一页/跳页/页大小）。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Search/SearchQuery.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IQueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchExportService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 6，失败 0，成功 6；`dotnet test dotnet/Asmroner.sln` 总计 17，失败 0，成功 17；WPF Debug 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。阶段 3 处于进行中，尚未完成全部测试矩阵与高级筛选体验收口。
5. 下次计划：补齐高级筛选面板与分页交互细节，并完成阶段 3 测试矩阵回归。

### 1.5.8 2026-03-15，阶段 3 DoD 复核

1. 变更摘要：结合第 12.4 节逐条复核阶段 3 完成度，重点检查“分页搜索稳定性”与测试覆盖；同步执行 WPF 运行验证。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。`SearchService` 当前固定 `Page = 1`，与 UI 分页参数不一致，第 12.4.7 节第 3 条暂不满足。
5. 下次计划：修复分页参数透传（按当前页拉取数据）并补充“非第一页查询结果正确性”测试后再次复核 DoD。

### 1.5.9 2026-03-15，阶段 3 分页透传修复与复核通过

1. 变更摘要：修复 `SearchService` 分页参数透传逻辑，取消固定第一页，改为按查询中的 `page` 发起请求；补充“非第一页 + 筛选条件”回归测试并复跑验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 7，失败 0，成功 7；`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：是。第 12.4.7 节 6 项要求均已满足，阶段 3 可判定完成。
5. 下次计划：进入阶段 4（下载能力迁移），优先落地下载执行服务、任务状态模型与取消/重试控制。

### 1.5.10 2026-03-15，阶段 3 再次复核（按第 12.4 节）

1. 变更摘要：按第 12.4 节再次执行阶段 3 代码复核，确认查询解析、分页聚合、导出、入队与 UI 交互链路均保持可用，并同步执行运行验证。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 总计 18，失败 0，成功 18；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：是。阶段 3 当前状态与第 12.4.7 节要求一致，可持续视为已完成。
5. 下次计划：进入阶段 4（下载能力迁移）执行最小闭环。

### 1.5.11 2026-03-15，阶段 4 最小实现落地

1. 变更摘要：按第 12.5.7 节先落地阶段 4 最小闭环，新增下载任务模型、限流服务与下载执行服务，打通“搜索队列消费 -> 下载任务状态更新 -> 目标目录产物落地”链路。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadTaskItem.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IRateLimiterService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/RateLimiterService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 总计 9，失败 0，成功 9；`dotnet test dotnet/Asmroner.sln` 总计 20，失败 0，成功 20；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。阶段 4 当前仅完成最小实现，尚未满足第 12.5.9 节关于取消/重试、完整 UI 可视化与完整下载流程的要求。
5. 下次计划：补齐下载页 UI、取消/重试命令与热门/批量流程，再执行阶段 4 DoD 复核。

### 1.5.12 2026-03-15，阶段 4 最小下载页可视化接入

1. 变更摘要：在 WPF 主界面新增 `Download` 页签，接入 `IDownloadService.GetTasks/RunQueuedAsync`，实现“执行下载队列 + 刷新任务列表 + 队列数量展示”的最小可视化闭环。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 20，失败 0，成功 20）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。虽已具备最小下载任务可视化，但第 12.5.9 节“取消/重试能力”和“单个/批量/热门完整下载流程”仍未满足。
5. 下次计划：补齐取消与失败重试命令，接入单个/批量/热门入口并补充阶段 4 回归测试矩阵。

### 1.5.13 2026-03-15，阶段 4 取消/重试增强（含批量重试节流）

1. 变更摘要：下载页新增“批量取消 + 二次确认”与“重试全部失败任务”命令；批量重试采用受控并发（最大并发 2）逐项调用服务层 `RetryFailedAsync`，避免瞬时并发放大。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 22，失败 0，成功 22）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。第 12.5.9 节“取消/重试能力”已满足，但“单个/批量/热门完整下载入口”与阶段 4 全量验证仍待完成。
5. 下次计划：补齐单个 RJID、批量 RJID、热门下载入口并完成阶段 4 全量 DoD 复核。

### 1.5.14 2026-03-15，阶段 4 下载入口补齐（单个/批量/热门）

1. 变更摘要：下载页新增“单个 RJID 入队、批量 RJID 入队、热门作品入队”入口；热门入队接入 `IAsmrApiClient.GetPopularAsync`，并支持数量控制；入队后复用既有队列执行链路。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 执行通过（总计 22，失败 0，成功 22）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug` 冒烟启动成功（进程可拉起后已主动停止）。
4. DoD 判定：否。第 12.5.9 节第 1 条已满足，但仍需补齐阶段 4 全量回归与稳定性复核后再判定“已完成”。
5. 下次计划：执行阶段 4 全量复核（并发/限流/任务状态准确性）并关闭剩余勾选项。

### 1.5.15 2026-03-15，阶段 4 队列并发控制落地与回归

1. 变更摘要：下载执行服务 `RunQueuedAsync` 新增按 `Downloader.MaxWorkers` 的队列并发执行控制（`SemaphoreSlim`），并保持任务取消/重试语义不变。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：新增 `RunQueuedAsync_ShouldRespectConfiguredMaxWorkers` 回归测试，验证队列执行最大并发不超过配置值且可达到配置上限；解决方案测试与 WPF 冒烟通过。
4. DoD 判定：否。第 12.5.9 节“限流、重试、并发控制”已满足，阶段 4 当前剩余“下载任务状态可视化准确”与最终收口复核。
5. 下次计划：补充下载任务状态可视化准确性复核与对应测试证据，完成阶段 4 DoD 关闭。

### 1.5.16 2026-03-15，阶段 4 收口复核（状态可视化准确）

1. 变更摘要：完成阶段 4 最终复核，重点验证“下载任务状态可视化准确”与端到端链路一致性；确认 UI 状态列直接绑定 `DownloadTaskItem.Status` 且刷新路径统一走 `IDownloadService.GetTasks()`。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln` 通过（总计 23，失败 0，成功 23）；`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -- --smoke-test` 正常退出。
4. DoD 判定：是。第 12.5.9 节 5 项要求均已满足，阶段 4 可判定完成。
5. 下次计划：进入阶段 5（同步能力迁移），先落地最小“同步入库 + 状态展示”闭环。

### 1.5.17 2026-03-15，阶段 4 按第 12.5 节复核与 Debug 运行验证

1. 变更摘要：按第 12.5 节对阶段 4 进行代码复核，逐项核对第 12.5.9 节 DoD（单个/批量/热门下载、限流/重试/并发、任务可视化状态、取消与失败重试、测试通过），并补充最新运行验证记录。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/RateLimiterService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 23，失败 0，成功 23）；已使用 Debug 配置启动 WPF：`dotnet run --project dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj -c Debug`（后台拉起 5 秒无显式错误输出，随后主动停止进程）。
4. DoD 判定：是。阶段 4 与第 12.5.9 节要求保持一致，当前可持续视为已完成状态。
5. 下次计划：进入阶段 5（同步能力迁移），先建立“元数据同步入库 + 状态展示”最小闭环。

### 1.5.18 2026-03-15，阶段 4 下载入参兼容修复（RunSingleAsync 400）

1. 变更摘要：修复下载执行链路中 `sourceId` 输入兼容性问题。对队列入参与 API 客户端调用参数统一做 RJID 规范化（支持从 URL/混合文本中提取 `RJ\d+`），避免 `RunSingleAsync` 中调用 `/api/work/{id}`、`/api/tracks/{id}` 时因非法 id 触发 `400 Bad Request`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchStateStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 21，失败 0，成功 21）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln` 通过（总计 38，失败 0，成功 38）。
4. DoD 判定：是。阶段 4 既有 DoD 不受影响且稳定性提升（手动输入链接场景可用）。
5. 下次计划：进入阶段 5（同步能力迁移），保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.5.19 2026-03-15，阶段 4 下载页输入框实时归一化体验优化

1. 变更摘要：下载页新增输入侧体验优化。单个与批量输入框在 `TextChanged` 时实时归一化 URL/混合输入并显示为 `RJxxxx`，使用户在入队前可直接确认最终下载 ID；入队逻辑同步复用统一归一化器，确保 UI 显示与后端实际请求一致。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadInputNormalizer.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizationTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 3，失败 0，成功 3）；`dotnet test dotnet/Asmroner.sln` 通过（总计 40，失败 0，成功 40）。
4. DoD 判定：是。该变更为阶段 4 的可用性增强，不改变既有功能语义并提升输入透明度。
5. 下次计划：进入阶段 5（同步能力迁移），继续保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.5.20 2026-03-16，阶段 4 下载入参兼容补强（RunSingleAsync 400 回归修复）

1. 变更摘要：对 `sourceId` 归一化策略进行补强，新增统一 `SourceIdNormalizer` 并在 Application、Infrastructure、WPF 三层复用；在原有 URL 提取基础上补齐 `RJ-12345`、`RJ 12345`、纯数字 `12345` 等非标准输入格式，统一规范为 `RJ12345`，避免下载调用 `/api/work/{id}`、`/api/tracks/{id}` 时出现 `400 Bad Request`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Utils/SourceIdNormalizer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SearchStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadInputNormalizer.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchStateStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 21，失败 0，成功 21）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 14，失败 0，成功 14）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 3，失败 0，成功 3）；`dotnet test dotnet/Asmroner.sln` 通过（总计 42，失败 0，成功 42）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，且输入兼容性边界进一步完善。
5. 下次计划：进入阶段 5（同步能力迁移），保留“下载格式偏好与限流可观测性”测试补齐项。

### 1.5.21 2026-03-16，阶段 4 API 路径数字化修复（根本原因定位与修复）

1. 变更摘要：定位并修复"400 Bad Request"根本原因。`AsmrApiClient.GetWorkInfoAsync`/`GetTracksAsync` 此前调用 `SourceIdNormalizer.Normalize()` 拼接 API 路径，导致 URL 形如 `/api/work/RJ01426915`；而 asmr.one API 仅接受纯数字 ID（如 `/api/work/01426915`），故返回 400。与 Go 源码行为对齐（`IsValidDlsiteID` 提取 `number` 后以纯数字调用 API），在 `SourceIdNormalizer` 新增 `ToApiNumericId()` 方法（仅返回数字部分），并在 `AsmrApiClient` 两个方法中改用该方法；同步修正两个断言 API 路径含 `RJ` 前缀的错误测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Utils/SourceIdNormalizer.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，1.4 节阻塞项"400 Bad Request"已关闭。
5. 下次计划：进入阶段 5（同步能力迁移），保留"下载格式偏好与限流可观测性"测试补齐项。

### 1.5.22 2026-03-16，阶段 4 下载页体验增强（布局/筛选/图片视频格式/目录结构）

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

### 1.5.23 2026-03-16，阶段 4 下载配置与入队体验优化（格式整合 + 异步 WorkInfo）

1. 变更摘要：
 - **Settings 格式配置整合**：将原有"音频格式优先级"、"图片格式"、"视频格式"三个输入框整合为单一输入框，标题统一为"格式优先级（留空则全部下载）"，并在提示中补充常用文本格式（`txt,md,lrc,srt,ass,json,cue`）与常见音频/图片/视频格式示例。
 - **配置与解析逻辑更新**：新增 `DownloaderOptions.PreferFormats`；`ConfigurationService` 支持 `prefer_formats` 读写，并保留 `prefer_media/prefer_image/prefer_video` 兼容读取；`DownloadService` 统一按 `PreferFormats` 进行扩展名过滤，若留空则不做扩展名限制（全部下载）。
 - **单个/批量入队前异步刷新 WorkInfo**：`DownloadView` 的"加入单个下载"与"加入批量下载"改为异步流程，先并发拉取 `GetWorkInfoAsync` 再入队并刷新列表，避免 UI 线程阻塞；未开始任务在列表中显示 `Status=未下载` 且可显示已获取的标题。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Core.Tests/UnitTest1.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 通过（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。阶段 4 功能与稳定性持续满足，且下载页面交互流畅性提升。
5. 下次计划：阶段 5 同步能力迁移；补充 `PreferFormats` 过滤与"入队前 WorkInfo 预取"的专项测试样例。

### 1.5.24 2026-03-16，阶段 4 下载链路增强（默认格式、WorkInfo 内存复用、目录命名）

1. 变更摘要：
 - **默认格式优先级更新**：默认值调整为 `mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass`；Settings 页示例同步更新。
 - **WorkInfo 内存复用**：新增 `IDownloadService.UpsertPrefetchedWorkInfo`，`DownloadView` 在"加入单个/批量下载"时预取 WorkInfo 后写入下载服务内存缓存；下载执行阶段优先命中缓存，不再重复请求 `GetWorkInfoAsync`。
 - **目录命名调整**：`BuildFolderName` 改为 `[{SourceId}]{Title}`，并保持非法字符清洗规则。
 - **专项测试补齐**：新增 `RunQueuedAsync_ShouldDownloadAllFormats_WhenPreferFormatsEmpty`（留空全下载）与 `RunQueuedAsync_ShouldUsePrefetchedWorkInfo_WithoutApiWorkInfoCall`（缓存命中不再查 WorkInfo）；同步更新目录命名断言与默认配置断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Core.Tests/UnitTest1.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln -c Debug --nologo` 通过（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过（总计 45，失败 0，成功 45）。
4. DoD 判定：是。阶段 4 既有 DoD 持续满足，下载链路一致性与性能进一步提升。
5. 下次计划：阶段 5 同步能力迁移；补齐“下载限流可观测性”专项测试。

### 1.5.25 2026-03-16，阶段 4 下载页交互修复与命名优化（输入/目录/列表/Search）

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

### 1.5.26 2026-03-16，阶段 4 Search/Download 联动修复与交互收口

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

### 1.5.27 2026-03-16，阶段 4 Download 交互补强与热门接口 404 修复

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

### 1.5.28 2026-03-16，阶段 4 立即下载复用修复与高级筛选默认值补强

1. 变更摘要：
 - **立即下载复用当前行**：`IDownloadService.StartAsync` 新增 `preferredTaskId` 参数，Download 页“立即下载选中任务”在选中 `Failed/Canceled` 行时优先复用该行对应任务对象并重置状态后执行，避免创建新行。
 - **高级筛选无结果修复**：修复搜索链路中 query 的二次 URL 编码问题（`AsmrApiClient.SearchAsync` 不再重复编码），恢复高级筛选条件可用性。
 - **默认高级筛选 age:general**：在查询解析层缺省注入 `age:general`；Dashboard 页默认将 age 输入设为 `general`，清空后也恢复该默认值。
 - **测试补齐**：新增“失败任务复用重启”“取消任务复用重启”“默认 age 注入”“Search 不二次编码”回归测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 54，失败 0，成功 54）。
4. DoD 判定：是。本轮 5 项需求全部完成，且全量回归通过。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.5.29 2026-03-16，阶段 4 立即下载新任务失败后行消失修复

1. 变更摘要：
 - **根本原因**：`DownloadTaskItem.TaskId` 默认为 `Guid.NewGuid()`，永远不是 `Guid.Empty`，导致 `StartAsync` 中的条件 `if (task.TaskId == Guid.Empty)` 恒为 false，使立即下载未执行过的新任务时，新建的 `DownloadTaskItem` 从未加入 `_tasks`。下载失败后 `GetTasks()` 查不到该任务，UI 刷新后行消失。
 - **修复**：将 `if (task.TaskId == Guid.Empty)` 改为 `if (!_tasks.Contains(task))`，确保任何新建任务（非复用行）都被加入 `_tasks`，失败后状态正常保留。
 - **测试补齐**：新增 `StartAsync_ShouldTrackFailedTask_WhenNewTaskFails` 回归测试。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 55，失败 0，成功 55）。
4. DoD 判定：是。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.5.30 2026-03-16，阶段 4 Download 页面视觉优化（仅界面）

1. 变更摘要：
 - **界面美化（不改逻辑）**：仅调整 `DownloadView.xaml` 的视觉层，新增页面背景渐变、卡片容器、按钮统一样式、输入框样式、表格表头/行配色与状态信息区样式。
 - **布局优化**：将“输入区 / 筛选区 / 操作区 / 任务表格”分区为卡片式结构，优化间距、层次与可读性；保留所有控件 `x:Name` 与事件绑定不变。
 - **测试补齐**：新增 `DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls`，校验 Download 页面 XAML 关键样式资源与核心控件仍存在。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 6，失败 0，成功 6）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 56，失败 0，成功 56）。
4. DoD 判定：是。本轮仅做界面层改造，逻辑与后端行为保持不变。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.5.31 2026-03-16，阶段 4 多页面 UI 统一风格与窗口适配（仅界面）

1. 变更摘要：
 - **默认窗口尺寸**：`MainWindow` 默认与最小尺寸统一调整为 `1280x720`，提升多区块页面在首屏显示稳定性。
 - **Download 适配优化（仅 XAML）**：输入与操作区改为可换行布局，避免在新窗口尺寸下控件挤压；任务表格开启水平/垂直滚动条自动显示。
 - **Search/Settings 风格统一（仅 XAML）**：`DashboardView` 与 `SettingsView` 引入与 Download 一致的卡片化容器、渐变背景、统一按钮/输入框/表头视觉规范；保留全部 `x:Name` 与事件绑定不变。
 - **测试补齐**：新增 `ShellAndPageXamlTests`，覆盖主窗体尺寸、Search 样式资源与核心控件、Settings 卡片分区与动作按钮存在性。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/ShellAndPageXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 10，失败 0，成功 10）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 59，失败 0，成功 59）。
4. DoD 判定：是。本轮严格限制在界面层（XAML）改造，未改动控件逻辑与后端代码。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.5.32 2026-03-16，阶段文案清理与 Search 下拉框对齐修复（仅界面）

1. 变更摘要：
 - **阶段文案清理**：移除 Download/Search/Settings 页面标题区域中“阶段 X：...”文案前缀，统一为纯功能描述文本。
 - **Search 下拉框样式修复**：为 `FieldComboBoxStyle` 增加 `VerticalContentAlignment` 与 `HorizontalContentAlignment`；新增 `ComboBoxItem` 样式，统一内容居中与内边距，修复下拉框文本不对齐问题。
 - **测试补齐**：在 `ShellAndPageXamlTests` 增加“页面不含阶段前缀文案”与“Search 下拉框对齐样式存在性”测试。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/ShellAndPageXamlTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 12，失败 0，成功 12）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 61，失败 0，成功 61）。
4. DoD 判定：是。本轮仅调整 XAML 与 XAML 文本断言测试，未改动任何控件逻辑及后端代码。
5. 下次计划：进入阶段 5（同步能力迁移），优先建立"同步执行状态可视化 + 失败重试"最小闭环。

### 1.5.33 2026-03-17，阶段 4 稳定性重构推进（解析逻辑下沉 + 并发回归）

1. 变更摘要：继续按“低风险渐进”执行稳定性重构。将 `DownloadService` 中纯解析/筛选逻辑（格式优先级解析、文件筛选解析、筛选匹配）下沉到 Core 新增工具 `DownloadFilterParser`，应用层仅保留调用与下载流程编排；并保持前序并发状态锁收敛语义不变。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadFilterParser.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Core.Tests/DownloadFilterParserTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj -c Release --nologo` 通过（总计 5，失败 0，成功 5）；`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮为阶段 4 内部可维护性与并发安全收敛，不改变对外行为；核心与应用层回归均通过。
5. 下次计划：继续执行“WPF 下载页逻辑最小下沉 + 对应测试补强”，并在完成后同步更新 progress 文档。

### 1.5.34 2026-03-17，阶段 4 WPF 下载页最小逻辑下沉（列表拼装与状态映射）

1. 变更摘要：将 Download 页 code-behind 中“任务行状态文本映射 + 队列/取消占位行拼装”逻辑下沉到可测试组件：新增 `DownloadTaskRowViewModel` 与 `DownloadTaskListComposer`；`DownloadView` 改为直接调用组合器输出行数据，UI 绑定字段保持不变。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskRowViewModel.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 14，失败 0，成功 14）。
4. DoD 判定：是。本轮仅做逻辑下沉与可测试性增强，不改下载交互语义。
5. 下次计划：继续执行 Download 页事件处理分支收敛（例如批量操作中的可复用流程抽取）并补对应测试。

### 1.5.35 2026-03-17，阶段 4 WPF 事件分支收敛（任务选择策略下沉）

1. 变更摘要：将 Download 页事件处理中可纯化的“任务选择规则”下沉到 `DownloadTaskSelectionPolicy`，包含“可取消任务筛选”与“立即下载目标去重筛选（按 SourceId）”；`DownloadView` 取消与立即下载事件改为调用该策略，减少 code-behind 内联分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskSelectionPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskSelectionPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 16，失败 0，成功 16）。
4. DoD 判定：是。本轮仅做选择策略下沉，不改变原有交互语义。
5. 下次计划：继续收敛 Download 页异步操作中的重复 UI 开关/状态更新流程，并补对应单测。

### 1.5.36 2026-03-17，阶段 4 WPF 异步护栏收敛（参数归一化 + UI 开关复用）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadExecutionArgs.NormalizeFileFilter` 统一文件筛选参数归一化（空白转 `null`）；在 `DownloadView` 中引入 `ExecuteGuardedAsync` 复用异步操作的按钮禁用/恢复、异常记录与失败状态文案更新，覆盖单个/批量入队、执行队列、单项重试、批量重试、立即下载等流程，保持交互语义不变。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadExecutionArgs.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadExecutionArgsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 20，失败 0，成功 20）。
4. DoD 判定：是。本轮仅做参数与异步护栏复用，不改变下载流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化分支（如提示文案组装与确认弹窗策略）并按同样方式小步下沉。

### 1.5.37 2026-03-17，阶段 4 WPF 提示文案下沉（确认弹窗文案纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationPrompts` 统一生成“取消任务确认”与“批量重试确认”文案；`DownloadView` 改为调用提示构造器，移除事件内联拼装细节。同步将取消任务执行流程接入 `ExecuteGuardedAsync`，与既有重试/执行队列流程保持一致的 UI 开关与异常处理路径。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrompts.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPromptsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 23，失败 0，成功 23）。
4. DoD 判定：是。本轮仅做提示文案下沉与护栏一致性收敛，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如状态文案模板与确认动作策略）并按同样方式小步下沉。

### 1.5.38 2026-03-17，阶段 4 WPF 状态文案模板下沉（结果文案纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationStatusTexts`，统一生成批量入队结果、执行队列结果、取消结果、单项重试结果、批量重试结果、立即下载结果文案；`DownloadView` 对应分支改为调用模板函数，减少事件内联三元分支与字符串拼装。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationStatusTexts.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationStatusTextsTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮仅做状态文案模板下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如确认动作策略与状态提示输出协作）并按同样方式小步下沉。

### 1.5.39 2026-03-17，阶段 4 WPF 确认动作策略下沉（确认决策纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadConfirmationPolicy`，将确认弹窗返回值到“是否继续执行/取消提示文案”的决策抽离为纯函数；`DownloadView` 新增统一 `ConfirmWithQuestion` 并接入“取消任务”“批量重试”两条流程，移除重复确认分支判断。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadConfirmationPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadConfirmationPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 33，失败 0，成功 33）。
4. DoD 判定：是。本轮仅做确认决策下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如筛选与状态提示协作策略）并按同样方式小步下沉。

### 1.5.40 2026-03-17，阶段 4 WPF 操作前置校验下沉（入口校验纯策略化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationPrecheckPolicy`，将取消/单项重试/批量重试/立即下载的入口校验（选中数量、状态合法性、占位任务限制）统一下沉为纯策略返回值；`DownloadView` 事件入口改为消费策略结果，减少内联校验分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationPrecheckPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationPrecheckPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 38，失败 0，成功 38）。
4. DoD 判定：是。本轮仅做入口校验下沉，不改变下载交互语义。
5. 下次计划：继续评估 Download 页剩余可纯化点（如操作流水线上下文对象化）并按同样方式小步下沉。

### 1.5.41 2026-03-17，阶段 4 WPF 下载目录路径策略下沉（路径选择纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadDirectoryPathPolicy`，将“配置目录为空则回退默认目录、否则去首尾空白后使用配置目录”的路径选择逻辑下沉为纯函数；`DownloadView` 的“打开下载目录”流程改为调用策略结果，减少事件内联条件分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadDirectoryPathPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadDirectoryPathPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 41，失败 0，成功 41）。
4. DoD 判定：是。本轮仅做目录路径选择逻辑下沉，不改变打开目录流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如流程上下文对象化与分支聚合）并按同样方式小步下沉。

### 1.5.42 2026-03-17，阶段 4 WPF 队列缓存对齐策略下沉（缓存合并纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadQueueCachePolicy`，将“预取标题合并 + 活跃任务 sourceId 清理（标题缓存/状态覆盖缓存）”的队列缓存对齐逻辑下沉为纯函数；`DownloadView.RefreshView` 改为消费策略结果并统一替换本地缓存，减少内联循环分支。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadQueueCachePolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 43，失败 0，成功 43）。
4. DoD 判定：是。本轮仅做缓存对齐策略下沉，不改变下载列表展示与状态计算对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中的执行参数上下文聚合）并按同样方式小步下沉。

### 1.5.43 2026-03-17，阶段 4 WPF WorkInfo 标题提取策略下沉（标题过滤纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadWorkInfoTitlePolicy`，将 WorkInfo 集合到“非空标题映射”的提取与过滤规则下沉为纯函数；`DownloadView` 的单项/批量入队与 `DownloadQueueCachePolicy` 的预取标题合并统一复用该策略，避免重复标题过滤逻辑。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadWorkInfoTitlePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadWorkInfoTitlePolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 45，失败 0，成功 45）。
4. DoD 判定：是。本轮仅做标题提取规则下沉与复用，不改变下载列表与入队流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如执行流程上下文对象化）并按同样方式小步下沉。

### 1.5.44 2026-03-17，阶段 4 WPF 执行参数上下文下沉（流程参数对象化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadOperationContext`，将执行相关流程（执行队列、单项重试、批量重试、立即下载）使用的筛选参数规范化结果统一封装为上下文对象；`DownloadView` 对应事件改为消费 `context.FileFilter`，减少事件内联参数处理。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadOperationContext.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadOperationContextTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 47，失败 0，成功 47）。
4. DoD 判定：是。本轮仅做执行参数上下文化，不改变下载流程对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中重复任务计数与状态文案协作）并按同样方式小步下沉。

### 1.5.45 2026-03-17，阶段 4 WPF 任务快照筛选策略下沉（任务筛选纯函数化）

1. 变更摘要：继续按“低风险渐进”推进 Download 页 code-behind 收敛。新增 `DownloadTaskSnapshotPolicy`，将“失败任务筛选”与“活跃 sourceId 集合提取”下沉为纯函数；`DownloadView` 的“批量重试失败任务”和 `RefreshView` 改为复用策略结果，减少事件内联筛选逻辑。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskSnapshotPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskSnapshotPolicyTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 49，失败 0，成功 49）。
4. DoD 判定：是。本轮仅做任务筛选策略下沉，不改变重试与列表刷新对外行为。
5. 下次计划：继续评估 Download 页剩余可纯化点（如事件流程中的批处理执行器下沉）并按同样方式小步下沉。

### 1.5.46 2026-03-17，阶段 4 WPF 单元测试文件按目标文件整合

1. 变更摘要：对 WPF 测试工程进行按目标文件的结构整合。将 `DownloadInputNormalizationTests` 更名为 `DownloadInputNormalizerTests`，将 `DownloadViewModelTests` 更名为 `DownloadCommandAvailabilityTests`；把原 `ShellAndPageXamlTests` 拆分为 `MainWindowXamlTests`、`DashboardViewXamlTests`、`SettingsViewXamlTests`，并在 `DownloadViewXamlTests` 中补上自身的“无阶段前缀文案”断言；新增共享的 `XamlTestPathLocator` 以复用 XAML 路径定位逻辑。
2. 关键文件：`dotnet/tests/Asmroner.Wpf.Tests/DownloadInputNormalizerTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DashboardViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/XamlTestPathLocator.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 51，失败 0，成功 51）。
4. DoD 判定：是。本轮只整合测试文件结构与文档映射，不改动生产代码逻辑。
5. 下次计划：继续按目标文件维度维护测试清单与覆盖关系，避免再次出现单文件覆盖多个目标文件的情况。

### 1.5.47 2026-03-17，阶段 4 后端单元测试文件按目标文件整合

1. 变更摘要：继续按“目标文件对应测试文件”的规则整合后端测试工程。将 `Application.Tests/UnitTest1.cs` 重命名为 `FirstRunServiceTests.cs`，将 `Core.Tests/UnitTest1.cs` 重命名为 `AppConfigTests.cs`；把 `Infrastructure.Tests/UnitTest1.cs` 中的配置读写用例并入 `ConfigurationServiceTests.cs`，数据库初始化用例拆分到 `DatabaseInitializerTests.cs`；把 `IntegrationTests/UnitTest1.cs` 并入 `ApplicationBootstrapperTests.cs`，统一由同一目标文件的测试文件承载。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/FirstRunServiceTests.cs`、`dotnet/tests/Asmroner.Core.Tests/AppConfigTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/DatabaseInitializerTests.cs`、`dotnet/tests/Asmroner.IntegrationTests/ApplicationBootstrapperTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）；`dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj -c Release --nologo` 通过（总计 5，失败 0，成功 5）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）；`dotnet test dotnet/tests/Asmroner.IntegrationTests/Asmroner.IntegrationTests.csproj -c Release --nologo` 通过（总计 3，失败 0，成功 3）。
4. DoD 判定：是。本轮只整合测试文件结构与文档映射，不改动生产代码逻辑。
5. 下次计划：继续检查 Infrastructure/Application 内是否仍存在跨目标文件混放的测试文件，并按同样规则收口。

### 1.5.48 2026-03-17，阶段 5 扫描并整合残留测试文件（开始）

1. 变更摘要：按“目标文件对应测试文件”规则继续向后端与 WPF 测试工程扩展，扫描并识别命名不当或覆盖多目标的测试样例，计划逐一拆分/重命名并补齐对应测试文件，保证每个生产目标文件对应一个明确的测试文件。
2. 关键文件（扫描发现）：`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（存在综合性基础设施用例，需按目标拆分）；扫描将继续并生成待改动清单。
3. 验证结果：扫描已发现候选文件（见上）；尚未实施拆分/重命名改动，当前测试基线保持不变。
4. DoD 判定：进行中。阶段 5 最终 DoD 为：所有跨目标或命名不当的测试文件已拆分/重命名并通过对应项目回归测试。
5. 下次计划：
 - 按待改动清单逐个拆分/重命名测试文件并运行受影响测试项目；
 - 每完成一项改动即在 `1.5` 节追加新的变更记录（遵守下述变更策略）。

变更策略：自本条起，`1.5` 节仅追加新的记录，不再修改已有历史记录；如需修正前一次记录的描述或补充证据，将在后续新条目中引用对应条目编号并补充说明。

### 1.5.49 2026-03-17，阶段 5 基础设施测试文件拆分（`ApiInfrastructureTests` 收口）

1. 变更摘要：将混合覆盖多个目标文件的 `dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs` 按生产目标文件拆分并并入现有专用测试文件：`AsmrApiClient_ShouldMapHttpErrors` 并入 `AsmrApiClientTests.cs`，`AuthService_ShouldStoreTokenAfterLogin` 与 `AuthService_ShouldThrowReadableErrorOnFailure` 并入 `AuthServiceTests.cs`，`EndpointDiscoveryService_ShouldPickFastestReachableCandidate` 并入 `EndpointDiscoveryServiceTests.cs`；同时新增 `TokenStoreTests.cs` 承载 `TokenStore_ShouldRoundTripToken`，随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/TokenStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且基础设施测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否仍存在“一个测试文件覆盖多个目标文件”或“文件名与目标文件不一致”的情况，并按同样规则逐项收口。

### 1.5.50 2026-03-17，阶段 5 应用层搜索测试文件拆分（`SearchWorkflowTests` 收口）

1. 变更摘要：将混合覆盖 `QueryParserService`、`SearchService`、`SearchExportService` 三个目标的 `dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs` 按目标文件拆分：`QueryParser_ShouldParseAdvancedQuery` 并入 `QueryParserServiceTests.cs`，`SearchService_ShouldAggregateMultiplePages` 与 `SearchService_ShouldRespectRequestedPageAndKeepFilters` 并入 `SearchServiceTests.cs`，`SearchExportService_ShouldExportCsvAndJson` 并入 `SearchExportServiceTests.cs`；随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchExportServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且应用层测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否存在跨目标聚合测试文件，并按同样规则拆分收口。

### 1.5.51 2026-03-17，阶段 5 下载服务测试文件并档（`DownloadWorkflowTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs` 中所有 `DownloadService` 行为用例并入 `DownloadServiceTests.cs`，统一由单文件承载同一目标 `DownloadService.cs` 的测试；为避免重复桩逻辑，扩展 `DownloadServiceTestDoubles.cs` 的 `ScriptedApiClient`（补充并发观测、延迟与失败次数控制）并新增 `DelayRateLimiterService`，随后删除 `DownloadWorkflowTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描其他测试工程是否仍存在“同一目标由多个测试文件分散承载”或“跨目标聚合”情况，并按同样规则收口。

### 1.5.52 2026-03-17，阶段 5 下载路径测试并档（`DownloadPathTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs` 中的目录命名兼容性用例 `DirectoryNameStrategy_ShouldMatchGoCompatibilityRule` 并入 `DownloadServiceTests.cs`，统一由同一测试文件承载 `DownloadService.cs` 的路径与工作流行为；随后删除原 `DownloadPathTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描 WPF/Infrastructure/Integration 测试工程中是否仍有同目标分散或跨目标聚合测试文件，并逐项收口。

### 1.5.53 2026-03-17，阶段 5 基础设施测试依赖收敛（`AsmrApiClientTests`）

1. 变更摘要：对 `AsmrApiClientTests.cs` 进行依赖收敛，`AsmrApiClient_ShouldMapHttpErrors` 用例不再实例化真实 `AuthService`，改为使用 `StubAuthService` 直接提供 token，仅验证 `AsmrApiClient` 在服务端返回 500 时的错误映射行为，避免该测试文件对 `AuthService` 目标行为产生耦合。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮未改变测试文件映射结构，但进一步收敛了测试职责边界，且基础设施测试工程回归通过。
5. 下次计划：继续做最后一轮全仓扫描，若无新的跨目标聚合/同目标分散候选，则进入阶段 5 测试结构收口总结。

### 1.5.54 2026-03-17，阶段 5 测试结构收口复核（全仓扫描 + 解决方案回归）

1. 变更摘要：执行全仓测试文件复核扫描（Application/Core/Infrastructure/Integration/Wpf），确认当前未发现新的“跨目标聚合”或“同目标分散”高优先级候选；对少量仍出现多类型构造的文件判定为测试桩与协作者注入场景，不再进行机械拆分，避免过度拆分导致可读性下降。
2. 关键文件：本轮无代码结构性变更，主要为扫描与验证。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 105，失败 0，成功 105）。
4. DoD 判定：是。阶段 5 当前测试结构收口目标已满足，且解决方案级回归通过。
5. 下次计划：进入阶段 5 业务能力迁移实现与验证，测试结构后续仅按增量功能做最小维护。

### 1.5.55 2026-03-17，阶段 3/4 搜索与下载规则修正（默认 age 移除 + 全局规则 + 入队状态修复）

1. 变更摘要：按计划完成 Search/Download 规则收口：移除 `age:general` 默认注入；新增 `downloader.global_search_rule` 配置并在 Search 页启动时填充高级筛选；高级筛选值分解支持空格/分号/逗号，并实现“值前负号 + 反选勾选”组合语义；修复 Search 重新入队时 `Canceled` 占位状态应回到 `Pending`；移除 Download 页“示例：+voice;-cover”可见文案。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/QueryParserService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（总计 30，失败 0，成功 30）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（总计 17，失败 0，成功 17）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（总计 52，失败 0，成功 52）；`dotnet test dotnet/Asmroner.sln` 通过（总计 107，失败 0，成功 107）。
4. DoD 判定：是。本轮计划内 9 项规则改造均已落地并经解决方案级回归验证通过。
5. 下次计划：进入下一批业务功能迁移时，沿用“配置项 + 行为测试 + 文档清单同步”同批提交策略，避免规则漂移。

### 1.5.56 2026-03-17，阶段 3/4 Search/Download 缺陷修复（二次入队状态回流 + 全局规则防重 + Pending 标题保留）

1. 变更摘要：按顺序完成三项缺陷修复：
 - 修复 Search 加入下载队列时，同 SourceId 已存在且状态为 `Canceled` 的任务应复用并回流为待执行状态，避免重复创建任务行。
 - 修复 Search 页面往返切换后“全局搜索规则”重复追加到高级筛选的问题，新增筛选值防重合并策略。
 - 修复 Download 执行队列后刷新列表时 Pending 任务标题丢失的问题：刷新缓存对齐不再清理活跃任务标题缓存，并在任务行组装时增加标题兜底。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SearchFilterValuePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadQueueCachePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadTaskListComposer.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchFilterValuePolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadQueueCachePolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadTaskListComposerTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 31，失败 0，成功 31）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 55，失败 0，成功 55）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 111，失败 0，成功 111）。
4. DoD 判定：是。本轮 3 项缺陷均完成修复并通过解决方案级回归。
5. 下次计划：进入后续功能迁移时继续沿用“先修复行为、再补测试、最后同步文档与整解回归”的同批闭环流程。

### 1.5.57 2026-03-17，阶段 4 Search/Download 增强优化（全局规则启动加载 + Canceled 状态同步 + 窗体自动增高 + 状态排序与标签 Attribute 化）

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

### 1.5.58 2026-03-18，阶段 2 URL 配置化与 Discover 调用收敛

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

### 1.5.59 2026-03-18，阶段 2 启动窗口阻塞修复（异步 warmup 解耦）

1. 变更摘要：
 - **根因修复**：`App` 启动路径移除 `DiscoverAndPersistAsync().GetAwaiter().GetResult()` 同步阻塞，避免 UI 线程在窗口展示前被网络发现流程卡住。
 - **启动流程调整**：改为 `MainWindow.Show()` 后触发后台 endpoint warmup，确保窗口可立即打开。
 - **容错与超时**：新增 `StartupEndpointWarmupService`，统一封装后台 Discover 执行、超时（默认 10 秒）与失败日志，失败不阻塞应用继续启动。
 - **单测补齐**：新增 `StartupEndpointWarmupServiceTests`，覆盖非阻塞调用、后台触发、异常吞吐、超时继续四类场景。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupEndpointWarmupService.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupEndpointWarmupServiceTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 73，失败 0，成功 73）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 140，失败 0，成功 140）。
4. DoD 判定：是。启动窗口可用性修复已完成，且新增 warmup 回归样例通过。
5. 下次计划：继续沿用“先修复行为，再补测试，再同步文档”的闭环节奏推进后续任务。

### 1.5.60 2026-03-26，状态列排序修正、重复入队防护、高清音频筛选与 CSV/JSON 导入

1. 变更摘要：
 - **Bug 1（状态排序）**：`DownloadTaskRowViewModel` 新增 `StatusSortOrder` 属性，`DownloadView.xaml` 状态列绑定 `SortMemberPath="StatusSortOrder"`，修复按字母序排序问题。
 - **Bug 2（重复入队防护）**：新增 `DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent()`，在 `DownloadView.OnAddSingleClicked`、`OnAddBatchClicked`、`DashboardView.OnQueueClicked` 入队前过滤已存在任务（任意状态）。
 - **Feature 3（高清音频筛选）**：`DownloadFilterParser.FilterHdAudioOnly<T>()` 新增泛型方法；`IDownloadService`/`DownloadService` 三个公开方法新增 `hdAudioOnly = false` 参数；`DownloadOperationContext` 新增 `HdAudioOnly` 字段；`DownloadView.xaml` 新增复选框；调用链完整透传。
 - **Feature 4（CSV/JSON 导入）**：新增 `ISearchImportService`（Core）与 `SearchImportService`（Application）；DI 注册；`DownloadView.xaml` 新增两个导入按钮；Handler 含 OpenFileDialog、去重过滤、WorkInfo 预取与 RefreshView。
2. 关键文件：`DownloadTaskRowViewModel.cs`、`DownloadEnqueueDuplicatePolicy.cs`（新建）、`DownloadOperationContext.cs`、`DownloadView.xaml`、`DownloadView.xaml.cs`、`DashboardView.xaml.cs`、`Asmroner.Wpf/Services/DownloadService.cs`、`App.xaml.cs`、`DownloadFilterParser.cs`、`IDownloadService.cs`、`ISearchImportService.cs`（新建）、`Application/DownloadService.cs`、`SearchImportService.cs`（新建）。
3. 验证结果：`dotnet build dotnet/Asmroner.sln` 成功（0 错误，0 警告）；`dotnet test dotnet/Asmroner.sln` 通过，总计 173，失败 0，成功 173（+33 新测试）。
4. DoD 判定：是。4 项需求均已实现、单测覆盖、文档同步，回归测试全通过。
5. 下次计划：继续沿用"代码改动 + 单测补齐 + progress 同步"同批提交策略推进后续任务。

### 1.5.61 2026-03-27，v0.4.1 版本对齐、Search 行为修正与高清音频默认增强

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

## 1.6 维护规则

- 每次代码提交后更新第 16.2 节状态表。
- 每个阶段完成后更新第 16.3 节勾选项。
- 出现阻塞时更新第 16.4 节。
- 第 16 节内容应与第 12 节阶段执行状态保持一致。

---

## 2. 单元测试清单

本节用于沉淀“当前已落地测试样例 + 下一步新增测试计划”，并与第 12 节阶段目标保持一致。

### 2.1 测试样例列表

说明：

- `已创建`：测试样例已存在于仓库。
- `已通过`：样例在最近一次可执行验证中通过；当前全量回归基线为 2026-03-27 的 `dotnet test dotnet/Asmroner.sln`（178/178）。

#### 2.1.1 Application.Tests / DownloadServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                 | 输入                                                | 期望输出                                    |
| ------ | ------ | ------- | ---------------------------------------------------------------------- | --------------------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4  | `DirectoryNameStrategy_ShouldMatchGoCompatibilityRule`                 | 含非法字符标题 + SourceId                           | 目录名格式为 `[{SourceId}]{Title}`          |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldCreateCompletedTaskAndFiles`                     | 入队 1 个 RJID 并执行                               | 任务 `Completed`，文件落地成功，队列清空    |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldMarkTaskFailed_WhenApiThrows`                    | WorkInfo API 抛异常                                 | 任务状态 `Failed` 且含错误信息              |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldCancelRunningTask`                                  | 长任务执行中调用 `CancelAsync`                      | 目标任务最终状态为 `Canceled`               |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldCancelQueuedTask_BeforeWorkerStarts`                | 单 worker 场景下取消排队任务                        | 排队任务状态更新为 `Canceled`               |
| [x]    | [x]    | 阶段 4  | `RetryFailedAsync_ShouldRetryAndCompleteTask`                          | 首次失败后重试                                      | 重试计数 +1，任务转为 `Completed`           |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldRespectConfiguredMaxWorkers`                     | 入队 4 条、`MaxWorkers=2`                           | 最大观测并发不超过且达到 2                  |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldTrackFailedTask_WhenNewTaskFails`                    | 新任务 `StartAsync` 失败                            | 失败任务仍可在 `GetTasks()` 中追踪          |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldReuseFailedTask_WhenPreferredTaskProvided`           | 选中失败任务立即下载（带 `preferredTaskId`）        | 复用原 `TaskId` 行并重启，不新增任务行      |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldReuseCanceledTask_WhenPreferredTaskProvided`         | 选中已取消任务立即下载（带 `preferredTaskId`）      | 复用原 `TaskId` 行并重启，不新增任务行      |
| [x]    | [x]    | 阶段 4  | `UpsertPrefetchedWorkInfo_ShouldExposeSnapshot_ForCrossViewTitleReuse` | Search 侧写入预取 WorkInfo 后读取快照               | 快照可读且包含对应标题映射                  |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldDownloadAllFormats_WhenPreferFormatsEmpty`       | `PreferFormats` 置空且存在多种扩展名轨道            | 不限扩展名，全部下载                        |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldUsePrefetchedWorkInfo_WithoutApiWorkInfoCall`    | 预先写入内存 WorkInfo 且 API 禁止 WorkInfo 调用     | 下载成功且不触发 WorkInfo API               |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldReuseCanceledTask_WhenSameSourceRequeued`        | 已取消任务再次由 Search 入队后执行队列              | 复用原任务并回流为待执行/完成，不新增重复行 |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldReturnFalse_WhenTaskDoesNotExist`                   | 随机 `TaskId` 调取消                                | 返回 `false`                                |
| [x]    | [x]    | 阶段 4  | `RetryFailedAsync_ShouldNotRetry_WhenTaskIsNotFailed`                  | 任务状态为 `Completed/Canceled` 调重试              | 返回空或拒绝重试                            |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldContinueOtherTasks_WhenSingleTaskFails`          | 批量队列中单任务失败                                | 其他任务继续完成                            |
| [x]    | [x]    | 阶段 5+ | `RunQueuedAsync_ShouldSkipTextSidecars_WhenHdAudioOnlyRemovesMp3`      | 同路径同名 `mp3+wav+txt/lrc/ass`                    | 保留 `wav`，移除对应 `mp3/txt/lrc/ass`      |
| [x]    | [x]    | 阶段 5+ | `RunQueuedAsync_ShouldKeepTextSidecars_WhenHdAudioOnlyIsFalse`         | 同路径同名 `mp3+wav+txt/lrc/ass`，hdAudioOnly=false | 全部文件保留并下载                          |

#### 2.1.2 Application.Tests / QueryParserServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                    | 输入                                    | 期望输出                              |
| ------ | ------ | ------ | --------------------------------------------------------- | --------------------------------------- | ------------------------------------- |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldParseAdvancedQuery`                    | 复杂高级检索语句（含负向条件+分页参数） | 解析字段正确，重建 query 保留关键参数 |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldReturnReadableError_WhenSyntaxInvalid` | 非法高级语法字符串                      | 返回可读解析错误                      |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldNotApplyDefaultAge_WhenAgeMissing`     | 查询未显式提供 age 条件                 | 不自动注入 `age` 默认值               |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldParseSemicolonSeparatedFilters`        | 高级筛选使用分号分隔                    | 分号语法可正确映射到筛选字段          |

#### 2.1.3 Application.Tests / SearchExportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                   | 输入                       | 期望输出                             |
| ------ | ------ | ------ | ------------------------------------------------------------------------ | -------------------------- | ------------------------------------ |
| [x]    | [x]    | 阶段 3 | `SearchExportService_ShouldExportCsvAndJson`                             | 1 条搜索结果导出到临时目录 | 生成 CSV/JSON 文件，内容包含关键字段 |
| [x]    | [x]    | 阶段 3 | `SearchExportService_ShouldEscapeCsvFields_WhenTextContainsCommaOrQuote` | 标题含逗号、引号、换行     | CSV 格式正确转义                     |

#### 2.1.4 Application.Tests / SearchServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                        | 期望输出                                    |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldAggregateMultiplePages`                | `pageSize=2`、目标返回 5 条 | 聚合总数=6、返回=5、调用页数=3、ID 顺序正确 |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldRespectRequestedPageAndKeepFilters`    | 指定 `page=2` 且含筛选参数  | 只请求第 2 页，过滤参数与分页参数保持不变   |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldReturnEmpty_WhenApiReturnsNoWorkItems` | API 返回空列表              | `ReturnedCount=0` 且不抛异常                |

#### 2.1.5 Application.Tests / SearchStateStoreTests.cs

- [x]/[x] 阶段 4 `EnqueueForDownload_ShouldNormalizeSourceIdExtractedFromUrl`：输入 RJID、作品链接、API 路径、`RJ-xxxx` 与纯数字混合值入队，期望队列统一归一化为 `RJxxxx` 且去重正确。
- [x]/[x] 阶段 4 `RemoveFromQueue_ShouldMatchNormalizedInput`：队列已有 `RJxxxx` 时使用 URL 形式出队，期望可匹配并成功移除。

#### 2.1.6 Application.Tests / FirstRunServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                   | 输入                                         | 期望输出                   |
| ------ | ------ | ------ | -------------------------------------------------------- | -------------------------------------------- | -------------------------- |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldNotRequireSetup_WhenConfigValid`  | `config=AppConfig`，校验错误为空             | 返回 `RequiresSetup=false` |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldRequireSetup_WhenConfigMissing`   | `config=null`，校验错误为空                  | 返回 `RequiresSetup=true`  |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldRequireSetup_WhenValidationFails` | `config=AppConfig`，校验错误含"账号不能为空" | 返回 `RequiresSetup=true`  |

#### 2.1.7 Core.Tests / AppConfigTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                   | 输入                 | 期望输出                                                                                                                              |
| ------ | ------ | ------ | ---------------------------------------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `AppConfig_ShouldHaveReasonableDefaults` | 新建默认 `AppConfig` | `MaxWorkers=4`、`MaxRetries=3`、`SyncWantedSize=5GB`、`PreferFormats=mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass` |

#### 2.1.8 Infrastructure.Tests / TokenStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                            | 输入                           | 期望输出                  |
| ------ | ------ | ------ | --------------------------------- | ------------------------------ | ------------------------- |
| [x]    | [x]    | 阶段 2 | `TokenStore_ShouldRoundTripToken` | 写入 `ApiToken(abc123)` 后读取 | 读取 token 不为空且值一致 |

#### 2.1.9 Infrastructure.Tests / AsmrApiClientTests.cs

- [x]/[x] 阶段 2 `AsmrApiClient_ShouldMapHttpErrors`：业务请求返回 500，期望抛出 `AsmrApiException`，错误码为 `api_request_failed`。
- [x]/[x] 阶段 4 `GetPopularAsync_ShouldUsePostAndMapWorks`：调用热门接口，期望使用 `POST /api/recommender/popular` 且正确映射返回 `works`。
- [x]/[x] 阶段 2 `AsmrApiClient_ShouldAttachBearerToken_OnAuthorizedCalls`：已登录态调用受保护接口，期望请求头包含 `Authorization: Bearer xxx`。
- [x]/[x] 阶段 4 `AsmrApiClient_ShouldNormalizeWorkUrlInput_ToWorkEndpointPath`：输入作品 URL 形式 id 调用 `GetWorkInfoAsync`，期望请求路径归一化为 `/api/work/{numericId}`（纯数字，无 `RJ` 前缀）并成功调用。
- [x]/[x] 阶段 4 `AsmrApiClient_ShouldNormalizeNonCanonicalSourceId_ToNumericApiPath`：输入 `RJ-xxxx`、纯数字、`RJxxxx` 形式 id 调用 `GetWorkInfoAsync`，期望请求路径归一化为 `/api/work/{numericId}`（纯数字，无 `RJ` 前缀）。
- [x]/[x] 阶段 4 `AsmrApiClient_SearchAsync_ShouldNotDoubleEncodeQuery`：输入已编码 query（含高级筛选 token）调用 `SearchAsync`，期望请求 URL 不出现 `%25` 二次编码序列。

#### 2.1.10 Infrastructure.Tests / AuthServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                 | 期望输出                                            |
| ------ | ------ | ------ | ---------------------------------------------------------- | -------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldRejectLogin_WhenAccountOrPasswordEmpty` | 空账号或空密码       | 抛出明确业务异常                                    |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldStoreTokenAfterLogin`                   | 登录返回 `jwt-token` | 返回 token 且 `TokenStore` 已持久化该 token         |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldThrowReadableErrorOnFailure`            | 登录接口返回 401     | 抛出 `AsmrApiException`，错误码 `auth_login_failed` |

#### 2.1.11 Infrastructure.Tests / ConfigurationServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                        | 输入                                   | 期望输出                                                    |
| ------ | ------ | ------ | ----------------------------------------------------------------------------- | -------------------------------------- | ----------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `ConfigurationService_ShouldReturnValidationErrors_WhenRequiredFieldsMissing` | 缺失账号/密码/同步目录的配置           | 返回可读校验错误集合                                        |
| [x]    | [x]    | 阶段 1 | `ConfigurationService_ShouldSaveAndLoadConfig`                                | 临时目录、包含账号与下载参数的配置对象 | 配置可保存并再次读取，关键字段（含 `GlobalSearchRule`）一致 |

#### 2.1.12 Infrastructure.Tests / EndpointDiscoveryServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                             | 输入                                  | 期望输出                         |
| ------ | ------ | ------ | ---------------------------------------------------------------------------------- | ------------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 2 | `EndpointDiscoveryService_ShouldPickFastestReachableCandidate`                     | 候选域名包含 slow/fast，fast 返回 200 | 选中 `https://fast.example.com`  |
| [x]    | [x]    | 阶段 2 | `EndpointDiscoveryService_ShouldFallbackToConfiguredBaseUrl_WhenAllCandidatesFail` | 所有候选地址不可达                    | 回退到配置基础地址或返回明确失败 |

#### 2.1.13 Infrastructure.Tests / DatabaseInitializerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                   | 输入                   | 期望输出                   |
| ------ | ------ | ------ | -------------------------------------------------------- | ---------------------- | -------------------------- |
| [x]    | [x]    | 阶段 1 | `DatabaseInitializer_ShouldCreateDatabaseFileAndConnect` | 临时目录下初始化数据库 | 数据库文件创建成功且可连接 |

#### 2.1.14 IntegrationTests / ApplicationBootstrapperTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                               | 期望输出                                                  |
| ------ | ------ | ------ | ---------------------------------------------------------- | ---------------------------------- | --------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldFailGracefully_WhenDatabaseInitThrows` | 模拟数据库初始化异常               | `IsSuccess=false` 且错误信息可读                          |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldRequireSetup_WhenConfigMissing`        | 缺失配置场景执行 `InitializeAsync` | `RequiresSetup=true`、`IsSuccess=false`，数据库文件已创建 |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldSucceed_WhenConfigValid`               | 有效配置场景执行 `InitializeAsync` | `IsSuccess=true`、`RequiresSetup=false`，同步目录存在     |

#### 2.1.15 Wpf.Tests / DownloadInputNormalizerTests.cs

- [x]/[x] 阶段 4 `NormalizeSingleInputDisplay_ShouldExtractRjIdFromWorkUrl`：单个输入框粘贴作品 URL，期望 UI 实时显示归一化 `RJxxxx`。
- [x]/[x] 阶段 4 `NormalizeBatchInputDisplay_ShouldNormalizeAndDeduplicateSourceIds`：批量输入框混合 RJID/URL/`RJ-xxxx`/纯数字/重复值，期望 UI 实时显示为归一化且去重后的 RJID 列表。
- [x]/[x] 阶段 4 `NormalizeBatchInputForSubmit_ShouldSupportCommaSemicolonSpaceAndNewline`：批量输入含逗号/分号/空格/换行混排，点击提交时应统一归一化并去重。

#### 2.1.16 Wpf.Tests / DownloadCommandAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                             | 期望输出                                          |
| ------ | ------ | ------ | --------------------------------------------------------------------- | -------------------------------- | ------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldToggleCommandAvailability_ByTaskState`                | 不同任务状态输入到命令可用性规则 | 取消/重试/批量重试/立即下载按钮状态与任务状态一致 |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancel_WhenQueuedTaskSelected`                   | 选中 `Queued` 状态任务           | “取消选中任务”按钮可用                            |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancelAndImmediateStart_WhenPendingTaskSelected` | 选中 `Pending` 状态任务          | “取消选中任务”与“立即下载选中任务”按钮可用        |

#### 2.1.17 Wpf.Tests / DownloadViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                   | 输入                                | 期望输出                                       |
| ------ | ------ | ------ | ------------------------------------------------------------------------ | ----------------------------------- | ---------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls` | 解析 `DownloadView.xaml` 的文本/XML | 关键样式资源与核心控件存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldNotContainStagePrefixText`                       | 解析 `DownloadView.xaml` 文本       | 页面不再包含“阶段 ”前缀文案。                  |

#### 2.1.18 Wpf.Tests / MainWindowXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                              | 输入                            | 期望输出                                            |
| ------ | ------ | ------ | --------------------------------------------------- | ------------------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `MainWindowXaml_ShouldUse1280x720DefaultWindowSize` | 解析 `MainWindow.xaml` 文本/XML | 默认与最小窗口尺寸为 `1280x720`，且 XAML 可被解析。 |

#### 2.1.19 Wpf.Tests / DashboardViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                               | 期望输出                                           |
| ------ | ------ | ------ | ------------------------------------------------------------------ | ---------------------------------- | -------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `DashboardViewXaml_ShouldContainUnifiedCardStyles_AndCoreControls` | 解析 `DashboardView.xaml` 文本/XML | 卡片化样式资源与核心控件存在，且 XAML 可被解析。   |
| [x]    | [x]    | 阶段 4 | `DashboardViewXaml_ShouldUseAlignedComboBoxStyles`                 | 解析 `DashboardView.xaml` 文本/XML | 下拉框与选项项样式包含对齐设置，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `DashboardViewXaml_ShouldNotContainStagePrefixText`                | 解析 `DashboardView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案。                      |

#### 2.1.20 Wpf.Tests / SettingsViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                              | 期望输出                                       |
| ------ | ------ | ------ | ------------------------------------------------------------- | --------------------------------- | ---------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldContainCardSections_AndActionButtons` | 解析 `SettingsView.xaml` 文本/XML | 卡片分区与核心动作按钮存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldNotContainStagePrefixText`            | 解析 `SettingsView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案。                  |

#### 2.1.21 Core.Tests / DownloadFilterParserTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                           | 输入                                        | 期望输出                                               |
| ------ | ------ | ------- | ---------------------------------------------------------------- | ------------------------------------------- | ------------------------------------------------------ |
| [x]    | [x]    | 阶段 4  | `ParsePreferExtensions_ShouldUseUnifiedPreferFormatsFirst`       | 同时提供 `PreferFormats` 与旧字段           | 优先采用 `PreferFormats`，并规范化为带点小写扩展名集合 |
| [x]    | [x]    | 阶段 4  | `ParsePreferExtensions_ShouldFallbackToLegacyFields`             | `PreferFormats` 为空，旧字段有值            | 回退合并旧字段并生成规范化扩展名集合                   |
| [x]    | [x]    | 阶段 4  | `ParseFileFilter_ShouldParseIncludeAndExcludeTerms`              | `+voice;-demo;chapter`                      | 正确解析必含/排除项，忽略空项                          |
| [x]    | [x]    | 阶段 4  | `MatchesFileFilter_ShouldApplyAllTerms`                          | 包含必含与排除的筛选规则 + 多种文件路径输入 | 仅匹配同时满足全部必含且不命中排除条件的路径           |
| [x]    | [x]    | 阶段 5+ | `FilterHdAudioOnly_ShouldRemoveMp3_WhenFlacExists`               | mp3 + flac 混合列表，hdAudioOnly=true       | 保留 flac，移除 mp3                                    |
| [x]    | [x]    | 阶段 5+ | `FilterHdAudioOnly_ShouldRemoveMp3_WhenWavExists`                | mp3 + wav 混合列表，hdAudioOnly=true        | 保留 wav，移除 mp3                                     |
| [x]    | [x]    | 阶段 5+ | `FilterHdAudioOnly_ShouldKeepMp3_WhenNoHdAudioExists`            | mp3 + jpg 列表，无 wav/flac                 | 原样返回，不过滤 mp3                                   |
| [x]    | [x]    | 阶段 5+ | `FilterHdAudioOnly_ShouldReturnUnchanged_WhenHdAudioOnlyIsFalse` | mp3 + flac，hdAudioOnly=false               | 原样返回所有条目                                       |

#### 2.1.22 Wpf.Tests / DownloadTaskListComposerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                       | 输入                                                                      | 期望输出                                                                  |
| ------ | ------ | ------ | ---------------------------------------------------------------------------- | ------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldIncludeQueuedPendingAndCanceledRows_BeforeActiveRows`     | 活跃任务 + 排队任务 + 已取消占位覆盖 + 标题映射                           | 输出按状态优先级排序（Running→Pending→Canceled），且状态/错误文案映射正确 |
| [x]    | [x]    | 阶段 4 | `From_ShouldMapCompletedStatusAndProgressText`                               | 已完成任务（含进度、目录字段）                                            | 行视图模型正确映射 `StatusText=已完成` 与 `ProgressText=100% (n/n)`       |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldResetCanceledOverrideToPending_WhenSourceRequeued`        | 已取消占位任务再次被加入下载队列                                          | 已重新入队任务状态显示为 `Pending/未下载`                                 |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldFillActiveTitleFromQueuedTitleCache_WhenTaskTitleIsEmpty` | 活跃任务标题为空且存在预取标题缓存                                        | 活跃任务行标题可从缓存回填，刷新后不丢失                                  |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldOrderByStatusAscending_ThenBySourceIdAscending`           | 混合6个任务，含Completed、Running、Queued、Pending、Failed、Canceled各1个 | 输出按状态排序优先级递增，同级按SourceId递增，验证Attribute驱动的排序正确 |

#### 2.1.23 Wpf.Tests / DownloadTaskSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                               | 期望输出                                                              |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------------------------ | --------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetCancelable_ShouldReturnOnlyPendingQueuedRunning`                  | 含 Pending/Queued/Running/Failed/Completed/Canceled 的混合选中任务 | 仅返回 Pending、Queued、Running 三类可取消任务                        |
| [x]    | [x]    | 阶段 4 | `GetImmediateStartTargets_ShouldFilterStatusAndDeduplicateBySourceId` | 含可立即下载与不可立即下载状态，且 SourceId 重复                   | 仅返回 Pending/Failed/Canceled 且按 SourceId 去重后的立即下载目标集合 |

#### 2.1.24 Wpf.Tests / DownloadExecutionArgsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                      | 期望输出                                    |
| ------ | ------ | ------ | --------------------------------------------------------------- | ----------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `NormalizeFileFilter_ShouldReturnNull_ForNullOrWhitespace`      | `null`、空字符串、纯空白字符串            | 统一归一化为 `null`，避免调用层重复空值判断 |
| [x]    | [x]    | 阶段 4 | `NormalizeFileFilter_ShouldTrimAndReturnValue_ForNonEmptyInput` | 含前后空白的筛选文本（如 `+voice;-demo`） | 返回去首尾空白后的原始筛选文本              |

#### 2.1.25 Wpf.Tests / DownloadOperationPromptsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                  | 输入                                              | 期望输出                                    |
| ------ | ------ | ------ | --------------------------------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildCancelConfirmMessage_ShouldContainCancelableCount`                                | 可取消任务数 `3`                                  | 返回“将取消 3 个任务，是否继续？”           |
| [x]    | [x]    | 阶段 4 | `BuildRetryAllConfirmMessage_ShouldContainPreviewAndEllipsis_WhenExceedingPreviewLimit` | 6 条失败任务 SourceId，默认预览上限 5，最大并发 2 | 返回含前 5 项预览和省略号的批量重试确认文案 |
| [x]    | [x]    | 阶段 4 | `BuildRetryAllConfirmMessage_ShouldNotUseEllipsis_WhenWithinPreviewLimit`               | 2 条失败任务 SourceId，最大并发 4                 | 返回不含省略号的批量重试确认文案            |

#### 2.1.26 Wpf.Tests / DownloadOperationStatusTextsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                               | 期望输出                                               |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ---------------------------------- | ------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `BuildBatchEnqueueResult_ShouldIncludeResolvedSuffix_WhenPartialResolved` | 总任务数 6，已解析 4               | 返回“已加入批量下载：6 个任务（4 个已更新作品信息）。” |
| [x]    | [x]    | 阶段 4 | `BuildRunQueueResult_ShouldShowEmptyMessage_WhenNoTaskCreated`            | 创建任务数 0                       | 返回“下载队列为空，无需执行。”                         |
| [x]    | [x]    | 阶段 4 | `BuildCancelResult_ShouldShowSummary_WhenAnyTaskCanceled`                 | 已取消 2，总计 3                   | 返回“已取消 2/3 个任务。”                              |
| [x]    | [x]    | 阶段 4 | `BuildRetryResult_ShouldReturnUnsupportedMessage_WhenNotRetried`          | `retried=false`，`sourceId=RJ3001` | 返回“仅失败状态任务支持重试。”                         |
| [x]    | [x]    | 阶段 4 | `BuildRetryAllResult_ShouldContainRetriedSummary`                         | 成功触发 3，总计 5                 | 返回“批量重试完成：成功触发 3/5。”                     |
| [x]    | [x]    | 阶段 4 | `BuildStartSelectedResult_ShouldReturnNoStartMessage_WhenNoneStarted`     | 启动数 0，总选中 2                 | 返回“没有可立即下载的任务。”                           |

#### 2.1.27 Wpf.Tests / DownloadConfirmationPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                              | 输入                              | 期望输出                                                     |
| ------ | ------ | ------ | --------------------------------------------------- | --------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldContinue_WhenResultIsYes`           | `MessageBoxResult.Yes`            | 返回 `ShouldContinue=true` 且 `StatusText=null`              |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldCancelOperation_WhenResultIsNotYes` | `MessageBoxResult.No/Cancel/None` | 返回 `ShouldContinue=false` 且 `StatusText=已取消本次操作。` |

#### 2.1.28 Wpf.Tests / DownloadOperationPrecheckPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                | 输入                                                | 期望输出                                                       |
| ------ | ------ | ------ | ----------------------------------------------------- | --------------------------------------------------- | -------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `CheckCancel_ShouldReturnPrompt_WhenSelectionEmpty`   | 空选中集合                                          | 返回不可继续且提示“请先选择要取消的任务。”                     |
| [x]    | [x]    | 阶段 4 | `CheckRetrySingle_ShouldRejectMultipleSelection`      | 选中 2 个失败任务                                   | 返回不可继续且提示“重试仅支持单个失败任务，请只选择一条记录。” |
| [x]    | [x]    | 阶段 4 | `CheckRetrySingle_ShouldRejectPlaceholderTask`        | 选中 1 个占位任务（`TaskId=Guid.Empty`）            | 返回不可继续且提示“该任务尚未开始执行，无需重试。”             |
| [x]    | [x]    | 阶段 4 | `CheckRetryAllFailed_ShouldRejectWhenEmpty`           | 失败任务集合为空                                    | 返回不可继续且提示“当前没有失败任务可重试。”                   |
| [x]    | [x]    | 阶段 4 | `CheckStartImmediate_ShouldReturnDeduplicatedTargets` | 含 Pending/Failed/Canceled/Running 且 SourceId 重复 | 返回可继续，目标集合按立即下载规则筛选且按 SourceId 去重       |

#### 2.1.29 Wpf.Tests / DownloadDirectoryPathPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                                                    | 期望输出                                         |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | ------------------------------------------------------- | ------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsNull`             | `configuredPath=null`，默认目录 `C:/downloads/default`  | 返回默认目录路径                                 |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnTrimmedConfiguredPath_WhenConfiguredPathProvided` | `configuredPath="  C:/downloads/custom  "`              | 返回去首尾空白后的配置目录 `C:/downloads/custom` |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsWhitespace`       | `configuredPath="   "`，默认目录 `C:/downloads/default` | 返回默认目录路径                                 |

#### 2.1.30 Wpf.Tests / DownloadQueueCachePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                                                            | 期望输出                                                            |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------------------------------------------------- | ------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Reconcile_ShouldMergePrefetchedTitles_AndKeepActiveTitles` | 活跃 sourceId 集合 + 预取标题 + 现有标题缓存 + 现有状态覆盖缓存 | 合并非空预取标题，保留活跃项标题缓存，仅清理活跃项状态覆盖          |
| [x]    | [x]    | 阶段 4 | `Reconcile_ShouldKeepCaseInsensitiveLookup`                 | 小写 sourceId 预取标题                                          | 输出标题字典支持不区分大小写查询（`RJxxxx` 可读取 `rjxxxx` 写入值） |

#### 2.1.31 Wpf.Tests / DownloadWorkInfoTitlePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                              | 期望输出                                    |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildNonEmptyTitleMap_ShouldFilterWhitespaceTitles`                  | 含正常标题、空字符串、纯空白标题的 WorkInfo 集合  | 仅保留非空白标题映射                        |
| [x]    | [x]    | 阶段 4 | `BuildNonEmptyTitleMap_ShouldUseCaseInsensitiveKeys_AndLastWriteWins` | 同一 sourceId 的大小写变体键（`rjxxxx`/`RJxxxx`） | 输出字典大小写不敏感，重复 key 采用后写入值 |

#### 2.1.32 Wpf.Tests / DownloadOperationContextTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                          | 输入                               | 期望输出                       |
| ------ | ------ | ------- | ----------------------------------------------- | ---------------------------------- | ------------------------------ |
| [x]    | [x]    | 阶段 4  | `Create_ShouldNormalizeWhitespaceFilter_ToNull` | `rawFileFilter="   "`              | `FileFilter=null`              |
| [x]    | [x]    | 阶段 4  | `Create_ShouldTrimFilter_WhenValueProvided`     | `rawFileFilter="  +voice;-demo  "` | `FileFilter="+voice;-demo"`    |
| [x]    | [x]    | 阶段 5+ | `Create_ShouldCarryHdAudioOnly_WhenFlagIsTrue`  | `hdAudioOnly=true`                 | `context.HdAudioOnly == true`  |
| [x]    | [x]    | 阶段 5+ | `Create_ShouldDefaultHdAudioOnly_ToFalse`       | 不传第二参数                       | `context.HdAudioOnly == false` |

#### 2.1.33 Wpf.Tests / DownloadTaskSnapshotPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                     | 期望输出                               |
| ------ | ------ | ------ | --------------------------------------------------------------- | ---------------------------------------- | -------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetFailedTasks_ShouldReturnOnlyFailedItems`                    | 含 Pending/Failed/Completed 混合任务快照 | 仅返回 `Status=Failed` 任务集合        |
| [x]    | [x]    | 阶段 4 | `GetActiveSourceIds_ShouldReturnCaseInsensitiveDeduplicatedSet` | 含 sourceId 大小写变体与重复任务快照     | 返回去重且不区分大小写的 sourceId 集合 |

#### 2.1.34 Wpf.Tests / SearchFilterValuePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                          | 输入                            | 期望输出                         |
| ------ | ------ | ------ | ----------------------------------------------- | ------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 3 | `MergeDistinct_ShouldAppendValue_WhenMissing`   | 已有值 `tag1`，新增值 `tag2`    | 合并结果为 `tag1,tag2`           |
| [x]    | [x]    | 阶段 3 | `MergeDistinct_ShouldNotDuplicateExistingValue` | 已有值 `tag1;tag2`，新增 `tag2` | 命中重复值时不追加，保持原值不变 |

#### 2.1.35 Wpf.Tests / DownloadTaskStatusExtensionsTests.cs （新建）

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                    | 期望输出                                                               |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------------- | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetDisplayName_ReturnsCorrectChineseLabel`（Theory，6 inline cases） | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应中文标签：未下载/待下载/下载中/已完成/已失败/已取消            |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_ReturnsCorrectSortOrder`（Theory，6 inline cases）      | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应排序优先级：3/2/1/0/4/5                                        |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_OrdersStatusesCorrectly`                                | 无序的 6 个枚举值输入                                   | 按排序优先级递增排列：Completed→Running→Queued→Pending→Failed→Canceled |

#### 2.1.36 Infrastructure.Tests / AsmrApiOptionsProviderTests.cs（新建）

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                                         | 期望输出                                              |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ------------------------------------------------------------ | ----------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldParseConfiguredUrlLists`                      | 自定义 `api_url` + 候选/发布源 URL 列表（含分号/逗号与重复） | 正确解析并去重，`BaseUrl` 与候选/发布源列表按预期生成 |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldFallbackToDefaults_WhenConfiguredUrlsMissing` | 空 `api_url` 与空列表字段                                    | 回退默认 API 地址、默认候选地址与默认发布源地址       |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldIgnoreInvalidUrls_InConfiguredLists`          | 列表中混入非法 URL                                           | 仅保留合法 URL 项，非法项被忽略                       |

#### 2.1.37 Infrastructure.Tests / ApiEndpointUrlServiceTests.cs（新建）

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                               | 期望输出                          |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ---------------------------------- | --------------------------------- |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldUpdateApiUrl_WhenConfigExists`         | 配置存在，发现结果返回新 BaseUrl   | `downloader.api_url` 被更新并保存 |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldSkipSave_WhenConfigMissing`            | 配置不存在，发现结果返回新 BaseUrl | 不写入配置文件，返回发现结果      |
| [x]    | [x]    | 阶段 2 | `GetCurrentBaseUrlAsync_ShouldReturnDefault_WhenConfigMissingOrEmpty` | 配置缺失或 `api_url` 为空白        | 返回默认 API 基础地址             |

#### 2.1.38 Infrastructure.Tests / ConnectivityProbeServiceTests.cs（新建）

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                       | 期望输出                              |
| ------ | ------ | ------ | ------------------------------------------------------------------ | -------------------------- | ------------------------------------- |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldDiscoverAndAuthenticate_WhenDependenciesSucceed` | Discover 成功 + 登录成功   | 返回可达且鉴权成功，包含 BaseUrl/延迟 |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldReturnFailureResult_WhenAuthenticationThrows`    | Discover 成功 + 登录抛异常 | 返回不可达结果并携带失败消息          |

#### 2.1.39 Infrastructure.Tests / Existing suite updates（本轮补充）

- [x]/[x] 阶段 2 `AsmrApiClient_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AsmrApiClientTests.cs`）：验证客户端请求链路仅使用当前 BaseUrl 服务，不触发 Discover。
- [x]/[x] 阶段 2 `AuthService_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AuthServiceTests.cs`）：验证登录链路仅使用当前 BaseUrl 服务，不触发 Discover。
- [x]/[x] 阶段 2 `EndpointDiscoveryService_ShouldUseConfiguredPublishSources_ForDynamicCandidates`（`EndpointDiscoveryServiceTests.cs`）：验证发布源地址来自配置，且可动态发现候选 API。

#### 2.1.40 Wpf.Tests / StartupEndpointWarmupServiceTests.cs（新建）

| 已创建 | 已通过 | 阶段   | 样例名                                                               | 输入                  | 期望输出                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------- | --------------------- | -------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldReturnImmediately_WhenDiscoveryIsSlow` | Discover 慢响应       | 启动 warmup 调用快速返回，不阻塞窗口启动路径 |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldInvokeDiscoverAndPersistAsync`         | 正常 discover 依赖    | 后台流程会触发一次 DiscoverAndPersist 调用   |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldNotThrow_WhenDiscoveryFails`           | Discover 抛异常       | 异常被吞吐并记录，不向上抛出                 |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldRespectTimeout_AndContinue`            | Discover 超时（50ms） | 超时后流程结束并继续，不阻塞应用             |

---


#### 2.1.41 Wpf.Tests / DownloadTaskRowViewModelTests.cs（新建）

| 已创建 | 已通过 | 阶段    | 样例名                                                                                   | 输入                       | 期望输出                                      |
| ------ | ------ | ------- | ---------------------------------------------------------------------------------------- | -------------------------- | --------------------------------------------- |
| [x]    | [x]    | 阶段 5+ | `From_ShouldSetStatusSortOrder_MatchingGetSortOrder`（全 6 状态 Theory）                 | 各 DownloadTaskStatus 枚举 | `vm.StatusSortOrder == status.GetSortOrder()` |
| [x]    | [x]    | 阶段 5+ | `CreatePending_ShouldSetStatusSortOrder_MatchingGetSortOrder`（Queued/Pending/Canceled） | 各待定状态                 | `vm.StatusSortOrder == status.GetSortOrder()` |

#### 2.1.42 Wpf.Tests / DownloadEnqueueDuplicatePolicyTests.cs（新建）

| 已创建 | 已通过 | 阶段    | 样例名                                                                                | 输入                             | 期望输出                 |
| ------ | ------ | ------- | ------------------------------------------------------------------------------------- | -------------------------------- | ------------------------ |
| [x]    | [x]    | 阶段 5+ | `FilterAlreadyPresent_ShouldExclude_WhenSourceIdExistsWithAnyStatus`（5 状态 Theory） | 已存在各状态任务，入队同 RJID    | 返回空列表               |
| [x]    | [x]    | 阶段 5+ | `FilterAlreadyPresent_ShouldInclude_WhenSourceIdNotInTaskList`                        | 已有 RJ001，入队 RJ002/RJ003     | 返回 [RJ002, RJ003]      |
| [x]    | [x]    | 阶段 5+ | `FilterAlreadyPresent_ShouldIgnoreCase`                                               | 已有小写 rj001，入队 RJ001/RJ002 | 跳过 RJ001，返回 [RJ002] |
| [x]    | [x]    | 阶段 5+ | `FilterAlreadyPresent_ShouldReturnAll_WhenNoExistingTasks`                            | 空任务列表，入队 2 项            | 返回所有 2 项            |
| [x]    | [x]    | 阶段 5+ | `FilterAlreadyPresent_ShouldReturnEmpty_WhenAllAlreadyExist`                          | 全部已存在                       | 返回空列表               |

#### 2.1.43 Application.Tests / SearchImportServiceTests.cs（新建）

| 已创建 | 已通过 | 阶段    | 样例名                                                  | 输入                           | 期望输出                                   |
| ------ | ------ | ------- | ------------------------------------------------------- | ------------------------------ | ------------------------------------------ |
| [x]    | [x]    | 阶段 5+ | `ParseCsvAsync_ShouldReturnItems_FromValidCsv`          | 有效 CSV（含标准 6 列）        | 正确解析 SourceId/Title/Release/Rate/Count |
| [x]    | [x]    | 阶段 5+ | `ParseCsvAsync_ShouldSkipHeaderAndEmptyLines`           | 含空行的 CSV                   | 只返回有效数据行                           |
| [x]    | [x]    | 阶段 5+ | `ParseCsvAsync_ShouldHandleQuotedTitle_WithComma`       | 标题含逗号（RFC4180 引号包裹） | 正确解析含逗号 title                       |
| [x]    | [x]    | 阶段 5+ | `ParseCsvAsync_ShouldHandleEmbeddedDoubleQuote_InTitle` | 标题含双引号（`""`转义）       | 正确解析含引号 title                       |
| [x]    | [x]    | 阶段 5+ | `ParseJsonAsync_ShouldDeserializeItems_FromValidJson`   | 有效 JSON 数组（camelCase）    | 正确反序列化 SourceId/Title                |
| [x]    | [x]    | 阶段 5+ | `ParseJsonAsync_ShouldReturnEmpty_ForEmptyJsonArray`    | `[]`                           | 返回空列表                                 |
| [x]    | [x]    | 阶段 5+ | `ParseJsonAsync_ShouldSkipEntries_WithEmptySourceId`    | 含空 sourceId 的条目           | 过滤空 sourceId，只返回有效项              |

#### 2.1.44 Wpf.Tests / SearchPagingPolicyTests.cs（新建）

| 已创建 | 已通过 | 阶段    | 样例名                                             | 输入                                  | 期望输出                        |
| ------ | ------ | ------- | -------------------------------------------------- | ------------------------------------- | ------------------------------- |
| [x]    | [x]    | 阶段 5+ | `SlicePage_ShouldClampPage_AndReturnExpectedItems` | 5 条数据，requestedPage=4，pageSize=2 | 纠正到有效页 3，并返回最后 1 条 |
| [x]    | [x]    | 阶段 5+ | `CanJump_ShouldReturnFalse_WhenOnlySinglePage`     | totalPages=1                          | 返回 false（禁用跳页）          |
| [x]    | [x]    | 阶段 5+ | `CanJump_ShouldReturnTrue_WhenMultiplePages`       | totalPages=2                          | 返回 true（允许跳页）           |

#### 2.1.45 Wpf.Tests / SearchQueueCountPolicyTests.cs（新建）

| 已创建 | 已通过 | 阶段    | 样例名                                                         | 输入                                   | 期望输出                                            |
| ------ | ------ | ------- | -------------------------------------------------------------- | -------------------------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 5+ | `Build_ShouldCountSkippedFromExistingQueuedAndInputDuplicates` | 混合输入重复 + 已在队列 + 已在任务列表 | `ToEnqueue` 与 `SkippedCount` 均与去重/跳过规则一致 |
| [x]    | [x]    | 阶段 5+ | `Build_ShouldReturnEmpty_WhenInputInvalid`                     | 空字符串与空白输入                     | 返回空队列且跳过数为 0                              |

### 2.2 测试覆盖分析

- Core（模型/配置）：默认值完整性，✅ 已覆盖。
- Application（应用服务）：首次启动、查询解析、搜索聚合、导出、下载主流程与容错，✅ 已覆盖。
- Infrastructure（基础设施）：配置读写、数据库初始化、鉴权、API 客户端、地址发现，✅ 已覆盖。
- Integration（集成）：引导器成功/失败/DB 初始化异常三条路径，✅ 已覆盖。
- WPF（UI 规则）：命令可用性规则与下载输入实时归一化显示，✅ 已覆盖。
- 下载入参规范化：RJID/作品 URL 混输下的入队与 API 调用兼容，✅ 已覆盖。
- 格式优先级下载：`PreferFormats` 过滤轨道与留空全下载，✅ 已覆盖。
- WorkInfo 预取复用：入队预取后下载阶段内存命中，✅ 已覆盖。
- 限流可观测性：`RateLimiter` 步骤间节流间隔，⬜ 待落地。

#### 2.3 关键实现修复（作为测试补齐的附带产物）

- `QueryParserService`：为筛选 token 缺值场景增加 `ValidateFilterToken`，抛出可读 `ArgumentException`。
- `AsmrApiClient`：在认证预热后显式附加 `Authorization: Bearer <token>` 请求头，使鉴权行为可被测试独立验证。
- `SourceIdNormalizer`：统一 `sourceId` 归一化（兼容 URL、`RJ-xxxx`、`RJ xxxx`、纯数字），在 Application/Infrastructure/WPF 三层复用，避免下载时触发 `400 Bad Request`。
- `DownloadInputNormalizer`：批量输入改为“提交时归一化”，避免实时改写影响输入符号与粘贴体验，同时保持入队前统一规范化与去重。
- `DownloadCommandAvailability`：从 `DownloadView.xaml.cs` 中抽取按钮可用性规则为独立纯状态类，消除 WPF 测试工程占位样例。

---

## 3. Github提交记录

本节用于记录本仓库与迁移文档相关的计划提交和已提交记录，便于追踪文档同步、仓库清理与代码迁移边界。

提交策略：如果仅有提交明细的改动，不做单独提交，与后续的代码改动一同提交。

### 3.1 计划与提交明细

| 提交日期   | 状态   | 提交总结(Summary)                                                          | 提交描述(Description)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Commit SHA |
| ---------- | ------ | -------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ---------- |
| 2026-03-18 | 已提交 | WPF migration first submission                                             | 1. Completed Phase 0: Initialize the .NET project<br>2. Completed Phase 1: Configure and initialize migration<br>3. Completed Phase 2: API and authentication migration<br>4. Completed Phase 3: Search capability migration<br>5. Completed Phase 4: Download capability migration                                                                                                                                                                                                                                                                                            | d36ead7    |
| 2026-03-18 | 已提交 | feat(dotnet): consolidate endpoint url flow and startup warmup             | 1. Add configurable endpoint discovery flow with persisted API base URL and runtime current-base-url usage in API/Auth services<br>2. Keep Discover calls only for startup warmup and connectivity probe, and make startup warmup non-blocking with timeout/fail-open behavior<br>3. Add infrastructure and WPF regression tests, then sync progress sections 1.5/2.1/3.1                                                                                                                                                                                                      | 43eddcd    |
| 2026-03-19 | 待提交 | align docs to WPF; align runtime/docs version to 0.4                       | 1. Adjust README.md to focus on the WPF client; and move the original Go CLI/WebUI examples to docs/legacy-go.md as historical references.<br>2. Add LICENSE with copyright holder kirahsosha.<br>3. Uniformly set Version to the 0.4 series in the WPF runtime projects.<br>4. Add v0.4 display in MainWindow, Settings, and startup logs, and synchronize the version identifiers in README and the WPF migration documentation.                                                                                                                                             | —          |
| 2026-03-19 | 待提交 | feat(dotnet): fix sort/dedup bugs, add hd-audio filter and csv/json import | 1. Fix status column sort order by adding `StatusSortOrder` property to `DownloadTaskRowViewModel`<br>2. Add `DownloadEnqueueDuplicatePolicy` to prevent duplicate RJID entries in download list<br>3. Add HD audio only checkbox and `FilterHdAudioOnly<T>()` to exclude mp3 when wav/flac exists<br>4. Add Import CSV/Import JSON buttons with `SearchImportService` (RFC4180 CSV + camelCase JSON)<br>5. Add regression tests (+31) and sync progress sections 1.5/2.1/3.1                                                                                                  | —          |
| 2026-03-27 | 待提交 | fix(search/download): preserve filters, accurate queue counts, v0.4.1 sync | 1. Bump runtime/docs version to v0.4.1 across csproj, WPF title/status and README.<br>2. Fix Search clear behavior to preserve advanced filters while clearing keyword/sort/pagination/results.<br>3. Keep popular query context during paging and disable jump controls when only one page.<br>4. Add queue-count policy to report accurate added/skipped counts.<br>5. Add hd_audio_only config default true and skip txt/lrc/ass sidecars when mp3 is removed by HD-audio filtering.<br>6. Add/refresh regression tests and validate with full solution test run (178/178). | —          |

---

## 4. 功能测试验证清单

本章用于指导测试人员对当前已交付的 WPF 客户端执行功能测试与回归测试。当前范围仅覆盖已完成并可使用的能力：阶段 1 到阶段 4，以及第 1.5.60 与第 1.5.61 条补充的功能修复与增强。阶段 5 及以后功能暂不纳入本清单。

AI约束：每次进行功能开发、缺陷修复或任何可能影响用户可见行为的改动时，必须先检查本章并将受影响的测试项重置为未勾选；待对应的功能测试或回归测试通过后，再重新勾选，并在必要时同步更新第 1.4 节和第 1.5 节记录。

说明：

- `[ ]`：本轮尚未验证，或因代码改动需要重新验证。
- `[x]`：本轮已验证通过。
- 任一项失败时，不得勾选该项；需要在第 1.4 节登记阻塞，或在第 1.5 节补充验证结果。

### 4.1 启动、配置与连接

- [x] 应用可正常启动，主窗口可显示 Search、Download、Settings 三个页签，且启动过程不因站点发现流程长时间阻塞。
- [x] Settings 页面可正确加载现有配置；默认下载目录、格式优先级、下载筛选默认规则、全局搜索规则等字段显示完整。
- [x] 在 Settings 页面修改有效配置后，“保存并重新初始化”可成功完成，状态提示明确，应用进入可用状态。
- [x] Settings 页面输入无效配置时，可给出可读错误提示，且应用不崩溃。
- [x] “测试连接”可完成站点发现与登录校验；成功时回填当前 BaseUrl 并显示延迟与鉴权结果，失败时显示明确原因。

### 4.2 Search 功能

- [x] 仅输入基础关键词即可成功搜索，并展示结果列表、总数和页码信息。
- [x] 高级筛选 `tag/circle/va/duration/rate/price/sell/age/lang` 可单独或组合生效，`反选` 语义正确。
- [x] Search 的排序、方向、字幕、是否包含翻译作品等选项生效，翻页后条件保持不丢失。
- [x] 上一页、下一页、跳页、页大小切换均可用，分页结果与页码信息正确。
- [x] “查询热门作品”可返回结果并展示到结果列表。
- [x] “清空”可重置关键词、排序选项、分页状态和当前结果，同时保留高级筛选输入与反选状态。
- [x] Settings 中配置的“全局搜索规则”会在 Search 页面首次加载时自动填充到高级筛选，页面切换往返后不会重复追加。
- [x] 选中部分结果点击“加入下载队列”时，仅将选中项入队；未选中任何结果时，可按当前结果集批量入队。
- [x] 已存在于下载列表中的作品不会重复入队，页面提示中会明确说明新增数量与跳过数量。
- [x] Search 结果可成功导出 CSV 与 JSON，导出文件内容可正常打开且关键字段完整。

### 4.3 Download 功能

- [x] 单个 RJID 入队支持 `RJxxxx`、作品 URL、`RJ-xxxx`、纯数字等输入形式，提交后可归一化并成功入队。
- [x] 批量入队支持逗号、分号、空格、换行混合分隔；重复项会去重，已存在任务不会重复加入。
- [x] 导入 CSV 与导入 JSON 可成功读取 Search 导出文件并入队，重复任务会被跳过且提示明确。
- [x] 执行下载队列后，任务列表与队列计数会刷新，任务状态、进度、目标目录、错误信息显示正确。
- [x] 状态列排序遵循业务顺序而非字母序；状态文案显示为中文且与实际状态一致。
- [x] “立即下载选中任务”可对 Pending、Failed、Canceled 等允许状态生效，不允许的状态不会误触发。
- [x] “取消选中任务”可取消 Pending、Queued、Running 任务，确认提示、取消结果与列表状态一致。
- [x] “重试失败任务”仅对单个失败任务可用；“重试全部失败任务”仅在存在失败任务时可用，并能输出正确汇总结果。
- [x] “打开下载目录”可打开当前生效的下载目录。
- [x] 文件筛选规则可生效；开启“只下载高清音频”后，在同时存在 flac/wav 与 mp3 的场景下不会重复下载 mp3。
- [x] 新启动的失败任务不会从列表中消失；失败、取消、完成后的任务状态可被稳定追踪。

### 4.4 Search/Download 联动与回归

- [x] Search 页面加入下载队列后，Download 页面可看到对应待执行任务，标题信息尽量不丢失。
- [x] 已取消任务再次从 Search 侧或 Download 侧触发下载时，可复用原任务行并回流为待执行/执行中状态，不新增重复行。
- [x] Search 与 Download 页面之间来回切换后，队列数量、任务状态和标题缓存保持一致，不出现旧状态残留。
- [x] Search 导出 -> Download 导入 -> 执行下载的链路可端到端跑通。
- [x] 重复入队防护在 Search 入队、Download 单个入队、Download 批量入队、CSV 导入、JSON 导入五条入口上行为一致。
- [x] 连续执行“搜索 -> 入队 -> 立即下载/执行队列 -> 刷新列表 -> 重试/取消”后，应用无崩溃、无明显 UI 状态错乱。
