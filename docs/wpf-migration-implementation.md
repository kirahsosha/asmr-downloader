# asmr-downloader WPF 迁移实施文档

## 1. 详细迁移实施方案

## 1.1 总体实施原则

### 原则 1：先迁服务层，再迁界面层

原因：

- 现有业务逻辑主要集中在 `internal/engine`、`internal/model`、`internal/database`
- 如果先画界面而没有稳定服务层，后续会出现大量 UI 改动

### 原则 2：保持数据目录和数据库兼容优先

建议在第一阶段尽量兼容：

- `.asmroner-data/`
- `config.toml`
- `asmroner.db`
- 同步下载目录命名规则

原因：

- 这样可以直接复用现有用户数据
- 避免首个迁移版本就同时做数据迁移

### 原则 3：保留现有搜索语法兼容能力

原因：

- 现有 CLI 用户可能已经依赖高级搜索表达式
- WPF 虽可增加图形化筛选器，但不应破坏已有查询语义

### 原则 4：后台任务必须与 UI 解耦

要求：

- 下载、同步、热门下载、重试必须运行在后台任务中
- UI 只负责绑定状态和发出命令
- 不允许把下载逻辑直接写进 ViewModel

## 1.2 分阶段实施步骤

### 阶段 0：创建解决方案骨架

目标：

- 创建 WPF 项目
- 创建 Core / Infrastructure / Application 类库项目
- 建立依赖注入和日志基础设施

输出：

- 可启动的空 WPF 应用
- 可被 WPF 注入的服务层骨架

### 阶段 1：迁移配置与初始化能力

目标：

- 迁移 `Config` 模型
- 实现配置文件读取与保存
- 实现首次启动检查
- 实现数据库初始化与目录创建

具体事项：

- 定义 `UserOptions`、`DownloaderOptions`、`LimitOptions`
- 实现 `ConfigurationService`
- 实现 `ApplicationBootstrapper`
- 实现 SQLite 初始化

建议：

- 第一阶段可继续读取现有 TOML，降低切换成本
- 若后续需要改为 JSON，应增加导入器，而不是直接破坏兼容

### 阶段 2：迁移 API 客户端与认证能力

目标：

- 完成 asmr.one 登录
- 完成最快站点检测
- 完成作品详情、音轨、搜索、热门列表、元数据同步接口访问

具体事项：

- 构建 `AsmrApiClient`
- 构建 `EndpointDiscoveryService`
- 构建 `AuthService`
- 完成 JWT 管理

### 阶段 3：迁移搜索能力

目标：

- 完成高级搜索语法迁移
- 完成分页搜索
- 完成搜索结果展示
- 完成搜索结果导出

具体事项：

- 迁移 `QueryParams` 逻辑
- 迁移 `SearchResult` 和 `SearchResultView`
- 实现 `SearchViewModel`
- 支持导出 CSV/JSON

### 阶段 4：迁移下载能力

目标：

- 完成单个下载、批量下载、热门下载
- 支持优先媒体格式过滤
- 支持限流、重试、并发控制
- 支持任务进度显示

具体事项：

- 迁移 `DownloadOne`
- 迁移 `DownloadBatchMedias`
- 迁移 `DownloadHot100`
- 实现后台任务模型 `DownloadTaskItem`
- 加入取消、失败重试、状态展示

### 阶段 5：迁移同步能力

目标：

- 完成元数据同步
- 完成同步下载
- 完成失败重试
- 完成导出状态记录
- 完成统计展示

具体事项：

- 迁移 `MetadataWork`、`WorkSyncInfo`
- 实现 `SyncService`
- 实现同步报告页
- 实现失败下载重试入口

### 阶段 6：迁移本地资源库与播放能力

目标：

- 替代 `listen` 命令和 WebUI
- 直接在 WPF 中浏览本地资源
- 支持文件树、音轨列表、音频播放

具体事项：

- 迁移目录扫描规则
- 构建 `LibraryScannerService`
- 构建 `PlayerService`
- 设计作品详情页与播放器页

播放器建议：

- 如果只需基础音频播放，可评估 WPF 原生能力或 NAudio
- 如果需要更强媒体兼容性，可优先评估 LibVLCSharp
- 如果必须保留近似 WebUI 的字幕交互，可在过渡期使用 WebView2，但不应作为最终架构中心

### 阶段 7：WPF 页面集成与体验收口

目标：

- 完成导航、主题、消息框、文件选择、进度通知、异常提示
- 收口所有分散操作到可视化界面

具体事项：

- 主窗口导航框架
- 全局 Snackbar / Dialog
- 后台任务托盘或状态栏
- 页面间共享状态与命令

### 阶段 8：最终验收、发布准备与 Go 功能下线

目标：

- 验证 WPF 版本功能完整度
- 保证下载目录、数据库、配置兼容
- 最终将 Go CLI 从主产品路径中移除

验证清单：

- 同一配置是否能成功登录
- 同一搜索语句结果是否大体一致
- 同一 RJID 下载目录结构是否一致
- 同步统计是否与原 Go 逻辑一致
- 失败重试是否生效
- 导出文件格式是否正确

---

## 2. 关键实现决策建议

## 2.1 配置文件策略

推荐第一阶段继续兼容现有配置位置与字段含义：

- 位置：`.asmroner-data/config.toml`
- 字段：保持 `user/downloader/limit` 三段结构语义

原因：

- 避免立即触发用户配置迁移
- 降低 AI 迁移难度

后续可选增强：

- 在 WPF 内提供“导入旧配置并迁移为新格式”的能力

## 2.2 数据库策略

推荐第一阶段继续使用 SQLite，并尽量保持现有表结构兼容：

- `metadata_works`
- `work_sync_infos`

原因：

- 现有同步与下载状态都依赖这两个表
- 保持兼容可以直接读取历史数据

## 2.3 下载目录命名策略

建议保留当前命名规则：

`RJ123456-YYYYMMDD-sub-标题`

实际规则来自以下维度：

- 作品 ID 前缀与编号
- 发售日期
- 是否带字幕
- 作品标题（经路径清洗）

原因：

- 现有 listen 和本地资源组织依赖该规则
- 用户本地已有文件目录也依赖该规则

## 2.4 高级搜索策略

建议保留现有字符串语法，同时增加图形化筛选器。

推荐 UI 模型：

- 普通关键字输入框
- 标签、社团、声优、时长、评分、销量、语言等高级筛选项
- 仍允许用户直接输入原始表达式

## 2.5 下载任务模型

建议新增显式任务实体，而不是直接把日志当进度。

至少应包含：

- 作品 ID
- 作品标题
- 当前状态
- 当前文件名
- 文件总数
- 已完成数
- 错误信息
- 开始时间
- 结束时间
- 可取消标记

## 2.6 日志模型

建议区分三层日志：

- 系统日志
- 任务日志
- 用户可见错误提示

不要再延续纯命令行式彩色日志输出模型。

---

## 3. 建议的 WPF 页面需求清单

## 3.1 首页 / 仪表盘

展示内容：

- 本地元数据总数
- 已同步下载数量
- 失败数量
- 带字幕与不带字幕数量
- 最近任务摘要

## 3.2 搜索页

必须支持：

- 普通关键字搜索
- 高级搜索
- 分页或加载更多
- 勾选批量下载
- 导出 CSV/JSON
- 查看作品详情

## 3.3 下载页

必须支持：

- 输入 RJID 下载
- 批量 RJID 下载
- 热门作品下载
- 下载队列列表
- 任务状态过滤
- 失败重试
- 取消任务

## 3.4 同步页

必须支持：

- 元数据同步
- 同步并下载
- 同步统计
- 导出失败项 / 成功项
- 重试失败下载

## 3.5 资源库页

必须支持：

- 扫描本地同步目录
- 浏览作品列表
- 搜索本地资源
- 展示目录树与文件树
- 进入播放页

## 3.6 播放页

必须支持：

- 播放音频
- 展示音轨列表
- 切换文件
- 展示作品标题、ID、日期、字幕标记

可选增强：

- 字幕支持
- 播放位置记忆
- 播放速度调节
- 最近播放记录

