# asmr-downloader WPF 项目进度跟踪

当前跟踪版本：v0.7.0

AI约束策略：章节1.5.1到1.5.115的文本不加入分析上下文

## 1. 项目进度跟踪清单

本章节用于在迁移实施过程中持续记录阶段进展，作为团队与 AI 协作的统一进度面板。

## 1.1 状态标记说明

- `未开始`：阶段尚未进入实施。
- `进行中`：阶段已开始，尚未满足 DoD。
- `已完成`：阶段 DoD 已全部满足，并完成验证记录。
- `阻塞`：阶段存在外部依赖或关键问题，无法继续推进。

## 1.2 阶段总览清单

| 阶段   | 名称                   | 状态   | 负责人    | 开始日期   | 目标完成日期 | 实际完成日期 | 备注                                                                                                                                    |
| ------ | ---------------------- | ------ | --------- | ---------- | ------------ | ------------ | --------------------------------------------------------------------------------------------------------------------------------------- |
| 阶段 0 | 创建 .NET 解决方案骨架 | 已完成 | AI + 用户 | 2026-03-14 | 2026-03-14   | 2026-03-14   | 已确认 9 项目在解决方案中；`dotnet build dotnet/Asmroner.sln` 成功。                                                                    |
| 阶段 1 | 配置与初始化迁移       | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成配置模型、程序目录 `config.json` 默认配置加载、SQLite读写与旧结构迁移、初始化编排、首次启动设置向导分支与“保存后重新初始化”闭环。 |
| 阶段 2 | API 与认证迁移         | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已修复 `HttpClient` 配置时机问题并完成回归。                                                                                            |
| 阶段 3 | 搜索能力迁移           | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成查询解析、分页聚合、搜索导出、入队与高级筛选/分页 UI 交互；补齐“无关键词调整排序类控件触发查询”修复。                             |
| 阶段 4 | 下载能力迁移           | 已完成 | AI + 用户 | 2026-03-15 | 2026-03-15   | 2026-03-15   | 已完成下载入口、并发/重试控制与状态可视化复核，并补齐 SQLite 状态持久化能力。                                                           |
| 阶段 5 | 同步能力迁移           | 已完成 | AI + 用户 | 2026-04-02 | 2026-04-03   | 2026-04-03   | 已补齐 SQLite UiState 同步进度持久化、断点继续、合并式开始/停止按钮与同步运行中手动刷新统计；阶段 5 增强回归已闭环。                    |
| 阶段 6 | 资源库与播放能力迁移   | 已完成 | AI + 用户 | 2026-04-15 | 2026-04-16   | 2026-04-16   | 已完成资源库扫描/索引、Library 页签、显式选中文件后的系统默认程序打开、异常扫描容错、共享可播放格式规则；阶段 6 DoD 已闭环。            |
| 阶段 7 | UI 集成与体验收口      | 进行中 | AI + 用户 | 2026-04-18 | 待填写       | 待填写       | 已落地壳层状态/导航/对话框共享服务与通用样式基线；加载态/空态统一与手工冒烟回归待补齐。                                                 |
| 阶段 8 | 最终验收与发布准备     | 未开始 | 待填写    | 待填写     | 待填写       | 待填写       | -                                                                                                                                       |

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
- [x] Search/Download 界面状态与未完成队列可持久化并在重启后恢复。
- [x] 阶段 4 DoD 已满足并记录证据。

### 阶段 5：同步能力迁移

- [x] 元数据同步、页级进度持久化与断点继续可用。
- [x] 同步下载、容量控制、作品级进度持久化与断点继续可用。
- [x] 失败重试、导出、温和停止与合并按钮交互可用。
- [x] 报表、统计展示、手动刷新与按钮状态控制正确。
- [x] 阶段 5 DoD 已满足并记录证据。

### 阶段 6：资源库与播放能力迁移

- [x] 本地资源扫描与索引可用。
- [x] 资源库页面浏览与筛选可用。
- [x] 显式选中可播放媒体文件后，可载入并通过系统默认程序打开。
- [x] 异常文件处理不导致应用崩溃。
- [x] 阶段 6 DoD 已满足并记录证据。

### 阶段 7：UI 集成与体验收口

- [x] 全局导航与页面路由稳定。
- [ ] 消息、对话框、加载态、空态统一。
- [x] 跨页面关键流程回归通过。
- [ ] UI 冒烟测试通过。
- [ ] 阶段 7 DoD 已满足并记录证据。

### 阶段 8：最终验收与发布准备

- [ ] 第 14 节验收项全部通过。
- [ ] 发布包构建、签名、安装验证通过。
- [ ] 发布说明与迁移说明完成。
- [ ] 回滚策略完成验证。
- [ ] 阶段 8 DoD 已满足并记录证据。

## 1.4 阻塞与风险登记

### 说明

- **当前状态**：`已解决` / `待验证` / `阻塞中`
- **填写规则**：只添加用户明确添加的项目，或者测试发现的**非本次改动**引入的明确bug。
- **日期规则**：已解决项填写实际解决日期；待验证/阻塞中项填写计划解决日期

### 风险与缺陷登记

| 日期       | 阶段   | 问题描述                                                                                                       | 影响范围                      | 当前状态 | 解决/计划日期 |
| ---------- | ------ | -------------------------------------------------------------------------------------------------------------- | ----------------------------- | -------- | ------------- |
| 2026-03-15 | 阶段 2 | `HttpClient` 请求发起后再次修改 `Timeout/BaseAddress` 导致测试失败                                             | API 与认证迁移                | 已解决   | 2026-03-15    |
| 2026-03-15 | 阶段 3 | `SearchService` 内部固定 `Page = 1`，翻页请求实际仍从第一页开始                                                | 分页搜索稳定性                | 已解决   | 2026-03-15    |
| 2026-03-16 | 阶段 4 | 下载流程 `RunSingleAsync` 报 400 Bad Request                                                                   | 下载链路稳定性                | 已解决   | 2026-03-16    |
| 2026-03-17 | 阶段 4 | Canceled 任务重新入队后 `StatusText` 未同步更新                                                                | Download 页面状态一致性       | 已解决   | 2026-03-27    |
| 2026-03-29 | 阶段 4 | 重启后"只下载高清音频""文件筛选"与未完成队列恢复项未通过                                                       | 状态持久化与重启恢复          | 已解决   | 2026-03-29    |
| 2026-03-30 | 阶段 1 | "保存并重新初始化"后自动切换到 Search 页签                                                                     | Settings 页面停留行为         | 已解决   | 2026-03-30    |
| 2026-03-31 | 阶段 4 | 连续导出时打开重复资源管理器窗口                                                                               | 导出体验                      | 已解决   | 2026-03-31    |
| 2026-03-31 | 阶段 4 | CSV/JSON 导入数量/跳过数量计算不准确                                                                           | Download 导入计数             | 已解决   | 2026-03-31    |
| 2026-03-31 | 阶段 4 | 启动后后台标题补拉失败导致列表标题空白                                                                         | Download 启动后可读性         | 已解决   | 2026-03-31    |
| 2026-04-01 | 阶段 4 | `BJ02370869` 等非 RJ 作品 `source_id` 与 `workId` 不一致导致解析失败                                           | 热门结果入队、翻译入队        | 已解决   | 2026-04-01    |
| 2026-04-01 | 阶段 4 | 清空任务列表后入队 `source_id/workId` 失配作品误报"失败 1 项"                                                  | Search->Download 联动         | 已解决   | 2026-04-01    |
| 2026-04-01 | 阶段 4 | 轨道标题自带扩展名时下载文件生成重复后缀                                                                       | 下载文件命名                  | 已解决   | 2026-04-01    |
| 2026-04-03 | 阶段 5 | Sync 页面缺少温和停止入口，重启后不记录未完成进度                                                              | 同步链路可恢复性与可控性      | 已解决   | 2026-04-03    |
| 2026-04-03 | 阶段 5 | Sync 页面开始/停止按钮分散，运行中禁用"刷新统计"                                                               | Sync 操作一致性与实时可观测性 | 已解决   | 2026-04-03    |
| 2026-04-03 | 阶段 5 | Sync 主按钮快速双击误触发 stop request                                                                         | 开始/停止按钮可控性           | 已解决   | 2026-04-03    |
| 2026-04-08 | 阶段 5 | Sync 落地文件被写成占位文本而非真实媒体                                                                        | Sync 下载结果正确性           | 已解决   | 2026-04-08    |
| 2026-04-08 | 阶段 4 | 双目录无匹配文件时写入占位文本而非实时下载                                                                     | 普通下载链路                  | 已解决   | 2026-04-08    |
| 2026-04-08 | 阶段 2 | 发布页入口脚本路径/引号形态解析不稳定                                                                          | 站点发现稳定性                | 已解决   | 2026-04-08    |
| 2026-04-08 | 阶段 5 | 元数据同步本地总量/字幕量未持久化，刷新后数量不正确                                                            | Sync 统计一致性               | 已解决   | 2026-04-08    |
| 2026-04-10 | 阶段 2 | "测试连接"不回写 `ApiCandidateUrls`，SQLite 候选集合无法随发布页更新                                           | 站点发现与配置持久化          | 已解决   | 2026-04-10    |
| 2026-04-12 | 阶段 2 | 发布页入口脚本使用相对路径/查询串/单引号配置时解析可能失败                                                     | 站点发现兼容性                | 已解决   | 2026-04-14    |
| 2026-04-12 | 阶段 2 | 发布页正文最新域名未按顺序补齐并持久化到 `ApiCandidateUrls`                                                    | 站点发现与配置持久化          | 已解决   | 2026-04-14    |
| 2026-04-12 | 阶段 5 | 网站总量与本地一致时未提示"无需同步"，可能重复写入数据                                                         | 同步效率与用户体验            | 待验证   | 待确定        |
| 2026-04-12 | 阶段 5 | 已完成状态下存在过期元数据时未自动执行过期刷新                                                                 | 元数据保鲜                    | 待验证   | 待确定        |
| 2026-04-12 | 阶段 5 | 同步完成后页面摘要未完整显示累计处理/新增数量                                                                  | 同步结果展示                  | 待验证   | 待确定        |
| 2026-04-12 | 阶段 5 | 同步下载完成后未重置进度并从头校验，可能重复处理已匹配作品                                                     | 同步下载重扫效率              | 待验证   | 待确定        |
| 2026-04-12 | 阶段 4 | 普通下载未区分完整下载与子集下载的同步完成态，可能误把子集结果写成 `COMPLETED`                                 | 普通下载与 Sync 增量判定      | 已解决   | 2026-04-12    |
| 2026-04-14 | 阶段 4 | Download 页面在“立即下载选中任务/执行下载队列”期间将 `OpenDownloadDirectoryButton` 误纳入队列变更禁用组        | Download 页面命令可用性       | 已解决   | 2026-04-14    |
| 2026-04-14 | 阶段 4 | Download 页面在“立即下载选中任务/执行下载队列”期间未将 `ImportFileButton` 纳入与其它入队入口一致的运行态禁用组 | Download 页面导入入口一致性   | 已解决   | 2026-04-14    |
| 2026-04-14 | 阶段 5 | Sync 页面把“重试失败项”与导出共用独占状态，导致重试期间 `RefreshStatusButton` 被连带禁用                       | Sync 页面实时可观测性         | 已解决   | 2026-04-14    |
| 2026-04-14 | 阶段 5 | Sync 页面在“重试失败项”期间点击“刷新统计”会用旧的持久化下载进度覆盖当前重试状态与详情                          | Sync 页面重试态可观测性       | 已解决   | 2026-04-14    |
| 2026-04-16 | 阶段 6 | Library 页右侧详情区与文件树使用自然高度布局，选中作品后长文本与文件树内容会超出窗口范围                       | Library 页面详情与文件树显示  | 已解决   | 2026-04-16    |
| 2026-04-16 | 阶段 6 | Library 页右侧文件树固定高度且右栏未随窗口拉高同步伸展，导致放大窗口后左侧作品列表与右侧文件树下边沿失衡       | Library 页面缩放对齐          | 已解决   | 2026-04-16    |
| 2026-04-16 | 阶段 6 | Library 页左右内容区缺少受限高度容器，缩小窗口时列表、详情与文件树内容会继续外溢而非使用内部滚动承载           | Library 页面缩放与滚动边界    | 已解决   | 2026-04-16    |

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

### 1.5.48 2026-03-17，阶段 4 扫描并整合残留测试文件（开始）

1. 变更摘要：按“目标文件对应测试文件”规则继续向后端与 WPF 测试工程扩展，扫描并识别命名不当或覆盖多目标的测试样例，计划逐一拆分/重命名并补齐对应测试文件，保证每个生产目标文件对应一个明确的测试文件。
2. 关键文件（扫描发现）：`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（存在综合性基础设施用例，需按目标拆分）；扫描将继续并生成待改动清单。
3. 验证结果：扫描已发现候选文件（见上）；尚未实施拆分/重命名改动，当前测试基线保持不变。
4. DoD 判定：进行中。阶段 4 最终 DoD 为：所有跨目标或命名不当的测试文件已拆分/重命名并通过对应项目回归测试。
5. 下次计划：
 - 按待改动清单逐个拆分/重命名测试文件并运行受影响测试项目；
 - 每完成一项改动即在 `1.5` 节追加新的变更记录（遵守下述变更策略）。

变更策略：自本条起，`1.5` 节仅追加新的记录，不再修改已有历史记录；如需修正前一次记录的描述或补充证据，将在后续新条目中引用对应条目编号并补充说明。

### 1.5.49 2026-03-17，阶段 4 基础设施测试文件拆分（`ApiInfrastructureTests` 收口）

1. 变更摘要：将混合覆盖多个目标文件的 `dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs` 按生产目标文件拆分并并入现有专用测试文件：`AsmrApiClient_ShouldMapHttpErrors` 并入 `AsmrApiClientTests.cs`，`AuthService_ShouldStoreTokenAfterLogin` 与 `AuthService_ShouldThrowReadableErrorOnFailure` 并入 `AuthServiceTests.cs`，`EndpointDiscoveryService_ShouldPickFastestReachableCandidate` 并入 `EndpointDiscoveryServiceTests.cs`；同时新增 `TokenStoreTests.cs` 承载 `TokenStore_ShouldRoundTripToken`，随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/TokenStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiInfrastructureTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且基础设施测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否仍存在“一个测试文件覆盖多个目标文件”或“文件名与目标文件不一致”的情况，并按同样规则逐项收口。

### 1.5.50 2026-03-17，阶段 4 应用层搜索测试文件拆分（`SearchWorkflowTests` 收口）

1. 变更摘要：将混合覆盖 `QueryParserService`、`SearchService`、`SearchExportService` 三个目标的 `dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs` 按目标文件拆分：`QueryParser_ShouldParseAdvancedQuery` 并入 `QueryParserServiceTests.cs`，`SearchService_ShouldAggregateMultiplePages` 与 `SearchService_ShouldRespectRequestedPageAndKeepFilters` 并入 `SearchServiceTests.cs`，`SearchExportService_ShouldExportCsvAndJson` 并入 `SearchExportServiceTests.cs`；随后删除原聚合测试文件。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchExportServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮已消除一个跨目标文件的聚合测试文件，且应用层测试工程回归通过。
5. 下次计划：继续扫描其余测试工程中是否存在跨目标聚合测试文件，并按同样规则拆分收口。

### 1.5.51 2026-03-17，阶段 4 下载服务测试文件并档（`DownloadWorkflowTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs` 中所有 `DownloadService` 行为用例并入 `DownloadServiceTests.cs`，统一由单文件承载同一目标 `DownloadService.cs` 的测试；为避免重复桩逻辑，扩展 `DownloadServiceTestDoubles.cs` 的 `ScriptedApiClient`（补充并发观测、延迟与失败次数控制）并新增 `DelayRateLimiterService`，随后删除 `DownloadWorkflowTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadWorkflowTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描其他测试工程是否仍存在“同一目标由多个测试文件分散承载”或“跨目标聚合”情况，并按同样规则收口。

### 1.5.52 2026-03-17，阶段 4 下载路径测试并档（`DownloadPathTests` 收口）

1. 变更摘要：将 `dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs` 中的目录命名兼容性用例 `DirectoryNameStrategy_ShouldMatchGoCompatibilityRule` 并入 `DownloadServiceTests.cs`，统一由同一测试文件承载 `DownloadService.cs` 的路径与工作流行为；随后删除原 `DownloadPathTests.cs`。
2. 关键文件：`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadPathTests.cs`（删除）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 29，失败 0，成功 29）。
4. DoD 判定：是。本轮完成同目标测试文件并档，且应用层测试工程回归通过。
5. 下次计划：继续扫描 WPF/Infrastructure/Integration 测试工程中是否仍有同目标分散或跨目标聚合测试文件，并逐项收口。

### 1.5.53 2026-03-17，阶段 4 基础设施测试依赖收敛（`AsmrApiClientTests`）

1. 变更摘要：对 `AsmrApiClientTests.cs` 进行依赖收敛，`AsmrApiClient_ShouldMapHttpErrors` 用例不再实例化真实 `AuthService`，改为使用 `StubAuthService` 直接提供 token，仅验证 `AsmrApiClient` 在服务端返回 500 时的错误映射行为，避免该测试文件对 `AuthService` 目标行为产生耦合。
2. 关键文件：`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 17，失败 0，成功 17）。
4. DoD 判定：是。本轮未改变测试文件映射结构，但进一步收敛了测试职责边界，且基础设施测试工程回归通过。
5. 下次计划：继续做最后一轮全仓扫描，若无新的跨目标聚合/同目标分散候选，则进入阶段 4 测试结构收口总结。

### 1.5.54 2026-03-17，阶段 4 测试结构收口复核（全仓扫描 + 解决方案回归）

1. 变更摘要：执行全仓测试文件复核扫描（Application/Core/Infrastructure/Integration/Wpf），确认当前未发现新的“跨目标聚合”或“同目标分散”高优先级候选；对少量仍出现多类型构造的文件判定为测试桩与协作者注入场景，不再进行机械拆分，避免过度拆分导致可读性下降。
2. 关键文件：本轮无代码结构性变更，主要为扫描与验证。
3. 验证结果：`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 105，失败 0，成功 105）。
4. DoD 判定：是。阶段 4 当前测试结构收口目标已满足，且解决方案级回归通过。
5. 下次计划：进入阶段 4 业务能力迁移实现与验证，测试结构后续仅按增量功能做最小维护。

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

### 1.5.62 2026-03-27，SQLite 状态持久化、清空任务列表与 v0.4.2 对齐

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

### 1.5.63 2026-03-29，AppConfig 分段存储、config.json 默认配置与 TOML 退场

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

### 1.5.64 2026-03-29，阶段 4/5 技术债清理（旧字段移除 + 默认回退收敛）

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

### 1.5.65 2026-03-29，阶段 4 恢复链路修复（Search 入队即时持久化）

1. 变更摘要：
 - **问题定位**：未完成队列此前主要在 Download 页刷新路径持久化；若用户仅在 Search 页入队后直接退出，可能出现重启后恢复不完整。
 - **修复实现**：在 Search 页 `OnQueueClicked` 入队成功后，立即触发未完成队列持久化，消除“必须进入 Download 页才落盘”的隐式前提。
 - **策略复用**：新增 `DownloadUnfinishedQueueSnapshotPolicy`，统一“活跃任务（Pending/Queued/Failed）+ 当前队列”快照构建规则；Download 与 Search 两侧共用，避免逻辑漂移。
 - **测试补齐**：新增 `DownloadUnfinishedQueueSnapshotPolicyTests`，覆盖状态筛选、跨来源去重与空输入场景。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadUnfinishedQueueSnapshotPolicy.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DashboardView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadUnfinishedQueueSnapshotPolicyTests.cs`（新建）。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 101，失败 0，成功 101）；`dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj -c Release --nologo` 通过（总计 35，失败 0，成功 35）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 189，失败 0，成功 189）。
4. DoD 判定：是。本轮恢复链路缺口已修复，相关回归通过。
5. 下次计划：补充“Search 入队后不切换 Download 页直接退出并重启”的手工功能验证并由用户勾选章节 4 对应项。

### 1.5.66 2026-03-29，v0.4.3 对齐 + Search 首次交互修复 + SearchView 命名统一

1. 变更摘要：
 - **版本对齐**：Core/Application/Infrastructure/Wpf、MainWindow 标题、Settings 版本文案、启动日志与 README 统一升级到 `v0.4.3`。
 - **缺陷修复**：修复 Search 页面“未先执行搜索时，调整排序/方向/字幕/页大小不触发查询”的问题；新增 option-only query 路径，允许仅携带分页/排序参数发起搜索。
 - **命名统一**：将 `DashboardView` 重命名为 `SearchView`，同步 DI、MainWindow 承载引用与 WPF 测试命名。
 - **测试补齐**：新增解析与服务层回归（option-only query），并补充 SearchView XAML 事件绑定与类名断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/QueryParserServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SearchServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/MainWindowXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SettingsViewXamlTests.cs`、`README.md`。
3. 验证结果：`dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj -c Release --nologo` 通过（总计 43，失败 0，成功 43）；`dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj -c Release --nologo` 通过（总计 103，失败 0，成功 103）；`dotnet test dotnet/Asmroner.sln -c Release --nologo` 通过（总计 193，失败 0，成功 193）。
4. DoD 判定：是。本轮代码与自动化回归已闭环，待章节 4 手工项由用户执行并勾选。
5. 下次计划：按章节 4 执行 Search 首次交互、跨页联动与版本显示的手工回归，确认通过后由用户完成勾选。

### 1.5.67 2026-03-30，阶段 3 Search 任务列表右键菜单（入队/导出/浏览器打开）

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

### 1.5.68 2026-03-30，v0.4.4 对齐 + Search 右键多选修复 + 导出全部/选中分流

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

### 1.5.69 2026-03-30，v0.4.5 对齐 + Settings 重初始化停留修复

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

### 1.5.70 2026-03-30，v0.4.6 版本升级 + NLog 结构化日志封装

1. 变更摘要：
 - **章节复核**：已复核章节 1.2/1.3/1.4；阶段状态维持不变。
 - **版本升级**：Core/Application/Infrastructure/Wpf 项目版本、启动日志统一升级到 `v0.4.6`。
 - **日志封装**：引入 `NLog 5.3.2`，新建 `IAppLogService` 接口（Core）与 `NLogAppLogService` 实现（Infrastructure）；以编程式配置替代 XML 配置文件，日志以 JSON Lines 格式写入滚动文件（`~/.asmroner-data/logs/`），文件超过 10 MB 或日期变更时自动归档；`IAppPathService` 新增 `LogsDirectory` 属性，`App.xaml.cs` 在 `Host.Build()` 后立即调用 `logService.Configure()`，`ConfigureLogging` 改用 `AddNLog()` 桥接到 MEL。
 - **测试同步**：新建 `NLogAppLogServiceTests`（5 个 Fact：FileTarget 存在性/JsonLayout/归档大小/按天归档/目录自动创建）与 `AppPathServiceTests`（`LogsDirectory_ShouldBeUnderMetadataDirectory`）共 6 个新测试样例；`DownloadServiceTestDoubles.TestAppPathService` 补全 `LogsDirectory` 接口成员。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`、`dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAppPathService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAppLogService.cs`（新建）、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AppPathService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/NLogAppLogService.cs`（新建）、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/NLogAppLogServiceTests.cs`（新建）、`dotnet/tests/Asmroner.Infrastructure.Tests/AppPathServiceTests.cs`（新建）、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 210，失败 0，成功 210）。
4. DoD 判定：是。版本升级与 NLog 日志封装均完成，自动化回归通过。
5. 下次计划：由用户执行章节 4.1 的手工回归项（Settings 版本展示规则 v0.4.6）并按实际结果勾选。

### 1.5.71 2026-03-30，NLog 升级至 6.1.1 + NLogAppLogService 代码优化

1. 变更摘要：
 - **NLog 版本升级**：`NLog` 由 `5.3.2` 升至 `6.1.1`（Infrastructure、Infrastructure.Tests），`NLog.Extensions.Logging` 由 `5.3.2` 升至 `6.1.2`（Wpf），与当前最新稳定版对齐。
 - **NLogAppLogService 代码优化**：提取 `BuildFileTarget` 为 `internal static` 方法以支持单元测试直接构造；新增 `LogManager.ReconfigExistingLoggers()` 调用确保存量 Logger 实例立即生效；JSON 时间戳改用 `${longdate}`；归档文件迁移至 `archive/` 子目录，扩展名统一为 `.json`；新增 `ConcurrentWrites = true` 与 `KeepFileOpen = true` 优化写入性能；`NLogAppLogServiceTests` 补充第 5 个 Fact：`Configure_ShouldCreateLogDirectory_WhenNotExists`。
 - **章节复核**：章节 1.2/1.3/1.4 状态无需变动；章节 2.1 基线维持 210/210。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/NLogAppLogService.cs`。
