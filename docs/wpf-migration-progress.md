# asmr-downloader WPF 项目进度跟踪

当前跟踪版本：v0.7.2

AI约束策略：变更与验证记录统一维护于 `docs/wpf-migration-history.md` 的 `§1`，单元测试清单统一维护于 `docs/wpf-migration-tests.md` 的 `§1`。

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
| 阶段 7 | UI 集成与体验收口      | 进行中 | AI + 用户 | 2026-04-18 | 待填写       | 待填写       | 已落地壳层状态/导航/对话框共享服务、页面级统一加载态/空态与状态栏消息桥接；UI 冒烟与手工回归待补齐。                                    |
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

## 1.5 维护规则

- 每次代码提交后检查并同步顶部当前跟踪版本、`1.2` 阶段总览、`2.1` 提交明细与 `3` 手工测试清单。
- 每个阶段状态变化或 DoD 进展变化后，更新 `1.2` 阶段总览与 `1.3` 阶段执行勾选清单。
- 出现阻塞、待验证问题或显式风险时，更新 `1.4` 阻塞与风险登记。
- 文档口径一致性必须保持：`docs/wpf-migration-progress.md` 与 `docs/wpf-migration-history.md`、`docs/wpf-migration-tests.md` 的版本口径、待提交说明与手工验证范围需同步。

## 2. Github提交记录

本节用于记录本仓库与迁移文档相关的计划提交和已提交记录，便于追踪文档同步、仓库清理与代码迁移边界。

提交策略：如果仅有提交明细的改动，不做单独提交，与后续的代码改动一同提交。

### 2.1 计划与提交明细

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
| 2026-04-18 | 已提交 | v0.7.1: unify shell page states and split migration docs                    | 1. Update runtime/docs version to v0.7.1.<br>2. Complete the stage 7 shell integration baseline.<br>3. Add shared page-state service, shell status-text synchronization, and shared page-state resource templates/styles.<br>4. Unify page-level state handling and panels.<br>5. Refactor prompt files and documents.<br>6. Expand WPF regression coverage, update regression tests and progress documentation.                   | c272014    |
| 2026-04-28 | 待提交 | v0.7.2: tighten shell status relay and extend stage 7 coverage              | 1. Update runtime/docs version to v0.7.2.<br>2. Refine stage 7 shell status relay policy to avoid hidden-page or placeholder message overrides.<br>3. Add/extend WPF tests for shell message relay and sync relay policy.<br>4. Sync migration progress/history/tests documentation and reset affected manual checklist items.                                                                                     | -          |

---

## 3. 功能测试验证清单

本章用于指导测试人员对当前已交付的 WPF 客户端执行功能测试与回归测试。当前范围覆盖阶段 1 到阶段 6，以及阶段 7 当前已落地的壳层导航、共享对话框、共享页面状态与共享样式基线。

AI约束：每次进行功能开发、缺陷修复或任何可能影响用户可见行为的改动时，必须先检查本章并将受影响的测试项重置为未勾选；AI 仅允许新增未勾选项或取消勾选需要重测的项目，重新勾选只能由人类用户在完成功能测试或回归测试后执行，并在必要时同步更新第 1.4 节和 `docs/wpf-migration-history.md` 中的 `§1` 记录。

说明：

- `[ ]`：本轮尚未验证，或因代码改动需要重新验证。
- `[x]`：本轮已验证通过。
- 任一项失败时，不得勾选该项；需要在第 1.4 节登记阻塞，或在 `docs/wpf-migration-history.md` 的 `§1` 中补充验证结果。

### 3.1 启动、配置与连接

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
- [ ] 主窗口标题不显示版本号，Settings 页面版本文案应显示 v0.7.2。

### 3.2 Search 功能