## 3.7 设置页

必须支持：

- 账号密码设置
- API 地址设置
- 代理设置
- 并发数与重试次数设置
- 同步目录设置
- 同步容量限制设置
- 优先媒体格式设置
- 限流与抖动设置
- 测试连接与保存

---

## 4. AI 迁移时的具体变更步骤建议

为了让后续 AI 迁移更稳定，建议按以下顺序执行，不要跨阶段混改：

1. 创建 .NET 解决方案和 WPF 项目，不接入任何业务逻辑。
2. 迁移配置模型、数据库模型、公共常量和工具能力。
3. 迁移 asmr.one API 客户端、认证、最快域名检测。
4. 迁移搜索和高级查询语法，并先用控制台或单元测试验证。
5. 迁移下载逻辑，包括目录命名、格式过滤、重试、限流、并发。
6. 迁移同步逻辑，包括元数据抓取、状态记录、统计输出。
7. 迁移本地资源扫描逻辑。
8. 再接入 WPF ViewModel 和页面。
9. 最后实现播放页、日志页、导出功能和用户体验优化。
10. 完成功能回归后，再考虑移除 Go CLI 在产品链路中的角色。

## 4.1 下一步：阶段 0 可执行落地清单（初始化工程）

本小节用于把“阶段 0”变成可直接执行的动作列表，供后续 AI 按清单落地。

### 4.1.1 目标

- 在当前仓库新增 `dotnet/` 根目录。
- 初始化一个可编译的 .NET 解决方案。
- 建立后端分层类库与 WPF 客户端项目。
- 打通最小依赖注入与日志基础设施。
- 保持与现有 Go 工程并存，不影响当前 Go 构建与运行。

### 4.1.2 建议目录与项目命名

建议创建如下结构：

```text
dotnet/
  Asmroner.sln
  Asmroner.Backend/
    Asmroner.Core/
    Asmroner.Application/
    Asmroner.Infrastructure/
  Asmroner.Wpf/
    Asmroner.Wpf/
  tests/
    Asmroner.Core.Tests/
    Asmroner.Application.Tests/
    Asmroner.Infrastructure.Tests/
    Asmroner.Wpf.Tests/
    Asmroner.IntegrationTests/
```

命名约定：

- 所有 .NET 项目统一以 `Asmroner.` 前缀命名。
- WPF 项目名固定为 `Asmroner.Wpf`。
- Core/Application/Infrastructure 三层分离，禁止循环引用。
- 单元测试项目统一使用 `.Tests` 后缀，集成测试项目使用 `.IntegrationTests` 后缀。

### 4.1.3 项目模板建议

- `Asmroner.Core`：`classlib`，承载实体、接口、常量、选项模型。
- `Asmroner.Application`：`classlib`，承载用例服务与业务编排。
- `Asmroner.Infrastructure`：`classlib`，承载 API、SQLite、文件系统、日志落地。
- `Asmroner.Wpf`：`wpf`，承载 View、ViewModel、导航和 UI 交互。
- `Asmroner.Core.Tests`：`xunit`，覆盖 Core 层纯逻辑与模型校验。
- `Asmroner.Application.Tests`：`xunit`，覆盖 Application 用例编排逻辑。
- `Asmroner.Infrastructure.Tests`：`xunit`，覆盖基础设施适配层（可结合 mock）。
- `Asmroner.Wpf.Tests`：`xunit`，覆盖 ViewModel 与 UI 相关可测试逻辑。
- `Asmroner.IntegrationTests`：`xunit`，覆盖跨层集成与最小端到端流程。

### 4.1.4 依赖关系约束

必须满足以下依赖方向：

- `Asmroner.Application` -> `Asmroner.Core`
- `Asmroner.Infrastructure` -> `Asmroner.Core`
- `Asmroner.Wpf` -> `Asmroner.Application` + `Asmroner.Infrastructure` + `Asmroner.Core`
- `Asmroner.Core` 不引用其他业务层项目
- `Asmroner.Core.Tests` -> `Asmroner.Core`
- `Asmroner.Application.Tests` -> `Asmroner.Application` + `Asmroner.Core`
- `Asmroner.Infrastructure.Tests` -> `Asmroner.Infrastructure` + `Asmroner.Core`
- `Asmroner.Wpf.Tests` -> `Asmroner.Wpf` + `Asmroner.Application` + `Asmroner.Core`
- `Asmroner.IntegrationTests` -> `Asmroner.Application` + `Asmroner.Infrastructure` + `Asmroner.Core`

### 4.1.5 阶段 0 执行步骤（供 AI 执行）

1. 创建 `dotnet/` 目录。
2. 在 `dotnet/` 下创建解决方案 `Asmroner.sln`。
3. 创建 9 个项目：Core/Application/Infrastructure/Wpf + 5 个测试项目。
4. 将 9 个项目全部加入解决方案。
5. 建立项目引用关系（按 12.1.4 约束）。
6. 在 WPF 项目中配置基础 `Host` 启动与依赖注入容器。
7. 在 WPF 项目中接入最小日志通道（控制台或文件均可，推荐文件）。
8. 新建占位页面：`MainWindow` + `DashboardView` + `SettingsView`。
9. 新建占位服务接口：`IConfigurationService`、`IAuthService`、`ISearchService`、`IDownloadService`、`ISyncService`。
10. 在各测试项目中创建至少 1 个占位测试，验证测试框架可执行。
11. 确保解决方案可编译通过，并执行一次测试命令验证通过。

### 4.1.6 阶段 0 交付物定义（DoD）

满足以下条件才算完成阶段 0：

1. `dotnet/Asmroner.sln` 存在且可加载。
2. 应用项目与测试项目均已创建并加入解决方案。
3. 引用关系符合分层约束。
4. WPF 客户端可成功启动并显示主窗口。
5. 依赖注入容器可解析至少一个占位服务。
6. 基础日志可写入（控制台或文件）。
7. 测试项目可执行且至少有一项测试通过。
8. `dotnet` 工程构建不影响原 Go 工程命令执行。

### 4.1.7 阶段 0 完成后的立即下一步（阶段 1 起点）

阶段 0 完成后，下一步应立即进入“配置与初始化能力迁移”，顺序如下：

1. 从 Go `internal/model/config.go` 映射 .NET 配置模型。
2. 落地 `ConfigurationService` 的读取与保存。
3. 落地 `ApplicationBootstrapper` 的目录与数据库初始化。
4. 完成首次启动设置页与配置保存联调。

## 4.2 下一步：阶段 1 可执行落地清单（配置与初始化迁移）

本小节是阶段 0 之后的直接执行规范，目标是先打通“可启动、可读取配置、可完成初始化”的最小闭环。

### 4.2.1 目标

- 在 .NET 侧完成配置模型与存储能力。
- 完成应用启动时的初始化管线。
- 完成数据目录与 SQLite 文件初始化。
- 完成首次启动时的“配置缺失 -> 设置向导”分支。

### 4.2.2 配置字段映射（Go -> .NET）

建议保持语义一致，便于复用现有用户配置。

| Go 字段                       | .NET 建议字段                      | 说明                         |
| ----------------------------- | ---------------------------------- | ---------------------------- |
| `user.account`                | `UserOptions.Account`              | 登录账号                     |
| `user.password`               | `UserOptions.Password`             | 登录密码                     |
| `downloader.api_url`          | `DownloaderOptions.ApiUrl`         | API 基地址，允许为空自动探测 |
| `downloader.proxy_url`        | `DownloaderOptions.ProxyUrl`       | HTTP/SOCKS5 代理             |
| `downloader.max_workers`      | `DownloaderOptions.MaxWorkers`     | 并发工作数                   |
| `downloader.max_retries`      | `DownloaderOptions.MaxRetries`     | 重试次数                     |
| `downloader.sync_data_folder` | `DownloaderOptions.SyncDataFolder` | 同步目录                     |
| `downloader.sync_wanted_size` | `DownloaderOptions.SyncWantedSize` | 同步容量上限                 |
| `downloader.prefer_media`     | `DownloaderOptions.PreferMedia`    | 音频格式优先级               |
| `limit.sync_qps`              | `LimitOptions.SyncQps`             | 同步 QPS                     |
| `limit.sync_jitter_min`       | `LimitOptions.SyncJitterMin`       | 同步抖动最小毫秒             |
| `limit.sync_jitter_max`       | `LimitOptions.SyncJitterMax`       | 同步抖动最大毫秒             |
| `limit.download_qps`          | `LimitOptions.DownloadQps`         | 下载 QPS                     |
| `limit.download_jitter_min`   | `LimitOptions.DownloadJitterMin`   | 下载抖动最小毫秒             |
| `limit.download_jitter_max`   | `LimitOptions.DownloadJitterMax`   | 下载抖动最大毫秒             |