3. 验证结果：`dotnet test dotnet/Asmroner.sln` 通过（总计 210，失败 0，成功 210）。
4. DoD 判定：是。NLog 版本对齐与代码结构优化完成，回归通过。
5. 下次计划：由用户执行章节 4.1 的手工回归项并按实际结果勾选。

### 1.5.72 2026-03-30，全量切换为 NLog 静态日志器 + 移除 `ILogger<T>` 注入

1. 变更摘要：
 - **日志实现统一**：将 Infrastructure/WPF 中剩余 `ILogger<T>` 注入改为 `NLog.LogManager.GetCurrentClassLogger()` 静态日志器，覆盖 `ApiEndpointUrlService`、`AuthService`、`ConnectivityProbeService`、`StartupEndpointWarmupService`、`MainWindow`、`SettingsView`、`SearchView`、`DownloadView`、`App.xaml.cs`。
 - **依赖收敛**：移除 WPF 项目中的 `Microsoft.Extensions.Logging.Console` 与 `Microsoft.Extensions.Logging.Debug` 包引用；保留 `NLog.Extensions.Logging` 作为桥接。
 - **测试同步**：移除相关测试中的 `NullLogger<T>.Instance` 构造参数，匹配新构造函数签名；`NLogAppLogServiceTests` 新增 `Configure_ShouldWriteLogEntry_WhenStaticLoggerIsUsed`，验证静态 logger 可落盘写入。
 - **回归结果**：`dotnet build dotnet/Asmroner.sln --configuration Debug` 成功（0 错误）；五个测试程序集直接执行通过，总计 211，失败 0。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AuthService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConnectivityProbeService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/StartupEndpointWarmupService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`、`dotnet/tests/Asmroner.Infrastructure.Tests/NLogAppLogServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConnectivityProbeServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/StartupEndpointWarmupServiceTests.cs`。
3. 验证结果：`dotnet build dotnet/Asmroner.sln --configuration Debug` 通过；`dotnet dotnet/tests/Asmroner.Core.Tests/bin/Debug/net8.0/Asmroner.Core.Tests.dll`、`dotnet dotnet/tests/Asmroner.Infrastructure.Tests/bin/Debug/net8.0/Asmroner.Infrastructure.Tests.dll`、`dotnet dotnet/tests/Asmroner.IntegrationTests/bin/Debug/net8.0/Asmroner.IntegrationTests.dll`、`dotnet dotnet/tests/Asmroner.Application.Tests/bin/Debug/net8.0/Asmroner.Application.Tests.dll`、`dotnet dotnet/tests/Asmroner.Wpf.Tests/bin/Debug/net8.0-windows/Asmroner.Wpf.Tests.dll` 全部通过（总计 211，失败 0，成功 211）。
4. DoD 判定：是。日志体系统一到 NLog 静态日志器，编译与自动化回归通过。
5. 下次计划：由用户执行章节 4.1 受影响项（启动与 Settings 操作路径）手工回归并按结果重新勾选。

### 1.5.73 2026-03-31，Search/Download 标签列 + 导出优化 + v0.4.7

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

### 1.5.74 2026-03-31，Bug 修复：资源管理器窗口去重 + 导入计数修正

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

### 1.5.75 2026-03-31，Bug 修复补充：Download 导入计数纳入待下载队列

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

### 1.5.76 2026-03-31，v0.4.8：常量集中化 + 启动未完成队列元数据后台刷新

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

### 1.5.77 2026-03-31，v0.4.8：启动补拉修复 + Search/Download 列表 UI 调整

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

### 1.5.78 2026-04-01，v0.4.8：列宽微调

1. 变更摘要：
 - **Search 列宽微调**：Search 结果表字幕列宽度调整为 `42`，日期列宽度调整为 `75`，继续保持不可拖拽改宽。
 - **Download 列宽微调**：Download 任务表状态列宽度调整为 `50` 且继续锁定；进度列保留 `96` 默认宽度，但取消显式锁定，允许手动调整列宽。
 - **测试同步**：更新 `SearchViewXamlTests` 与 `DownloadViewXamlTests` 断言，并回归启动 warmup/未完成队列补拉测试，确认本轮 UI 调整未影响既有启动修复。
 - **章节复核**：章节 2.1 基线更新为 2026-04-01 的 226/226，并更新 2.1.17、2.1.19；章节 3.1 继续合并到同一条 `v0.4.8` 待提交记录；章节 4.2/4.3 受影响手工项重置为未勾选。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/SearchViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadViewXamlTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：先执行定向回归（`SearchViewXamlTests.cs`、`DownloadViewXamlTests.cs`、`StartupEndpointWarmupServiceTests.cs`、`StartupUnfinishedQueueMetadataRefreshServiceTests.cs`），总计 20，失败 0，成功 20；再执行 VS Code 测试运行器全量回归，总计 226，失败 0，成功 226。
4. DoD 判定：是。本轮列宽微调、断言更新与启动补拉缺陷说明同步均已完成。
5. 下次计划：由用户执行章节 4.2/4.3 受影响项手工回归，重点验证 Search 字幕/日期列宽、Download 状态/进度列宽交互，以及重启后缺失标题后台补拉。

### 1.5.79 2026-04-01，v0.4.9：翻译作品优先入队与版本流程手册

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

### 1.5.80 2026-04-01，v0.4.9：BJ 作品编号修复与启动补拉失败可视化

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

### 1.5.81 2026-04-01，v0.4.9：翻译入队提示补强与重复后缀修复

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

### 1.5.82 2026-04-02，v0.4.10：WorkInfo 共享缓存与版本同步

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

### 1.5.83 2026-04-02，v0.4.11：SQLite 收藏夹与 Search/Download 收藏链路

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

### 1.5.84 2026-04-02，v0.4.11：Search/Download 文件入口收敛与重试按钮合并

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

### 1.5.85 2026-04-02，阶段 5 首批启动

1. 变更摘要：按第 4.6 节启动阶段 5，新增同步元数据 API 与 DTO、SQLite `MetadataWork/WorkSyncInfo` 表、`MetadataSyncService` 和最小 `Sync` 页签；同时将运行时与文档版本统一提升到 `v0.5.0`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Api/MetadataSyncPageDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj` 通过（56/56）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（59/59）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（276/276）。
4. DoD 判定：否。阶段 5 当前仅完成首批元数据同步闭环，`sync download / retry / export / report` 与完整统计展示尚未落地。
5. 下次计划：继续实现同步下载、失败重试、导出报表与更完整的 Sync 页面交互。

### 1.5.86 2026-04-03，阶段 5 第二批：同步下载与容量控制

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncDownloadService`、`SyncService` 编排层、`WorkSyncInfo` SQLite 状态回写、`SyncWantedSize` 容量阈值控制与 Sync 页面“开始同步下载”入口。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadSnapshot.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadPathPolicy.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncSizeText.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（61/61）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（57/57）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（279/279）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步与同步下载容量控制两批，失败重试、导出和报表仍未落地。
5. 下次计划：继续实现失败重试、状态导出、统计报表以及更完整的 Sync 页面状态反馈。

### 1.5.87 2026-04-03，阶段 5 第三批：失败重试

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncRetryRunResult`、失败记录查询与 `SyncDownloadService.RetryFailedAsync`，并在 Sync 页面补充“重试失败项”入口与执行摘要展示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncRetryRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（63/63）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（58/58）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（282/282）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步、同步下载容量控制与失败重试三批，导出和报表仍未落地。
5. 下次计划：继续实现状态导出、统计报表以及更完整的 Sync 页面状态反馈。

### 1.5.88 2026-04-03，阶段 5 第四批：失败/成功记录导出

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncExportService`、`SyncExportStatus/SyncExportResult`、`WorkSyncInfo` 按状态查询导出链路，并在 Sync 页面补充“导出失败记录”“导出成功记录”入口与保存对话框流程。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncExportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMetadataSyncStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncExportStatus.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncExportResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncExportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/MetadataSyncStore.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncExportServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（65/65）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（59/59）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（285/285）。
4. DoD 判定：否。阶段 5 当前已完成元数据同步、同步下载容量控制、失败重试与状态导出四批，统计报表与更完整的 Sync 统计展示仍未落地。
5. 下次计划：继续实现 Sync 统计报表与页面汇总展示，并补齐对应手工回归证据。

### 1.5.89 2026-04-03，阶段 5 第五批：统计报表与页面数据面板

1. 变更摘要：继续按第 4.6 节推进阶段 5，新增 `SyncReportService/SyncReportSnapshot` 统计快照，按 Go `sync report` 口径汇总元数据总量、字幕拆分、完成/失败/待处理数量与同步进度，并在 Sync 页面新增统计卡片与数据面板展示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncReportSnapshot.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncReportService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncReportServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（67/67）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（147/147）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（287/287）。
4. DoD 判定：是。阶段 5 五批能力已全部落地，`sync / sync download / retry / export / report` 对应的 WPF 页面能力与测试证据已闭环。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.5.90 2026-04-03，阶段 5 增强：同步进度持久化与温和停止

1. 变更摘要：在既有阶段 5 同步链路上新增 SQLite `UiState` 元数据同步/同步下载进度记录，支持未完成状态下的断点继续、已完成状态下的重置起跑语义，并在 Sync 页面补充“停止同步元数据”“停止同步下载”按钮与按钮可用性控制。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncDownloadProgressState.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/SyncProgressStatuses.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IUiStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/UiStateStore.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（63/63）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（71/71）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（150/150）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（298/298）。
4. DoD 判定：是。阶段 5 在原有同步、重试、导出与报表能力之上，已补齐进度持久化、断点继续、温和停止与 0.5.1 版本口径同步。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.5.91 2026-04-03，阶段 5 交互收口：合并同步按钮并支持运行中刷新统计

1. 变更摘要：在既有阶段 5 增强基础上，进一步将 Sync 页“开始同步元数据 / 停止同步元数据”和“开始同步下载 / 停止同步下载”各自合并为单一主按钮，并保持“刷新统计”在同步运行期间仍可手动触发，以实时查看当前报表和 UiState 进度；同时用非时间型防重入保护合并按钮，避免双击后直接误触 stop request。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（153/153）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（71/71）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（301/301）。
4. DoD 判定：是。Sync 页已收敛为两枚同步主按钮，运行中可手动刷新统计，且 stop-request 与断点继续语义保持不变。
5. 下次计划：进入阶段 6（资源库与播放能力迁移），先梳理本地资源扫描、目录索引与最小浏览页骨架。

### 1.5.92 2026-04-03，阶段 5 缺陷修复：合并按钮 1 秒防抖窗口

1. 变更摘要：修复 Sync 页合并后的同步主按钮在“开始后快速双击”场景下的顺序型防抖失效问题；将开始后的第二次快速点击改为在 1 秒窗口内直接忽略，避免误发 stop request，并同步将相同防抖策略复用于同步下载主按钮。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncActionDebouncePolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncActionDebouncePolicyTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（306/306）。
4. DoD 判定：是。合并后的元数据同步 / 同步下载主按钮均具备 1 秒启动防抖窗口，快速双击不会再错误切入 stopping 态，且超过窗口后的显式停止语义保持不变。
5. 下次计划：保持当前 v0.5.1 待提交记录，待用户确认后再创建提交并继续阶段 6（资源库与播放能力迁移）规划。

### 1.5.93 2026-04-07，v0.5.1：下载/同步目录拆分、元数据保鲜与 completed-state 重扫

1. 变更摘要：拆分 Settings 中的普通下载目录与同步下载目录配置，新增“元数据有效期(天)”；新增 `MetadataWork` 优先解析链路，Search/Download 未开启翻译时先命中本地元数据、过期再补拉 API 并回写摘要；同步元数据在 completed-state 下按有效期触发过期刷新；同步下载在 completed-state 下从头校验全部作品，并复用普通下载/同步下载双目录中的已存在文件；同时统一 Search/Download/Sync 三页状态信息面板样式。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ConfigurationService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/DatabaseInitializer.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ConfigurationServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/DatabaseInitializerTests.cs`。
3. 验证结果：`rtk dotnet test tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（74/74）；`rtk dotnet test tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（63/63）；`rtk dotnet test tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test Asmroner.sln --nologo` 通过（309/309）。
4. DoD 判定：是。用户本轮要求的 Settings 配置拆分、`MetadataWork` 优先解析、completed-state 过期刷新/全量重扫、双目录复用、状态面板统一与回归测试均已落地。
5. 下次计划：由用户执行章节 4.1/4.2/4.3/4.4/4.5 受影响项手工回归，重点验证 Settings 新字段、Search/Download 元数据保鲜与双目录复用、以及 Sync completed-state 刷新/校验；章节 1.2/1.3/1.4 状态维持不变，章节 2.1 基线更新为 2026-04-07 的 `309/309` 并补充 2.1.1/2.1.11/2.1.13/2.1.17/2.1.19/2.1.20/2.1.61/2.1.62/2.1.63，章节 3.1 新增单一 `v0.5.1` 待提交记录，章节 4.1/4.2/4.3/4.4/4.5 受影响项已重置为未勾选。

### 1.5.94 2026-04-08，v0.5.2：真实媒体下载、修复 Sync 错误数据与双目录实时补齐

1. 变更摘要：运行时版本升级到 `v0.5.2`；为 `IAsmrApiClient` / `AsmrApiClient` 新增 `mediaDownloadUrl` 直链流式下载能力；`DownloadService` 不再写入 `source/title/url` 占位文本，改为优先复用双目录中的真实文件、识别并忽略旧占位文件、在双目录都缺失时直接实时下载真实媒体；`SyncDownloadService` 为同步下载与失败重试显式透传数值 `WorkId`，确保 Sync 页面复用普通下载链路时落地的也是正确媒体文件。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IAsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadStartOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiClient.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/CachedAsmrApiClient.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（76/76）；`rtk dotnet test tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（64/64）；`rtk dotnet test tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test Asmroner.sln --nologo` 通过（312/312）。
4. DoD 判定：是。用户本轮要求的 `v0.5.2` 版本升级、普通下载真实媒体落盘、Sync 错误数据修复、WorkId 透传、单测补齐与文档同步均已完成。
5. 下次计划：由用户执行章节 4.1/4.3/4.5 的受影响手工回归，重点验证 Settings 版本文案显示 `v0.5.2`、普通下载在双目录缺失时直接实时下载、普通下载对同步目录真实文件的复用，以及 Sync 页面新下载结果不再出现旧的占位文本。

### 1.5.95 2026-04-08，v0.5.2：统一 Search/Download/Sync 的作品 info DTO 边界

1. 变更摘要：新增 `ISyncWorkInfoResolver` / `SyncWorkInfoResolver`，让 Sync 侧与 Search/Download 一样，在应用层统一使用 `WorkInfoDto` 作为作品 info DTO；`SyncDownloadService` 不再直接把 `MetadataWorkItem` 作为跨页作品信息载体，而是在进入同步下载流程前统一映射为 `WorkInfoDto`，仅在写回 SQLite `WorkSyncInfo` 的 pending 记录时再转换回持久化模型；页面行模型与 SQLite 存储模型继续保持分层，不做强制合并。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ISyncWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncWorkInfoResolver.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（313/313）。
4. DoD 判定：是。Search/Download/Sync 三页的作品 info 解析边界已统一到共享 `WorkInfoDto`，并保留 `SearchWorkItem`、`DownloadTaskItem`、`MetadataWorkItem`、`WorkSyncInfoItem` 作为页面/任务/存储模型，不再把 Sync 元数据存储实体直接当作跨页作品 info DTO 使用。
5. 下次计划：由用户执行章节 4.4/4.5 的受影响手工回归，重点验证 Search 入队、Download 标题刷新与 Sync 创建待处理记录时的标题、`SourceId` 与字幕标记在共享 `WorkInfoDto` 之后仍保持一致；章节 2.1 基线更新为 2026-04-08 的 `313/313` 并补充 2.1.63 新样例，章节 3.1 继续保持单一 `v0.5.2` 待提交记录，章节 4.4 新增 DTO 一致性专项回归项并保持未勾选。

### 1.5.96 2026-04-08，v0.5.3：增强站点发现、同步实时统计与 Settings 连接逻辑

1. 变更摘要：运行时版本升级到 `v0.5.3`；`EndpointDiscoveryService` 对发布页 HTML/入口脚本抓取改为非致命容错，放宽入口脚本标签匹配并补充浏览器 `User-Agent`；`MetadataSyncService` 在 UiState 中持续写入当前本地总量/字幕量，`SyncView` 在元数据同步运行中按秒刷新顶部摘要与状态文本；`SettingsView` 提取保存/测试连接共用配置动作流程，保持“先保存再测试”语义不变；同时为四个 `View.xaml.cs` 的直接控件事件处理方法补齐 `///` 注释。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（66/66）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（315/315）。
4. DoD 判定：是。用户本轮要求的 `v0.5.3` 版本升级、两项 bug 修复、“测试连接”逻辑优化、四个 `View.xaml.cs` 事件注释、单测与文档同步均已落地；受影响的手工功能项已在章节 4 中重置，待用户执行回归。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证测试连接在发布源 HTML/脚本抓取失败时仍可回退到可用 BaseUrl、Settings 页面版本文案显示 `v0.5.3`，以及 Sync 页面运行中本地元数据数量实时增长且完成后摘要一致；章节 2.1 基线更新为 2026-04-08 的 `315/315`，并更新 `EndpointDiscoveryServiceTests` 与 `MetadataSyncServiceTests` 记录；章节 3.1 新增单一 `v0.5.3` 待提交记录，章节 4.1/4.5 受影响项已重置为未勾选。

### 1.5.97 2026-04-08，v0.5.3：收敛 Sync 手动刷新口径并补强入口脚本解析

1. 变更摘要：在保留 `MetadataSyncService` 本地总量/字幕量持久化修复的前提下，移除 `SyncView` 元数据同步运行中的 `DispatcherTimer` 自动刷新，改回仅通过“刷新统计”读取最新持久化进度；`EndpointDiscoveryService` 改为解析更宽松的入口 script `src` 形态，兼容相对路径、查询串与单引号 `link` 配置；同步补强 `UiStateStore` 元数据进度 round-trip 断言。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（67/67）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（316/316）。
4. DoD 判定：是。Endpoint 发布源入口脚本抓取已按更宽松的真实页面形态解析；Sync 页不再在运行中自动轮询统计，但“刷新统计”和完成态摘要均可读取到正确的本地总量/字幕量；受影响手工项已在章节 4 重置为未勾选。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证测试连接对相对路径/查询串入口脚本的解析，以及 Sync 页面在同步运行中只有点击“刷新统计”后才更新顶部摘要与状态文本；章节 2.1 基线更新为 2026-04-08 的 `316/316`，并补充 `EndpointDiscoveryServiceTests` 新样例与 `UiStateStoreTests` 的进度字段说明。

### 1.5.98 2026-04-08，v0.5.3：修正 Endpoint 探测端点并补齐元数据同步处理口径

1. 变更摘要：`EndpointDiscoveryService` 的候选延迟探测不再对 `GET /api/recommender/popular` 做健康检查，而改为使用支持 `GET` 的 works 查询端点，避免 Go 版本 API 因 404 被误判为不可用；发布源 HTML/脚本抓取仍保持对超时与 `HttpRequestException` 的非致命回退。`MetadataSyncService` 在保留“新增数”语义的前提下，新增“累计处理条数”并持续写入 `UiState`，`SyncView` 的“刷新统计”和完成态详情同步显示累计处理、累计新增与当前本地总量/字幕量，降低“分页推进但像没写入”的误判风险；同时补齐基础设施与应用层回归样例。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/MetadataSyncService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncRunResult.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Sync/MetadataSyncProgressState.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/MetadataSyncStoreTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/UiStateStoreTests.cs`、`dotnet/tests/Asmroner.Application.Tests/MetadataSyncServiceTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（70/70）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo` 通过（320/320）。
4. DoD 判定：是。启动/“测试连接”链路不再因错误的 GET 探测端点把健康候选误判为 404；MetadataSync 的完成态、停止态和“刷新统计”已统一显示累计处理、累计新增与当前本地总量，相关测试与文档同步完成；章节 4 的受影响手工项继续保留未勾选，待用户执行回归。
5. 下次计划：由用户执行章节 4.1/4.5 的受影响手工回归，重点验证“测试连接”在候选 API 对 `GET /api/recommender/popular` 返回 404 时仍可通过 works 探测选中可用 BaseUrl，以及 Sync 页面在前几页仅更新已有记录时，点击“刷新统计”后能同时看到累计处理、累计新增与当前本地总量；章节 2.1 基线更新为 2026-04-08 的 `320/320`，章节 3.1 继续维持单一 `v0.5.3` 待提交记录。

### 1.5.99 2026-04-10，v0.5.4：health 探测、发布页正文域名抓取与候选列表持久化

1. 变更摘要：运行时版本升级到 `v0.5.4`；默认候选 API 地址补齐 `https://api.asmr-300.com`、`https://api.asmr-200.com`、`https://api.asmr-100.com` 与 `https://api.asmr.one`；`EndpointDiscoveryService` 的候选探测切换到 `GET /api/health?cache=false`，并优先从发布页正文直接提取最新站点域名，仍保留入口脚本解析兼容；`AsmrApiOptionsProvider` 会把旧版内置候选子集扩展为完整默认集合并保持当前 BaseUrl 优先；`ApiEndpointUrlService` 发现成功后会把 `ApiUrl` 与 `ApiCandidateUrls` 一并写回 SQLite，同时保留自定义 endpoint 配置边界；`SettingsView` 在“测试连接”后会刷新内部候选列表缓存，避免后续保存把新候选覆盖回旧值。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Configuration/DownloaderOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Api/AsmrApiPaths.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/AsmrApiOptionsProvider.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/config.json`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiOptionsProviderTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、`README.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（74/74）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（324/324）。
4. DoD 判定：是。用户本轮要求的 `v0.5.4` 版本升级、health API 探测、`apiCandidateUrls` 新增 `api.asmr-200.com` / `api.asmr-100.com`、发布页正文最新域名抓取、SQLite 候选列表写回、单测补齐与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1 的受影响手工回归，重点验证启动/“测试连接”改走 `GET /api/health?cache=false`、发布页正文中的最新域名会写回 SQLite `ApiCandidateUrls` 且后续“保存并重新初始化”不会覆盖回旧值，以及 Settings 页面版本文案显示 `v0.5.4`；章节 1.2/1.3 状态维持不变，章节 1.4 新增一条已解决站点发现/SQLite 持久化问题，章节 2.1 基线更新为 2026-04-10 的 `324/324` 并补充 `EndpointDiscoveryServiceTests`、`AsmrApiOptionsProviderTests`、`ApiEndpointUrlServiceTests` 与 `AppVersionInfoTests` 的口径，章节 3.1 合并为单一 `v0.5.4` 待提交记录，章节 4.1 受影响项已重置为未勾选。

### 1.5.100 2026-04-11，v0.5.5：版本升级与匿名探测客户端收敛

1. 变更摘要：运行时版本升级到 `v0.5.5`；保留 `AsmrProbe` 作为匿名站点探测客户端，并新增 `EndpointDiscoveryHttpTransport` 统一收敛客户端名称、HTTP/1.1 传输约束与默认 User-Agent；`EndpointDiscoveryService` 在单次发现流程内复用同一个 probe client，并统一发布页抓取与 health 探测请求头策略；同时清理 `AuthServiceTests` 中与登录链路无关的冗余 `AsmrProbe` 注册。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryHttpTransport.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/EndpointDiscoveryServiceTests.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AuthServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（75/75）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（78/78）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（325/325）。
4. DoD 判定：是。用户本轮要求的 `v0.5.5` 版本升级、`AsmrProbe` 可用性确认与逻辑收敛、单元测试同步、progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1 的受影响手工回归，重点验证应用启动不阻塞、“测试连接”继续可用且发布页/health 探测链路正常，以及 Settings 页面版本文案显示 `v0.5.5`；章节 2.1 基线更新为 2026-04-11 的 `325/325`，章节 3.1 新增单一 `v0.5.5` 待提交记录，章节 4.1 受影响项已重置为未勾选。

### 1.5.101 2026-04-11，v0.5.6：Track size 完整度校验与重下补强