- [ ] 仅输入基础关键词即可成功搜索，并展示结果列表（含标签列）、总数和页码信息。
- [x] Search 结果列表列头应显示完整边框；首列标题显示“作品ID”；字幕列与日期列宽保持 `42/75` 且不可拖拽改宽，拖拽仅改变列顺序；当标题或标签过宽时可通过横向滚动查看完整数据。
- [x] 高级筛选 `tag/circle/va/duration/rate/price/sell/age/lang` 可单独或组合生效，`反选` 语义正确。
- [x] Search 的排序、方向、字幕、“包含翻译作品”“加入翻译作品”等选项生效，翻页后条件保持不丢失。
- [x] 上一页、下一页、跳页、页大小切换均可用，分页结果与页码信息正确。
- [x] “查询热门作品”可返回结果并展示到结果列表，日期、字幕、标签信息正确。
- [x] “清空”可重置关键词、排序选项、分页状态和当前结果，同时保留高级筛选输入与反选状态。
- [x] Search 页面在搜索或查询热门作品期间会显示统一加载态；当结果为空时显示统一空态文案，且恢复结果后空态会自动消失。
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

### 3.3 Download 功能

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
- [x] Download 页面在执行下载、立即下载、刷新列表与清空任务列表期间会显示统一加载态；当任务列表为空时显示统一空态文案。
- [x] Download 页面会恢复上一次运行时的“只下载高清音频”“加入翻译作品”“文件筛选”与未完成队列（`Pending/Queued/Failed` 恢复为 `Pending`），并在后台补拉缺失作品标题后刷新列表显示。
- [x] Search 页面加入下载队列后，若未切换至 Download 页面即退出并重启，未完成队列仍可恢复。
- [x] 文件筛选规则可生效；开启“只下载高清音频”后，在同时存在 flac/wav 与 mp3 的场景下不会重复下载 mp3。
- [x] 新启动的失败任务不会从列表中消失；失败、取消、完成后的任务状态可被稳定追踪。
- [x] Download 页面开启“加入翻译作品”后，单个入队、批量入队、从文件导入（CSV/JSON）三条入口都会按“简体中文 -> 繁体中文 -> 日本語”选择最终版本；关闭后保持输入或导入文件中的原始 `SourceId`。
- [x] Download 页面开启“加入翻译作品”且实际切换语言版本时，单个入队、批量入队、从文件导入（CSV/JSON）的状态提示会显示“其中 X 项已切换为翻译作品”。
- [x] 启动后台补拉结束后，Download 页状态提示统一使用“作品信息更新完成/失败”文案，不再出现“启动补拉完成/失败”。
- [x] 当轨道标题已自带扩展名时，下载落地文件名不会出现 `.mp3.mp3`、`.png.png` 等重复后缀；无扩展名标题仍会补齐正确后缀。
- [x] 启动后台补拉作品信息失败时，Download 列表中的未完成队列占位项会显示为 `Failed` 且错误信息可读。

### 3.4 Search/Download 联动与回归

- [x] Search 页面加入下载队列后，Download 页面可看到对应待执行任务，标题信息尽量不丢失。
- [x] Search、Download、Library、Settings、Sync 五个页面的状态信息面板与共享页面状态展示保持一致，长文本可换行显示且不遮挡正文内容。
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

### 3.5 Sync 功能

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
- [x] 在元数据同步、同步下载、失败重试、导出记录与刷新统计期间，Sync 页面会显示统一加载态，并在操作结束后自动恢复当前统计卡片与详情区。
- [x] 点击“重试失败项”后，“刷新统计”仍可点击，并能刷新当前下载状态、报表与摘要面板，同时保留当前重试详情，不被旧的 `UiState` 下载进度覆盖。
- [x] Sync 状态面板以两行分别显示“同步状态”和“下载状态”，且元数据同步与同步下载的状态文本不会互相覆盖。
- [x] “刷新统计”在自身执行期间会临时禁用，但不会把当前同步主按钮从 stop 语义切回 start 语义。
- [x] 点击“刷新统计”或完成同步/重试后，统计卡片与摘要面板需同步刷新，最近更新时间应同时反映元数据与同步记录的最新时间。

### 3.6 Library 功能