### 4.2.3 建议新增组件

- `IConfigurationService` / `ConfigurationService`
- `IAppPathService` / `AppPathService`
- `IDatabaseInitializer` / `DatabaseInitializer`
- `IApplicationBootstrapper` / `ApplicationBootstrapper`
- `IFirstRunService` / `FirstRunService`

职责边界：

- `ConfigurationService`：读取、保存、校验配置。
- `AppPathService`：集中管理 `.asmroner-data`、数据库文件、日志文件路径。
- `DatabaseInitializer`：创建数据库、应用迁移、最小连接探测。
- `ApplicationBootstrapper`：统一启动初始化编排。
- `FirstRunService`：判断是否首次启动及是否需要进入设置向导。

### 4.2.4 阶段 1 执行步骤（供 AI 执行）

1. 在 `Asmroner.Core` 新增 `UserOptions`、`DownloaderOptions`、`LimitOptions`、`AppConfig`。
2. 在 `Asmroner.Infrastructure` 落地 `ConfigurationService`，支持读取与保存配置。
3. 在 `Asmroner.Infrastructure` 落地 `AppPathService`，统一生成元数据目录和文件路径。
4. 在 `Asmroner.Infrastructure` 落地 `DatabaseInitializer`，初始化 SQLite 文件。
5. 在 `Asmroner.Application` 落地 `ApplicationBootstrapper`，按顺序执行：
   - 确保目录存在
   - 读取配置
   - 初始化数据库
   - 确保同步目录存在
6. 在 `Asmroner.Wpf` 启动流程中接入 `ApplicationBootstrapper`。
7. 若配置不存在或配置不完整，自动导航到设置页并阻断后续业务页面。
8. 在设置页实现“保存配置并重新初始化”的闭环。
9. 完成异常提示策略：初始化失败时给出可读错误信息。
10. 补充最小单元测试与集成测试。

### 4.2.5 阶段 1 验证用例

建议至少覆盖以下用例：

1. 配置文件不存在：应用启动后自动进入设置页。
2. 配置文件存在且合法：应用可正常进入主页面。
3. 配置字段缺失：提示缺失字段并引导修复。
4. 同步目录不存在：启动后自动创建目录。
5. 数据库文件不存在：启动后自动创建。
6. 数据库初始化失败：给出错误提示，不进入业务页面。
7. 设置页修改配置后保存成功：重新初始化通过。

### 4.2.6 阶段 1 交付物定义（DoD）

满足以下条件才算阶段 1 完成：

1. WPF 启动时可稳定执行初始化管线。
2. 配置读取与保存已贯通。
3. SQLite 文件可自动创建且可连接。
4. 首次启动向导分支可用。
5. 配置缺失与初始化失败场景有明确提示。
6. 阶段 1 相关测试全部通过。

### 4.2.7 阶段 1 完成后的立即下一步（阶段 2 起点）

阶段 1 完成后，下一步进入“API 客户端与认证迁移”：

1. 落地 `AsmrApiClient` 与基础 HTTP 管线。
2. 落地 `EndpointDiscoveryService`（最快站点探测）。
3. 落地 `AuthService`（JWT 获取与缓存）。
4. 实现登录连通性验证入口（可在设置页提供“测试连接”按钮）。

## 4.3 下一步：阶段 2 可执行落地清单（API 与认证迁移）

本小节用于把“API 与认证迁移”落成可执行动作，目标是打通 `.NET 客户端 -> asmr.one API` 的稳定通信链路。

### 4.3.1 目标

- 建立统一的 HTTP 客户端基础设施。
- 完成 API 基地址解析与最快站点探测。
- 完成登录与 JWT 令牌管理。
- 完成关键只读接口的最小可用调用。
- 提供可在设置页触发的连通性测试能力。

### 4.3.2 迁移来源与边界

阶段 2 主要对齐以下 Go 能力：

- `internal/engine/checker.go`：站点探测、最快域名选择。
- `internal/engine/engine.go`：`AuthLogin`、`GetWorkInfo`、`GetVoiceTracks`、`SearchForCountResult`、`DownloadHot100` 的接口访问模式。
- `internal/consts/consts.go`：默认 API 地址与路径常量。

阶段 2 不包含：

- 文件下载落地
- 元数据同步入库
- 播放与本地资源扫描

### 4.3.3 建议新增组件

- `IAsmrApiOptionsProvider` / `AsmrApiOptionsProvider`
- `IEndpointDiscoveryService` / `EndpointDiscoveryService`
- `IAuthService` / `AuthService`
- `ITokenStore` / `TokenStore`
- `IAsmrApiClient` / `AsmrApiClient`
- `IConnectivityProbeService` / `ConnectivityProbeService`

职责边界：

- `AsmrApiOptionsProvider`：解析 API 地址、超时、重试、代理等运行选项。
- `EndpointDiscoveryService`：获取候选域名并选出最快可用地址。
- `AuthService`：登录、刷新、注销、鉴权状态查询。
- `TokenStore`：JWT 与过期时间缓存（内存 + 可选本地持久化）。
- `AsmrApiClient`：封装请求、响应解析、错误归一化。
- `ConnectivityProbeService`：测试 API 可达、鉴权是否可用。

### 4.3.4 建议 DTO 与模型

- `AuthLoginRequest` / `AuthLoginResponse`
- `ApiToken`（`AccessToken`、`ExpiresAt`、`Scheme`）
- `WorkInfoDto`
- `TrackDto`
- `SearchResultDto`
- `HotWorkDto`
- `ApiError`（`Code`、`HttpStatus`、`Message`、`RawBody`）

模型约束：

- DTO 字段命名优先兼容上游 JSON。
- 领域层使用单独 Model，避免 UI 直接绑定 DTO。
- `ApiError` 必须可序列化并进入日志链路。

### 4.3.5 接口路径与配置建议

建议在 `Asmroner.Core` 统一维护 API 路径常量：

- `/api/auth/me`
- `/api/work/{id}`
- `/api/tracks/{id}`
- `/api/search/{query}`
- `/api/recommender/popular`
- `/api/works?...`（同步阶段使用，阶段 2 先保留常量）

API 地址策略：

1. 优先使用用户配置 `ApiUrl`。
2. 为空时使用默认地址常量。
3. 可选触发 `EndpointDiscoveryService` 做覆盖。

### 4.3.6 HTTP 管线建议

建议基于 `HttpClientFactory` 构建命名客户端 `AsmrApi`，并接入：

- 统一超时策略
- 统一请求头（`User-Agent`、`Accept` 等）
- 统一错误映射中间层
- 可选重试策略（推荐 Polly）
- 可选代理配置（HTTP/SOCKS5）

鉴权注入建议：

- 通过 DelegatingHandler 自动附加 `Authorization: Bearer <token>`。
- 若 token 缺失，自动触发登录或返回明确错误。

### 4.3.7 阶段 2 执行步骤（供 AI 执行）

1. 在 `Asmroner.Core` 新增 API 选项与路径常量。
2. 在 `Asmroner.Infrastructure` 创建 `HttpClientFactory` 命名客户端配置。
3. 落地 `EndpointDiscoveryService`，实现候选域名探测与最快站点选择。
4. 落地 `TokenStore`，实现令牌读写与有效期检查。
5. 落地 `AuthService`，实现登录并缓存 JWT。
6. 落地 `AsmrApiClient`，先实现只读接口：
   - 作品详情
   - 音轨列表
   - 搜索
   - 热门列表