1. 变更摘要：运行时版本升级到 `v0.5.6`；`TrackDto` 新增 `Size` 字段并接收 track API 的 `size` 元数据；`DownloadService` 在普通下载目录、同步下载目录候选文件与真实下载落盘后三个阶段统一引入基于 track size 的完整度判断，仅在文件大小匹配时复用已有文件，大小不一致时忽略旧文件并重新下载；同时保留 legacy placeholder 与空文件防护逻辑。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Api/TrackDto.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/AsmrApiClientTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（75/75）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo` 通过（82/82）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（158/158）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（329/329）。
4. DoD 判定：是。用户本轮要求的 `v0.5.6` 版本升级、track `size` 接入、已有文件大小不一致时重新下载、单元测试同步与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.6`、普通下载/同步下载目录中仅在文件大小匹配时复用已有文件、大小不一致时触发重下，以及 Sync completed-state 重扫只补齐目录缺失或文件大小不一致的作品；章节 2.1 基线更新为 2026-04-11 的 `329/329`，章节 3.1 新增单一 `v0.5.6` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.5.102 2026-04-12，v0.5.7：普通下载同步镜像、同目录防重与 Sync 完成态护栏

1. 变更摘要：运行时与 README 版本升级到 `v0.5.7`；`DownloadStartOptions` 新增 `DownloadExecutionPurpose`，普通下载默认走 `Standard`，Sync 页面触发的下载显式走 `SyncManaged`；`DownloadService` 在普通下载目录与同步下载目录相同的场景下避免对同一路径做重复复制/重复处理；当普通下载是完整下载且普通目录/同步目录都缺少目标文件时，会在普通目录落盘真实媒体的同时镜像一份到同步下载目录，并将 SQLite `WorkSyncInfo` 直接写为 `COMPLETED`；若普通下载属于子集下载（如 `fileFilter`、`hdAudioOnly`），则只镜像实际下载的文件，不再误写 `COMPLETED`，避免污染后续 Sync 增量判定。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Download/DownloadStartOptions.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/DownloadService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/SyncDownloadService.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTests.cs`、`dotnet/tests/Asmroner.Application.Tests/DownloadServiceTestDoubles.cs`、`dotnet/tests/Asmroner.Application.Tests/SyncDownloadServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj` 通过（84/84）；`rtk dotnet test dotnet/Asmroner.sln` 通过（331/331）。
4. DoD 判定：是。用户本轮要求的 `v0.5.7` 版本对齐、普通下载/同步目录同路径防重、完整普通下载同步镜像与 SQLite `COMPLETED` 回写、SyncManaged 路径隔离、子集下载完成态护栏、回归测试与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.7`、完整普通下载在双目录都为空时会同时落普通目录和同步目录并写 SQLite `COMPLETED`、下载目录与同步目录同路径时不会重复处理，以及子集普通下载不会误写 `COMPLETED`；章节 2.1 基线更新为 2026-04-12 的 `331/331`，章节 3.1 保持单一 `v0.5.7` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.5.103 2026-04-14，v0.5.8 下载/同步状态修复

1. 变更摘要：运行时与 README 版本升级到 `v0.5.8`；Download 页新增 `DownloadToolbarAvailability`，纠正“立即下载选中任务/执行下载队列”期间顶部按钮分组，使“打开下载目录”保持可点击，并让“从文件导入”与其它入队入口保持一致的禁用/恢复行为；Sync 页拆分重试与导出状态，允许“重试失败项”期间继续“刷新统计”，并把状态面板拆为“同步状态 / 下载状态”两行显示。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/DownloadToolbarAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncCommandAvailability.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/SyncStatusTextBuilder.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/DownloadToolbarAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncCommandAvailabilityTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncStatusTextBuilderTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/SyncViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo --no-restore` 通过（165/165）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo --no-restore` 通过（84/84）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（338/338）。
4. DoD 判定：是。用户本轮要求的 `v0.5.8` 版本对齐、Download 两处按钮状态修复、Sync 页面“重试失败项”期间刷新统计恢复可用、Sync 状态文本拆分为两行、单元测试补充与 progress 文档同步均已落地。
5. 下次计划：由用户执行章节 4.1、4.3、4.4 与 4.5 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.8`、Download 页在“立即下载选中任务/执行下载队列”期间“打开下载目录”仍可点击且“从文件导入”与其它入队入口行为一致、Sync 页在“重试失败项”期间仍可手动“刷新统计”，以及状态面板按“同步状态 / 下载状态”两行分别显示；章节 1.2/1.3 已检查且无需调整，章节 2.1 基线更新为 2026-04-14 的 `338/338`，章节 3.1 新增单一 `v0.5.8` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.5.104 2026-04-14，v0.5.9：发布页抓取链路收敛与候选集合增量持久化

1. 变更摘要：运行时与 README 版本升级到 `v0.5.9`；`EndpointDiscoveryService` 不再直接创建临时 `HttpClient` 抓取发布页，而是通过 `EndpointDiscoveryHttpTransport` + `IHttpClientFactory` 新增发布页客户端，统一收敛 HTML/脚本抓取的 HTTPS 处理、HTTP/1.1 约束、请求头、超时、取消与可测试性；同时保留“正文优先提取最新域名、入口脚本回退解析”的发现链路。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryHttpTransport.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/EndpointDiscoveryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Infrastructure/Services/ApiEndpointUrlService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/InfrastructureTestDoubles.cs`、`dotnet/tests/Asmroner.Infrastructure.Tests/ApiEndpointUrlServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --nologo` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo` 通过（168/168）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（343/343）。
4. DoD 判定：是。用户本轮要求的 `v0.5.9` 版本对齐、发布页抓取链路 review/优化、发现成功后按“新旧候选合并且总数增加”规则写回 SQLite、单元测试补齐与 progress 文档同步均已落地；当前 Windows 10 机器无法访问目标 URL 的实网限制已按要求排除在本批处理范围外。
5. 下次计划：由用户在 Windows 11 环境执行章节 4.1 的受影响手工回归，重点验证 Settings 页面版本文案显示 `v0.5.9`、“测试连接”在发布页正文/入口脚本两条路径下均可发现最新域名，以及当发现结果与已保存候选集合合并后数量增加时，SQLite `ApiCandidateUrls` 会保留旧候选并写回合并后的集合；章节 1.2/1.3 已检查且无需调整，章节 2.1 基线更新为 2026-04-14 的 `343/343`，章节 3.1 新增单一 `v0.5.9` 待提交记录，章节 4 的受影响项已重置为未勾选。

### 1.5.105 2026-04-15，v0.6.0：阶段 6 首批资源库扫描、Library 页签与播放上下文抽象

1. 变更摘要：运行时、README 与 progress 文档版本升级到 `v0.6.0`；启动阶段 6 首批迁移，新增资源库目录双格式解析、Library 扫描/查询服务、播放上下文抽象服务，并在主窗口接入 `Library` 页签与资源库页面骨架。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Library/LibraryModels.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ILibraryScannerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/ILibraryQueryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryQueryService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、新增阶段 6 测试文件、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj --nologo --no-restore` 通过（14/14）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --nologo --no-restore` 通过（90/90）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --nologo --no-restore` 通过（171/171）；`rtk dotnet test dotnet/Asmroner.sln --nologo --no-restore` 通过（355/355）。
4. DoD 判定：否。阶段 6 当前仅完成首批资源扫描/索引、列表/筛选与播放上下文装载骨架；真实播放器接入、播放控制与更完整的异常场景验证仍待后续批次完成。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证主窗口新增 `Library` 页签、Settings 版本文案 `v0.6.0`、双格式目录扫描、筛选/分页、文件树与上下文装载提示；随后继续阶段 6 下一批真实播放器接入与异常处理补强。

### 1.5.107 2026-04-15，v0.6.1：阶段 6 第二批最小真实播放与 Library 播放控制

1. 变更摘要：运行时、README 与 progress 文档版本升级到 `v0.6.1`；继续阶段 6 第二批迁移，新增底层播放引擎抽象与基于 WPF `MediaPlayer` 的最小播放实现，升级 `PlayerService` 为真实播放编排，并在 `Library` 页接入播放/暂停/停止与进度显示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlaybackEngine.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackFailedEventArgs.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/WpfMediaPlaybackEngine.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/PlayerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（95/95）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（171/171）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（361/361）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与最小真实播放，但更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.1`、Library 页的载入/切换文件、播放/暂停/停止、时间进度显示，以及缺失文件/不支持格式时的失败提示；随后继续阶段 6 的异常处理与播放体验补强。

### 1.5.108 2026-04-15，v0.6.1：阶段 6 从内嵌播放切换到系统默认程序打开

1. 变更摘要：移除基于 WPF `MediaPlayer` 的内嵌播放链路，保留 `Library` 页的文件上下文与“播放”入口，但改为仅对显式选中的可播放媒体文件调用系统默认关联程序打开；同时删除“暂停 / 停止”按钮与时间进度显示。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IMediaLauncher.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Interfaces/IPlayerService.cs`、`dotnet/Asmroner.Backend/Asmroner.Core/Playback/PlaybackContext.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/PlayerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellMediaLauncher.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryPlaybackSelectionPolicy.cs`、`dotnet/tests/Asmroner.Application.Tests/PlayerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryPlaybackSelectionPolicyTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（96/96）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（172/172）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（363/363）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与显式选中文件后的系统默认程序打开，但更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证显式选中文件后的“载入/切换文件”与“播放”交互、系统默认程序打开行为、失败提示，以及界面不再出现“暂停 / 停止 / 时间进度”相关控件或文案；随后继续阶段 6 的异常处理与体验补强。

### 1.5.109 2026-04-15，v0.6.1：修复 Shell 成功打开却误报失败

1. 变更摘要：修复 `Library` 页系统默认程序打开链路中对 Windows Shell 返回值的错误判定；当 `Process.Start` 已成功触发关联程序但返回 `null` 时，不再误报“打开失败”；同时为 `ShellMediaLauncher` 增加专项单元测试覆盖 `null` 返回与异常抛出路径。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/ShellMediaLauncher.cs`、`dotnet/tests/Asmroner.Wpf.Tests/ShellMediaLauncherTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（96/96）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（174/174）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（365/365）。
4. DoD 判定：否。阶段 6 当前已完成资源库扫描/索引、列表/筛选与显式选中文件后的系统默认程序打开，并修复成功打开误报失败问题；更完整的异常/格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证系统默认程序已成功拉起但底层 Shell 调用返回 `null` 时不再误报失败，以及真实打开失败时仍显示明确错误提示；随后继续阶段 6 的异常处理与体验补强。

### 1.5.110 2026-04-16，v0.6.2：补强 Library 异常扫描容错与选择反馈

1. 变更摘要：继续推进阶段 6，在保留系统默认程序打开方案的前提下补强资源库链路；`LibraryScannerService` 新增可测试的文件系统枚举 seam，并将嵌套目录/文件读取失败收敛为局部错误记录，避免单个异常目录拖垮整项扫描；`LibraryView` 新增显式选择反馈策略，对目录、不可播放文件、缺失媒体文件与可播放文件分别给出明确提示，并收紧“载入/切换文件”“播放”按钮可用性；同步将运行时/UI/README 版本对齐到 `v0.6.2`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibrarySelectionFeedbackPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryScannerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibrarySelectionFeedbackPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（98/98）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（178/178）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（371/371）。
4. DoD 判定：否。当前已完成资源库扫描/索引、列表/筛选、显式选中文件后的系统默认程序打开，以及异常扫描容错与选择反馈补强；更完整的格式兼容验证与阶段 6 完整 DoD 仍待后续批次完成。
5. 下次计划：由用户执行章节 4.6 的受影响手工回归，重点验证目录/不可播放文件/缺失媒体文件的明确提示、按钮禁用状态，以及可播放文件仍能完成载入与系统默认程序打开；随后继续阶段 6 的格式兼容与体验收口。

### 1.5.111 2026-04-16，v0.6.3：收口 Library 可播放格式规则与作品级选择引导

1. 变更摘要：继续推进阶段 6，在保持系统默认程序打开方案不变的前提下，把可播放扩展名判断抽到共享 `LibraryPlayableMediaPolicy`，统一扫描层与 UI 侧的格式规则；`LibraryView` / `LibrarySelectionFeedbackPolicy` 在仅选中作品、尚未选中文件时新增作品级引导，可提示当前作品是否包含可播放媒体文件、首个候选路径，以及无可播放文件时的支持格式说明；同步将运行时/UI/README 版本对齐到 `v0.6.3`。
2. 关键文件：`dotnet/Asmroner.Backend/Asmroner.Core/Library/LibraryPlayableMediaPolicy.cs`、`dotnet/Asmroner.Backend/Asmroner.Application/Services/LibraryScannerService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibrarySelectionFeedbackPolicy.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Core.Tests/LibraryPlayableMediaPolicyTests.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryScannerServiceTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibrarySelectionFeedbackPolicyTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Core.Tests/Asmroner.Core.Tests.csproj --no-restore` 通过（19/19）；`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（99/99）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（180/180）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（378/378）。
4. DoD 判定：否。当前已补齐共享可播放格式规则与作品级选择引导，阶段 6 在代码与自动化测试层面已进一步收口；但章节 4.1 / 4.6 的受影响手工项已按规则重置为未勾选，完整 DoD 证据仍待用户手工回归确认。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.3`、仅选中作品时的首个候选提示、无可播放文件时的支持格式说明，以及大小写混合支持扩展名文件仍可被识别并完成载入与系统默认程序打开；随后视人工验证结果决定是否关闭阶段 6。

### 1.5.112 2026-04-16，v0.6.3：收口 Library 播放上下文表达与 DoD 闭环准备

1. 变更摘要：继续推进阶段 6，在保持 `v0.6.3` 与当前 shell-open 方案不变的前提下，新增 `LibraryPlaybackContextTextBuilder`，把 Library 右侧的“当前选择”与“当前已载入上下文”拆开显示；`LibraryView` 保留“载入/切换文件”“播放”“清空上下文”三按钮，但上下文区不再混写当前选择反馈与已载入状态，便于人工验证清空上下文、重新载入和系统默认程序打开的真实状态。
2. 关键文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryPlaybackContextTextBuilder.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryPlaybackContextTextBuilderTests.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`docs/wpf-migration-progress.md`。
3. 验证结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（99/99）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（183/183）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（381/381）。
4. DoD 判定：否。当前已补齐播放上下文表达与自动化证据，阶段 6 在代码层面进一步收口；但章节 4.6 的受影响 Library 手工项仍待用户执行，阶段 6 DoD 仍不能由 AI 单方面闭环。
5. 下次计划：由用户执行章节 4.1 与 4.6 的受影响手工回归，重点验证 Settings 版本文案 `v0.6.3`、“当前选择 / 当前已载入上下文”分离展示、清空上下文后的保留选择提示、以及显式选中文件后的系统默认程序打开；若人工验证通过，再回写阶段 6 完成状态。

### 1.5.113 2026-04-16，v0.6.3：确认阶段 6 手工回归通过并关闭 DoD

1. 变更摘要：用户已完成并确认章节 4.1 与 4.6 的受影响手工回归通过，本次据此回写阶段 6 完成状态；阶段总览切换为“已完成”，阶段执行勾选中的“阶段 6 DoD 已满足并记录证据”同步闭环，并将待提交行整理为包含共享格式规则、作品级引导、播放上下文分离展示与手工回归确认的单一 v0.6.3 提交说明。
2. 关键文件：`docs/wpf-migration-progress.md`。
3. 验证结果：用户已明确确认章节 4.1 / 4.6 的受影响手工回归通过；最近一次自动化回归基线保持为 `rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（381/381）。
4. DoD 判定：是。阶段 6 的代码、自动化测试与用户手工回归证据已闭环。
5. 下次计划：进入阶段 7（UI 集成与体验收口）的方案制定与实施准备。

---

### 1.5.114 2026-04-16，v0.6.4：补齐 Library 分页并修正资源库页缩放布局