- [x] Library 页签可成功显示资源库页面；顶部副标题显示为“本地资源库，文件查看与播放。”，首次刷新后可看到作品总数、页码信息与状态文本更新。
- [x] 当下载目录/同步目录中同时存在 `[{SourceId}]{Title}` 与 `sourceId-date-sub/nosub-title` 两种命名格式时，Library 页面都能识别作品；其中 legacy 标题包含连字符时也不会漏显；非法目录会被跳过并在状态中体现，不导致应用崩溃。
- [x] 首次刷新或切换分页期间，窗口保持可响应；刷新/翻页/跳页/page size 切换期间相关按钮在扫描完成前会临时禁用，完成后恢复。
- [x] Library 页的关键字筛选、“仅显示带字幕作品”“仅显示含音频作品”与上一页/下一页/跳页/page size 切换均可正确更新列表结果。
- [x] 选中作品后，右侧独立“作品详情”区与文件树会同步更新；选中文件后点击“载入/切换文件”可更新当前打开目标；点击“清空上下文”会清除当前播放器状态，并按当前保留的作品/文件选择显示对应提示。
- [x] Library 作品列表首列标题显示“作品ID”；当标题、路径或结果较多时可通过横向与纵向滚动条查看完整内容，不会把显示内容撑出窗口范围。
- [x] Library 页面支持上一页、下一页、跳页与 page size 切换，页码输入、结果数量与页码信息保持一致。
- [x] 手动拉高窗口时，左侧作品列表与右侧文件树会随可用高度同步扩展，下边沿保持对齐，不再因固定高度出现错位。
- [x] 手动压低窗口时，作品列表、详情区、文件树与上下文文本都保持在窗口范围内显示，超出部分通过各自滚动区域承载。
- [x] 当资源库无结果时，Library 页面会显示统一空态文案；刷新恢复到有结果后，空态会自动隐藏。
- [x] 仅选中作品但未选中文件时，Library 页会提示当前作品是否包含可播放媒体文件，并给出首个候选路径或“未发现可播放媒体文件”的明确说明。
- [x] 当在文件树中选中目录、不可播放文件、缺失媒体文件与可播放媒体文件时，Library 页会分别显示明确提示，并正确更新“载入/切换文件”“播放”按钮可用性。
- [x] 当作品文件使用大小写混合的支持扩展名（如 `.FLAC`、`.OpUs`）时，Library 页仍会识别为可播放媒体文件，并允许后续载入与播放。
- [x] 显式选中可播放媒体文件后，Library 页可点击“播放”调用系统默认关联程序打开；未选中文件时“播放”不可点击。
- [x] 当选中文件缺失或系统默认关联程序打开失败时，Library 页会显示明确失败提示，不导致应用崩溃。
- [x] 当系统默认关联程序已成功拉起但底层 Shell 调用返回 `null` 时，Library 页不应误报“打开失败”。
- [x] Library 页不再显示“暂停 / 停止”按钮和时间进度文本；“当前选择”与“当前已载入上下文”会分开展示，且已载入区会显示状态、作品、当前文件与最近一次打开结果。
- [x] 清空上下文后，“当前已载入上下文”应恢复为未载入状态，但“当前选择”区仍保留当前作品或文件对应的提示，不与已载入状态混淆。

### 3.7 阶段 7 壳层与体验收口

- [x] 初始化完成前，Settings 页签保持可用，Search/Download/Library/Sync 四个主功能页签保持禁用；初始化成功后四个主功能页签统一恢复可用。
- [x] Search、Download、Library、Settings、Sync 五页在 `1280x720` 与更大窗口尺寸下共用卡片/输入框/按钮与共享页面状态视觉基线，不出现局部样式回退或边距错位。
- [x] Search 页面导出、Download 页面从文件导入、Sync 页面导出失败/成功记录均复用统一文件对话框流程，CSV/JSON 默认后缀补齐正确。
- [x] 在 Settings 页面执行“保存并重新初始化”成功或失败后，主窗口壳层状态栏会通过统一壳层消息区显示结果，且当前页仍保持在 Settings。
- [x] Search、Download、Library、Settings、Sync 五页在长耗时操作时会显示统一加载态，结束后自动隐藏且不遮挡原有核心内容。
- [x] Search、Download、Library、Sync 页面在无结果、无任务、空资源库或无同步记录场景下会显示统一空态文案；恢复数据后空态可自动消失。
- [x] Search、Download、Library、Settings、Sync 页内 `StatusTextBlock` 变化会同步到主窗口壳层状态栏；页面切换后壳层消息栏保持最近一次已发布的有效消息，且不会被隐藏页更新或占位文案错误覆盖。