7. 落地 `ConnectivityProbeService`，输出“可达性 + 鉴权状态 + 延迟”。
8. 在设置页增加“测试连接”按钮并展示结果。
9. 补充统一错误模型与日志打点，确保失败可追踪。
10. 补充单元测试与集成测试，覆盖关键成功/失败路径。

### 4.3.8 阶段 2 测试矩阵

单元测试建议覆盖：

1. `EndpointDiscoveryService` 在多候选地址下能选出可用最快地址。
2. `AuthService` 登录成功后可写入 TokenStore。
3. `AuthService` 登录失败时返回可读错误。
4. `AsmrApiClient` 对 2xx/4xx/5xx 的错误映射正确。
5. 代理配置开启时客户端能正确构建请求。

集成测试建议覆盖：

1. 使用测试账号完成一次登录并获取 token。
2. 登录后可调用作品详情接口。
3. 登录后可调用搜索接口并解析分页字段。
4. 无 token 场景调用受保护接口时返回鉴权错误。
5. API 地址不可达时“测试连接”返回失败且附带原因。

### 4.3.9 阶段 2 交付物定义（DoD）

满足以下条件才算阶段 2 完成：

1. API 命名客户端与统一 HTTP 管线已落地。
2. 登录与 token 缓存机制可稳定工作。
3. 作品详情、音轨、搜索、热门接口可调用。
4. 设置页“测试连接”可输出明确结果。
5. 关键错误可结构化记录并可追踪。
6. 阶段 2 相关单元测试与集成测试通过。

### 4.3.10 阶段 2 完成后的立即下一步（阶段 3 起点）

阶段 2 完成后，进入“搜索能力迁移”：

1. 迁移高级查询语法解析（等效 Go `QueryParams`）。
2. 实现搜索分页聚合逻辑与结果映射。
3. 打通搜索页 ViewModel 与搜索服务绑定。
4. 补齐搜索导出（CSV/JSON）最小闭环。

## 4.4 下一步：阶段 3 可执行落地清单（搜索能力迁移）

本小节用于将“搜索能力迁移”落成可执行步骤，目标是在 WPF 端形成完整的“查询输入 -> 结果展示 -> 导出/下载入口”闭环。

### 4.4.1 目标

- 完成高级查询语法解析能力迁移（兼容 Go `QueryParams` 语义）。
- 完成分页搜索与结果聚合。
- 完成搜索结果展示、导出与后续下载入口。
- 在 UI 层支持简单模式与高级模式并存。

### 4.4.2 迁移来源与边界

阶段 3 主要对齐以下 Go 能力：

- `internal/model/queryparam.go`：查询字符串解析与拼接。
- `internal/model/search.go`：搜索结果 DTO 与视图模型字段。
- `cmd/search.go`：搜索、导出、搜索后下载的业务流程。
- `internal/engine/engine.go`：`SearchForCountResult` 的分页聚合逻辑。

阶段 3 不包含：

- 下载文件落地（由阶段 4 负责）
- 元数据同步（由阶段 5 负责）

### 4.4.3 建议新增组件

- `IQueryParserService` / `QueryParserService`
- `ISearchService` / `SearchService`
- `ISearchExportService` / `SearchExportService`
- `ISearchStateStore` / `SearchStateStore`
- `SearchViewModel`

职责边界：

- `QueryParserService`：解析与构建 asmr 查询字符串。
- `SearchService`：调用 API 并处理分页聚合。
- `SearchExportService`：导出 CSV/JSON。
- `SearchStateStore`：缓存当前查询条件、分页状态、最近搜索。
- `SearchViewModel`：承载 UI 命令和状态绑定。

### 4.4.4 查询语法兼容要求

需要兼容以下输入语义：

- 普通关键词（支持逗号分词）
- 搜索键值对：`tag/circle/va/duration/rate/price/sell/age/lang`
- 反选条件：如 `-tag:`、`-lang:`
- 分页参数：`order/sort/page/pageSize/subtitle/includeTranslationWorks`

兼容策略建议：

1. 第一阶段保持与 Go 语法兼容优先。
2. UI 上提供“高级筛选表单”，但底层仍复用统一查询构建器。
3. 对非法语法返回可读错误，不抛裸异常。

### 4.4.5 阶段 3 执行步骤（供 AI 执行）

1. 在 `Asmroner.Core` 新增搜索领域模型：`SearchQuery`、`SearchFilter`、`SearchPageOptions`。
2. 在 `Asmroner.Application` 落地 `QueryParserService`，对齐 Go 解析规则。
3. 在 `Asmroner.Application` 落地 `SearchService`，实现分页聚合与结果裁剪。
4. 在 `Asmroner.Application` 落地 `SearchExportService`，支持 CSV/JSON 导出。
5. 在 `Asmroner.Wpf` 落地 `SearchViewModel`，提供命令：
   - 执行搜索
   - 清空条件
   - 导出结果
   - 加入下载队列（仅入队，不执行下载）
6. 在 `Asmroner.Wpf` 新建搜索页面并绑定 DataGrid。
7. 增加分页交互：上一页、下一页、跳页、页大小切换。
8. 增加高级筛选器 UI（可折叠面板）。
9. 增加错误提示与空结果提示。
10. 补充单元测试与 UI 绑定层测试。

### 4.4.6 阶段 3 测试矩阵

单元测试建议覆盖：

1. 纯关键词查询构建正确。
2. 含搜索键值对与反选条件时构建正确。
3. 分页参数拼接正确。
4. 非法输入返回可读错误。
5. 聚合多页结果时数量与顺序正确。
6. 导出 CSV/JSON 字段完整且顺序正确。

集成测试建议覆盖：

1. 搜索接口返回 1 页时可正确显示。
2. 搜索接口返回多页时可正确聚合。
3. 导出按钮可生成文件且内容正确。
4. 搜索失败时 UI 能展示错误并保持可继续操作。

### 4.4.7 阶段 3 交付物定义（DoD）

满足以下条件才算阶段 3 完成：

1. 查询解析逻辑与 Go 语义兼容。
2. 搜索页支持基本与高级查询。
3. 分页搜索可稳定运行。
4. 搜索结果可导出 CSV/JSON。
5. 可从搜索结果触发“加入下载队列”动作。
6. 阶段 3 相关单元测试与集成测试通过。

### 4.4.8 阶段 3 完成后的立即下一步（阶段 4 起点）

阶段 3 完成后，进入“下载能力迁移”：

1. 迁移单个/批量/热门下载流程。
2. 迁移限流、重试、并发控制。
3. 实现下载任务状态模型与进度上报。
4. 打通“搜索结果入队 -> 下载页执行”全链路。

## 4.5 下一步：阶段 4 可执行落地清单（下载能力迁移）

本小节用于将“下载能力迁移”落成可执行步骤，目标是在 WPF 中形成稳定的下载任务系统与可视化进度管理。

### 4.5.1 目标

- 完成单个、批量、热门下载能力迁移。
- 完成限流、抖动、并发、重试等关键控制能力。
- 完成下载任务生命周期与状态管理。
- 完成下载失败可恢复机制（重试、错误归档）。

### 4.5.2 迁移来源与边界

阶段 4 主要对齐以下 Go 能力：

- `cmd/download.go`：单个/批量/hot100 入口流程。
- `internal/engine/engine.go`：`DownloadOne`、`SimpleDownload`、`DownloadBatchMedias`、`DownloadHot100`、`downloadFile`。
- `internal/engine/limiter.go`：令牌桶 + 随机抖动限速。
- `internal/utils/utils.go`：目录创建、路径规范、空目录清理。

阶段 4 不包含：

- 元数据同步入库（阶段 5）
- 本地资源库扫描与播放（阶段 6）

### 4.5.3 建议新增组件

- `IDownloadService` / `DownloadService`
- `IDownloadQueueService` / `DownloadQueueService`
- `IDownloadWorkerService` / `DownloadWorkerService`
- `IRateLimiterService` / `RateLimiterService`
- `IDownloadRetryPolicy` / `DownloadRetryPolicy`
- `IDownloadTaskStore` / `DownloadTaskStore`
- `DownloadViewModel`