1. 本次变更摘要：Library 页面补齐跳页与 `page size` 控件，并将作品列表、详情区、文件树与上下文区改为受限高度 + 内部滚动布局，修复选中作品后右侧内容越界、窗口放大后左右下边沿错位、窗口缩小时内容溢出的三个界面问题。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml.cs`、`dotnet/tests/Asmroner.Wpf.Tests/LibraryViewXamlTests.cs`、`dotnet/tests/Asmroner.Application.Tests/LibraryQueryServiceTests.cs`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（100/100）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（184/184）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（383/383）。
4. DoD 判定：是。阶段 6 状态保持“已完成”，本次作为阶段 6 后续维护收口；自动化回归通过，手工验证项已重置为待验证。

### 1.5.115 2026-04-18，v0.7.0：启动阶段 7 壳层集成与共享交互收口

1. 本次变更摘要：运行时与 README 版本统一升级到 `v0.7.0`；新增 `ShellViewModel`、`NavigationService`、`UiMessageService`、`DialogService` 与 `ShellResources.xaml`，将主窗口页签启用状态、选中页签与底部状态文案收敛到壳层状态；同时把 Search/Download/Sync 的文件对话框入口统一到共享服务，并让 Search/Download/Library/Settings 四页卡片/输入/按钮样式改为基于共享壳层资源。
2. 关键修改文件：`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/App.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/MainWindow.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Styles/ShellResources.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/ViewModels/ShellViewModel.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/NavigationService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/UiMessageService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Services/DialogService.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SearchView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/DownloadView.xaml(.cs)`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SyncView.xaml.cs`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/SettingsView.xaml`、`dotnet/Asmroner.Wpf/Asmroner.Wpf/Views/LibraryView.xaml`、`dotnet/tests/Asmroner.Wpf.Tests/*`、四个运行时 `.csproj`、`README.md`、`docs/wpf-migration-progress.md`。
3. 构建与测试结果：`rtk dotnet test dotnet/tests/Asmroner.Application.Tests/Asmroner.Application.Tests.csproj --no-restore` 通过（100/100）；`rtk dotnet test dotnet/tests/Asmroner.Infrastructure.Tests/Asmroner.Infrastructure.Tests.csproj --no-restore` 通过（77/77）；`rtk dotnet test dotnet/tests/Asmroner.Wpf.Tests/Asmroner.Wpf.Tests.csproj --no-restore` 通过（198/198）；`rtk dotnet test dotnet/Asmroner.sln --no-restore` 通过（397/397）。
4. DoD 判定：否。阶段 7 已完成壳层优先的第一批基线收口与自动化验证，但加载态/空态统一、UI 冒烟与用户手工回归尚未闭环。
5. 下次计划：继续补齐阶段 7 的加载态/空态统一与剩余壳层体验收口；由用户执行章节 4.1/4.2/4.3/4.4/4.5/4.7 的受影响手工回归，重点验证 `v0.7.0` 版本文案、Settings 保持页签、共享状态栏消息、统一文件对话框行为与共享样式在多窗口尺寸下的一致性。

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
- `已通过`：样例在最近一次可执行验证中通过；当前全量回归基线为 2026-04-18 的解决方案级回归（397/397）。

#### 2.1.1 Application.Tests / DownloadServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                                   | 输入                                                   | 期望输出                                                                                                |
| ------ | ------ | ------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------ | ------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4  | `DirectoryNameStrategy_ShouldMatchGoCompatibilityRule`                                                   | 含非法字符标题 + SourceId                              | 目录名格式为 `[{SourceId}]{Title}`                                                                      |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldCreateCompletedTaskAndFiles`                                                       | 入队 1 个 RJID 并执行                                  | 任务 `Completed`，文件落地成功，队列清空                                                                |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldMarkTaskFailed_WhenApiThrows`                                                      | WorkInfo API 抛异常                                    | 任务状态 `Failed` 且含错误信息                                                                          |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldCancelRunningTask`                                                                    | 长任务执行中调用 `CancelAsync`                         | 目标任务最终状态为 `Canceled`                                                                           |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldCancelQueuedTask_BeforeWorkerStarts`                                                  | 单 worker 场景下取消排队任务                           | 排队任务状态更新为 `Canceled`                                                                           |
| [x]    | [x]    | 阶段 4  | `RetryFailedAsync_ShouldRetryAndCompleteTask`                                                            | 首次失败后重试                                         | 重试计数 +1，任务转为 `Completed`                                                                       |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldRespectConfiguredMaxWorkers`                                                       | 入队 4 条、`MaxWorkers=2`                              | 最大观测并发不超过且达到 2                                                                              |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldTrackFailedTask_WhenNewTaskFails`                                                      | 新任务 `StartAsync` 失败                               | 失败任务仍可在 `GetTasks()` 中追踪                                                                      |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldReuseFailedTask_WhenPreferredTaskProvided`                                             | 选中失败任务立即下载（带 `preferredTaskId`）           | 复用原 `TaskId` 行并重启，不新增任务行                                                                  |
| [x]    | [x]    | 阶段 4  | `StartAsync_ShouldReuseCanceledTask_WhenPreferredTaskProvided`                                           | 选中已取消任务立即下载（带 `preferredTaskId`）         | 复用原 `TaskId` 行并重启，不新增任务行                                                                  |
| [x]    | [x]    | 阶段 4  | `UpsertPrefetchedWorkInfo_ShouldExposeSnapshot_ForCrossViewTitleReuse`                                   | Search 侧写入预取 WorkInfo 后读取快照                  | 快照可读且包含对应标题映射                                                                              |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldDownloadAllFormats_WhenPreferFormatsEmpty`                                         | `PreferFormats` 置空且存在多种扩展名轨道               | 不限扩展名，全部下载                                                                                    |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldUsePrefetchedWorkInfo_WithoutApiWorkInfoCall`                                      | 预先写入内存 WorkInfo 且 API 禁止 WorkInfo 调用        | 下载成功且不触发 WorkInfo API                                                                           |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldFetchWorkInfo_WhenOnlySummaryCacheExists`                                          | 仅命中摘要缓存且允许 API 继续补拉完整详情              | 下载成功，且补拉一次 Full WorkInfo                                                                      |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldReuseCanceledTask_WhenSameSourceRequeued`                                          | 已取消任务再次由 Search 入队后执行队列                 | 复用原任务并回流为待执行/完成，不新增重复行                                                             |
| [x]    | [x]    | 阶段 4  | `CancelAsync_ShouldReturnFalse_WhenTaskDoesNotExist`                                                     | 随机 `TaskId` 调取消                                   | 返回 `false`                                                                                            |
| [x]    | [x]    | 阶段 4  | `RetryFailedAsync_ShouldNotRetry_WhenTaskIsNotFailed`                                                    | 任务状态为 `Completed/Canceled` 调重试                 | 返回空或拒绝重试                                                                                        |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldContinueOtherTasks_WhenSingleTaskFails`                                            | 批量队列中单任务失败                                   | 其他任务继续完成                                                                                        |
| [x]    | [x]    | 阶段 4+ | `ClearAllTasksAsync_ShouldStopRunningAndClearQueueAndTasks`                                              | 运行中任务 + 待下载队列混合场景                        | 运行任务被停止、任务列表清空、队列清空                                                                  |
| [x]    | [x]    | 阶段 4+ | `ClearAllTasksAsync_ShouldKeepPrefetchedWorkInfoSnapshot_UntilProcessEnds`                               | 已写入作品缓存后执行 `ClearAllTasksAsync`              | 清空任务/队列后，进程内作品快照仍可读取                                                                 |
| [x]    | [x]    | 阶段 4+ | `RunQueuedAsync_ShouldSkipTextSidecars_WhenHdAudioOnlyRemovesMp3`                                        | 同路径同名 `mp3+wav+txt/lrc/ass`                       | 保留 `wav`，移除对应 `mp3/txt/lrc/ass`                                                                  |
| [x]    | [x]    | 阶段 4+ | `RunQueuedAsync_ShouldKeepTextSidecars_WhenHdAudioOnlyIsFalse`                                           | 同路径同名 `mp3+wav+txt/lrc/ass`，hdAudioOnly=false    | 全部文件保留并下载                                                                                      |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldUsePrefetchedWorkId_ForNonRjTrackLookup`                                           | 非 `RJ` `SourceId` + 预取 `WorkId`                     | `tracks` 查询使用数值 `WorkId` 并下载成功                                                               |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldNotAppendDuplicateExtension_WhenTrackTitleAlreadyContainsExtension`                | 轨道标题已带 `.mp3`，下载扩展名仍为 `.mp3`             | 落地文件名仅保留单个 `.mp3` 后缀                                                                        |
| [x]    | [x]    | 阶段 4  | `RunQueuedAsync_ShouldAppendExtension_WhenTrackTitleDoesNotContainExtension`                             | 轨道标题无扩展名，下载扩展名为 `.wav`                  | 落地文件名自动补齐 `.wav` 后缀                                                                          |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldCopyMatchingFiles_FromSyncDirectory`                                                   | 下载目录缺失目标文件、同步下载目录已有匹配文件         | 复用同步下载目录中的真实文件到普通下载目录，不重复请求下载接口，并将 SQLite 同步记录写为 `COMPLETED`    |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldSkipDownload_WhenExistingTargetFileMatchesTrackSize`                                   | 普通下载目录已存在与 track `size` 一致的目标文件       | 跳过下载请求，直接复用现有文件                                                                          |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldRedownload_WhenExistingTargetFileSizeDiffersFromTrackSize`                             | 普通下载目录已存在与 track `size` 不一致的目标文件     | 忽略旧文件并重新下载，最终落盘文件大小与 track `size` 一致                                              |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldDownload_WhenSyncLookupFileSizeDiffersFromTrackSize`                                   | 同步下载目录存在同路径但大小不一致的候选文件           | 不复制候选文件，改为真实下载并在普通下载目录重新落盘                                                    |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldFail_WhenDownloadedFileSizeDiffersFromTrackSize`                                       | track `size` 与实际下载响应体大小不一致                | 下载失败并删除不完整目标文件                                                                            |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldDownloadRealFiles_WhenNoExistingFileInEitherDirectory`                                 | 普通下载目录与同步下载目录均无目标文件                 | 直接调用 `mediaDownloadUrl` 下载真实文件并同时镜像到同步下载目录，SQLite 写为 `COMPLETED`，不写占位文本 |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldRegisterCompletedSyncInfo_WithoutDuplicatingFiles_WhenDownloadAndSyncDirectoriesMatch` | 普通下载目录与同步下载目录相同，且两侧初始均无目标文件 | 仅落盘单份真实文件，不重复复制或处理，并将 SQLite 同步记录写为 `COMPLETED`                              |
| [x]    | [x]    | 阶段 4+ | `StartAsync_ShouldMirrorSubsetDownloadWithoutMarkingSyncCompleted_WhenFileFilterIsUsed`                  | 普通下载使用 `fileFilter` 仅下载子集轨道               | 只镜像实际下载的子集文件，但不会将 SQLite 同步记录写为 `COMPLETED`                                      |

#### 2.1.2 Application.Tests / QueryParserServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                    | 输入                                    | 期望输出                               |
| ------ | ------ | ------ | --------------------------------------------------------- | --------------------------------------- | -------------------------------------- |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldParseAdvancedQuery`                    | 复杂高级检索语句（含负向条件+分页参数） | 解析字段正确，重建 query 保留关键参数  |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldReturnReadableError_WhenSyntaxInvalid` | 非法高级语法字符串                      | 返回可读解析错误                       |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldNotApplyDefaultAge_WhenAgeMissing`     | 查询未显式提供 age 条件                 | 不自动注入 `age` 默认值                |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldParseSemicolonSeparatedFilters`        | 高级筛选使用分号分隔                    | 分号语法可正确映射到筛选字段           |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldParseOptionOnlyQuery`                  | 仅包含分页/排序参数的查询串             | 可正确解析为无关键词查询并保留页面参数 |

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
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldSearch_WhenOnlyPageOptionsProvided`    | 仅包含分页/排序参数的查询串 | 可正常发起搜索并返回结果                    |

#### 2.1.5 Application.Tests / SearchStateStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                       | 输入                                                   | 期望输出                             |
| ------ | ------ | ------ | ------------------------------------------------------------ | ------------------------------------------------------ | ------------------------------------ |
| [x]    | [x]    | 阶段 4 | `EnqueueForDownload_ShouldNormalizeSourceIdExtractedFromUrl` | RJID、作品链接、API 路径、`RJ-xxxx` 与纯数字混合值入队 | 队列统一归一化为 `RJxxxx` 且去重正确 |
| [x]    | [x]    | 阶段 4 | `RemoveFromQueue_ShouldMatchNormalizedInput`                 | 队列已有 `RJxxxx` 时使用 URL 形式出队                  | 可匹配并成功移除                     |

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

| 已创建 | 已通过 | 阶段    | 样例名                                                                                     | 输入                                                                                       | 期望输出                                                                                          |
| ------ | ------ | ------- | ------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 2  | `AsmrApiClient_ShouldMapHttpErrors`                                                        | 业务请求返回 500                                                                           | 抛出 `AsmrApiException`，错误码为 `api_request_failed`                                            |
| [x]    | [x]    | 阶段 4  | `GetPopularAsync_ShouldUsePostAndMapWorks`                                                 | 调用热门接口                                                                               | 使用 `POST /api/recommender/popular` 且正确映射返回 `works`，包括 Release、HasSubtitle、Tags      |
| [x]    | [x]    | 阶段 2  | `AsmrApiClient_ShouldAttachBearerToken_OnAuthorizedCalls`                                  | 已登录态调用受保护接口                                                                     | 请求头包含 `Authorization: Bearer xxx`                                                            |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_ShouldNormalizeWorkUrlInput_ToWorkEndpointPath`                             | 输入作品 URL 形式 id 调用 `GetWorkInfoAsync`                                               | 请求路径归一化为 `/api/work/{numericId}`（纯数字，无 `RJ` 前缀）并成功调用                        |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_ShouldNormalizeNonCanonicalSourceId_ToNumericApiPath`                       | 输入 `RJ-xxxx`/纯数字/`RJxxxx` 形式 id 调用 `GetWorkInfoAsync`                             | 请求路径归一化为 `/api/work/{numericId}`（纯数字，无 `RJ` 前缀）                                  |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_SearchAsync_ShouldNotDoubleEncodeQuery`                                     | 输入已编码 query（含高级筛选 token）调用 `SearchAsync`                                     | 请求 URL 不出现 `%25` 二次编码序列                                                                |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_ShouldDeserializeTranslationMetadata_OnWorkInfoResponse`                    | 含 `translation_info`、`language_editions`、`other_language_editions_in_db` 的作品详情响应 | 正确反序列化当前语言、关联翻译版本与原作标记，供入队优先级选择复用                                |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_ShouldResolveNonNumericSourceId_ToNumericWorkEndpointPath`                  | 输入 `BJ02370869` 这类 `source_id` 调用 `GetWorkInfoAsync`                                 | 先通过搜索结果解析数值 `workId`，再请求 `/api/work/{numericId}`                                   |
| [x]    | [x]    | 阶段 4  | `AsmrApiClient_GetTracksAsync_ShouldResolveNonNumericSourceId_ToNumericTracksEndpointPath` | 输入 `BJ02370869` 这类 `source_id` 调用 `GetTracksAsync`，且 tracks 响应包含 `size` 字段   | 先通过搜索结果解析数值 `workId`，再请求 `/api/tracks/{numericId}`，并正确反序列化 `TrackDto.Size` |
| [x]    | [x]    | 阶段 5  | `GetMetadataWorksAsync_ShouldUseWorksEndpoint_AndSubtitleFlag`                             | 调用 `/api/works` 元数据分页接口，`page=2`、`pageSize=50`、`subtitle=1`                    | 请求路径保留分页与字幕参数，并正确反序列化元数据分页结果                                          |
| [x]    | [x]    | 阶段 4+ | `DownloadFileAsync_ShouldWriteResponseBody_ToDestinationPath`                              | 传入绝对 `mediaDownloadUrl` 与嵌套目标路径                                                 | 以流式方式写入响应体、自动创建目标目录，并保留授权请求头                                          |

#### 2.1.10 Infrastructure.Tests / AuthServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                 | 期望输出                                            |
| ------ | ------ | ------ | ---------------------------------------------------------- | -------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldRejectLogin_WhenAccountOrPasswordEmpty` | 空账号或空密码       | 抛出明确业务异常                                    |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldStoreTokenAfterLogin`                   | 登录返回 `jwt-token` | 返回 token 且 `TokenStore` 已持久化该 token         |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldThrowReadableErrorOnFailure`            | 登录接口返回 401     | 抛出 `AsmrApiException`，错误码 `auth_login_failed` |

#### 2.1.11 Infrastructure.Tests / ConfigurationServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                        | 输入                                                                       | 期望输出                                                       |
| ------ | ------ | ------- | ----------------------------------------------------------------------------- | -------------------------------------------------------------------------- | -------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `ConfigurationService_ShouldReturnValidationErrors_WhenRequiredFieldsMissing` | 缺失账号/密码、`MetadataValidityDays` 非法，且 `SyncWantedSize` 非法的配置 | 返回可读校验错误集合，且包含元数据有效期与同步容量上限相关提示 |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldSaveAndLoadConfig_FromSplitSqliteSections`        | 临时目录、包含账号与下载参数的配置对象                                     | 按 `user/downloader/limit` 三段写入 SQLite 并正确读取          |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldLoadFromDefaultConfigJson_WhenSqliteMissing`      | 无 SQLite 配置，仅程序目录 `config.json`                                   | 可读取默认配置并返回                                           |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldPreferSqliteOverDefaultConfigJson`                | 同时存在 SQLite 与 `config.json`                                           | 优先读取 SQLite 实际配置                                       |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldMigrateLegacySingleRowAppConfig`                  | 旧 `Id=1` 单行 AppConfig                                                   | 自动迁移为分段结构并可正常读取，旧目录字段被清空且不再保留     |

#### 2.1.12 Infrastructure.Tests / EndpointDiscoveryServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                                      | 输入                                                              | 期望输出                                                                                        |
| ------ | ------ | ------- | ----------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldPickFastestReachableCandidate`                                              | 候选域名包含 slow/fast，fast 返回 200                             | 选中 `https://fast.example.com`                                                                 |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldProbeCandidates_UsingHealthEndpoint`                                        | 健康候选对旧探测端点不可用，但 `GET /api/health?cache=false` 可达 | 候选探测改走 health 端点并正确选中健康候选                                                      |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldExtractPublishedCandidatesFromHtmlText_AndSkipEntryScriptFetch`             | 发布页正文直接包含 `asmr-300/200/100/one` 最新域名                | 按页面文本顺序解析最新候选域名，且无需再请求入口脚本                                            |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldFallbackToConfiguredBaseUrl_WhenAllCandidatesFail`                          | 所有候选地址不可达                                                | 回退到配置基础地址或返回明确失败                                                                |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldFallbackToConfiguredCandidate_WhenPublishAssetRequestFails`                 | 发布页可访问，但入口脚本请求 404/失败，配置地址可达               | 忽略入口脚本失败并回退到配置候选地址                                                            |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldParseScriptTag_WhenAttributesUseDifferentOrder`                             | 发布页脚本标签属性顺序变化                                        | 仍可解析入口脚本并动态发现 API 候选地址                                                         |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldParseRelativeEntryScript_WithQuerySuffix_AndSingleQuotedLink`               | 发布页入口脚本使用相对路径、查询串与单引号 link 配置              | 仍可解析入口脚本并动态发现 API 候选地址                                                         |
| [x]    | [x]    | 阶段 2  | `EndpointDiscoveryService_ShouldIgnorePublishSourceTimeoutAndHttpFailures_AndFallbackToConfiguredCandidate` | 发布源先后出现超时与 `HttpRequestException`，配置候选可达         | 发布源抓取失败不打断整条探测链路，最终回退到配置候选                                            |
| [x]    | [x]    | 阶段 2+ | `EndpointDiscoveryService_ShouldSendProbeUserAgent_OnPublishAndHealthRequests`                              | 发布页正文候选抓取与 health 探测请求                              | 发布页抓取与 health 探测均携带统一的匿名 probe User-Agent，且发布页请求附带 HTML/脚本 Accept 头 |

#### 2.1.13 Infrastructure.Tests / DatabaseInitializerTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                      | 输入                        | 期望输出                                                                               |
| ------ | ------ | ------- | --------------------------------------------------------------------------- | --------------------------- | -------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `DatabaseInitializer_ShouldCreateNewSchema_AndEnsureSyncTables`             | 临时目录下初始化数据库      | 创建 `AppConfig/UiState/FavoriteWork/MetadataWork/WorkSyncInfo` 结构，并补齐同步表字段 |
| [x]    | [x]    | 阶段 4+ | `DatabaseInitializer_ShouldMigrateLegacySingleRowAppConfig_ToSplitSections` | 预置旧单行 AppConfig 数据库 | 初始化后自动迁移到分段结构，且 `user/downloader/limit` 三段均存在，旧目录字段被清空    |

#### 2.1.14 IntegrationTests / ApplicationBootstrapperTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                               | 期望输出                                                  |
| ------ | ------ | ------ | ---------------------------------------------------------- | ---------------------------------- | --------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldFailGracefully_WhenDatabaseInitThrows` | 模拟数据库初始化异常               | `IsSuccess=false` 且错误信息可读                          |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldRequireSetup_WhenConfigMissing`        | 缺失配置场景执行 `InitializeAsync` | `RequiresSetup=true`、`IsSuccess=false`，数据库文件已创建 |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldSucceed_WhenConfigValid`               | 有效配置场景执行 `InitializeAsync` | `IsSuccess=true`、`RequiresSetup=false`，同步目录存在     |

#### 2.1.15 Wpf.Tests / DownloadInputNormalizerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                            | 期望输出                                |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------------------- | --------------------------------------- |
| [x]    | [x]    | 阶段 4 | `NormalizeSingleInputDisplay_ShouldExtractRjIdFromWorkUrl`                | 单个输入框粘贴作品 URL                          | UI 实时显示归一化 `RJxxxx`              |
| [x]    | [x]    | 阶段 4 | `NormalizeBatchInputDisplay_ShouldNormalizeAndDeduplicateSourceIds`       | 批量输入框混合 RJID/URL/`RJ-xxxx`/纯数字/重复值 | UI 实时显示为归一化且去重后的 RJID 列表 |
| [x]    | [x]    | 阶段 4 | `NormalizeBatchInputForSubmit_ShouldSupportCommaSemicolonSpaceAndNewline` | 批量输入含逗号/分号/空格/换行混排，点击提交     | 统一归一化并去重                        |

#### 2.1.16 Wpf.Tests / DownloadCommandAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                      | 输入                             | 期望输出                                                   |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------- | -------------------------------- | ---------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldToggleCommandAvailability_ByTaskState`                                      | 不同任务状态输入到命令可用性规则 | 取消/重试/立即下载按钮状态与任务状态一致                   |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancel_WhenQueuedTaskSelected`                                         | 选中 `Queued` 状态任务           | “取消选中任务”按钮可用                                     |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancelAndImmediateStart_WhenPendingTaskSelected`                       | 选中 `Pending` 状态任务          | “取消选中任务”与“立即下载选中任务”按钮可用                 |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldDisableRetry_ForFailedPlaceholderRow_WhenSelectionExists`                   | 选中 `TaskId=Empty` 的失败占位行 | 在存在选中项时禁用“重试失败任务”，但允许“立即下载选中任务” |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowRetryWithoutSelection_WhenAnyFailedTaskExists`                         | 无选中项，任务列表中存在失败任务 | “重试失败任务”按钮可用，并回退为批量重试全部失败任务       |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowRetry_WhenSelectionContainsRetryableFailedTask_AndIgnoreOtherStatuses` | 混合选中 Failed/Completed 等状态 | 只要选中集合中存在可重试失败任务，即允许触发重试           |

#### 2.1.17 Wpf.Tests / DownloadViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                       | 输入                                | 期望输出                                                                                                       |
| ------ | ------ | ------ | -------------------------------------------------------------------------------------------- | ----------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls`                     | 解析 `DownloadView.xaml` 的文本/XML | 关键样式资源、核心控件、“从文件导入”“从收藏夹导入”按钮与统一状态面板样式存在，且 XAML 可被解析。               |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldNotContainStagePrefixText`                                           | 解析 `DownloadView.xaml` 文本       | 页面不再包含“阶段 ”前缀文案。                                                                                  |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldUseHeaderBorders_AndLockStatusWidthWhileLeavingProgressResizable`    | 解析 `DownloadView.xaml` 文本/XML   | 列头显示边框；状态列宽保持 `50` 且不可拖拽改宽；进度列宽保持 `96` 且允许调整；数据过宽时支持横向滚动与列重排。 |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldContainQueueTranslationCheckbox`                                     | 解析 `DownloadView.xaml` 文本/XML   | 页面包含默认勾选的“加入翻译作品”复选框，且位于“只下载高清音频”右侧。                                           |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldPlaceRunQueueBetweenStartSelectedAndRefresh_AndRemoveRetryAllButton` | 解析 `DownloadView.xaml` 文本       | “执行下载队列”位于“立即下载选中任务”和“刷新任务列表”之间，且页面不再包含“重试全部失败任务”按钮。               |

#### 2.1.18 Wpf.Tests / MainWindowXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                            | 期望输出                                                                                                                               |
| ------ | ------ | ------ | ------------------------------------------------------------- | ------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `MainWindowXaml_ShouldUse1280x720DefaultWindowSize`           | 解析 `MainWindow.xaml` 文本/XML | 默认与最小窗口尺寸为 `1280x720`，主窗口标题为 `Asmroner`，并包含 `Library/Sync` 页签、壳层状态栏绑定与页签启用绑定，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `MainWindowXaml_ShouldNotContainVersionInWindowTitle`         | 解析 `MainWindow.xaml` 文本     | 标题不包含版本号前缀（例如 `Asmroner v`）。                                                                                            |
| [x]    | [x]    | 阶段 6 | `MainWindowXaml_ShouldPlaceLibraryTab_BetweenDownloadAndSync` | 解析 `MainWindow.xaml` 文本     | `Library` 页签位于 `Download` 与 `Sync` 之间，且存在 `LibraryHost` 宿主控件。                                                          |

#### 2.1.19 Wpf.Tests / SearchViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                            | 期望输出                                                                                                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | ------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldContainUnifiedCardStyles_AndCoreControls`            | 解析 `SearchView.xaml` 文本/XML | 卡片化样式资源、核心控件、“导出到文件”“收藏作品”按钮与统一状态面板样式存在，且 XAML 可被解析。                               |
| [x]    | [x]    | 阶段 7 | `SearchViewXaml_ShouldUseAlignedComboBoxStyles`                            | 解析 `SearchView.xaml` 文本/XML | 下拉框与选项项样式基于共享壳层 `ComboBox`/`ComboBoxItem` 样式，且 XAML 可被解析。                                            |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldNotContainStagePrefixText`                           | 解析 `SearchView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案。                                                                                                |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldUseHeaderBorders_AndLockSubtitleAndDateColumnWidths` | 解析 `SearchView.xaml` 文本/XML | 结果列表列头显示边框；首列标题使用本地化“作品ID”；字幕/日期列宽保持 `42/75` 且不可拖拽改宽；数据过宽时支持横向滚动与列重排。 |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldUseSearchViewClassName`                              | 解析 `SearchView.xaml` 文本     | `x:Class` 为 `Asmroner.Wpf.Views.SearchView`。                                                                               |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldWireSelectionChangedHandlersForQueryOptions`         | 解析 `SearchView.xaml` 文本     | 排序/方向/字幕/页大小下拉均绑定 `SelectionChanged`                                                                           |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldContainResultsGridContextMenuItems`                  | 解析 `SearchView.xaml` 文本     | 结果表格包含右键菜单四项操作及对应事件绑定                                                                                   |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldContainSeparateQueueTranslationCheckbox`             | 解析 `SearchView.xaml` 文本     | 搜索筛选用“包含翻译作品”与入队用“加入翻译作品”两个复选框并存，且后者位于前者右侧并默认勾选。                                 |

#### 2.1.20 Wpf.Tests / SettingsViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                              | 期望输出                                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------- | --------------------------------- | ---------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldContainCardSections_AndActionButtons` | 解析 `SettingsView.xaml` 文本/XML | 卡片分区、下载目录/同步下载目录/元数据有效期输入框与核心动作按钮存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldNotContainStagePrefixText`            | 解析 `SettingsView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案。                                                            |

#### 2.1.21 Core.Tests / DownloadFilterParserTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                           | 输入                                        | 期望输出                                     |
| ------ | ------ | ------- | ---------------------------------------------------------------- | ------------------------------------------- | -------------------------------------------- |
| [x]    | [x]    | 阶段 4  | `ParsePreferExtensions_ShouldUseUnifiedPreferFormatsFirst`       | `PreferFormats=mp3,m4a,TXT`                 | 解析结果为规范化扩展名集合 `.mp3/.m4a/.txt`  |
| [x]    | [x]    | 阶段 4  | `ParsePreferExtensions_ShouldReturnEmpty_WhenPreferFormatsEmpty` | `PreferFormats` 为空                        | 返回空扩展名集合（表示不按扩展名过滤）       |
| [x]    | [x]    | 阶段 4  | `ParseFileFilter_ShouldParseIncludeAndExcludeTerms`              | `+voice;-demo;chapter`                      | 正确解析必含/排除项，忽略空项                |
| [x]    | [x]    | 阶段 4  | `MatchesFileFilter_ShouldApplyAllTerms`                          | 包含必含与排除的筛选规则 + 多种文件路径输入 | 仅匹配同时满足全部必含且不命中排除条件的路径 |
| [x]    | [x]    | 阶段 4+ | `FilterHdAudioOnly_ShouldRemoveMp3_WhenFlacExists`               | mp3 + flac 混合列表，hdAudioOnly=true       | 保留 flac，移除 mp3                          |
| [x]    | [x]    | 阶段 4+ | `FilterHdAudioOnly_ShouldRemoveMp3_WhenWavExists`                | mp3 + wav 混合列表，hdAudioOnly=true        | 保留 wav，移除 mp3                           |
| [x]    | [x]    | 阶段 4+ | `FilterHdAudioOnly_ShouldKeepMp3_WhenNoHdAudioExists`            | mp3 + jpg 列表，无 wav/flac                 | 原样返回，不过滤 mp3                         |
| [x]    | [x]    | 阶段 4+ | `FilterHdAudioOnly_ShouldReturnUnchanged_WhenHdAudioOnlyIsFalse` | mp3 + flac，hdAudioOnly=false               | 原样返回所有条目                             |

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

| 已创建 | 已通过 | 阶段   | 样例名                                                                                            | 输入                                                  | 期望输出                                                      |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------------- | ----------------------------------------------------- | ------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildCancelConfirmMessage_ShouldContainCancelableCount`                                          | 可取消任务数 `3`                                      | 返回“将取消 3 个任务，是否继续？”                             |
| [x]    | [x]    | 阶段 4 | `BuildRetryConfirmMessage_ShouldContainSelectedScopePreviewAndEllipsis_WhenExceedingPreviewLimit` | 6 条选中失败任务 SourceId，默认预览上限 5，最大并发 2 | 返回含“选中的”范围说明、前 5 项预览和省略号的批量重试确认文案 |
| [x]    | [x]    | 阶段 4 | `BuildRetryConfirmMessage_ShouldContainAllScopeWithoutEllipsis_WhenWithinPreviewLimit`            | 2 条全部失败任务 SourceId，最大并发 4                 | 返回含“全部”范围说明且不带省略号的批量重试确认文案            |

#### 2.1.26 Wpf.Tests / DownloadOperationStatusTextsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                             | 输入                                    | 期望输出                                             |
| ------ | ------ | ------ | ---------------------------------------------------------------------------------- | --------------------------------------- | ---------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildBatchEnqueueResult_ShouldIncludeResolvedSuffix_WhenPartialResolved`          | 总任务数 6，已解析 4                    | 返回“已加入批量下载：6 个任务（4 个已更新作品信息）” |
| [x]    | [x]    | 阶段 4 | `AppendTranslationSwitchClause_ShouldAppendSwitchSummary_WhenCountPositive`        | 原始消息“已加入下载队列 3 项”，切换数 2 | 返回“已加入下载队列 3 项；其中 2 项已切换为翻译作品” |
| [x]    | [x]    | 阶段 4 | `AppendTranslationSwitchClause_ShouldKeepOriginalMessage_WhenCountIsZero`          | 原始消息“已加入下载队列 3 项”，切换数 0 | 返回原始消息，不追加翻译切换说明                     |
| [x]    | [x]    | 阶段 4 | `BuildRunQueueResult_ShouldShowEmptyMessage_WhenNoTaskCreated`                     | 创建任务数 0                            | 返回“下载队列为空，无需执行。”                       |
| [x]    | [x]    | 阶段 4 | `BuildCancelResult_ShouldShowSummary_WhenAnyTaskCanceled`                          | 已取消 2，总计 3                        | 返回“已取消 2/3 个任务。”                            |
| [x]    | [x]    | 阶段 4 | `BuildRetryResult_ShouldReturnUnsupportedMessage_WhenNotRetried`                   | `retried=false`，`sourceId=RJ3001`      | 返回“仅失败状态任务支持重试。”                       |
| [x]    | [x]    | 阶段 4 | `BuildRetryAllResult_ShouldContainRetriedSummary`                                  | 成功触发 3，总计 5                      | 返回“批量重试完成：成功触发 3/5。”                   |
| [x]    | [x]    | 阶段 4 | `BuildStartSelectedResult_ShouldReturnNoStartMessage_WhenNoneStarted`              | 启动数 0，总选中 2                      | 返回“没有可立即下载的任务。”                         |
| [x]    | [x]    | 阶段 4 | `BuildWorkInfoRefreshResult_ShouldUseUpdatedCompletedText_WhenMixedResultReturned` | 成功更新 3，失败 1                      | 返回“作品信息更新完成：成功更新 3 项，失败 1 项。”   |
| [x]    | [x]    | 阶段 4 | `BuildWorkInfoRefreshResult_ShouldUseUpdatedFailureText_WhenNothingUpdated`        | 成功更新 0，失败 2                      | 返回“作品信息更新失败：共有 2 项未能更新。”          |

#### 2.1.27 Wpf.Tests / DownloadConfirmationPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                              | 输入                              | 期望输出                                                     |
| ------ | ------ | ------ | --------------------------------------------------- | --------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldContinue_WhenResultIsYes`           | `MessageBoxResult.Yes`            | 返回 `ShouldContinue=true` 且 `StatusText=null`              |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldCancelOperation_WhenResultIsNotYes` | `MessageBoxResult.No/Cancel/None` | 返回 `ShouldContinue=false` 且 `StatusText=已取消本次操作。` |

#### 2.1.28 Wpf.Tests / DownloadOperationPrecheckPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                | 期望输出                                                 |
| ------ | ------ | ------ | --------------------------------------------------------------------- | --------------------------------------------------- | -------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `CheckCancel_ShouldReturnPrompt_WhenSelectionEmpty`                   | 空选中集合                                          | 返回不可继续且提示“请先选择要取消的任务。”               |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldReturnSelectedFailedTargets_AndIgnoreOtherStatuses` | 选中 Failed/Completed 混合任务                      | 返回可继续，仅提取选中的失败任务作为重试目标             |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldRejectWhenSelectionHasNoRetryableFailedTargets`     | 选中占位失败行与非失败行                            | 返回不可继续且提示“选中项中没有可重试的失败任务。”       |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldFallbackToAllFailed_WhenSelectionEmpty`             | 无选中项，任务列表中存在失败任务                    | 返回可继续，并回退为全部失败任务重试目标                 |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldRejectWhenSelectionEmpty_AndNoFailedTasksExist`     | 无选中项，任务列表中没有失败任务                    | 返回不可继续且提示“当前没有失败任务可重试。”             |
| [x]    | [x]    | 阶段 4 | `CheckStartImmediate_ShouldReturnDeduplicatedTargets`                 | 含 Pending/Failed/Canceled/Running 且 SourceId 重复 | 返回可继续，目标集合按立即下载规则筛选且按 SourceId 去重 |

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
| [x]    | [x]    | 阶段 4+ | `Create_ShouldCarryHdAudioOnly_WhenFlagIsTrue`  | `hdAudioOnly=true`                 | `context.HdAudioOnly == true`  |
| [x]    | [x]    | 阶段 4+ | `Create_ShouldDefaultHdAudioOnly_ToFalse`       | 不传第二参数                       | `context.HdAudioOnly == false` |

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

#### 2.1.35 Wpf.Tests / DownloadTaskStatusExtensionsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                    | 期望输出                                                               |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------------- | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetDisplayName_ReturnsCorrectChineseLabel`（Theory，6 inline cases） | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应中文标签：未下载/待下载/下载中/已完成/已失败/已取消            |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_ReturnsCorrectSortOrder`（Theory，6 inline cases）      | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应排序优先级：3/2/1/0/4/5                                        |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_OrdersStatusesCorrectly`                                | 无序的 6 个枚举值输入                                   | 按排序优先级递增排列：Completed→Running→Queued→Pending→Failed→Canceled |

#### 2.1.36 Infrastructure.Tests / AsmrApiOptionsProviderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                                         | 期望输出                                              |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ------------------------------------------------------------ | ----------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldParseConfiguredUrlLists`                      | 自定义 `api_url` + 候选/发布源 URL 列表（含分号/逗号与重复） | 正确解析并去重，`BaseUrl` 与候选/发布源列表按预期生成 |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldFallbackToDefaults_WhenConfiguredUrlsMissing` | 空 `api_url` 与空列表字段                                    | 回退默认 API 地址、默认候选地址与默认发布源地址       |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldExpandLegacyBuiltInCandidateSubset`           | 旧版内置候选子集 `api.asmr-300.com;api.asmr.one`             | 自动扩展为当前完整默认候选集合并保持 BaseUrl 优先     |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldIgnoreInvalidUrls_InConfiguredLists`          | 列表中混入非法 URL                                           | 仅保留合法 URL 项，非法项被忽略                       |

#### 2.1.37 Infrastructure.Tests / ApiEndpointUrlServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                     | 输入                                                      | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------ | --------------------------------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldUpdateApiUrl_AndCandidateUrls_WhenConfigExists`             | 配置存在，发现结果返回新 BaseUrl 与最新公开候选集合       | `downloader.api_url` 与 `downloader.api_candidate_urls` 被一起更新并保存 |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldMergeDiscoveredAndSavedCandidates_WhenMergedCountIncreases` | 已保存公开候选子集 + discovery 仅返回部分新候选           | 按 discovery 优先顺序合并新旧候选，且保留旧候选后写回 SQLite             |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldKeepCustomCandidateList_WhenDiscoveryAddsPublicCandidates`  | 自定义候选集合 + discovery 返回公开候选与新自定义 BaseUrl | 保留自定义候选集合边界，仅把新自定义 BaseUrl 前置保存                    |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldSkipSave_WhenBaseUrlAndCandidatesAreUnchanged`              | 已有 BaseUrl/候选集合与发现结果一致                       | 不重复写入配置，直接返回发现结果                                         |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldKeepSavedCandidateList_WhenMergedCountDoesNotIncrease`      | 已保存候选集合已覆盖 discovery 结果                       | 不因 discovery 子集结果缩减或覆盖已保存候选集合                          |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldSkipSave_WhenConfigMissing`                                 | 配置不存在，发现结果返回新 BaseUrl                        | 不写入配置文件，返回发现结果                                             |
| [x]    | [x]    | 阶段 2 | `GetCurrentBaseUrlAsync_ShouldReturnDefault_WhenConfigMissingOrEmpty`                      | 配置缺失或 `api_url` 为空白                               | 返回默认 API 基础地址                                                    |

#### 2.1.38 Infrastructure.Tests / ConnectivityProbeServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                       | 期望输出                              |
| ------ | ------ | ------ | ------------------------------------------------------------------ | -------------------------- | ------------------------------------- |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldDiscoverAndAuthenticate_WhenDependenciesSucceed` | Discover 成功 + 登录成功   | 返回可达且鉴权成功，包含 BaseUrl/延迟 |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldReturnFailureResult_WhenAuthenticationThrows`    | Discover 成功 + 登录抛异常 | 返回不可达结果并携带失败消息          |

#### 2.1.39 Infrastructure.Tests / Existing suite updates

| 已创建 | 已通过 | 阶段   | 样例名                                                                                                                  | 输入           | 期望输出                                 |
| ------ | ------ | ------ | ----------------------------------------------------------------------------------------------------------------------- | -------------- | ---------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AsmrApiClient_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AsmrApiClientTests.cs`）                              | 客户端请求链路 | 仅使用当前 BaseUrl 服务，不触发 Discover |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AuthServiceTests.cs`）                                  | 登录链路       | 仅使用当前 BaseUrl 服务，不触发 Discover |
| [x]    | [x]    | 阶段 2 | `EndpointDiscoveryService_ShouldUseConfiguredPublishSources_ForDynamicCandidates`（`EndpointDiscoveryServiceTests.cs`） | 发布源地址配置 | 来自配置，且可动态发现候选 API           |

#### 2.1.40 Wpf.Tests / StartupEndpointWarmupServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                         | 期望输出                                              |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | ---------------------------- | ----------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldReturnImmediately_WhenDiscoveryIsSlow`   | Discover 慢响应              | 启动 warmup 调用快速返回，不阻塞窗口启动路径          |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldInvokeDiscoverAndPersistAsync`           | 正常 discover 依赖           | 后台流程会触发一次 DiscoverAndPersist 调用            |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldReuseInFlightWarmupTask_AndDiscoverOnce` | 两次并发 startup warmup 调用 | 复用同一 warmup 任务，且只执行一次 DiscoverAndPersist |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldNotThrow_WhenDiscoveryFails`             | Discover 抛异常              | 异常被吞吐并记录，不向上抛出                          |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldRespectTimeout_AndContinue`              | Discover 超时（50ms）        | 超时后流程结束并继续，不阻塞应用                      |

---


#### 2.1.41 Wpf.Tests / DownloadTaskRowViewModelTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                   | 输入                       | 期望输出                                      |
| ------ | ------ | ------- | ---------------------------------------------------------------------------------------- | -------------------------- | --------------------------------------------- |
| [x]    | [x]    | 阶段 4+ | `From_ShouldSetStatusSortOrder_MatchingGetSortOrder`（全 6 状态 Theory）                 | 各 DownloadTaskStatus 枚举 | `vm.StatusSortOrder == status.GetSortOrder()` |
| [x]    | [x]    | 阶段 4+ | `CreatePending_ShouldSetStatusSortOrder_MatchingGetSortOrder`（Queued/Pending/Canceled） | 各待定状态                 | `vm.StatusSortOrder == status.GetSortOrder()` |

#### 2.1.42 Wpf.Tests / DownloadEnqueueDuplicatePolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                | 输入                                                 | 期望输出                         |
| ------ | ------ | ------- | ------------------------------------------------------------------------------------- | ---------------------------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldExclude_WhenSourceIdExistsWithAnyStatus`（5 状态 Theory） | 已存在各状态任务，入队同 RJID                        | 返回空列表                       |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldInclude_WhenSourceIdNotInTaskList`                        | 已有 RJ001，入队 RJ002/RJ003                         | 返回 [RJ002, RJ003]              |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldIgnoreCase`                                               | 已有小写 rj001，入队 RJ001/RJ002                     | 跳过 RJ001，返回 [RJ002]         |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldReturnAll_WhenNoExistingTasks`                            | 空任务列表，入队 2 项                                | 返回所有 2 项                    |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldReturnEmpty_WhenAllAlreadyExist`                          | 全部已存在                                           | 返回空列表                       |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldDeduplicateIncoming`                                      | 含重复和大小写重复项 [RJ001,RJ002,rj001,RJ003,RJ002] | 返回 [RJ001,RJ002,RJ003]（3 项） |

#### 2.1.43 Application.Tests / SearchImportServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                  | 输入                           | 期望输出                             |
| ------ | ------ | ------- | ------------------------------------------------------- | ------------------------------ | ------------------------------------ |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldReturnItems_FromValidCsv`          | 有效 CSV（含标准 5 列）        | 正确解析 SourceId/Title/Release/Tags |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldSkipHeaderAndEmptyLines`           | 含空行的 CSV                   | 只返回有效数据行                     |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldHandleQuotedTitle_WithComma`       | 标题含逗号（RFC4180 引号包裹） | 正确解析含逗号 title                 |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldHandleEmbeddedDoubleQuote_InTitle` | 标题含双引号（`""`转义）       | 正确解析含引号 title                 |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldDeserializeItems_FromValidJson`   | 有效 JSON 数组（camelCase）    | 正确反序列化 SourceId/Title          |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldReturnEmpty_ForEmptyJsonArray`    | `[]`                           | 返回空列表                           |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldSkipEntries_WithEmptySourceId`    | 含空 sourceId 的条目           | 过滤空 sourceId，只返回有效项        |

#### 2.1.44 Wpf.Tests / SearchPagingPolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                             | 输入                                  | 期望输出                        |
| ------ | ------ | ------- | -------------------------------------------------- | ------------------------------------- | ------------------------------- |
| [x]    | [x]    | 阶段 4+ | `SlicePage_ShouldClampPage_AndReturnExpectedItems` | 5 条数据，requestedPage=4，pageSize=2 | 纠正到有效页 3，并返回最后 1 条 |
| [x]    | [x]    | 阶段 4+ | `CanJump_ShouldReturnFalse_WhenOnlySinglePage`     | totalPages=1                          | 返回 false（禁用跳页）          |
| [x]    | [x]    | 阶段 4+ | `CanJump_ShouldReturnTrue_WhenMultiplePages`       | totalPages=2                          | 返回 true（允许跳页）           |

#### 2.1.45 Wpf.Tests / SearchQueueCountPolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                         | 输入                                   | 期望输出                                            |
| ------ | ------ | ------- | -------------------------------------------------------------- | -------------------------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldCountSkippedFromExistingQueuedAndInputDuplicates` | 混合输入重复 + 已在队列 + 已在任务列表 | `ToEnqueue` 与 `SkippedCount` 均与去重/跳过规则一致 |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldSkipQueuedItems_WhenTaskListEmpty`                | 任务列表为空 + 队列已含同批 SourceId   | 返回空队列且跳过数等于输入项数                      |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldReturnEmpty_WhenInputInvalid`                     | 空字符串与空白输入                     | 返回空队列且跳过数为 0                              |

#### 2.1.46 Infrastructure.Tests / UiStateStoreTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                    | 输入                                                     | 期望输出                                                                                               |
| ------ | ------ | ------- | ------------------------------------------------------------------------- | -------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4+ | `SaveSearchUiStateAsync_ShouldRoundTrip`                                  | 保存 includeTranslation、queueTranslation 与高级筛选状态 | 重新加载后字段一致                                                                                     |
| [x]    | [x]    | 阶段 4+ | `SaveDownloadUiStateAsync_ShouldRoundTrip`                                | 保存 fileFilter、hdAudioOnly、queueTranslation 状态      | 重新加载后字段一致                                                                                     |
| [x]    | [x]    | 阶段 4+ | `LoadDownloadUiStateAsync_ShouldReturnDefaults_WhenNoUiStatePersisted`    | 无 UI 状态记录                                           | 返回默认状态：`fileFilter=空`、`hdAudioOnly=true`、`queueTranslationWorks=true`                        |
| [x]    | [x]    | 阶段 4+ | `SaveUnfinishedQueueAsync_ShouldNormalizeDeduplicate_AndClear`            | 混合 URL/RJID/重复/空白的未完成队列并执行清空操作        | 存储结果规范化去重，清空后读取为空                                                                     |
| [x]    | [x]    | 阶段 5  | `SaveMetadataSyncProgressAsync_ShouldRoundTrip_AndPreserveStopRequest`    | 元数据同步进度写回 + 运行中发起停止请求                  | 进度字段 round-trip 正确，含本地总量/字幕量/累计处理条数字段，且运行中 stop request 不会被后续保存覆盖 |
| [x]    | [x]    | 阶段 5  | `LoadMetadataSyncProgressAsync_ShouldReturnDefaults_WhenNoStatePersisted` | 无元数据同步进度记录                                     | 返回默认状态：`IDLE`、`NextPage=1`、`ProcessedPageCount=0`、`ProcessedWorkCount=0`                     |
| [x]    | [x]    | 阶段 5  | `SaveSyncDownloadProgressAsync_ShouldRoundTrip_AndPreserveStopRequest`    | 同步下载进度写回 + 运行中发起停止请求                    | 进度字段 round-trip 正确，且运行中 stop request 不会被后续保存覆盖                                     |
| [x]    | [x]    | 阶段 5  | `LoadSyncDownloadProgressAsync_ShouldReturnDefaults_WhenNoStatePersisted` | 无同步下载进度记录                                       | 返回默认状态：`IDLE`、空 `LastProcessedSourceId` 与 0 计数                                             |

#### 2.1.47 Wpf.Tests / DownloadUnfinishedQueueSnapshotPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                                             | 期望输出                                                            |
| ------ | ------ | ------ | ------------------------------------------------------------------ | ------------------------------------------------ | ------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldIncludePendingQueuedFailedAndQueuedSourceIds` | 活跃任务含多状态 + 队列含重复与空白项            | 仅保留 Pending/Queued/Failed + 队列项，大小写去重后按 SourceId 排序 |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldReturnEmpty_WhenNoActiveAndQueueEmpty`        | 活跃任务空 + 队列空                              | 返回空快照                                                          |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldDeduplicateCaseInsensitiveAcrossSources`      | 活跃任务与队列同时包含同一 SourceId 的大小写变体 | 跨来源按大小写不敏感去重，仅保留唯一 SourceId                       |

#### 2.1.48 Wpf.Tests / SearchWorkPageUrlPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                       | 输入                                     | 期望输出                                        |
| ------ | ------ | ------ | ------------------------------------------------------------ | ---------------------------------------- | ----------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldReplacePlaceholder_WithNormalizedSourceId`   | 模板含 `{RJID}` + 作品 URL 形式 sourceId | 生成标准 `https://www.asmr.one/work/RJxxxx` URL |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldAppendSourceId_WhenPlaceholderMissing`       | 模板不含占位符 + `rj1001`                | 自动追加 `/RJ1001`                              |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFallbackToDefaultTemplate_WhenTemplateEmpty` | 空模板 + 纯数字 sourceId                 | 回退默认模板并成功生成 URL                      |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFail_WhenSourceIdInvalid`                    | 空白 sourceId                            | 返回失败并给出可读错误                          |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFail_WhenTemplateInvalid`                    | 非法模板 `not-a-url-{RJID}`              | 返回失败并提示检查 `workPageUrlTemplate`        |

#### 2.1.49 Wpf.Tests / SearchExportScopePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                | 期望输出                                           |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------- | -------------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnAllResults_WhenScopeIsAll`                             | 全量结果 + 部分选中，scope=All      | 返回全量结果且不触发回退                           |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnSelectedResults_WhenScopeIsSelectedAndSelectionExists` | 全量结果 + 非空选中，scope=Selected | 返回选中结果且不触发回退                           |
| [x]    | [x]    | 阶段 3 | `Build_ShouldFallbackToAll_WhenScopeIsSelectedAndSelectionEmpty`          | 全量结果 + 空选中，scope=Selected   | 回退到全量结果，`FallbackToAll=true`               |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnEmpty_WhenNoResultsAndSelectionEmpty`                  | 全量空 + 选中空，scope=Selected     | 返回空结果，`FallbackToAll=true`（后续由 UI 提示） |

#### 2.1.50 Infrastructure.Tests / NLogAppLogServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                 | 输入                                    | 期望输出                                    |
| ------ | ------ | ------ | ------------------------------------------------------ | --------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `Configure_ShouldWriteLogEntry_WhenStaticLoggerIsUsed` | 调用 `Configure` 后使用静态 logger 写入 | 目标日志文件存在且包含 `test-message-` 文本 |

#### 2.1.51 Wpf.Tests / AppVersionInfoTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                            | 输入                     | 期望输出                                    |
| ------ | ------ | ------- | --------------------------------------------------------------------------------- | ------------------------ | ------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `GetDisplayVersion_ShouldReturnThreePartAssemblyVersion`                          | 当前程序集版本 `0.6.4.0` | 返回三段式版本文本 `0.6.4`                  |
| [x]    | [x]    | 阶段 1+ | `BuildSettingsVersionText_AndStartupMessage_ShouldUseDisplayVersionWithoutSuffix` | 动态版本文案构建         | Settings 文案与启动日志共用相同三段式版本号 |

#### 2.1.52 Wpf.Tests / StartupUnfinishedQueueMetadataRefreshServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                        | 输入                                         | 期望输出                                                     |
| ------ | ------ | ------ | ----------------------------------------------------------------------------- | -------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `StartInBackgroundAsync_ShouldReturnImmediately_WhenRefreshIsSlow`            | WorkInfo API 慢响应                          | 启动刷新调用立即返回，不阻塞窗口启动                         |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldReturnZero_WhenQueueEmpty`                                | 未完成队列为空                               | 返回空结果，且不触发 API/缓存写入                            |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldOnlyFetchMissingOrUntitledWorkInfo_AndUpsertFetchedItems` | 队列含已缓存标题、空标题、缺失标题的混合场景 | 仅补拉缺失/空标题项，且只执行一次 Full 级缓存写回            |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldContinue_WhenSingleFetchFails`                            | 混合成功与失败的 WorkInfo 请求               | 单项失败不影响整体流程，成功项写回缓存，失败项返回可展示错误 |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldWaitForWarmupBeforeFetchingWorkInfo`                      | warmup 阻塞 + 单条缺失标题队列               | 在 warmup 完成前不发起 WorkInfo 请求，完成后再补拉并写回缓存 |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldContinue_WhenWarmupFails`                                 | warmup 抛异常 + 单条缺失标题队列             | warmup 失败时仍继续补拉作品信息并写回缓存                    |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldReturnGlobalFailures_WhenTimeoutOccurs`                   | 单条缺失标题队列 + 后台补拉超时              | 返回失败明细，供 Download 页面标记为 `Failed` 占位行         |

#### 2.1.53 Application.Tests / WorkLanguageSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                              | 输入                                                             | 期望输出                         |
| ------ | ------ | ------ | ----------------------------------------------------------------------------------- | ---------------------------------------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldPreferSimplifiedChinese_WhenAvailable`                | 日文原作 + 关联简中/繁中版本                                     | 选择简体中文版本作为最终入队目标 |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldPreferTraditionalChinese_WhenSimplifiedMissing`       | 日文原作 + 仅有关联繁中版本                                      | 回退选择繁体中文版本             |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldKeepJapaneseCurrentWork_WhenNoChineseEditionExists`   | 日文原作 + 仅有非中文关联版本                                    | 保持当前日本語作品入队           |
| [x]    | [x]    | 阶段 4 | `ResolveCurrentWorkLanguage_ShouldUseWorkAttributes_WhenTranslationInfoLangMissing` | `translation_info.lang` 缺失，但 `work_attributes` 含 `CHI_HANT` | 解析出当前作品语言为繁体中文     |

#### 2.1.54 Application.Tests / EnqueueWorkInfoResolverTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                                             | 输入                                         | 期望输出                                                                            |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------- | ----------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldCreateSelectedEditionWorkInfo_WhenPreferredEditionOnlyExistsInRelatedEditions` | 仅拉到原始作品详情，关联版本列表中存在简中版 | 合成最终简中版 `WorkInfoDto`，并以优先版本 `SourceId` 返回，`SwitchedSourceCount=1` |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldReuseFetchedPreferredEdition_AndDeduplicateFinalSourceIds`                     | 原始作品与优先翻译版都已被拉取               | 复用已获取的优先翻译版详情，最终入队 `SourceId` 去重，`SwitchedSourceCount=1`       |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldReturnFailedSourceIds_WhenFetchFails`                                          | 混合成功与失败的作品详情拉取                 | 返回成功项并单独记录失败 `SourceId`，不阻塞其余作品入队，`SwitchedSourceCount=0`    |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldUseWorkIdWhenProvided`                                                         | 传入 `SourceId + WorkId` 的 BJ 作品          | 直接使用数值 `WorkId` 拉取详情并保留原始 `SourceId`，`SwitchedSourceCount=0`        |

#### 2.1.55 Core.Tests / SourceIdNormalizerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                               | 期望输出                                           |
| ------ | ------ | ------ | --------------------------------------------------------------- | -------------------------------------------------- | -------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Normalize_ShouldExtractBjId_FromDlsiteBooksUrl`                | `https://www.dlsite.com/books/work/.../BJ02370869` | 从 URL 中提取并归一化为 `BJ02370869`               |
| [x]    | [x]    | 阶段 4 | `ToApiNumericId_ShouldNotTreatBjSourceIdSuffix_AsNumericWorkId` | `BJ02370869`                                       | 不误将 `BJ` 后缀数字当作 API `workId` 直接截断返回 |

#### 2.1.56 Infrastructure.Tests / MemoryWorkInfoCacheTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                | 输入                               | 期望输出                                          |
| ------ | ------ | ------ | ----------------------------------------------------- | ---------------------------------- | ------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `TryGet_ShouldRequireFullEntry_WhenRequirementIsFull` | 仅写入 Summary 级作品缓存          | `Any` 命中，`Full` 要求不命中                     |
| [x]    | [x]    | 阶段 4 | `Set_ShouldPreserveFullEntry_WhenSummaryArrivesLater` | 先写入 Full，后写入同作品 Summary  | 缓存保留完整作品详情，不被摘要覆盖                |
| [x]    | [x]    | 阶段 4 | `TryGet_ShouldResolveNumericAlias_WhenWorkIdKnown`    | `BJ` 作品缓存 + 数值 `WorkId` 查找 | 可经数值 `WorkId` 命中同一作品缓存                |
| [x]    | [x]    | 阶段 4 | `Entries_ShouldExpirePerItem_AfterConfiguredTtl`      | 两条缓存按不同时间写入并等待过期   | 逐条按 TTL 过期，先写入项先失效，后写入项仍可命中 |

#### 2.1.57 Infrastructure.Tests / CachedAsmrApiClientTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                                               | 期望输出                                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `GetWorkInfoAsync_ShouldReturnCachedFullEntry_WithoutCallingInnerClient`   | Full 级作品缓存 + 内层 API 客户端                  | 直接返回缓存详情，不调用内层 `GetWorkInfoAsync`              |
| [x]    | [x]    | 阶段 4 | `GetWorkInfoAsync_ShouldUseCachedNumericId_WhenSummaryWasWarmedBySearch`   | Search 结果预热 Summary + 后续详情查询             | 使用缓存内 `WorkId` 补拉完整详情并回写 Full 缓存             |
| [x]    | [x]    | 阶段 4 | `GetTracksAsync_ShouldUseCachedNumericId_WhenPopularWarmupHasSummaryEntry` | 热门结果预热 Summary + 非 `RJ` `SourceId` 轨道查询 | `tracks` 查询复用缓存内数值 `WorkId`，避免直接用 `source_id` |

#### 2.1.58 Infrastructure.Tests / FavoriteStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                   | 输入                                         | 期望输出                                                                                          |
| ------ | ------ | ------ | ------------------------------------------------------------------------ | -------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SaveFavoriteFolderItemsAsync_ShouldRoundTripAndSkipExistingDuplicates`  | 同一收藏夹重复保存同一 `SourceId` 与新增作品 | SQLite 收藏夹按 `FolderTitle + SourceId` 去重，新增/跳过计数正确，已存在作品可更新标题与 `WorkId` |
| [x]    | [x]    | 阶段 4 | `LoadFavoriteFolderTitlesAsync_ShouldDistinctTrimAndSortCaseInsensitive` | 混合空白、大小写变体与带首尾空格的收藏夹标题 | 收藏夹标题去空白、大小写不敏感去重，并按字母序稳定返回                                            |

#### 2.1.59 Wpf.Tests / FavoriteFolderSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                                | 期望输出                                               |
| ------ | ------ | ------ | ------------------------------------------------------------- | ----------------------------------- | ------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `NormalizeFolderTitle_ShouldTrimWhitespace`                   | 含首尾空白的收藏夹标题              | 返回去首尾空白后的标题                                 |
| [x]    | [x]    | 阶段 4 | `BuildFolderTitles_ShouldDistinctAndSortCaseInsensitive`      | 大小写变体、空白标题与混合顺序标题  | 输出大小写不敏感去重且按稳定顺序排序后的收藏夹标题列表 |
| [x]    | [x]    | 阶段 4 | `CanConfirm_ShouldAllowNewTitle_WhenCustomInputEnabled`       | 允许新建收藏夹 + 新标题             | 允许确认                                               |
| [x]    | [x]    | 阶段 4 | `CanConfirm_ShouldRejectUnknownTitle_WhenCustomInputDisabled` | 仅允许选择现有收藏夹 + 未知标题输入 | 拒绝确认                                               |

#### 2.1.60 Wpf.Tests / FavoriteFolderDialogXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                      | 期望输出                                                    |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------------- | ----------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `FavoriteFolderDialogXaml_ShouldContainEditableComboBoxAndConfirmButtons` | 解析 `FavoriteFolderDialog.xaml` 文本/XML | 弹窗包含可编辑下拉框、保存/取消按钮，且 XAML 可被正确解析。 |

#### 2.1.61 Application.Tests / MetadataSyncServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                                                          | 期望输出                                                                                                                            |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | ------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldInsertAllPages_WhenRemoteHasNewWorks`                    | 网站元数据总量 `101`、本地为空，分页返回 `100 + 1` 条元数据   | 顺序请求总量页与 2 个同步分页，新增 101 条，本地总量追平到 101 条，并把完成态进度写为本地总量 `101` / 字幕 `1`                      |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldTrackProcessedWorks_WhenExistingPagesContainOnlyUpdates` | 第 1 页 100 条均为本地已存在记录，第 2 页新增 1 条元数据      | 本次累计处理 `101` 条、累计新增 `1` 条；已有页更新会写入 SQLite，完成态进度保留累计处理条数                                         |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldSkip_WhenRemoteCountMatchesLocalCount`                   | 网站总量与本地总量相同，且不存在过期元数据                    | 仅查询网站总量，不执行分页同步，返回“无需同步”                                                                                      |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldRefreshExpiredMetadata_WhenPreviousRunCompleted`         | 上次进度为 `COMPLETED`，且本地存在超过元数据有效期的元数据    | 再次执行时触发过期刷新，并更新本地 `MetadataWork` 摘要                                                                              |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldReport_WhenLocalCountExceedsRemoteCount`                 | 本地元数据数量大于网站                                        | 不执行分页同步，返回“本地元数据数量高于网站，未执行同步”                                                                            |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldResumeFromSavedProgress_WhenStateIsUnfinished`           | 已保存 `STOPPED` 元数据进度，`NextPage=2`，本地已有第一页数据 | 只从第二页继续同步，结果标记 `ResumedFromProgress=true`，最终进度写成 `COMPLETED`，并保留本地总量 `101` / 字幕 `1` / 累计处理 `101` |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldStopAfterCurrentPage_WhenStopRequested`                  | 两页元数据同步，第一页写回后触发 stop request                 | 当前页完成后停止，结果标记 `WasStopped=true`，UiState `NextPage=2`，并保留当前本地总量 `100` / 字幕 `1` / 累计处理 `100`            |

#### 2.1.62 Wpf.Tests / SyncViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                          | 期望输出                                                                                                                                                                                                                       |
| ------ | ------ | ------ | ---------------------------------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `SyncViewXaml_ShouldContainPrimaryActions_AndStatusFields` | 解析 `SyncView.xaml` 文本/XML | 页面包含两枚合并后的同步主按钮、“重试失败项”“导出失败记录”“导出成功记录”“刷新统计”按钮、下载摘要字段、统计卡片字段、统一状态面板样式、`StatusTextBlock + DownloadStatusTextBlock` 双状态文本框，且旧的独立停止按钮命名已移除。 |

#### 2.1.63 Application.Tests / SyncDownloadServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                                             | 期望输出                                                                                                     |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ---------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldStopAfterReachingConfiguredSizeLimit`              | 3 条元数据作品 + `SyncWantedSize=120B` + 成功下载大小 `70/60/50` | 仅处理前 2 项，累计落盘 `130 B` 后停止，剩余待同步数量为 1                                                   |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldMarkFailedItems_AndContinueToNextCandidate`        | 2 条元数据作品，其中首项下载失败、次项成功                       | 将失败项写为 `FAILED`、成功项写为 `COMPLETED`，并继续处理后续候选作品                                        |
| [x]    | [x]    | 阶段 5 | `RetryFailedAsync_ShouldReDownloadFailedItems_AndIncrementRetryCount`       | 预置 1 条失败同步记录 + 重试后下载成功                           | 删除旧失败目录、重试成功后写回 `COMPLETED`，并将 `RetryCount` 加 1                                           |
| [x]    | [x]    | 阶段 5 | `RetryFailedAsync_ShouldKeepFailedStatus_WhenRetryFailsAgain`               | 预置 1 条失败同步记录 + 重试后再次失败                           | 删除旧失败目录、失败状态保持为 `FAILED`，并累计重试次数与失败原因                                            |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldStopAfterCurrentWork_WhenStopRequested`            | 三条同步候选作品，首项下载完成前触发 stop request                | 当前作品完成后停止，结果标记 `WasStopped=true`，UiState 保留最近处理作品与未完成状态                         |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldResumeFromSavedProgress_WhenStateIsUnfinished`     | 已保存 `STOPPED` 下载进度 + 首项 `COMPLETED` 同步记录            | 继续处理剩余候选，不重下已有完成项，结果标记 `ResumedFromProgress=true`，最终进度写成 `COMPLETED`            |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldRescanAllWorks_WhenPreviousRunCompleted`           | 上次进度为 `COMPLETED`，且本地全部作品已有 `COMPLETED` 同步记录  | 再次执行时从头校验全部作品，并按当前下载结果更新同步记录                                                     |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldPassMetadataWorkId_ToDownloadService`              | 元数据作品的 `sourceId` 与数值 `WorkId` 不一致                   | 同步下载调用普通下载入口时显式透传数值 `WorkId` 并标记 `SyncManaged`，避免轨道解析漂移与普通下载自动写库路径 |
| [x]    | [x]    | 阶段 5 | `SyncDownloadAsync_ShouldUseUnifiedWorkInfoDto_WhenCreatingPendingSyncInfo` | SQLite `MetadataWork` 标题与统一 `WorkInfoDto` 标题不一致        | Sync 创建 `PENDING` 记录时使用统一 `WorkInfoDto` 的标题与字幕标记，不再直接复用存储实体作为 info DTO         |

#### 2.1.64 Infrastructure.Tests / MetadataSyncStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                                                       | 期望输出                                                                     |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | ---------------------------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldTrackSyncDownloadSnapshot_AndCleanupPendingRows`         | SQLite 中预置 2 条元数据并写入一条 Completed、一条 Pending | 快照正确统计完成/待处理数量与已落盘大小，且清理 Pending 后目录与记录一并移除 |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldReturnFailedSyncDownloads_ForRetry`                      | SQLite 中预置失败/成功两类同步记录                         | 仅返回 `FAILED` 记录，并保留重试次数、失败原因与时间信息                     |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldReturnSyncDownloadsByStatus_ForExport`                   | SQLite 中预置 Completed/Failed 两类同步记录                | 可按状态分别返回导出候选记录，并保留目录大小、失败原因与字幕标记             |
| [x]    | [x]    | 阶段 5 | `UpsertMetadataWorksAsync_ShouldUpdateExistingRows_WithoutCountingThemAsInserted` | SQLite 中已存在同 Id 记录，再次 upsert 更新标题与字幕标记  | 已有记录被正确更新，快照同步反映新字段值，且返回的“新增数”为 `0`             |

#### 2.1.65 Application.Tests / SyncExportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                 | 输入                                 | 期望输出                                                                |
| ------ | ------ | ------ | ------------------------------------------------------ | ------------------------------------ | ----------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `ExportAsync_ShouldWriteCsvAndJson_ForRequestedStatus` | 失败记录导出 CSV + 成功记录导出 JSON | 按状态筛选导出，CSV/JSON 内容正确且 CSV 正确转义逗号与引号              |
| [x]    | [x]    | 阶段 5 | `ExportAsync_ShouldReturnNoOp_WhenNoItemsMatchStatus`  | 仅存在成功记录时导出失败记录         | 返回 0 条导出结果，不创建输出文件，并给出“没有可导出的失败同步记录”提示 |

#### 2.1.66 Application.Tests / SyncReportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                         | 输入                                              | 期望输出                                                      |
| ------ | ------ | ------ | -------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `GetReportAsync_ShouldBuildBreakdownAndProgress_FromSnapshots` | 元数据 10 条、已完成 3 条、失败 2 条、待处理 1 条 | 正确汇总字幕/无字幕拆分、完成大小与总进度/字幕进度/无字幕进度 |
| [x]    | [x]    | 阶段 5 | `GetReportAsync_ShouldReturnZeroProgress_WhenMetadataIsEmpty`  | 本地元数据为空，下载快照仅含失败统计              | 所有进度百分比安全回落为 `0.00%`，不出现除零异常或无效值      |

#### 2.1.67 Wpf.Tests / SyncCommandAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                    | 期望输出                                                                     |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldExposeStartTexts_WhenIdle`                                | Sync 页面空闲态                         | 两个同步主按钮均可点击，文案分别为“开始同步元数据”“开始同步下载”             |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldKeepRefreshEnabled_WhenMetadataSyncIsRunning`             | 元数据同步运行中，其余操作空闲          | 元数据主按钮切换为“停止同步元数据”，下载/重试/导出禁用，但“刷新统计”仍可点击 |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldShowStoppingText_WhenMetadataStopAlreadyRequested`        | 元数据同步运行中，且已发起 stop request | 元数据主按钮显示“正在停止元数据...”，按钮禁用，刷新统计仍可点击              |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldShowStoppingText_WhenDownloadStopAlreadyRequested`        | 同步下载运行中，且已发起 stop request   | 下载主按钮显示“正在停止下载...”，按钮禁用，刷新统计仍可点击                  |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldDisableRefreshWhileRefreshIsRunning_ButKeepStopAvailable` | 元数据同步运行中，且刷新统计正在执行    | “刷新统计”仅在自身执行期间禁用，不影响当前同步主按钮保持 stop 语义           |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldKeepRefreshEnabled_WhenRetryFailedIsRunning`              | “重试失败项”执行中                      | 两个同步主按钮、重试与导出禁用，但“刷新统计”仍可点击                         |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldDisableRefresh_WhenExportOperationRunning`                | 导出附属操作执行中                      | 两个同步主按钮、重试、导出与刷新统计全部禁用，避免导出与刷新并发冲突         |

#### 2.1.68 Wpf.Tests / SyncActionDebouncePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                        | 期望输出                                                     |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldReturnStart_WhenSyncIsIdle`                                 | 同步未运行 + 任意最近开始时间               | 返回 `Start`，允许开始新的同步                               |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnoreMetadataDoubleClick_WhenSecondClickIsWithinOneSecond` | 元数据同步运行中 + 最近开始时间距今 `< 1s`  | 返回 `Ignore`，第二次快速点击不会误发 stop request           |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldRequestMetadataStop_WhenSecondClickIsAfterOneSecond`        | 元数据同步运行中 + 最近开始时间距今 `>= 1s` | 返回 `RequestStop`，超过防抖窗口后可正常进入停止流程         |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnoreDownloadDoubleClick_WhenSecondClickIsWithinOneSecond` | 同步下载运行中 + 最近开始时间距今 `< 1s`    | 返回 `Ignore`，同步下载主按钮具备与元数据相同的 1 秒防抖策略 |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnore_WhenStopAlreadyRequested`                            | 同步运行中且已进入 stopping 态              | 返回 `Ignore`，后续重复点击不会覆盖既有停止请求              |

#### 2.1.69 Wpf.Tests / DownloadToolbarAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                               | 输入                      | 期望输出                                                         |
| ------ | ------ | ------ | -------------------------------------------------------------------- | ------------------------- | ---------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldEnableToolbarActions_WhenNoQueueMutationIsRunning`   | Download 顶部按钮空闲态   | 入队入口、执行队列、清空任务列表与打开下载目录均可点击           |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldKeepOpenDirectoryEnabled_WhenQueueMutationIsRunning` | “立即下载/执行队列”运行态 | 入队入口、执行队列与清空任务列表禁用，但“打开下载目录”保持可点击 |

#### 2.1.70 Wpf.Tests / SyncStatusTextBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                | 输入                                | 期望输出                                      |
| ------ | ------ | ------ | ----------------------------------------------------- | ----------------------------------- | --------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `BuildMetadataStatus_ShouldDescribeRunningProgress`   | 元数据同步进行中                    | 返回带“同步状态：”前缀的分页/累计处理状态文本 |
| [x]    | [x]    | 阶段 5 | `BuildMetadataStatus_ShouldDescribeStoppedProgress`   | 元数据同步 `STOPPED` 断点继续态     | 返回带“同步状态：”前缀的断点继续提示          |
| [x]    | [x]    | 阶段 5 | `BuildDownloadStatus_ShouldDescribeStoppingProgress`  | 同步下载进行中且已发起 stop request | 返回带“下载状态：”前缀的 stopping 提示        |
| [x]    | [x]    | 阶段 5 | `BuildDownloadStatus_ShouldDescribeCompletedProgress` | 同步下载 `COMPLETED` 完成态         | 返回带“下载状态：”前缀的完成摘要              |

#### 2.1.71 Wpf.Tests / SyncProgressDetailsBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                          | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `BuildRetryPendingDetails_ShouldDescribeRetryInFlight`                    | “重试失败项”刚进入执行态                      | 返回“正在重试失败同步下载，完成后这里会显示本次重试结果。”占位详情文本   |
| [x]    | [x]    | 阶段 5 | `BuildPersistedProgressDetails_ShouldDescribeMetadataAndDownloadProgress` | 已持久化的元数据进度 + 已持久化的同步下载进度 | 返回包含元数据与同步下载两组累计状态、容量与最近更新时间的详情区文本     |
| [x]    | [x]    | 阶段 5 | `BuildRetryRefreshDetails_ShouldDescribeCurrentRetrySnapshot`             | 重试运行中的报表快照                          | 返回当前重试态摘要，保留下载状态上下文，不再回退为旧的持久化下载进度详情 |

#### 2.1.72 Core.Tests / LibraryDirectoryNameParserTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                            | 输入                                                | 期望输出                                                                     |
| ------ | ------ | ------ | ----------------------------------------------------------------- | --------------------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseBracketedDirectoryName`                      | 目录名 `[{SourceId}]{Title}`                        | 可正确解析 `SourceId/Title/Scheme=Bracketed`。                               |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseLegacyDirectoryName`                         | 目录名 `sourceId-date-sub/nosub-title`              | 可正确解析 `SourceId/Title/Release/HasSubtitle/Scheme=LegacyListen`。        |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseLegacyDirectoryName_WhenTitleContainsHyphen` | 目录名 `sourceId-date-sub/nosub-legacy-title-part2` | 标题部分即使包含连字符，也会被完整保留并正确解析为 `LegacyListen` 作品目录。 |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldReturnFalse_ForInvalidDirectoryName`              | 非法资源目录名                                      | 返回 `false` 且结果对象保持空值，不把非法目录误识别为作品目录。              |

#### 2.1.73 Application.Tests / LibraryScannerServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                                      | 期望输出                                                                                                   |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldCollectBracketedAndLegacyDirectories_AndOverlayMetadata` | 下载目录含 bracketed/legacy 目录、非法目录与本地 metadata | 扫描结果可同时识别两种目录格式，非法目录进入 skipped 列表，metadata 可覆盖标题/日期/字幕并统计音频文件数。 |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldPreferDuplicateWorkEntry_WithMorePlayableFiles`          | 下载根与同步根同时存在同一 `SourceId` 的目录              | 合并后优先保留音频文件更多的目录项，避免同一作品在资源库中重复展示。                                       |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldKeepReadableFiles_WhenNestedDirectoryEnumerationThrows`  | 嵌套目录子目录枚举抛异常，但当前目录文件仍可读取          | 扫描结果保留可读文件树并记录错误，不因单个嵌套目录异常而放弃整项作品。                                     |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldSkipUnreadableFile_WhenFileInspectionThrows`             | 单个媒体文件长度探测抛异常                                | 跳过不可读文件、保留其余文件并记录错误，不因单文件异常导致整项扫描失败。                                   |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCase`         | 目录内存在 `.FLAC`、`.OpUs` 等大小写混合支持扩展名文件    | 扫描结果会按共享可播放格式规则识别大小写混合扩展名文件，并正确统计 `AudioFileCount/IsPlayable`。           |

#### 2.1.74 Application.Tests / LibraryQueryServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                                               | 期望输出                                                                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | -------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldFilterByKeywordSubtitleAndAudio_AndReturnPagingMetadata` | 预置扫描结果 + 关键字/字幕/音频筛选 + `pageSize=1` | 仅返回满足条件的作品，并保留 `SkippedDirectories/Errors/ScannedRootCount/ScannedWorkCount`。 |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldClampPageToLastPage_WhenRequestedPageExceedsRange`       | 预置 3 条扫描结果，请求超出范围的页码              | 查询页码会被钳制到最后一页，且返回按日期倒序后的尾页数据。                                   |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldHonorRequestedPageSize_WhenBuildingPagedItems`           | 预置 5 条扫描结果，请求 `page=2,pageSize=2`        | 返回第 2 页的 2 条数据，并正确输出 `PageSize/TotalPages/TotalCount` 等分页元数据。           |

#### 2.1.75 Application.Tests / PlayerServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                                        | 期望输出                                                                           |
| ------ | ------ | ------ | ------------------------------------------------------------------ | ------------------------------------------- | ---------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `LoadContext_ShouldLoadSelectedPlayableFile_WhenFileIsProvided`    | 作品树中显式选中一个可播放媒体文件          | 播放上下文状态为 `Ready`，并提示“已载入可播放媒体文件，可通过系统默认程序打开。”。 |
| [x]    | [x]    | 阶段 6 | `LoadContext_ShouldKeepEmptyContext_WhenSelectedFileIsNotPlayable` | 选中文本文件等不可播放文件                  | 保持 `None` 状态，并提示需要先在文件树中选择可播放媒体文件。                       |
| [x]    | [x]    | 阶段 6 | `LoadContext_ShouldMarkFailed_WhenSelectedFileDoesNotExist`        | 选中文件路径不存在的可播放作品              | 播放上下文状态为 `Failed`，并给出缺失文件提示，不会崩溃。                          |
| [x]    | [x]    | 阶段 6 | `Play_ShouldOpenLoadedFile_WithSystemLauncher`                     | 已载入可播放媒体文件后执行 `Play`           | 调用系统默认程序打开当前文件，并将播放上下文状态更新为 `Launched`。                |
| [x]    | [x]    | 阶段 6 | `Play_ShouldMarkFailed_WhenLauncherThrows`                         | 系统启动器抛出异常                          | 播放上下文状态为 `Failed`，并显示系统默认程序打开失败的错误信息。                  |
| [x]    | [x]    | 阶段 6 | `Play_ShouldRequireReload_WhenCurrentContextIsFailed`              | 失败态上下文直接执行 `Play`                 | 保持 `Failed` 状态并提示需要重新载入文件，不会误报为已打开。                       |
| [x]    | [x]    | 阶段 6 | `Play_ShouldKeepLoadedContext_WhenCurrentFileMatchesSelection`     | 当前载入文件与显式选择文件一致时执行 `Play` | 复用当前已载入上下文并调用系统默认程序打开，不会丢失当前作品与文件信息。           |
| [x]    | [x]    | 阶段 6 | `ClearContext_ShouldResetInitialState`                             | 已载入文件后执行 `ClearContext`             | 清空作品/文件与状态，恢复“尚未载入任何可播放媒体文件。”初始提示。                  |

#### 2.1.76 Wpf.Tests / LibraryViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                             | 期望输出                                                                                                                                                      |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldContainCoreLibraryControls_AndStatusBlocks`     | 解析 `LibraryView.xaml` 文本/XML | `Library` 页包含当前副标题文案、关键筛选控件、上一页/下一页/跳页/page size 控件、独立“作品详情”区、文件树、系统打开区、状态区与播放主按钮，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldContainExpectedGridColumns_AndFileTreeTemplate` | 解析 `LibraryView.xaml` 文本/XML | 作品表格包含本地化列头 `作品ID/标题/日期/字幕/音频/文件`，且文件树使用 `LibraryFileItem` 层级模板。                                                           |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldUseScrollableLayout_ForPagingAndRightPane`      | 解析 `LibraryView.xaml` 文本/XML | 右侧详情/文件树/上下文区采用受限高度与内部滚动布局，`FileTreeView` 不再固定 `220` 高度，且跳页/page size 事件绑定存在。                                       |

#### 2.1.77 Wpf.Tests / LibraryPlaybackSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                              | 期望输出                                                                             |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `ShouldReloadContext_ShouldReturnTrue_WhenSelectedFileHasDifferentFullPath` | 同一 `SourceId` 与相同 `RelativePath`、不同根目录 | 当显式选中文件的 `FullPath` 与当前上下文不同，即使相对路径相同也必须重新载入上下文。 |

#### 2.1.78 Wpf.Tests / ShellMediaLauncherTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                            | 输入                                       | 期望输出                                                                       |
| ------ | ------ | ------ | ----------------------------------------------------------------- | ------------------------------------------ | ------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `Open_ShouldTreatNullProcessAsSuccess_WhenShellStartDoesNotThrow` | Shell 启动器底层调用返回 `null` 且未抛异常 | 仍视为成功打开，不抛出异常，并保持 `UseShellExecute=true` 与目标文件路径不变。 |
| [x]    | [x]    | 阶段 6 | `Open_ShouldRethrow_WhenShellStartThrows`                         | Shell 启动器底层调用直接抛出异常           | `Open()` 继续抛出原始异常，供上层 `PlayerService` 转换成失败提示。             |

#### 2.1.79 Wpf.Tests / LibrarySelectionFeedbackPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                              | 输入                             | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------- | -------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldGuide_WhenWorkIsSelectedWithoutTreeItem`            | 仅选中作品、尚未选中文件树节点   | 返回作品级引导，提示当前作品包含可播放媒体文件数量与首个候选路径。       |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldExplainSupportedFormats_WhenWorkHasNoPlayableFiles` | 仅选中一个无可播放文件的作品     | 返回“未发现可播放媒体文件”提示，并附带共享支持格式列表。                 |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenDirectoryIsSelected`                       | 选中文件树中的目录节点           | 返回“当前选择是目录”提示，并在存在候选时给出建议选择的媒体文件。         |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenNonPlayableFileIsSelected`                 | 选中文件树中的不可播放文件       | 返回“当前选择的文件不可播放”提示，并在存在候选时给出建议选择的媒体文件。 |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenPlayableFileDoesNotExist`                  | 选中一个路径缺失的可播放媒体文件 | 返回“当前选择的媒体文件不存在”提示，并在存在其它候选时给出可改选文件。   |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldEnableActions_WhenPlayableFileExists`               | 选中一个实际存在的可播放媒体文件 | 返回“已选择可播放媒体文件”提示，并允许载入上下文与播放。                 |

#### 2.1.80 Core.Tests / LibraryPlayableMediaPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                             | 期望输出                                                   |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | -------------------------------- | ---------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `IsPlayableExtension_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCaseAndDot` | 支持扩展名的大写/缺少点号输入    | 共享格式规则会忽略大小写，并兼容带点或不带点的扩展名判断。 |
| [x]    | [x]    | 阶段 6 | `CountPlayableFiles_ShouldCountNestedPlayableMediaFiles`                          | 含嵌套目录与多种媒体文件的作品树 | 可递归统计作品树中的可播放媒体文件数量。                   |
| [x]    | [x]    | 阶段 6 | `FindFirstPlayableFile_ShouldReturnFirstDepthFirstPlayableFile`                   | 目录优先展示的嵌套作品树         | 会按当前树结构的深度优先顺序返回首个可播放媒体文件。       |
| [x]    | [x]    | 阶段 6 | `FindFirstPlayableFile_ShouldReturnNull_WhenNoPlayableFileExists`                 | 不含任何支持扩展名文件的作品树   | 返回 `null`，供上层显示“未发现可播放媒体文件”的明确提示。  |

#### 2.1.81 Wpf.Tests / LibraryPlaybackContextTextBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                           | 输入                           | 期望输出                                                                               |
| ------ | ------ | ------ | ---------------------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `Build_ShouldProvideDefaultTexts_WhenNothingIsSelectedOrLoaded`  | 无当前选择、无已载入上下文     | “当前选择”区显示显式选择引导，“当前已载入上下文”区显示未载入状态与默认说明。           |
| [x]    | [x]    | 阶段 6 | `Build_ShouldKeepSelectionFeedbackSeparated_FromLoadedContext`   | 有作品级选择提示，尚未载入文件 | “当前选择”区保留作品级候选提示，“当前已载入上下文”区仍显示未载入状态，不与选择区混写。 |
| [x]    | [x]    | 阶段 6 | `Build_ShouldIncludeLoadedWorkAndFileDetails_WhenContextIsReady` | 已载入一个可播放媒体文件       | “当前已载入上下文”区显示状态、作品、文件与说明，“当前选择”区保持当前选择反馈。         |

#### 2.1.82 Wpf.Tests / AppXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                        | 输入                     | 期望输出                                                     |
| ------ | ------ | ------ | --------------------------------------------- | ------------------------ | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 7 | `AppXaml_ShouldMergeShellResourcesDictionary` | 解析 `App.xaml` 文本/XML | 应用资源字典已合并 `ShellResources.xaml`，且 XAML 可被解析。 |

#### 2.1.83 Wpf.Tests / ShellResourcesXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                    | 输入                                | 期望输出                                                             |
| ------ | ------ | ------ | --------------------------------------------------------- | ----------------------------------- | -------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ShellResourcesXaml_ShouldContainSharedShellResourceKeys` | 解析 `ShellResources.xaml` 文本/XML | 共享壳层背景、卡片、标题、输入框与按钮基样式存在，且 XAML 可被解析。 |

#### 2.1.84 Wpf.Tests / ShellViewModelTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                  | 期望输出                                                               |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------- | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `DefaultState_ShouldStartOnSettingsWithInitializingMessage` | 新建 `ShellViewModel` | 默认选中 `Settings`，初始化消息为“初始化中...”，且主功能页签默认禁用。 |
| [x]    | [x]    | 阶段 7 | `SelectedPageIndex_ShouldClampToValidRange`                 | 写入越界页签索引      | 页签索引会被约束到合法范围，并同步回正确的 `ShellPage`。               |

#### 2.1.85 Wpf.Tests / NavigationServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                       | 期望输出                                                               |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------ | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `NavigateTo_ShouldUpdateCurrentPageAndSelectedIndex`                  | 通过导航服务切换到 `Library`               | `CurrentPage` 与 `SelectedPageIndex` 同步到目标页签。                  |
| [x]    | [x]    | 阶段 7 | `SetPrimaryPagesEnabled_ShouldToggleContentTabsAndFallbackToSettings` | 先启用主内容页签，再禁用并停留在内容页签上 | Search/Download/Library/Sync 会统一禁用，且当前页会回退到 `Settings`。 |

#### 2.1.86 Wpf.Tests / UiMessageServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                     | 输入                         | 期望输出                                                 |
| ------ | ------ | ------ | ------------------------------------------ | ---------------------------- | -------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ShowError_ShouldUpdateShellStatusMessage` | 通过消息服务写入错误状态文本 | 当前壳层消息与 `ShellViewModel.StatusMessage` 同步更新。 |

#### 2.1.87 Wpf.Tests / DialogFileNamePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                               | 期望输出                                                    |
| ------ | ------ | ------ | ---------------------------------------------------------- | ---------------------------------- | ----------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ResolveExtension_ShouldRespectFileNameAndFilterSelection` | 文件名已有/缺失后缀 + 保存筛选索引 | 能按现有文件名或当前筛选索引决策正确后缀。                  |
| [x]    | [x]    | 阶段 7 | `EnsureExtension_ShouldAppendOrReplaceExtension`           | 目标路径缺失/带错后缀              | 会自动补齐或替换成正确后缀，统一 Search/Sync 导出路径行为。 |

### 2.2 测试覆盖分析

- Core（模型/配置）：默认值完整性，✅ 已覆盖。
- Application（应用服务）：首次启动、查询解析、搜索聚合、导出、下载主流程与容错，✅ 已覆盖。
- Infrastructure（基础设施）：配置读写、数据库初始化、鉴权、API 客户端、地址发现、UI 状态 SQLite 持久化与 Sync 进度状态持久化，✅ 已覆盖。
- Integration（集成）：引导器成功/失败/DB 初始化异常三条路径，✅ 已覆盖。
- WPF（UI 规则）：命令可用性规则、下载输入实时归一化显示与 `Sync` 页面合并同步主按钮、重试/导出/刷新入口、统计卡片和摘要骨架，✅ 已覆盖。
- 阶段 5 同步增强：元数据分页、SQLite 入库、同步下载容量控制、失败重试、失败/成功记录导出、统计报表、`WorkSyncInfo` 状态回写、UiState 进度持久化、断点继续、合并式开始/停止按钮与运行中手动刷新统计，✅ 已覆盖。
- 下载入参规范化：RJID/作品 URL 混输下的入队与 API 调用兼容，✅ 已覆盖。
- 格式优先级下载：`PreferFormats` 过滤轨道与留空全下载，✅ 已覆盖。
- WorkInfo 预取复用：入队预取后下载阶段内存命中，✅ 已覆盖。
- WorkInfo 共享缓存：摘要预热、Full 升级、数值 `WorkId` 别名命中与逐条 TTL，✅ 已覆盖。
- 收藏夹存储与导出：SQLite 收藏夹写入、共享收藏夹弹窗、Search 收藏与 Download 导出联动，✅ 已覆盖。
- 未完成队列快照构建：Search/Download 共用快照规则并在入队后持久化，✅ 已覆盖。
- 启动未完成队列元数据补拉：非阻塞启动、仅补拉缺失标题并刷新 Download 列表，✅ 已覆盖。
- 版本文案动态化：Settings 页面与启动日志共用程序集三段式版本号，✅ 已覆盖。
- 阶段 7 壳层收口：主窗口页签绑定、共享导航/消息服务、统一文件对话框后缀策略与共享样式资源基线，✅ 已覆盖（自动化）；加载态/空态与 UI 冒烟，⬜ 待落地。
- 阶段 6 资源库与系统默认程序打开：目录名双格式解析、扫描去重、关键字/字幕/音频过滤、显式选中文件后的系统打开、刷新后跨根目录同相对路径文件的重载判定、Shell `null` 返回成功判定、失败提示、Library 打开控件与播放上下文分离展示，✅ 已覆盖。
- 翻译作品优先入队：当前语言识别、关联版本优先级选择、UI 勾选持久化与 Search/Download 四类入口回归，✅ 已覆盖。
- `source_id/workId` 双键兼容：BJ 等非 `RJ` 作品的详情/轨道解析、Search WorkId 透传与作品页链接生成，✅ 已覆盖。
- 启动补拉失败可视化：补拉超时/单项失败后在 Download 列表中显示 `Failed` 占位与错误信息，✅ 已覆盖。
- 限流可观测性：`RateLimiter` 步骤间节流间隔，⬜ 待落地。

### 2.3 关键实现修复（作为测试补齐的附带产物）

- `QueryParserService`：为筛选 token 缺值场景增加 `ValidateFilterToken`，抛出可读 `ArgumentException`。
- `AsmrApiClient`：在认证预热后显式附加 `Authorization: Bearer <token>` 请求头，使鉴权行为可被测试独立验证。
- `WorkInfoDto` / `EnqueueWorkInfoResolver`：补齐 `translation_info`、`language_editions`、`other_language_editions_in_db` 元数据，并统一 Search/Download/CSV/JSON 入队时的翻译版本优选逻辑，按“简体中文 -> 繁体中文 -> 日本語”选择最终 `SourceId`。
- `FavoriteStore` / `FavoriteFolderDialog` / `SearchView` / `DownloadView`：新增 SQLite 收藏夹、共享收藏夹弹窗与 Search 保存 / Download 导出联动，Search 侧保存收藏时复用“加入翻译作品”优选逻辑。
- `SourceIdNormalizer` / `AsmrApiClient` / `SearchWorkItem`：统一保留 `source_id` 与数值 `workId` 两套标识；兼容 `BJ` 作品 URL 归一化，并在缺少数值编号时通过搜索结果回填 `workId`，避免详情/轨道接口继续误用 `source_id`。
- `AsmrApiClient` / `MetadataSyncService`：新增 `/api/works` 元数据分页同步链路，并按网站总量与本地 SQLite 总量决定是否执行全量同步。
- `UiStateStore` / `MetadataSyncService` / `SyncDownloadService` / `SyncView`：新增元数据同步 / 同步下载进度状态持久化、未完成状态断点继续、已完成状态重置起跑语义与“当前页 / 当前作品完成后停止”的温和停止入口。
- `SyncDownloadService` / `SyncService`：新增同步下载编排层与失败重试入口，按 `SyncWantedSize` 控制累计落盘大小，并支持对 `FAILED` 记录执行目录清理后重试。
- `SyncExportService` / `MetadataSyncStore`：新增按 `FAILED/COMPLETED` 状态筛选的导出链路，支持写出 CSV/JSON，并统一保留 `metadata_work_id/source_id/dir_size/status/file_path/fail_reason/retry_count/has_subtitle` 等字段。
- `SyncReportService` / `SyncView`：新增统计快照与 Sync 页面卡片化报表展示，统一输出元数据总量、字幕拆分、同步完成量、失败/待处理数量和三组同步进度百分比。
- `DatabaseInitializer` / `MetadataSyncStore`：恢复 `MetadataWork/WorkSyncInfo` 为运行时表结构，支持同步元数据去重 upsert、本地统计读取、同步下载候选查询、失败记录查询、Pending 清理与状态回写。
- `ConfigurationService` / `SyncSizeText`：对 `SyncWantedSize` 增加格式校验与字节换算，避免非法容量配置进入同步下载流程。
- `MainWindow` / `SyncView`：新增 `Sync` 页签的失败重试与失败/成功记录导出入口，并扩展同步下载/失败重试/导出摘要展示。
- `SyncView` / `SyncCommandAvailability`：将元数据同步和同步下载各自收敛为单一主按钮，运行中切换为 stop 语义并在 stop request 发出后显示“正在停止...”，同时保持“刷新统计”在同步运行期间仍可手动刷新实时报表与 UiState 进度。
- `DownloadInputNormalizer`：批量输入改为“提交时归一化”，避免实时改写影响输入符号与粘贴体验，同时保持入队前统一规范化与去重。
- `DownloadCommandAvailability`：从 `DownloadView.xaml.cs` 中抽取按钮可用性规则为独立纯状态类，消除 WPF 测试工程占位样例。
- `StartupUnfinishedQueueMetadataRefreshService` / `DownloadTaskListComposer`：启动补拉改为返回成功/失败明细，Download 页面会将失败项显示为 `Failed` 占位行并附带错误信息。
- `ShellViewModel` / `NavigationService` / `UiMessageService`：新增壳层状态、导航与状态消息服务，将主窗口页签启用状态、选中页签与底部状态栏文案收敛到统一状态源。
- `DialogService` / `DialogFileNamePolicy`：统一 Search 导出、Download 从文件导入与 Sync 导出保存对话框流程，并集中处理 CSV/JSON 默认后缀决策。
- `ShellResources.xaml`：抽取 Search/Download/Library/Settings 共享卡片、输入框、按钮与背景基样式，减少页面内重复视觉定义。

---

## 3. Github提交记录

本节用于记录本仓库与迁移文档相关的计划提交和已提交记录，便于追踪文档同步、仓库清理与代码迁移边界。

提交策略：如果仅有提交明细的改动，不做单独提交，与后续的代码改动一同提交。

### 3.1 计划与提交明细

| 提交日期   | 状态   | 提交总结(Summary)                                                           | 提交描述(Description)                                                                                                                                                                                                                                                                                                                                                                                                              | Commit SHA |
| ---------- | ------ | --------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------- |
| 2026-03-18 | 已提交 | WPF migration first submission                                              | 1. Completed Phase 0: Initialize the .NET project<br>2. Completed Phase 1: Configure and initialize migration<br>3. Completed Phase 2: API and authentication migration<br>4. Completed Phase 3: Search capability migration<br>5. Completed Phase 4: Download capability migration                                                                                                                                                | d36ead7    |
| 2026-03-18 | 已提交 | feat(dotnet): consolidate endpoint url flow and startup warmup              | 1. Add configurable endpoint discovery flow with persisted API base URL and runtime current-base-url usage in API/Auth services<br>2. Keep Discover calls only for startup warmup and connectivity probe, and make startup warmup non-blocking with timeout/fail-open behavior<br>3. Add infrastructure and WPF regression tests, then sync progress sections 1.5/2.1/3.1                                                          | 43eddcd    |
| 2026-03-27 | 已提交 | align docs to WPF; align runtime/docs version to 0.4                        | 1. Adjust README.md to focus on the WPF client; and move the original Go CLI/WebUI examples to docs/legacy-go.md as historical references.<br>2. Add LICENSE with copyright holder kirahsosha.<br>3. Uniformly set Version to the 0.4 series in the WPF runtime projects.<br>4. Add v0.4 display in MainWindow, Settings, and startup logs, and synchronize the version identifiers in README and the WPF migration documentation. | f388a2f    |
| 2026-03-27 | 已提交 | feat(dotnet): fix sort/dedup bugs, add hd-audio filter and csv/json import  | 1. Fix status column sort order.<br>2. Prevent duplicate RJID entries in download list.<br>3. Add HD audio only checkbox to exclude mp3 when wav/flac exists.<br>4. Add Import CSV/Import JSON buttons.<br>5. Add regression tests and sync progress sections.                                                                                                                                                                     | 146c9f5    |
| 2026-03-27 | 已提交 | fix(search/download): preserve filters, accurate queue counts, v0.4.1 sync  | 1. Update version to v0.4.1.<br>2. Fix Search clear behavior to preserve advanced filters.<br>3. Fix paging searching logic.<br>4. Add queue-count policy to report accurate added/skipped counts.<br>5. Add `hd_audio_only` config.<br>6. Add/refresh regression tests.                                                                                                                                                           | 527c02e    |
| 2026-03-29 | 已提交 | feat(state/config): sqlite persistence + restore-path hardening             | 1. Update version to v0.4.2.<br>2. Keep settings/search/download UI-state persistence in SQLite.<br>3. Add program-directory `config.json` as default configuration source.<br>4. Remove `config.toml` runtime dependency and `Tomlyn`.<br>5. Remove legacy downloader fields and obsolete fallback branches.<br>6. Update regression tests.                                                                                       | 3afbee3    |
| 2026-03-29 | 已提交 | fix(search): trigger option-only query + rename DashboardView to SearchView | 1. Bump runtime/UI/docs version to v0.4.3 and align related assertions.<br>2. Fix Search behavior.<br>3. Rename DashboardView to SearchView and sync DI/shell host naming.<br>4. Update regression tests.                                                                                                                                                                                                                          | b3374e2    |
| 2026-03-30 | 已提交 | feat(search): preserve multi-select + export scopes, sync v0.4.4            | 1. Add right click menu in Search ResultsGrid.<br>2. Update version to v0.4.4.<br>3. Update regression tests.                                                                                                                                                                                                                                                                                                                      | 83c84fa    |
| 2026-03-30 | 已提交 | fix(settings): stay on settings after reinit, sync v0.4.5                   | 1. Update version to v0.4.5.<br>2. Remove version from main window title.<br>3. Fix Settings saving success path to stay on Settings tab.<br>4. Update regression tests.                                                                                                                                                                                                                                                           | 0f014d6    |
| 2026-03-30 | 已提交 | v0.4.6: replace ILogger injections with static NLog logger                  | 1. Update version to v0.4.6.<br>2. Replace Logging with NLog 6.1.1.<br>3. JSON format, size/date rolling, archive to subdirectory.<br>4. Update regression tests.                                                                                                                                                                                                                                                                  | 68c44bf    |
| 2026-03-31 | 已提交 | v0.4.7: tags column, shell folder reuse, import count fixes                 | 1. Update version to v0.4.7.<br>2. Search DataGrid removes 评分/销量 and adds 标签 column.<br>3. CSV header `source_id,has_subtitle,release,tags,title`.<br>4. Update regression tests.                                                                                                                                                                                                                                            | da37fcb    |
| 2026-03-31 | 已提交 | v0.4.8: harden startup refresh and tune Search/Download columns             | 1. Update runtime version to v0.4.8.<br>2. Switch version text to dynamic assembly version.<br>3. Add centralized constants/query class.<br>4. Use startup warmup to fix blank titles after restart.<br>5. Tune Search/Download DataGrid layout.<br>6. Update regression tests.                                                                                                                                                    | 0b7644c    |
| 2026-04-01 | 已提交 | v0.4.9: translated queue, BJ/source fixes, prompt polish                    | 1. Update runtime version to v0.4.9.<br>2. Update AI-readable prompts.<br>3. Add persisted “加入翻译作品” options.<br>4. Preserve Search `WorkId`, resolve non-RJ `source_id` values via numeric work-id fallback, and fix valid browser/work/tracks paths.<br>5. Adjust UI display.<br>6. Avoid duplicate output suffixes.<br>7. Update regression tests and api samples.                                                         | eccd533    |
| 2026-04-02 | 已提交 | v0.4.10: add shared workinfo cache                                          | 1. Update runtime/docs version to v0.4.10.<br>2. Add shared WorkInfo cache with 1-hour per-entry TTL.<br>3. Warm Summary cache from Search/Popular result and upgrade to Full details on demand for downloads/tracks.<br>4. Route Search/Download/startup refresh through the shared cache contract.<br>5. Update regression tests and progress documentation.                                                                     | 60e2f70    |
| 2026-04-02 | 已提交 | v0.4.11: add favorites flow and unify search/download actions               | 1. Update runtime/docs version to v0.4.11.<br>2. Add SQLite favorite storage, and Search favorite-save flow.<br>3. Add Download favorite-import flow and queue integration.<br>4. Merge Search export actions for the main button and context menu, and merge Download CSV/JSON import actions.<br>5. Merge retry behavior and adjust the Download action-button.<br>6. Update regression tests and progress documentation.        | 6566c05    |
| 2026-04-03 | 已提交 | v0.5.0: land phase 5 metadata/download/retry/export/report flow             | 1. Update runtime/docs version to v0.5.0.<br>2. Start phase 5 with metadata sync API, SQLite store, and application service.<br>3. Add sync-download orchestration with capacity control, failure retry, export report, and status persistence.<br>4. Expand the Sync tab with metadata/download/retry/export actions plus statistics cards and local summary panels.<br>5. Update regression tests and progress documentation.    | 78fbe7d    |
| 2026-04-03 | 已提交 | v0.5.1: persist sync progress and harden sync controls                      | 1. Update runtime/docs version to v0.5.1.<br>2. Persist metadata-sync and sync-download progress in SQLite and resume unfinished runs on restart.<br>3. Replace separate start/stop controls with merged sync action buttons, and add a 1-second debounce window.<br>4. Keep Refresh button available during running sync and refresh live report/progress snapshots.<br>5. Update regression tests and progress documentation.    | f3a2b42    |
| 2026-04-08 | 已提交 | v0.5.2: unify workinfo dto, refresh stale metadata, stream real downloads   | 1. Update runtime/docs version to v0.5.2.<br>2. Modify configuration.<br>3. Prefer local MetadataWork for Search/Download before refresh from API.<br>4. Refresh expired metadata and rescan completed sync downloads.<br>5. Replace placeholder download outputs with real download streaming.<br>6. Unify Search/Download/Sync work-info flows on shared Dto.<br>7. Update regression tests and progress documentation.          | e0ff664    |
| 2026-04-08 | 已提交 | v0.5.3: fix endpoint probe path and clarify sync progress counts            | 1. Update runtime/docs version to v0.5.3.<br>2. Fix HTML/script fetch failures, flexible script markup, replace the candidate latency probe with a GET-safe endpoint.<br>3. Persist metadata-sync local counts plus cumulative processed-work counts.<br>4. Refactor Settings save/test-connection flow.<br>5. Add XML docs to direct control handlers in View files.<br>6. Update regression tests and progress documentation.    | 81a8fcb    |
| 2026-04-10 | 已提交 | v0.5.4: persist published API candidates and harden endpoint discovery      | 1. Update runtime/docs version to v0.5.4.<br>2. Keep the pending metadata-sync count persistence, Settings save/test-connection flow cleanup.<br>3. Switch candidate latency probing to health api.<br>4. Expand built-in API candidates, and persist discovered candidate lists back to SQLite.<br>5. Fix test connection and the discovered candidate list logic.<br>6. Update regression tests and progress documentation.      | d6be49d    |
| 2026-04-11 | 已提交 | v0.5.5: align runtime version and streamline anonymous endpoint probe       | 1. Update runtime/docs version to v0.5.5.<br>2. Keep `AsmrProbe` as the anonymous endpoint probe client and centralize its shared HTTP transport configuration.<br>3. Reuse one probe client across endpoint discovery and keep publish-source / health requests on the same probe header policy.<br>4. Update regression tests and progress documentation.                                                                        | 71a771a    |
| 2026-04-11 | 已提交 | v0.5.6: validate track size before reusing existing downloads               | 1. Update runtime/docs version to v0.5.6.<br>2. Add `TrackDto.Size` to capture track api size metadata.<br>3. Reuse existing files only when the local file size matches the track size; otherwise redownload the file.<br>4. Validate downloaded file size against track metadata.<br>5. Update regression tests and progress documentation.                                                                                      | 2d9ac15    |
| 2026-04-12 | 已提交 | v0.5.7: mirror standard downloads into sync root                            | 1. Update runtime/docs version to v0.5.7.<br>2. Avoid duplicate file processing when the normal download root equals the sync root.<br>3. Mirror full standard downloads to the sync root when both roots start empty, and write `WorkSyncInfo` as `COMPLETED`.<br>4. Update regression tests and progress documentation.                                                                                                          | 107b7e2    |
| 2026-04-14 | 已提交 | v0.5.8: fix download and sync command states                                | 1. Update runtime/docs version to v0.5.8.<br>2. Fix Download toolbar state.<br>3. Allow Sync Refresh Status during Retry Failed and split the Sync status panel into separate metadata/download lines.<br>4. Update regression tests and progress documentation.                                                                                                                                                                   | 97e3dc4    |
| 2026-04-14 | 已提交 | v0.5.9: harden publish-source discovery and merge candidate persistence     | 1. Update runtime/docs version to v0.5.9.<br>2. Refactor publish-source HTML/script fetching to use a dedicated client with shared transport rules, timeout, cancellation and test seams.<br>3. Persist merged discovered-plus-saved API candidates only when the merged candidate count grows, while keeping current BaseUrl synchronization.<br>4. Update regression tests and progress documentation.                           | 116c8ad    |
| 2026-04-15 | 已提交 | v0.6.0: kick off phase 6 library scan/query shell                           | 1. Update runtime/docs version to v0.6.0.<br>2. Start phase 6 with dual-format local library scanning, metadata overlay, query/paging and playback-context abstraction.<br>3. Add the `Library` tab and WPF library page shell.<br>4. Add Core/Application/WPF regression tests for the stage 6 first batch.<br>5. Update regression tests and progress documentation.                                                             | 04dea2f    |
| 2026-04-15 | 已提交 | v0.6.1: switch Library playback to shell-open selected media                | 1. Update runtime/docs version to v0.6.1.<br>2. Replace the MediaPlayer-based Library playback flow with explicit selected-media loading plus system-default-app open behavior.<br>3. Remove pause/stop and inline progress UI.<br>4. Update regression tests and progress documentation.                                                                                                                                          | 994ba3c    |
| 2026-04-16 | 已提交 | v0.6.2: harden Library scan tolerance and selection feedback                | 1. Update runtime/docs version to v0.6.2.<br>2. Harden Library scan tolerance so nested directory/file errors become local error records instead of failing the whole work item.<br>3. Add explicit Library selection feedback for directory, non-playable, missing and playable file states, and tighten action-button availability.<br>4. Update regression tests and progress documentation.                                    | f3e6443    |
| 2026-04-16 | 已提交 | v0.6.3: close phase 6 Library DoD                                           | 1. Update runtime/docs version to v0.6.3.<br>2. Extract shared playable-media rules for Library scanning and selection guidance.<br>3. Add work-level Library guidance plus separated current-selection and loaded-context messaging in the existing Library page while keeping explicit file selection before load/play.<br>4. Update regression tests and progress documentation.                                                | 1a35bc6    |
| 2026-04-17 | 已提交 | v0.6.4: polish Library styles                                               | 1. Update runtime/docs version to v0.6.4.<br>2. Polish the Library page styles by simplifying the subtitle copy, splitting the filter and details cards, localizing the works-grid header text, and aligning the top-level visual layout.<br>3. Localize the Search results first-column header text to match the current UI wording.<br>4. Update regression tests and progress documentation.                                    | ab0a79e    |
| 2026-04-18 | 待提交 | v0.7.0: kick off phase 7 shell integration                                  | 1. Update runtime/docs version to v0.7.0.<br>2. Add shared state/navigation/message/dialog services and resource dictionary.<br>3. Bind the main window to shell-driven page state.<br>4. Route Search/Download/Sync file dialogs through the shared dialog service.<br>5. Align Search/Download/Library/Settings page styles to the shared base styles.<br>6. Update regression tests and progress documentation.                 | -          |

---

## 4. 功能测试验证清单

本章用于指导测试人员对当前已交付的 WPF 客户端执行功能测试与回归测试。当前范围覆盖阶段 1 到阶段 6，以及阶段 7 当前已落地的壳层导航、共享对话框与共享样式基线。

AI约束：每次进行功能开发、缺陷修复或任何可能影响用户可见行为的改动时，必须先检查本章并将受影响的测试项重置为未勾选；待对应的功能测试或回归测试通过后，再重新勾选，并在必要时同步更新第 1.4 节和第 1.5 节记录。

说明：

- `[ ]`：本轮尚未验证，或因代码改动需要重新验证。
- `[x]`：本轮已验证通过。
- 任一项失败时，不得勾选该项；需要在第 1.4 节登记阻塞，或在第 1.5 节补充验证结果。

### 4.1 启动、配置与连接

- [x] 应用可正常启动，主窗口可显示 Search、Download、Library、Sync、Settings 五个页签，且启动过程不因站点发现流程长时间阻塞。
- [x] Settings 页面可正确加载现有配置；下载目录、同步下载目录、元数据有效期与格式优先级等字段显示完整。
- [x] 程序目录 `config.json` 可作为默认配置来源；当 SQLite 中无配置记录时，应用可读取该默认配置并完成设置页加载。
- [x] 在 Settings 页面修改有效配置后，“保存并重新初始化”可成功完成，状态提示明确，应用进入可用状态。
- [x] Settings 页面可分别编辑下载目录、同步下载目录与元数据有效期，并在“保存并重新初始化”后生效。
- [x] 在 Settings 页面点击“保存并重新初始化”后，当前页应保持在 Settings，不应自动跳转到 Search。
- [x] Settings 页面输入无效配置（含非法 `SyncWantedSize`）时，可给出可读错误提示，且应用不崩溃。
- [x] “测试连接”可完成站点发现与登录校验；成功时回填当前 BaseUrl 并显示延迟与鉴权结果，失败时显示明确原因。
- [x] 当发布源 HTML 或入口脚本抓取失败时，“测试连接”仍会回退到可用 BaseUrl，不因 `GetStringWithTimeoutAsync` 的非 2xx / 超时失败而中断整条探测链路。
- [x] 当发布页入口脚本使用相对路径、查询串或单引号 `link` 配置时，“测试连接”仍可正确解析入口脚本并发现可用 BaseUrl。
- [x] “测试连接”应通过 `GET /api/health?cache=false` 完成候选探测；当旧探测端点不可用但 health API 可达时，仍能选中可用 BaseUrl。
- [x] 当发布页正文直接提供 `asmr-300/200/100/one` 最新域名时，“测试连接”会按正文顺序补齐候选列表并写回 SQLite `ApiCandidateUrls`；随后再次“保存并重新初始化”不会把新候选覆盖回旧值。
- [x] 当发布页 discovery 仅返回部分新候选时，若与已保存候选集合合并后的总数更大，“测试连接”会保留旧候选并把合并后的 `ApiCandidateUrls` 写回 SQLite。
- [x] 主窗口标题不显示版本号，Settings 页面版本文案应显示 v0.7.0。

### 4.2 Search 功能

- [x] 仅输入基础关键词即可成功搜索，并展示结果列表（含标签列）、总数和页码信息。
- [x] Search 结果列表列头应显示完整边框；首列标题显示“作品ID”；字幕列与日期列宽保持 `42/75` 且不可拖拽改宽，拖拽仅改变列顺序；当标题或标签过宽时可通过横向滚动查看完整数据。
- [x] 高级筛选 `tag/circle/va/duration/rate/price/sell/age/lang` 可单独或组合生效，`反选` 语义正确。
- [x] Search 的排序、方向、字幕、“包含翻译作品”“加入翻译作品”等选项生效，翻页后条件保持不丢失。
- [x] 上一页、下一页、跳页、页大小切换均可用，分页结果与页码信息正确。
- [x] “查询热门作品”可返回结果并展示到结果列表，日期、字幕、标签信息正确。
- [x] “清空”可重置关键词、排序选项、分页状态和当前结果，同时保留高级筛选输入与反选状态。
- [x] Search 页面会恢复上一次运行时的“包含翻译作品”“加入翻译作品”与高级筛选输入/反选状态，页面切换与重启后保持一致。
- [x] 选中部分结果点击“加入下载队列”时，仅将选中项入队；未选中任何结果时，可按当前结果集批量入队。
- [x] Search 页面未开启“加入翻译作品”时，入队会优先复用未过期 `MetadataWork`；当元数据缺失或超过元数据有效期时，会自动补拉 API 后继续入队。
- [x] 已存在于下载列表中的作品不会重复入队，页面提示中会明确说明新增数量、跳过数量与翻译切换数量（如适用）。
- [x] Search 页面开启“加入翻译作品”后，选中入队与当前结果批量入队都会按“简体中文 -> 繁体中文 -> 日本語”选择最终版本；关闭后保持当前结果的原始 `SourceId`。
- [x] Search 页面开启“加入翻译作品”且实际切换语言版本时，选中入队与当前结果批量入队的状态提示会显示“其中 X 项已切换为翻译作品”。
- [x] Search 页面在未选中任何结果时点击“收藏作品”，左下角应提示“请先选择需要加入收藏的作品”。
- [x] Search 页面“收藏作品”弹窗可选择现有收藏夹，也可输入新的收藏夹标题并保存到本地 SQLite。
- [x] Search 页面开启“加入翻译作品”后保存收藏时，应按“简体中文 -> 繁体中文 -> 日本語”保存最终版本；关闭后保持当前结果的原始 `SourceId`。
- [x] Search 页面“导出到文件”可成功导出全部任务到 CSV 与 JSON，导出文件内容可正常打开且关键字段完整，导出后自动打开文件夹。
- [x] Search 页面在选中行存在时可通过“导出到文件”导出选中任务到 CSV 与 JSON；无选中行时会回退导出全部任务。
- [x] Search 任务列表支持右键菜单，且包含“加入下载队列 / 导出全部任务到文件 / 导出选中任务到文件 / 在浏览器打开”四项操作；两个导出入口均可继续选择 CSV 或 JSON。
- [x] Search 结果在多选状态下右键未选中行时，不应清空或追加现有选中集合。
- [x] Search 任务列表右键“在浏览器打开”仅对当前右键命中项生效，且 URL 按 `workPageUrlTemplate` 与 `{RJID}` / `WorkId` 替换规则生成。
- [x] 查询热门作品命中 `BJ02370869` 这类 `source_id/workId` 不一致的作品时，开启“加入翻译作品”后仍可正常入队，不出现解析失败或空入队。

### 4.3 Download 功能

- [x] 单个 RJID 入队支持 `RJxxxx`、作品 URL、`RJ-xxxx`、纯数字等输入形式，提交后可归一化并成功入队。
- [x] 批量入队支持逗号、分号、空格、换行混合分隔；重复项会去重，已存在任务不会重复加入。
- [x] “从文件导入”可选择 CSV 或 JSON 文件并成功读取 Search 导出结果入队，重复任务会被跳过且提示明确。
- [x] “从收藏夹导入”可弹出仅允许选择现有收藏夹的下拉框，并将所选收藏夹中的作品加入下载队列。
- [x] 从收藏夹导入时，已存在于下载列表或待下载队列中的作品会被跳过，状态提示会显示新增数量、跳过数量与当前队列总数。
- [x] 执行下载队列后，任务列表与队列计数会刷新，任务状态、进度、目标目录、错误信息显示正确。
- [x] 状态列排序遵循业务顺序而非字母序；状态文案显示为中文且与实际状态一致。
- [x] Download 任务列表列头应显示完整边框；状态列宽保持 `50` 且不可拖拽改宽，进度列宽保持 `96` 且允许调整，拖拽仅改变列顺序；当目录或错误信息过宽时可通过横向滚动查看完整数据。
- [x] “立即下载选中任务”可对 Pending、Failed、Canceled 等允许状态生效，不允许的状态不会误触发。
- [x] “取消选中任务”可取消 Pending、Queued、Running 任务，确认提示、取消结果与列表状态一致。
- [x] Download 页面操作按钮顺序应为“立即下载选中任务 -> 执行下载队列 -> 刷新任务列表”。
- [x] “重试失败任务”在存在选中项时仅重试选中的失败任务并忽略非失败项；无选中项时会批量重试全部失败任务，并能输出正确汇总结果。
- [x] “打开下载目录”可打开当前生效的普通下载目录。
- [x] 点击“立即下载选中任务”或“执行下载队列”后，“打开下载目录”仍保持可点击，并能打开当前生效的普通下载目录。
- [x] 点击“立即下载选中任务”或“执行下载队列”后，“从文件导入”与“加入单个下载 / 加入批量下载 / 从收藏夹导入”保持一致的禁用与恢复行为。
- [x] 普通下载目录缺失目标文件且同步下载目录已存在同路径、同大小的匹配文件时，执行下载会优先复用同步下载目录中的真实文件，不重复请求下载接口，并将对应 SQLite 同步记录写为 `COMPLETED`。
- [x] 当目标文件或同步下载目录候选文件已存在但大小与 track `size` 不一致时，下载流程会忽略旧文件并重新下载真实文件。
- [x] 当普通下载目录和同步下载目录都没有匹配文件时，执行下载会直接请求 `mediaDownloadUrl` 实时落盘；完整普通下载会同时镜像一份到同步下载目录，并将 SQLite 的 `WorkSyncInfo` 写为 `COMPLETED`，不写 `source/title/url` 占位文本。
- [x] 当普通下载目录和同步下载目录配置为同一路径时，执行下载只会在该目录落盘单份文件，不会在同一路径重复复制或重复处理，同时会将 SQLite 的 `WorkSyncInfo` 写为 `COMPLETED`。
- [x] “清空任务列表”可停止运行中任务、清空下载队列并删除任务列表项，且清空后重启不会回流旧未完成队列。
- [x] Download 页面会恢复上一次运行时的“只下载高清音频”“加入翻译作品”“文件筛选”与未完成队列（`Pending/Queued/Failed` 恢复为 `Pending`），并在后台补拉缺失作品标题后刷新列表显示。
- [x] Search 页面加入下载队列后，若未切换至 Download 页面即退出并重启，未完成队列仍可恢复。
- [x] 文件筛选规则可生效；开启“只下载高清音频”后，在同时存在 flac/wav 与 mp3 的场景下不会重复下载 mp3。
- [x] 新启动的失败任务不会从列表中消失；失败、取消、完成后的任务状态可被稳定追踪。
- [x] Download 页面开启“加入翻译作品”后，单个入队、批量入队、从文件导入（CSV/JSON）三条入口都会按“简体中文 -> 繁体中文 -> 日本語”选择最终版本；关闭后保持输入或导入文件中的原始 `SourceId`。
- [x] Download 页面开启“加入翻译作品”且实际切换语言版本时，单个入队、批量入队、从文件导入（CSV/JSON）的状态提示会显示“其中 X 项已切换为翻译作品”。
- [x] 启动后台补拉结束后，Download 页状态提示统一使用“作品信息更新完成/失败”文案，不再出现“启动补拉完成/失败”。
- [x] 当轨道标题已自带扩展名时，下载落地文件名不会出现 `.mp3.mp3`、`.png.png` 等重复后缀；无扩展名标题仍会补齐正确后缀。
- [x] 启动后台补拉作品信息失败时，Download 列表中的未完成队列占位项会显示为 `Failed` 且错误信息可读。

### 4.4 Search/Download 联动与回归

- [x] Search 页面加入下载队列后，Download 页面可看到对应待执行任务，标题信息尽量不丢失。
- [x] Search、Download、Sync 三个页面的状态信息面板样式保持一致，长文本可换行显示且不遮挡正文内容。
- [x] Search、Download、Sync 三页的作品信息解析统一走共享 `WorkInfoDto` 后，Search 入队、Download 标题刷新与 Sync 创建待处理记录时的标题、`SourceId` 与字幕标记保持一致，不出现 DTO 分歧。
- [x] 已取消任务再次从 Search 侧或 Download 侧触发下载时，可复用原任务行并回流为待执行/执行中状态，不新增重复行。
- [x] Search 与 Download 页面之间来回切换后，队列数量、任务状态和标题缓存保持一致，不出现旧状态残留。
- [x] Search 页面主按钮或右键菜单“导出到文件” -> Download “从文件导入” -> 执行下载的链路可端到端跑通（标签列 + 打开文件夹）。
- [x] Search 保存收藏 -> Download 从收藏夹导入 -> 执行下载的链路可端到端跑通，且收藏夹中保存的最终 `SourceId` 在导入后保持一致。
- [x] 重启应用后，Search 与 Download 页面中的收藏夹弹窗仍可读取同一批 SQLite 收藏夹与作品。
- [x] 重复入队防护在 Search 入队、Download 单个入队、Download 批量入队、从文件导入（CSV/JSON）四条入口上行为一致。
- [x] Search 与 Download 页面之间来回切换、或在重启后恢复未完成队列时，若最终入队版本被切换为翻译作品，标题与 `SourceId` 显示仍保持一致，不出现原始版本与最终版本错位。
- [x] 清空任务列表后不关闭程序，重新从 Search/Download 对同一作品入队时应优先复用进程内作品缓存；关闭程序或超过 1 小时后再次操作时仍能自动补拉并保持标题正确。
- [x] 清空任务列表后再从 Search 页面重新入队时，状态提示与实际新增数量一致，不会出现“失败 1 项”但实际 0 项入队的误报。
- [x] 连续执行“搜索 -> 入队 -> 立即下载/执行队列 -> 刷新列表 -> 重试/取消”后，应用无崩溃、无明显 UI 状态错乱。

### 4.5 Sync 功能

- [x] Sync 页面可显示本地元数据总量、字幕数量与最近更新时间。
- [x] Sync 页面可显示同步下载完成/失败/待处理数量、队列中数量与当前已落盘大小摘要。
- [x] 元数据同步主按钮在空闲态显示“开始同步元数据”，点击后会切换为“停止同步元数据”。
- [x] 点击元数据同步主按钮开始同步后，可成功拉取网站元数据并写入 SQLite `MetadataWork` 表。
- [x] 当网站总量与本地一致且不存在过期元数据时，Sync 页面会提示“无需同步”，且不会重复写入数据。
- [ ] 当上次元数据同步状态为已完成且存在超过“元数据有效期”的本地记录时，再次点击“开始同步元数据”会执行过期刷新，并在摘要中显示过期刷新结果。
- [x] 当 SQLite `UiState` 中存在未完成元数据同步进度时，再次点击元数据同步主按钮会从记录页码继续执行，而不是从第一页重新开始。
- [x] 点击元数据同步主按钮发出 stop request 后，按钮会切换为“正在停止元数据...”，并在当前页完成后恢复为空闲态。
- [x] 元数据同步开始后的 1 秒内连续双击主按钮时，第二次点击会被防抖忽略，按钮保持“停止同步元数据”，不会误切到“正在停止元数据...”。
- [x] 元数据同步进行中时，点击“刷新统计”后，顶部“本地元数据”摘要、状态文本与详情区会显示当前已持久化的本地总量、字幕量、累计处理条数与累计新增数。
- [x] 同步完成后，页面摘要与详情区会显示网站总量、本地总量、累计处理条数、累计新增数量与分页处理结果。
- [x] 下载同步主按钮在空闲态显示“开始同步下载”，点击后会切换为“停止同步下载”。
- [x] 点击下载同步主按钮开始同步后，可按 `SyncWantedSize` 逐项处理待同步作品、下载真实媒体文件，并将 `WorkSyncInfo` 写为 `COMPLETED/FAILED`。
- [ ] 当累计落盘大小达到 `SyncWantedSize` 后，Sync 页面会提示已达到容量上限，且停止后续候选作品处理。
- [x] 同步下载完成后，页面摘要会显示成功数、失败数、同步前后已落盘大小与剩余待同步数量。
- [x] 当 SQLite `UiState` 中存在未完成同步下载进度时，再次点击下载同步主按钮会继续后续候选作品，且不会重下已有 `COMPLETED/FAILED` 记录。
- [x] 点击下载同步主按钮发出 stop request 后，按钮会切换为“正在停止下载...”，并在当前作品完成后恢复为空闲态。
- [x] 同步下载开始后的 1 秒内连续双击主按钮时，第二次点击会被防抖忽略，按钮保持“停止同步下载”，不会误切到“正在停止下载...”。
- [ ] 当同步下载进度已完成时，再次点击下载同步主按钮会重置 UiState 进度，并从头校验已有 `WorkSyncInfo` 记录；仅对目录缺失或文件大小不一致的作品补齐，不重复写入已匹配作品；对于已由完整普通下载写成 `COMPLETED` 的作品，会直接跳过，不重复处理。
- [x] 点击“重试失败项”后，Sync 页面会逐项清理旧失败目录并重试全部 `FAILED` 记录。
- [x] 失败重试完成后，页面摘要会显示本次重试数量、恢复成功数量、剩余失败数量与当前已完成总数。
- [x] 点击“导出失败记录”后，可通过保存对话框将全部 `FAILED` 同步记录导出为 CSV 或 JSON 文件。
- [x] 点击“导出成功记录”后，可通过保存对话框将全部 `COMPLETED` 同步记录导出为 CSV 或 JSON 文件。
- [x] 导出文件内容需包含 `metadata_work_id/source_id/dir_size/status/file_path/updated_at/fail_reason/retry_count/failed_at/has_subtitle` 字段，且导出完成后可在资源管理器中定位文件。
- [x] Sync 页面统计卡片需显示元数据总量、字幕数量、无字幕数量、同步完成数量、失败数量、待处理数量与已落盘大小。
- [x] Sync 页面统计卡片需显示总进度、字幕进度与无字幕进度，且当本地元数据为空时应显示 `0.00%`，不出现异常值。
- [x] 在元数据同步或同步下载运行期间，“刷新统计”仍可点击，并能刷新当前报表、摘要面板和 UiState 进度详情。
- [x] 点击“重试失败项”后，“刷新统计”仍可点击，并能刷新当前下载状态、报表与摘要面板，同时保留当前重试详情，不被旧的 `UiState` 下载进度覆盖。
- [x] Sync 状态面板以两行分别显示“同步状态”和“下载状态”，且元数据同步与同步下载的状态文本不会互相覆盖。
- [x] “刷新统计”在自身执行期间会临时禁用，但不会把当前同步主按钮从 stop 语义切回 start 语义。
- [x] 点击“刷新统计”或完成同步/重试后，统计卡片与摘要面板需同步刷新，最近更新时间应同时反映元数据与同步记录的最新时间。

### 4.6 Library 功能

- [x] Library 页签可成功显示资源库页面；顶部副标题显示为“本地资源库，文件查看与播放。”，首次刷新后可看到作品总数、页码信息与状态文本更新。
- [x] 当下载目录/同步目录中同时存在 `[{SourceId}]{Title}` 与 `sourceId-date-sub/nosub-title` 两种命名格式时，Library 页面都能识别作品；其中 legacy 标题包含连字符时也不会漏显；非法目录会被跳过并在状态中体现，不导致应用崩溃。
- [x] 首次刷新或切换分页期间，窗口保持可响应；刷新/翻页/跳页/page size 切换期间相关按钮在扫描完成前会临时禁用，完成后恢复。
- [x] Library 页的关键字筛选、“仅显示带字幕作品”“仅显示含音频作品”与上一页/下一页/跳页/page size 切换均可正确更新列表结果。
- [x] 选中作品后，右侧独立“作品详情”区与文件树会同步更新；选中文件后点击“载入/切换文件”可更新当前打开目标；点击“清空上下文”会清除当前播放器状态，并按当前保留的作品/文件选择显示对应提示。
- [x] Library 作品列表首列标题显示“作品ID”；当标题、路径或结果较多时可通过横向与纵向滚动条查看完整内容，不会把显示内容撑出窗口范围。
- [x] Library 页面支持上一页、下一页、跳页与 page size 切换，页码输入、结果数量与页码信息保持一致。
- [x] 手动拉高窗口时，左侧作品列表与右侧文件树会随可用高度同步扩展，下边沿保持对齐，不再因固定高度出现错位。
- [x] 手动压低窗口时，作品列表、详情区、文件树与上下文文本都保持在窗口范围内显示，超出部分通过各自滚动区域承载。
- [x] 仅选中作品但未选中文件时，Library 页会提示当前作品是否包含可播放媒体文件，并给出首个候选路径或“未发现可播放媒体文件”的明确说明。
- [x] 当在文件树中选中目录、不可播放文件、缺失媒体文件与可播放媒体文件时，Library 页会分别显示明确提示，并正确更新“载入/切换文件”“播放”按钮可用性。
- [x] 当作品文件使用大小写混合的支持扩展名（如 `.FLAC`、`.OpUs`）时，Library 页仍会识别为可播放媒体文件，并允许后续载入与播放。
- [x] 显式选中可播放媒体文件后，Library 页可点击“播放”调用系统默认关联程序打开；未选中文件时“播放”不可点击。
- [x] 当选中文件缺失或系统默认关联程序打开失败时，Library 页会显示明确失败提示，不导致应用崩溃。
- [x] 当系统默认关联程序已成功拉起但底层 Shell 调用返回 `null` 时，Library 页不应误报“打开失败”。
- [x] Library 页不再显示“暂停 / 停止”按钮和时间进度文本；“当前选择”与“当前已载入上下文”会分开展示，且已载入区会显示状态、作品、当前文件与最近一次打开结果。
- [x] 清空上下文后，“当前已载入上下文”应恢复为未载入状态，但“当前选择”区仍保留当前作品或文件对应的提示，不与已载入状态混淆。

### 4.7 阶段 7 壳层与体验收口

- [x] 初始化完成前，Settings 页签保持可用，Search/Download/Library/Sync 四个主功能页签保持禁用；初始化成功后四个主功能页签统一恢复可用。
- [x] Search、Download、Library、Settings 四页在 `1280x720` 与更大窗口尺寸下共用卡片/输入框/按钮视觉基线，不出现局部样式回退或边距错位。
- [x] Search 页面导出、Download 页面从文件导入、Sync 页面导出失败/成功记录均复用统一文件对话框流程，CSV/JSON 默认后缀补齐正确。
- [x] 在 Settings 页面执行“保存并重新初始化”成功或失败后，主窗口底部状态栏会通过统一壳层消息区显示结果，且当前页仍保持在 Settings。