职责边界：

- `DownloadService`：提供上层统一下载 API（单个/批量/热门）。
- `DownloadQueueService`：负责任务入队、出队、取消、暂停/恢复（可选）。
- `DownloadWorkerService`：执行具体下载工作流。
- `RateLimiterService`：QPS + 抖动控制。
- `DownloadRetryPolicy`：错误分类与重试间隔策略。
- `DownloadTaskStore`：任务状态持久化（内存 + 可选本地存储）。
- `DownloadViewModel`：UI 绑定与操作命令。

### 4.5.4 任务模型建议

建议定义 `DownloadTaskItem`，至少包含以下字段：

- `TaskId`
- `SourceId`（RJID）
- `Title`
- `Status`（`Queued`/`Running`/`Completed`/`Failed`/`Canceled`）
- `TotalFiles`
- `CompletedFiles`
- `CurrentFile`
- `ProgressPercent`
- `RetryCount`
- `ErrorMessage`
- `StartedAt`
- `FinishedAt`
- `TargetDirectory`

状态约束：

- 任务状态必须单向推进，禁止回退到非法状态。
- `Failed` 与 `Canceled` 必须记录最终原因。

### 4.5.5 下载目录与命名兼容要求

需要保持与现有 Go 命名规则一致：

- 目录名结构：`{Prefix}{Number}-{ReleaseDate}-{sub|nosub}-{SanitizedTitle}`
- 标题需做路径安全清洗（特殊字符替换）

兼容策略建议：

1. 优先复用现有命名逻辑语义。
2. 命名规则封装在单独策略类，避免分散在 UI 或下载流程中。
3. 目录创建失败要返回可读错误并中止任务。

### 4.5.6 重试与错误策略建议

重试建议：

- 网络类瞬时错误：指数退避重试。
- 429/5xx：可重试，且记录重试次数。
- 4xx 业务错误（非 429）：默认不重试，直接失败。

错误归一化建议：

- 统一映射到 `DownloadError`（类型、HTTP 状态、消息、原始错误、是否可重试）。
- 所有失败记录进入任务日志和系统日志。

### 4.5.7 阶段 4 执行步骤（供 AI 执行）

1. 在 `Asmroner.Core` 新增下载任务模型与状态枚举。
2. 在 `Asmroner.Application` 落地 `RateLimiterService`（QPS + jitter）。
3. 在 `Asmroner.Application` 落地 `DownloadRetryPolicy`（错误分类 + 重试策略）。
4. 在 `Asmroner.Application` 落地 `DownloadQueueService` 与 `DownloadWorkerService`。
5. 在 `Asmroner.Application` 落地 `DownloadService`，封装单个/批量/热门下载入口。
6. 在 `Asmroner.Application` 落地目录命名与路径清洗策略。
7. 在 `Asmroner.Wpf` 落地 `DownloadViewModel` 与下载列表页面。
8. 打通“搜索页加入下载队列 -> 下载页执行”链路。
9. 增加任务操作命令：取消、重试、清理已完成（可选）。
10. 补充单元测试、集成测试、长任务稳定性测试。

### 4.5.8 阶段 4 测试矩阵

单元测试建议覆盖：

1. 单任务下载状态流转正确。
2. 批量任务入队顺序与并发上限控制正确。
3. 限流器在不同 QPS 配置下行为正确。
4. 重试策略对不同错误类型判断正确。
5. 目录命名与路径清洗输出符合兼容规则。

集成测试建议覆盖：

1. 单个 RJID 下载可成功落地文件。
2. 批量下载中某任务失败不影响其他任务继续执行。
3. 热门下载可按数量限制正确执行。
4. 取消任务后可及时停止并标记 `Canceled`。
5. 失败任务点击重试后能重新进入队列。

稳定性测试建议：

1. 长时间批量下载过程中内存无明显泄漏。
2. 高并发 + 低 QPS 场景下任务不会死锁。
3. 网络抖动时系统仍可恢复并继续处理后续任务。

### 4.5.9 阶段 4 交付物定义（DoD）

满足以下条件才算阶段 4 完成：

1. 单个/批量/热门下载流程可稳定执行。
2. 限流、重试、并发控制可配置且生效。
3. 下载任务在 UI 可见且状态准确。
4. 失败任务可重试，取消任务可生效。
5. 目录命名与现有 Go 规则兼容。
6. 阶段 4 相关测试通过。

### 4.5.10 阶段 4 完成后的立即下一步（阶段 5 起点）

阶段 4 完成后，进入“同步能力迁移”：

1. 迁移元数据抓取与入库流程。
2. 迁移同步下载状态表与统计逻辑。
3. 打通失败重试、导出、报表页面。
4. 形成“同步 -> 下载 -> 统计”全链路闭环。

## 4.6 下一步：阶段 5 可执行落地清单（同步能力迁移）

本小节用于将“同步能力迁移”落成可执行步骤，目标是在 WPF 中形成“元数据同步 + 同步下载 + 失败重试 + 导出报表”的完整闭环。

### 4.6.1 目标

- 完成站点元数据同步到本地 SQLite。
- 完成同步下载流程与容量阈值控制。
- 完成失败重试、状态导出、统计报表能力。
- 完成同步过程的可视化进度与任务反馈。

### 4.6.2 迁移来源与边界

阶段 5 主要对齐以下 Go 能力：

- `cmd/sync.go`：`sync`、`sync download`、`sync retry`、`sync export`、`sync report`。
- `internal/engine/engine.go`：`SyncMetadata`、分页抓取、写库、统计打印。
- `internal/model/model.go`：`MetadataWork`、`WorkSyncInfo`。
- `internal/database/db.go`：SQLite 初始化与表结构。
- `internal/utils/utils.go`：目录大小计算、文件大小单位换算。

阶段 5 不包含：

- 播放页与资源库浏览（阶段 6）
- 复杂 UI 动效与体验增强（阶段 7）

### 4.6.3 建议新增组件

- `ISyncService` / `SyncService`
- `IMetadataSyncService` / `MetadataSyncService`
- `ISyncDownloadService` / `SyncDownloadService`
- `ISyncRetryService` / `SyncRetryService`
- `ISyncExportService` / `SyncExportService`
- `ISyncReportService` / `SyncReportService`
- `SyncViewModel`

职责边界：

- `MetadataSyncService`：抓取元数据、分页合并、入库去重。
- `SyncDownloadService`：按容量限制执行同步下载并更新状态。
- `SyncRetryService`：针对失败记录进行重试下载。
- `SyncExportService`：导出成功/失败记录到 CSV/JSON。
- `SyncReportService`：聚合统计数据供仪表盘与报表页展示。
- `SyncService`：对外编排同步相关流程。
- `SyncViewModel`：承载 UI 交互与状态绑定。

### 4.6.4 数据模型与状态约束

建议保留并映射以下核心实体：

- `MetadataWork`：站点作品元数据
- `WorkSyncInfo`：同步下载状态记录

状态建议维持兼容：

- `PENDING`
- `COMPLETED`
- `FAILED`

字段建议至少保留：

- `SourceId`
- `MetadataWorkId`
- `Status`
- `DirSize`
- `FilePath`
- `FailReason`
- `RetryCount`
- `UpdatedAt`
- `FailedAt`

### 4.6.5 同步流程与策略要求

元数据同步策略：

1. 先拉取首页统计（总量、分页信息）。
2. 对比本地数量，决定是否执行完整同步。
3. 分页抓取，按唯一键去重写入。
4. 同步过程记录进度并可取消（可选）。

同步下载策略：

1. 依据 `SyncWantedSize` 控制下载总量。
2. 逐批选取待下载作品并记录为 `PENDING`。
3. 下载后更新 `COMPLETED` 或 `FAILED`。
4. 达到容量阈值时主动停止后续批次。

### 4.6.6 阶段 5 执行步骤（供 AI 执行）

1. 在 `Asmroner.Infrastructure` 建立 `MetadataWork` 与 `WorkSyncInfo` 的 EF Core 映射。
2. 在 `Asmroner.Application` 落地 `MetadataSyncService`，实现分页抓取与入库去重。
3. 在 `Asmroner.Application` 落地 `SyncDownloadService`，实现容量阈值控制与状态更新。
4. 在 `Asmroner.Application` 落地 `SyncRetryService`，实现失败任务重试。
5. 在 `Asmroner.Application` 落地 `SyncExportService`，支持 CSV/JSON 导出。
6. 在 `Asmroner.Application` 落地 `SyncReportService`，输出统计指标。
7. 在 `Asmroner.Application` 落地 `SyncService`，编排上述能力。
8. 在 `Asmroner.Wpf` 落地 `SyncViewModel` 与同步管理页面。
9. 增加同步页操作入口：开始同步、同步下载、重试失败、导出、刷新统计。
10. 补充单元测试、集成测试和数据库一致性校验测试。

### 4.6.7 阶段 5 测试矩阵

单元测试建议覆盖：

1. 分页同步时页码与去重逻辑正确。
2. 容量阈值判断正确（边界值覆盖）。
3. 同步下载状态流转正确。
4. 重试逻辑对失败记录筛选正确。
5. 导出服务可正确生成 CSV/JSON。
6. 报表聚合统计口径正确。

集成测试建议覆盖：

1. 执行一次完整元数据同步并写入数据库。
2. 执行同步下载后能生成 `COMPLETED/FAILED` 记录。
3. 失败重试后状态与重试次数更新正确。
4. 导出失败记录文件内容与数据库一致。
5. 报表页展示指标与数据库统计一致。

一致性测试建议：

1. 同步中断后再次启动可继续执行且不重复污染数据。
2. 多次同步后唯一键不重复、统计口径稳定。

### 4.6.8 阶段 5 交付物定义（DoD）

满足以下条件才算阶段 5 完成：

1. 元数据同步可稳定执行并写入 SQLite。
2. 同步下载可按容量阈值控制执行。
3. 失败重试能力可用且状态更新正确。
4. 成功/失败记录可导出 CSV/JSON。
5. 同步统计指标可在 UI 正确展示。
6. 阶段 5 相关测试通过。

### 4.6.9 监控与运维建议（阶段 5）

建议在此阶段增加最小可观测能力：

- 同步任务开始/结束日志
- 分页抓取耗时与失败次数
- 同步下载总量与失败比例
- 重试成功率

这些指标可作为后续性能优化与故障定位依据。

### 4.6.10 阶段 5 完成后的立即下一步（阶段 6 起点）

阶段 5 完成后，进入“资源库与播放能力迁移”：

1. 迁移本地目录扫描与媒体索引构建。
2. 设计资源库页与播放页的数据模型。
3. 选型并接入播放器组件。
4. 打通“同步完成资源 -> 资源库展示 -> 播放”链路。

## 4.7 下一步：阶段 6 可执行落地清单（资源库与播放能力迁移）

本小节用于将“资源库与播放能力迁移”落成可执行步骤，目标是在 WPF 中替代 `listen + WebUI` 的浏览与播放能力。

### 4.7.1 目标

- 完成本地资源库扫描与索引构建。
- 完成资源列表、文件树、筛选与详情展示。
- 完成音频播放、切换、进度控制与基础播放状态持久化。
- 打通“同步下载结果 -> 资源库可见 -> 可播放”的闭环。

### 4.7.2 迁移来源与边界

阶段 6 主要对齐以下 Go 能力：

- `cmd/listen.go`：目录扫描、分页列表、目录命名解析。
- `webui/webui.go` + `webui/public/`：播放页交互语义（作品列表、音轨选择、播放控制）。
- `internal/utils/utils.go`：路径处理与目录遍历相关能力。

阶段 6 不包含：

- 全局 UI 主题与高级交互动效（阶段 7）
- 最终发布打包与安装流程（阶段 8）

### 4.7.3 播放器技术选型建议

候选方案：

1. `NAudio`：实现轻量，适合基础音频播放。
2. `LibVLCSharp`：兼容性强，适合复杂媒体与格式要求。
3. `WPF MediaElement`：依赖系统编解码，能力最弱但接入简单。

推荐策略：

- 先以 `NAudio` 或 `LibVLCSharp` 实现最小可播版本。
- 若未来需要更复杂格式与字幕能力，优先 `LibVLCSharp`。

### 4.7.4 建议新增组件

- `ILibraryScannerService` / `LibraryScannerService`
- `ILibraryIndexService` / `LibraryIndexService`
- `ILibraryQueryService` / `LibraryQueryService`
- `IPlayerService` / `PlayerService`
- `IPlaybackStateStore` / `PlaybackStateStore`
- `LibraryViewModel`
- `PlayerViewModel`

职责边界：

- `LibraryScannerService`：扫描同步目录并构建原始条目。
- `LibraryIndexService`：解析目录命名并生成可查询索引。
- `LibraryQueryService`：提供分页、过滤、排序、检索。
- `PlayerService`：管理播放、暂停、跳转、切曲、音量。
- `PlaybackStateStore`：记录最近播放位置、最近播放列表（可选）。
- `LibraryViewModel`：资源库页面绑定。
- `PlayerViewModel`：播放器页面绑定。

### 4.7.5 数据模型建议

建议新增以下模型：

- `LibraryWorkItem`：作品级资源（RJID、标题、日期、字幕标记、目录路径）。
- `LibraryFileItem`：文件级资源（相对路径、文件名、扩展名、时长、大小）。
- `PlaybackContext`：当前作品、当前文件、播放位置、播放状态。

兼容要求：

- 目录解析规则需兼容 `RJxxxxxx-YYYYMMDD-sub/nosub-title`。
- 扫描器应忽略不符合规则的目录并输出警告日志。

### 4.7.6 阶段 6 执行步骤（供 AI 执行）

1. 在 `Asmroner.Application` 落地 `LibraryScannerService`，支持全量扫描与增量刷新。
2. 在 `Asmroner.Application` 落地 `LibraryIndexService`，解析目录命名并映射领域模型。
3. 在 `Asmroner.Application` 落地 `LibraryQueryService`，支持分页/过滤/关键词检索。
4. 在 `Asmroner.Infrastructure` 接入播放器组件并实现 `PlayerService`。
5. 在 `Asmroner.Application` 落地 `PlaybackStateStore`（内存 + 可选本地持久化）。
6. 在 `Asmroner.Wpf` 新建资源库页面，展示作品列表和文件树。
7. 在 `Asmroner.Wpf` 新建播放页面，提供播放控制条与当前曲目信息。
8. 打通“资源库选中 -> 播放页载入 -> 播放控制”链路。
9. 增加异常处理：文件缺失、格式不支持、解码失败等提示。
10. 补充单元测试、集成测试和可用性回归测试。

### 4.7.7 阶段 6 测试矩阵

单元测试建议覆盖：

1. 目录命名解析正确（含边界与非法样例）。
2. 资源扫描能正确构建作品与文件索引。
3. 检索与分页结果正确。
4. 播放状态切换（播放/暂停/停止）正确。
5. 播放位置保存与恢复逻辑正确。

集成测试建议覆盖：

1. 同步下载目录内容可被完整扫描并展示。
2. 选择音频文件后可正常播放。
3. 切换作品或切换音轨时播放器状态正确更新。
4. 非法文件或损坏文件不会导致应用崩溃。
5. 重启应用后可恢复最近播放上下文（如启用持久化）。

可用性测试建议：

1. 大目录场景（大量作品）下资源库可正常滚动与筛选。
2. 连续切歌与拖拽进度条时 UI 不出现卡死。

### 4.7.8 阶段 6 交付物定义（DoD）

满足以下条件才算阶段 6 完成：

1. 资源库可稳定展示本地作品与文件树。
2. 播放器可执行播放、暂停、停止、切换音轨。
3. 目录解析规则与 Go 版本兼容。
4. 播放异常可被捕获并提示，不导致应用崩溃。
5. 阶段 6 相关测试通过。

### 4.7.9 阶段 6 体验增强建议

建议在本阶段末尾预留以下增强项（可延后到阶段 7）：

- 最近播放与收藏
- 倍速播放
- 播放列表连续播放
- 键盘快捷键控制

### 4.7.10 阶段 6 完成后的立即下一步（阶段 7 起点）

阶段 6 完成后，进入“UI 集成与体验收口”：

1. 完善全局导航、状态栏、消息中心。
2. 统一错误提示、加载状态与空状态组件。
3. 优化性能与可用性细节。
4. 完成跨页面流程回归测试。

## 4.8 下一步：阶段 7 可执行落地清单（UI 集成与体验收口）

本小节用于将“UI 集成与体验收口”落成可执行步骤，目标是在功能可用基础上形成一致、稳定、易用的桌面交互体验。

### 4.8.1 目标

- 完成全局导航、页面路由与状态共享机制。
- 统一错误提示、加载态、空态、成功反馈样式与行为。
- 完成跨页面流程闭环与关键交互优化。
- 完成性能与可用性基线优化。

### 4.8.2 迁移来源与边界

阶段 7 主要对齐此前阶段已完成功能的 UI 收口工作：

- 配置与初始化页面（阶段 1）
- 搜索页（阶段 3）
- 下载页（阶段 4）
- 同步页（阶段 5）
- 资源库与播放页（阶段 6）

阶段 7 不包含：

- 发布打包、安装器与升级策略（阶段 8）
- 大规模视觉重构（除非影响可用性）

### 4.8.3 建议新增组件

- `INavigationService` / `NavigationService`
- `IUiMessageService` / `UiMessageService`
- `IDialogService` / `DialogService`
- `IAppStateStore` / `AppStateStore`
- `IPageLoadStateService` / `PageLoadStateService`
- `ShellViewModel`

职责边界：

- `NavigationService`：统一页面跳转与参数传递。
- `UiMessageService`：统一 Toast/状态栏消息。
- `DialogService`：统一确认框、错误框、文件选择对话框。
- `AppStateStore`：全局状态共享（当前任务、当前页面上下文等）。
- `PageLoadStateService`：统一加载态与空态控制。
- `ShellViewModel`：主窗体导航与全局命令绑定。

### 4.8.4 统一交互规范建议

建议定义以下统一规范：

- 错误提示：统一错误文案模板与展示位置。
- 加载状态：所有耗时操作都必须有可见 loading。
- 空状态：列表空结果统一空态组件。
- 危险操作：删除、清理、重置等必须二次确认。
- 长任务反馈：同步/下载必须可见进度与可取消能力。

### 4.8.5 阶段 7 执行步骤（供 AI 执行）

1. 在 `Asmroner.Wpf` 落地 `Shell` 主框架（导航区 + 内容区 + 状态区）。
2. 在 `Asmroner.Wpf` 落地 `NavigationService`，统一页面跳转入口。
3. 在 `Asmroner.Wpf` 落地 `UiMessageService` 与 `DialogService`。
4. 抽取统一的错误处理与异常边界机制（全局捕获 + 页面级处理）。
5. 为搜索、下载、同步、资源库页面接入统一加载态与空态组件。
6. 为关键页面接入统一命令栏（刷新、重试、导出、取消等）。
7. 增加跨页面参数传递能力（如从搜索页跳转下载页并带入任务）。
8. 优化大列表渲染性能（虚拟化、延迟加载、分页策略）。
9. 进行 UI 一致性巡检并修复视觉/交互不一致项。
10. 补充 UI 自动化冒烟测试与关键路径回归测试。

### 4.8.6 阶段 7 测试矩阵

功能回归测试建议覆盖：

1. 配置 -> 搜索 -> 下载 -> 同步 -> 资源库 -> 播放全链路可执行。
2. 页面跳转参数可正确传递。
3. 错误提示、成功提示、确认对话框行为一致。
4. 长任务中断与恢复后 UI 状态正确。

可用性测试建议覆盖：

1. 新用户首次启动能在 3 步内完成基础配置并开始使用。
2. 常用操作（搜索、下载、同步）路径清晰且反馈及时。
3. 大量数据场景下页面响应可接受。

稳定性测试建议覆盖：

1. 连续页面切换与长时间运行无明显内存持续上涨。
2. 异常注入场景下应用不会崩溃，且可恢复操作。

### 4.8.7 阶段 7 交付物定义（DoD）

满足以下条件才算阶段 7 完成：

1. 全局导航与页面路由稳定可用。
2. 统一消息、对话框、加载态、空态机制落地。
3. 核心页面交互一致且无明显断裂。
4. 关键流程回归测试通过。
5. 阶段 7 相关 UI 冒烟测试通过。

### 4.8.8 阶段 7 性能与体验基线建议

建议建立以下基线并在后续版本持续监控：

- 首次启动到主页面可交互时间
- 搜索结果首次渲染耗时
- 下载页大列表滚动流畅度
- 同步页刷新响应时间

### 4.8.9 阶段 7 风险控制建议

建议在本阶段执行“冻结规则”：

- 非必要不再新增业务功能。
- 只做 UI 收口、体验修复与稳定性优化。
- 所有 UI 改动必须附回归用例。

### 4.8.10 阶段 7 完成后的立即下一步（阶段 8 起点）

阶段 7 完成后，进入“最终验收与发布准备”：

1. 完整执行功能验收清单。
2. 执行性能与稳定性压测。
3. 准备打包、签名、安装与升级策略。
4. 完成发布说明与迁移说明文档。

## 4.9 下一步：阶段 8 可执行落地清单（最终验收与发布准备）

本小节用于将“最终验收与发布准备”落成可执行步骤，目标是在上线前完成可量化验收、可回滚发布与可追踪交付。

### 4.9.1 目标

- 完成全量功能验收与非功能验收。
- 完成发布包构建、签名、安装验证。
- 完成回滚策略与发布后监控基线。
- 完成用户迁移说明与版本发布说明。

### 4.9.2 阶段边界

阶段 8 聚焦“交付与上线准备”，不再新增业务功能。

阶段 8 包含：

- 验收执行
- 发布打包
- 安装验证
- 文档收口

阶段 8 不包含：

- 新功能开发
- 大规模架构重构

### 4.9.3 建议新增组件与产物

- `ReleaseChecklist.md`（发布检查清单）
- `MigrationGuide.md`（老用户迁移说明）
- `KnownIssues.md`（已知问题与规避方案）
- 安装包与校验信息（版本号、哈希、签名信息）

建议在 `docs/release/` 下集中管理发布文档。

### 4.9.4 第 14 节验收标准映射（发布前检查清单）

以下条目逐项映射自第 14 节，发布前必须全部通过：

1. 客户端可正常启动并完成初始化。
2. 可读取或导入现有配置。
3. 可成功登录并请求 asmr.one 接口。
4. 可使用普通与高级搜索语法完成搜索。
5. 可完成单个、批量、热门作品下载。
6. 可执行元数据同步、同步下载、失败重试。
7. 可查看同步统计与导出成功/失败记录。
8. 可浏览本地资源库并播放音频。
9. 下载目录结构与原 Go 版本兼容。
10. SQLite 数据库可复用或可平滑迁移。

执行要求：

- 每项必须有“测试证据”（日志、截图、测试报告或结果文件）。
- 任一关键项失败即阻断发布。

### 4.9.5 阶段 8 执行步骤（供 AI 执行）

1. 生成发布候选版本号（RC）。
2. 执行全量自动化测试（单元、集成、UI 冒烟）。
3. 执行第 14 节 10 项功能验收并留存证据。
4. 执行性能与稳定性回归（长任务、批量下载、同步场景）。
5. 构建安装包并执行签名。
6. 在干净环境执行安装/卸载/重装验证。
7. 执行数据兼容验证（旧配置、旧数据库、旧目录）。
8. 编写发布说明（变更、已知问题、升级注意事项）。
9. 编写迁移说明（从 Go CLI 到 WPF 的迁移路径）。
10. 评审通过后打正式版本并归档交付物。

### 4.9.6 阶段 8 测试矩阵

验收测试建议覆盖：

1. 首次安装与首次启动路径。
2. 已有用户数据升级路径。
3. 离线/弱网场景下的失败提示与恢复。
4. 大数据量目录下资源库与下载页表现。

安装测试建议覆盖：

1. 安装成功后可直接启动。
2. 升级安装不破坏用户数据。
3. 卸载后残留文件符合预期。

回滚测试建议覆盖：

1. 版本回退后核心功能可用。
2. 回滚后用户关键数据不丢失。

### 4.9.7 阶段 8 交付物定义（DoD）

满足以下条件才算阶段 8 完成：

1. 第 14 节全部验收项通过且有证据。
2. 发布包构建、签名、安装验证通过。
3. 迁移说明与发布说明已完成并可对外使用。
4. 回滚策略已验证可执行。
5. 发布评审结论为“可发布”。

### 4.9.8 发布门禁建议

建议设定以下发布门禁：

- 自动化测试通过率达到预设阈值。
- 关键流程无 P0/P1 缺陷。
- 性能指标不低于阶段 7 基线。
- 安全与签名检查通过。

### 4.9.9 发布后观察窗口建议

建议上线后设置观察窗口并重点关注：

- 崩溃率
- 登录失败率
- 下载失败率
- 同步失败率
- 播放异常率

若关键指标异常，按回滚策略执行。

### 4.9.10 阶段 8 完成后的状态定义

阶段 8 完成后，迁移项目状态可定义为：

- 主迁移目标已达成。
- Go CLI 进入维护或退役阶段（按团队策略执行）。
- 后续版本进入常规产品迭代节奏。

## 4.10 AI 执行协议（强制）与文档评审结论

本节是对整份需求文档的执行级 review 结论，用于确保不同 AI 在不同轮次执行时仍能产出一致结果。

### 4.10.1 Review 结论（影响开发的一致性问题）

已识别并修复的关键问题：

1. 阶段计划完整，但缺少统一执行门禁，容易出现“跳阶段开发”。
2. 缺少统一命令基线，不同 AI 可能使用不同构建/测试入口。
3. 缺少变更边界约束，容易在单阶段中引入跨阶段改动。
4. 缺少失败分支处理规范，失败后可能反复试错且不可追踪。

结论：

- 文档现在已具备阶段化执行能力。
- 为保证 AI 可稳定落地，以下协议为强制执行项。

### 4.10.2 阶段门禁规则（强制）

门禁规则：

1. 只有当前阶段 DoD 全部通过，才能进入下一阶段。
2. 不允许在当前阶段引入“下一阶段核心能力代码”。
3. 每阶段必须输出“变更清单 + 验证结果 + 遗留风险”。

阶段推进最小条件：

- 阶段 0：工程可构建，测试工程可运行。
- 阶段 1：配置与初始化闭环通过。
- 阶段 2：登录与核心 API 可调用。
- 阶段 3：搜索闭环通过。
- 阶段 4：下载闭环通过。
- 阶段 5：同步闭环通过。
- 阶段 6：资源库与播放闭环通过。
- 阶段 7：UI 收口与回归通过。
- 阶段 8：验收、发布、回滚验证通过。

### 4.10.3 标准命令基线（强制）

后续 AI 执行时，统一使用如下命令基线（按实际路径调整）：

```powershell
# 构建
dotnet build dotnet/Asmroner.sln

# 运行全部测试
dotnet test dotnet/Asmroner.sln

# 运行单项目测试（示例）
dotnet test dotnet/tests/Asmroner.Application.Tests

# 发布（示例）
dotnet publish dotnet/Asmroner.Wpf/Asmroner.Wpf -c Release
```

约束：

- 禁止以“仅本地运行通过”为验收结论。
- 必须至少执行一次解决方案级测试。

### 4.10.4 变更边界与提交策略（强制）

变更边界：

1. 每次迭代只处理一个阶段的主任务。
2. 非必要不改动已通过 DoD 的前序阶段代码。
3. 若必须跨阶段修复，需在变更说明中标注原因与影响面。

提交策略：

- 每个阶段至少拆分为：
  - `feat(stage-x): core implementation`
  - `test(stage-x): add/adjust tests`
  - `docs(stage-x): update migration docs`

### 4.10.5 AI 输出模板（强制）

每次 AI 开发完成后，必须输出以下结构：

1. `阶段编号` 与 `目标`。
2. `修改文件列表`。
3. `完成项`（对应本阶段执行步骤编号）。
4. `未完成项/阻塞项`。
5. `验证结果`（命令与结果摘要）。
6. `是否满足 DoD`（是/否 + 理由）。

### 4.10.6 失败分支处理规范（强制）

当 AI 执行失败时必须按如下顺序处理：

1. 先记录失败点（步骤编号 + 错误信息 + 影响范围）。
2. 提供不超过 2 条修复路径（保守方案优先）。
3. 选择一条路径继续，禁止无限重试。
4. 若连续 3 次失败，停止扩展开发并回到最近通过的门禁点。

### 4.10.7 文档维护规范（强制）

后续每完成一个阶段，必须同步更新：

- 对应 `12.x` 小节的执行状态
- 第 14 节验收状态（可附阶段性通过记录）
- 第 15 节结论中的当前迁移进度

若文档与代码状态不一致，以“代码 + 测试结果”为准，并立即修正文档。

---

## 5. 迁移风险与应对策略

## 5.1 风险：API 行为变化

说明：

- asmr.one 域名、登录接口、搜索接口可能变化

应对：

- 保留最快域名探测逻辑
- 所有 API 封装统一在单独客户端中
- 为关键接口增加最小集成测试

## 5.2 风险：WPF 播放能力不等价

说明：

- 当前 WebUI 依赖浏览器播放器生态
- WPF 原生媒体支持未必等价

应对：

- 尽早确定播放器组件选型
- 先实现最小可播版本，再补字幕和增强功能

## 5.3 风险：后台任务阻塞 UI

说明：

- 下载、同步都属于长耗时操作

应对：

- 全部使用后台任务
- ViewModel 只绑定状态
- 使用取消令牌和进度回调

## 5.4 风险：兼容旧数据失败

说明：

- 一旦配置、数据库、目录命名改变，旧用户数据可能无法复用

应对：

- 第一阶段优先兼容旧路径和旧结构
- 如果必须变更，提供迁移器

## 5.5 风险：AI 一次性改动过大

说明：

- 如果让 AI 一次完成全部迁移，容易产生耦合和不可验证代码

应对：

- 严格分阶段提交
- 每阶段都有可运行、可验证结果

---

## 6. 验收标准

迁移完成后，至少应满足以下验收项：

1. WPF 客户端可以正常启动并完成初始化。
2. 可读取或导入现有配置。
3. 可成功登录并请求 asmr.one 接口。
4. 可使用普通与高级搜索语法完成搜索。
5. 可完成单个、批量、热门作品下载。
6. 可执行元数据同步、同步下载、失败重试。
7. 可查看同步统计与导出成功/失败记录。
8. 可浏览本地资源库并播放音频。
9. 下载目录结构与现有 Go 版本保持兼容。
10. SQLite 数据库可复用或可平滑迁移。

---

## 7. 最终结论

### 7.1 对现有项目的结论

当前项目已经具备完整的业务核心，但其交互形式仍停留在：

- CLI 操作
- 浏览器 WebUI 播放

它本质上还不是一个原生 Windows 客户端。

### 7.2 对迁移方案的结论

最佳迁移方向是：

- 以现有 Go 代码为业务参考源
- 以 .NET 服务层重建核心能力
- 以 WPF + MVVM 重建界面层
- 第一阶段尽量兼容现有配置、数据库和目录规则

### 7.3 对后续 AI 迁移执行的结论

后续 AI 迁移不应从 UI 开始，也不应采用“WPF 调 CLI”方式快速拼接，而应按以下主线推进：

1. 先建立 .NET 服务层骨架。
2. 再迁配置、数据库、API、搜索、下载、同步。
3. 最后接入 WPF 页面、播放能力和交互体验。

这条路径最稳，兼容性最好，也最适合持续迭代。

---
