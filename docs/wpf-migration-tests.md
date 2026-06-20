# asmr-downloader WPF 单元测试清单

本文用于维护 WPF 迁移测试样例与基线口径，采用“一级目录 + 项目二级目录 + 文件三级目录”的结构维护。

## 1. 单元测试清单

本节用于沉淀“当前已落地测试样例 + 下一步新增测试计划”，并与第 12 节阶段目标保持一致。

说明：

- `已创建`：测试样例已存在于仓库。
- `已通过`：样例在最近一次可执行验证中通过；当前全量回归基线为 2026-06-20 的解决方案级回归（424/424），本轮受影响回归为 2026-06-20 的 `Asmroner.Wpf.Tests`（225/225）。

维护规则：

- 新增样例时按项目二级目录追加到对应文件三级目录，并保持项目分组结构稳定。
- 更新测试基线时同步更新顶部基线口径与受影响样例说明。
- 测试覆盖范围、版本口径与手工回归范围需与 `docs/wpf-migration-progress.md`、`docs/wpf-migration-history.md` 保持一致。

### 1.1 Application.Tests

#### 1.1.1 DownloadServiceTests.cs

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

#### 1.1.2 QueryParserServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                    | 输入                                    | 期望输出                               |
| ------ | ------ | ------ | --------------------------------------------------------- | --------------------------------------- | -------------------------------------- |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldParseAdvancedQuery`                    | 复杂高级检索语句（含负向条件+分页参数） | 解析字段正确，重建 query 保留关键参数  |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldReturnReadableError_WhenSyntaxInvalid` | 非法高级语法字符串                      | 返回可读解析错误                       |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldNotApplyDefaultAge_WhenAgeMissing`     | 查询未显式提供 age 条件                 | 不自动注入 `age` 默认值                |
| [x]    | [x]    | 阶段 4 | `QueryParser_ShouldParseSemicolonSeparatedFilters`        | 高级筛选使用分号分隔                    | 分号语法可正确映射到筛选字段           |
| [x]    | [x]    | 阶段 3 | `QueryParser_ShouldParseOptionOnlyQuery`                  | 仅包含分页/排序参数的查询串             | 可正确解析为无关键词查询并保留页面参数 |

#### 1.1.3 SearchExportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                   | 输入                       | 期望输出                             |
| ------ | ------ | ------ | ------------------------------------------------------------------------ | -------------------------- | ------------------------------------ |
| [x]    | [x]    | 阶段 3 | `SearchExportService_ShouldExportCsvAndJson`                             | 1 条搜索结果导出到临时目录 | 生成 CSV/JSON 文件，内容包含关键字段 |
| [x]    | [x]    | 阶段 3 | `SearchExportService_ShouldEscapeCsvFields_WhenTextContainsCommaOrQuote` | 标题含逗号、引号、换行     | CSV 格式正确转义                     |

#### 1.1.4 SearchServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                        | 期望输出                                    |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldAggregateMultiplePages`                | `pageSize=2`、目标返回 5 条 | 聚合总数=6、返回=5、调用页数=3、ID 顺序正确 |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldRespectRequestedPageAndKeepFilters`    | 指定 `page=2` 且含筛选参数  | 只请求第 2 页，过滤参数与分页参数保持不变   |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldReturnEmpty_WhenApiReturnsNoWorkItems` | API 返回空列表              | `ReturnedCount=0` 且不抛异常                |
| [x]    | [x]    | 阶段 3 | `SearchService_ShouldSearch_WhenOnlyPageOptionsProvided`    | 仅包含分页/排序参数的查询串 | 可正常发起搜索并返回结果                    |

#### 1.1.5 SearchStateStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                       | 输入                                                   | 期望输出                             |
| ------ | ------ | ------ | ------------------------------------------------------------ | ------------------------------------------------------ | ------------------------------------ |
| [x]    | [x]    | 阶段 4 | `EnqueueForDownload_ShouldNormalizeSourceIdExtractedFromUrl` | RJID、作品链接、API 路径、`RJ-xxxx` 与纯数字混合值入队 | 队列统一归一化为 `RJxxxx` 且去重正确 |
| [x]    | [x]    | 阶段 4 | `RemoveFromQueue_ShouldMatchNormalizedInput`                 | 队列已有 `RJxxxx` 时使用 URL 形式出队                  | 可匹配并成功移除                     |

#### 1.1.6 FirstRunServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                   | 输入                                         | 期望输出                   |
| ------ | ------ | ------ | -------------------------------------------------------- | -------------------------------------------- | -------------------------- |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldNotRequireSetup_WhenConfigValid`  | `config=AppConfig`，校验错误为空             | 返回 `RequiresSetup=false` |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldRequireSetup_WhenConfigMissing`   | `config=null`，校验错误为空                  | 返回 `RequiresSetup=true`  |
| [x]    | [x]    | 阶段 1 | `FirstRunService_ShouldRequireSetup_WhenValidationFails` | `config=AppConfig`，校验错误含"账号不能为空" | 返回 `RequiresSetup=true`  |

#### 1.1.7 SearchImportServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                  | 输入                           | 期望输出                             |
| ------ | ------ | ------- | ------------------------------------------------------- | ------------------------------ | ------------------------------------ |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldReturnItems_FromValidCsv`          | 有效 CSV（含标准 5 列）        | 正确解析 SourceId/Title/Release/Tags |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldSkipHeaderAndEmptyLines`           | 含空行的 CSV                   | 只返回有效数据行                     |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldHandleQuotedTitle_WithComma`       | 标题含逗号（RFC4180 引号包裹） | 正确解析含逗号 title                 |
| [x]    | [x]    | 阶段 4+ | `ParseCsvAsync_ShouldHandleEmbeddedDoubleQuote_InTitle` | 标题含双引号（`""`转义）       | 正确解析含引号 title                 |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldDeserializeItems_FromValidJson`   | 有效 JSON 数组（camelCase）    | 正确反序列化 SourceId/Title          |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldReturnEmpty_ForEmptyJsonArray`    | `[]`                           | 返回空列表                           |
| [x]    | [x]    | 阶段 4+ | `ParseJsonAsync_ShouldSkipEntries_WithEmptySourceId`    | 含空 sourceId 的条目           | 过滤空 sourceId，只返回有效项        |

#### 1.1.8 WorkLanguageSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                              | 输入                                                             | 期望输出                         |
| ------ | ------ | ------ | ----------------------------------------------------------------------------------- | ---------------------------------------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldPreferSimplifiedChinese_WhenAvailable`                | 日文原作 + 关联简中/繁中版本                                     | 选择简体中文版本作为最终入队目标 |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldPreferTraditionalChinese_WhenSimplifiedMissing`       | 日文原作 + 仅有关联繁中版本                                      | 回退选择繁体中文版本             |
| [x]    | [x]    | 阶段 4 | `SelectPreferredEdition_ShouldKeepJapaneseCurrentWork_WhenNoChineseEditionExists`   | 日文原作 + 仅有非中文关联版本                                    | 保持当前日本語作品入队           |
| [x]    | [x]    | 阶段 4 | `ResolveCurrentWorkLanguage_ShouldUseWorkAttributes_WhenTranslationInfoLangMissing` | `translation_info.lang` 缺失，但 `work_attributes` 含 `CHI_HANT` | 解析出当前作品语言为繁体中文     |

#### 1.1.9 EnqueueWorkInfoResolverTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                                             | 输入                                         | 期望输出                                                                            |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------- | ----------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldCreateSelectedEditionWorkInfo_WhenPreferredEditionOnlyExistsInRelatedEditions` | 仅拉到原始作品详情，关联版本列表中存在简中版 | 合成最终简中版 `WorkInfoDto`，并以优先版本 `SourceId` 返回，`SwitchedSourceCount=1` |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldReuseFetchedPreferredEdition_AndDeduplicateFinalSourceIds`                     | 原始作品与优先翻译版都已被拉取               | 复用已获取的优先翻译版详情，最终入队 `SourceId` 去重，`SwitchedSourceCount=1`       |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldReturnFailedSourceIds_WhenFetchFails`                                          | 混合成功与失败的作品详情拉取                 | 返回成功项并单独记录失败 `SourceId`，不阻塞其余作品入队，`SwitchedSourceCount=0`    |
| [x]    | [x]    | 阶段 4 | `ResolvePreferTranslatedAsync_ShouldUseWorkIdWhenProvided`                                                         | 传入 `SourceId + WorkId` 的 BJ 作品          | 直接使用数值 `WorkId` 拉取详情并保留原始 `SourceId`，`SwitchedSourceCount=0`        |

#### 1.1.10 MetadataSyncServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                                                          | 期望输出                                                                                                                            |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | ------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldInsertAllPages_WhenRemoteHasNewWorks`                    | 网站元数据总量 `101`、本地为空，分页返回 `100 + 1` 条元数据   | 顺序请求总量页与 2 个同步分页，新增 101 条，本地总量追平到 101 条，并把完成态进度写为本地总量 `101` / 字幕 `1`                      |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldTrackProcessedWorks_WhenExistingPagesContainOnlyUpdates` | 第 1 页 100 条均为本地已存在记录，第 2 页新增 1 条元数据      | 本次累计处理 `101` 条、累计新增 `1` 条；已有页更新会写入 SQLite，完成态进度保留累计处理条数                                         |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldSkip_WhenRemoteCountMatchesLocalCount`                   | 网站总量与本地总量相同，且不存在过期元数据                    | 仅查询网站总量，不执行分页同步，返回“无需同步”                                                                                      |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldRefreshExpiredMetadata_WhenPreviousRunCompleted`         | 上次进度为 `COMPLETED`，且本地存在超过元数据有效期的元数据    | 再次执行时触发过期刷新，并更新本地 `MetadataWork` 摘要                                                                              |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldReport_WhenLocalCountExceedsRemoteCount`                 | 本地元数据数量大于网站                                        | 不执行分页同步，返回“本地元数据数量高于网站，未执行同步”                                                                            |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldResumeFromSavedProgress_WhenStateIsUnfinished`           | 已保存 `STOPPED` 元数据进度，`NextPage=2`，本地已有第一页数据 | 只从第二页继续同步，结果标记 `ResumedFromProgress=true`，最终进度写成 `COMPLETED`，并保留本地总量 `101` / 字幕 `1` / 累计处理 `101` |
| [x]    | [x]    | 阶段 5 | `SyncMetadataAsync_ShouldStopAfterCurrentPage_WhenStopRequested`                  | 两页元数据同步，第一页写回后触发 stop request                 | 当前页完成后停止，结果标记 `WasStopped=true`，UiState `NextPage=2`，并保留当前本地总量 `100` / 字幕 `1` / 累计处理 `100`            |

#### 1.1.11 SyncDownloadServiceTests.cs

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

#### 1.1.12 SyncExportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                 | 输入                                 | 期望输出                                                                |
| ------ | ------ | ------ | ------------------------------------------------------ | ------------------------------------ | ----------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `ExportAsync_ShouldWriteCsvAndJson_ForRequestedStatus` | 失败记录导出 CSV + 成功记录导出 JSON | 按状态筛选导出，CSV/JSON 内容正确且 CSV 正确转义逗号与引号              |
| [x]    | [x]    | 阶段 5 | `ExportAsync_ShouldReturnNoOp_WhenNoItemsMatchStatus`  | 仅存在成功记录时导出失败记录         | 返回 0 条导出结果，不创建输出文件，并给出“没有可导出的失败同步记录”提示 |

#### 1.1.13 SyncReportServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                         | 输入                                              | 期望输出                                                      |
| ------ | ------ | ------ | -------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `GetReportAsync_ShouldBuildBreakdownAndProgress_FromSnapshots` | 元数据 10 条、已完成 3 条、失败 2 条、待处理 1 条 | 正确汇总字幕/无字幕拆分、完成大小与总进度/字幕进度/无字幕进度 |
| [x]    | [x]    | 阶段 5 | `GetReportAsync_ShouldReturnZeroProgress_WhenMetadataIsEmpty`  | 本地元数据为空，下载快照仅含失败统计              | 所有进度百分比安全回落为 `0.00%`，不出现除零异常或无效值      |

#### 1.1.14 LibraryScannerServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                                      | 期望输出                                                                                                   |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldCollectBracketedAndLegacyDirectories_AndOverlayMetadata` | 下载目录含 bracketed/legacy 目录、非法目录与本地 metadata | 扫描结果可同时识别两种目录格式，非法目录进入 skipped 列表，metadata 可覆盖标题/日期/字幕并统计音频文件数。 |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldPreferDuplicateWorkEntry_WithMorePlayableFiles`          | 下载根与同步根同时存在同一 `SourceId` 的目录              | 合并后优先保留音频文件更多的目录项，避免同一作品在资源库中重复展示。                                       |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldKeepReadableFiles_WhenNestedDirectoryEnumerationThrows`  | 嵌套目录子目录枚举抛异常，但当前目录文件仍可读取          | 扫描结果保留可读文件树并记录错误，不因单个嵌套目录异常而放弃整项作品。                                     |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldSkipUnreadableFile_WhenFileInspectionThrows`             | 单个媒体文件长度探测抛异常                                | 跳过不可读文件、保留其余文件并记录错误，不因单文件异常导致整项扫描失败。                                   |
| [x]    | [x]    | 阶段 6 | `ScanAsync_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCase`         | 目录内存在 `.FLAC`、`.OpUs` 等大小写混合支持扩展名文件    | 扫描结果会按共享可播放格式规则识别大小写混合扩展名文件，并正确统计 `AudioFileCount/IsPlayable`。           |

#### 1.1.15 LibraryQueryServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                                               | 期望输出                                                                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | -------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldFilterByKeywordSubtitleAndAudio_AndReturnPagingMetadata` | 预置扫描结果 + 关键字/字幕/音频筛选 + `pageSize=1` | 仅返回满足条件的作品，并保留 `SkippedDirectories/Errors/ScannedRootCount/ScannedWorkCount`。 |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldClampPageToLastPage_WhenRequestedPageExceedsRange`       | 预置 3 条扫描结果，请求超出范围的页码              | 查询页码会被钳制到最后一页，且返回按日期倒序后的尾页数据。                                   |
| [x]    | [x]    | 阶段 6 | `QueryAsync_ShouldHonorRequestedPageSize_WhenBuildingPagedItems`           | 预置 5 条扫描结果，请求 `page=2,pageSize=2`        | 返回第 2 页的 2 条数据，并正确输出 `PageSize/TotalPages/TotalCount` 等分页元数据。           |

#### 1.1.16 PlayerServiceTests.cs

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

### 1.2 Core.Tests

#### 1.2.1 AppConfigTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                   | 输入                 | 期望输出                                                                                                                              |
| ------ | ------ | ------ | ---------------------------------------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `AppConfig_ShouldHaveReasonableDefaults` | 新建默认 `AppConfig` | `MaxWorkers=4`、`MaxRetries=3`、`SyncWantedSize=5GB`、`PreferFormats=mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass` |

#### 1.2.2 DownloadFilterParserTests.cs

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

#### 1.2.3 SourceIdNormalizerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                               | 期望输出                                           |
| ------ | ------ | ------ | --------------------------------------------------------------- | -------------------------------------------------- | -------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Normalize_ShouldExtractBjId_FromDlsiteBooksUrl`                | `https://www.dlsite.com/books/work/.../BJ02370869` | 从 URL 中提取并归一化为 `BJ02370869`               |
| [x]    | [x]    | 阶段 4 | `ToApiNumericId_ShouldNotTreatBjSourceIdSuffix_AsNumericWorkId` | `BJ02370869`                                       | 不误将 `BJ` 后缀数字当作 API `workId` 直接截断返回 |

#### 1.2.4 LibraryDirectoryNameParserTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                            | 输入                                                | 期望输出                                                                     |
| ------ | ------ | ------ | ----------------------------------------------------------------- | --------------------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseBracketedDirectoryName`                      | 目录名 `[{SourceId}]{Title}`                        | 可正确解析 `SourceId/Title/Scheme=Bracketed`。                               |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseLegacyDirectoryName`                         | 目录名 `sourceId-date-sub/nosub-title`              | 可正确解析 `SourceId/Title/Release/HasSubtitle/Scheme=LegacyListen`。        |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldParseLegacyDirectoryName_WhenTitleContainsHyphen` | 目录名 `sourceId-date-sub/nosub-legacy-title-part2` | 标题部分即使包含连字符，也会被完整保留并正确解析为 `LegacyListen` 作品目录。 |
| [x]    | [x]    | 阶段 6 | `TryParse_ShouldReturnFalse_ForInvalidDirectoryName`              | 非法资源目录名                                      | 返回 `false` 且结果对象保持空值，不把非法目录误识别为作品目录。              |

#### 1.2.5 LibraryPlayableMediaPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                             | 期望输出                                                   |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | -------------------------------- | ---------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `IsPlayableExtension_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCaseAndDot` | 支持扩展名的大写/缺少点号输入    | 共享格式规则会忽略大小写，并兼容带点或不带点的扩展名判断。 |
| [x]    | [x]    | 阶段 6 | `CountPlayableFiles_ShouldCountNestedPlayableMediaFiles`                          | 含嵌套目录与多种媒体文件的作品树 | 可递归统计作品树中的可播放媒体文件数量。                   |
| [x]    | [x]    | 阶段 6 | `FindFirstPlayableFile_ShouldReturnFirstDepthFirstPlayableFile`                   | 目录优先展示的嵌套作品树         | 会按当前树结构的深度优先顺序返回首个可播放媒体文件。       |
| [x]    | [x]    | 阶段 6 | `FindFirstPlayableFile_ShouldReturnNull_WhenNoPlayableFileExists`                 | 不含任何支持扩展名文件的作品树   | 返回 `null`，供上层显示“未发现可播放媒体文件”的明确提示。  |

### 1.3 Infrastructure.Tests

#### 1.3.1 TokenStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                            | 输入                           | 期望输出                  |
| ------ | ------ | ------ | --------------------------------- | ------------------------------ | ------------------------- |
| [x]    | [x]    | 阶段 2 | `TokenStore_ShouldRoundTripToken` | 写入 `ApiToken(abc123)` 后读取 | 读取 token 不为空且值一致 |

#### 1.3.2 AsmrApiClientTests.cs

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

#### 1.3.3 AuthServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                 | 期望输出                                            |
| ------ | ------ | ------ | ---------------------------------------------------------- | -------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldRejectLogin_WhenAccountOrPasswordEmpty` | 空账号或空密码       | 抛出明确业务异常                                    |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldStoreTokenAfterLogin`                   | 登录返回 `jwt-token` | 返回 token 且 `TokenStore` 已持久化该 token         |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldThrowReadableErrorOnFailure`            | 登录接口返回 401     | 抛出 `AsmrApiException`，错误码 `auth_login_failed` |

#### 1.3.4 ConfigurationServiceTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                        | 输入                                                                       | 期望输出                                                       |
| ------ | ------ | ------- | ----------------------------------------------------------------------------- | -------------------------------------------------------------------------- | -------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `ConfigurationService_ShouldReturnValidationErrors_WhenRequiredFieldsMissing` | 缺失账号/密码、`MetadataValidityDays` 非法，且 `SyncWantedSize` 非法的配置 | 返回可读校验错误集合，且包含元数据有效期与同步容量上限相关提示 |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldSaveAndLoadConfig_FromSplitSqliteSections`        | 临时目录、包含账号与下载参数的配置对象                                     | 按 `user/downloader/limit` 三段写入 SQLite 并正确读取          |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldLoadFromDefaultConfigJson_WhenSqliteMissing`      | 无 SQLite 配置，仅程序目录 `config.json`                                   | 可读取默认配置并返回                                           |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldPreferSqliteOverDefaultConfigJson`                | 同时存在 SQLite 与 `config.json`                                           | 优先读取 SQLite 实际配置                                       |
| [x]    | [x]    | 阶段 4+ | `ConfigurationService_ShouldMigrateLegacySingleRowAppConfig`                  | 旧 `Id=1` 单行 AppConfig                                                   | 自动迁移为分段结构并可正常读取，旧目录字段被清空且不再保留     |

#### 1.3.5 EndpointDiscoveryServiceTests.cs

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

#### 1.3.6 DatabaseInitializerTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                      | 输入                        | 期望输出                                                                               |
| ------ | ------ | ------- | --------------------------------------------------------------------------- | --------------------------- | -------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `DatabaseInitializer_ShouldCreateNewSchema_AndEnsureSyncTables`             | 临时目录下初始化数据库      | 创建 `AppConfig/UiState/FavoriteWork/MetadataWork/WorkSyncInfo` 结构，并补齐同步表字段 |
| [x]    | [x]    | 阶段 4+ | `DatabaseInitializer_ShouldMigrateLegacySingleRowAppConfig_ToSplitSections` | 预置旧单行 AppConfig 数据库 | 初始化后自动迁移到分段结构，且 `user/downloader/limit` 三段均存在，旧目录字段被清空    |

#### 1.3.7 AsmrApiOptionsProviderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                                         | 期望输出                                              |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ------------------------------------------------------------ | ----------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldParseConfiguredUrlLists`                      | 自定义 `api_url` + 候选/发布源 URL 列表（含分号/逗号与重复） | 正确解析并去重，`BaseUrl` 与候选/发布源列表按预期生成 |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldFallbackToDefaults_WhenConfiguredUrlsMissing` | 空 `api_url` 与空列表字段                                    | 回退默认 API 地址、默认候选地址与默认发布源地址       |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldExpandLegacyBuiltInCandidateSubset`           | 旧版内置候选子集 `api.asmr-300.com;api.asmr.one`             | 自动扩展为当前完整默认候选集合并保持 BaseUrl 优先     |
| [x]    | [x]    | 阶段 2 | `AsmrApiOptionsProvider_ShouldIgnoreInvalidUrls_InConfiguredLists`          | 列表中混入非法 URL                                           | 仅保留合法 URL 项，非法项被忽略                       |

#### 1.3.8 ApiEndpointUrlServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                     | 输入                                                      | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------ | --------------------------------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldUpdateApiUrl_AndCandidateUrls_WhenConfigExists`             | 配置存在，发现结果返回新 BaseUrl 与最新公开候选集合       | `downloader.api_url` 与 `downloader.api_candidate_urls` 被一起更新并保存 |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldMergeDiscoveredAndSavedCandidates_WhenMergedCountIncreases` | 已保存公开候选子集 + discovery 仅返回部分新候选           | 按 discovery 优先顺序合并新旧候选，且保留旧候选后写回 SQLite             |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldKeepCustomCandidateList_WhenDiscoveryAddsPublicCandidates`  | 自定义候选集合 + discovery 返回公开候选与新自定义 BaseUrl | 保留自定义候选集合边界，仅把新自定义 BaseUrl 前置保存                    |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldSkipSave_WhenBaseUrlAndCandidatesAreUnchanged`              | 已有 BaseUrl/候选集合与发现结果一致                       | 不重复写入配置，直接返回发现结果                                         |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldKeepSavedCandidateList_WhenMergedCountDoesNotIncrease`      | 已保存候选集合已覆盖 discovery 结果                       | 不因 discovery 子集结果缩减或覆盖已保存候选集合                          |
| [x]    | [x]    | 阶段 2 | `DiscoverAndPersistAsync_ShouldSkipSave_WhenConfigMissing`                                 | 配置不存在，发现结果返回新 BaseUrl                        | 不写入配置文件，返回发现结果                                             |
| [x]    | [x]    | 阶段 2 | `GetCurrentBaseUrlAsync_ShouldReturnDefault_WhenConfigMissingOrEmpty`                      | 配置缺失或 `api_url` 为空白                               | 返回默认 API 基础地址                                                    |

#### 1.3.9 ConnectivityProbeServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                       | 期望输出                              |
| ------ | ------ | ------ | ------------------------------------------------------------------ | -------------------------- | ------------------------------------- |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldDiscoverAndAuthenticate_WhenDependenciesSucceed` | Discover 成功 + 登录成功   | 返回可达且鉴权成功，包含 BaseUrl/延迟 |
| [x]    | [x]    | 阶段 2 | `ProbeAsync_ShouldReturnFailureResult_WhenAuthenticationThrows`    | Discover 成功 + 登录抛异常 | 返回不可达结果并携带失败消息          |

#### 1.3.10 Existing suite updates

| 已创建 | 已通过 | 阶段   | 样例名                                                                                                                  | 输入           | 期望输出                                 |
| ------ | ------ | ------ | ----------------------------------------------------------------------------------------------------------------------- | -------------- | ---------------------------------------- |
| [x]    | [x]    | 阶段 2 | `AsmrApiClient_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AsmrApiClientTests.cs`）                              | 客户端请求链路 | 仅使用当前 BaseUrl 服务，不触发 Discover |
| [x]    | [x]    | 阶段 2 | `AuthService_ShouldUseCurrentBaseUrlService_WithoutDiscovery`（`AuthServiceTests.cs`）                                  | 登录链路       | 仅使用当前 BaseUrl 服务，不触发 Discover |
| [x]    | [x]    | 阶段 2 | `EndpointDiscoveryService_ShouldUseConfiguredPublishSources_ForDynamicCandidates`（`EndpointDiscoveryServiceTests.cs`） | 发布源地址配置 | 来自配置，且可动态发现候选 API           |

#### 1.3.11 UiStateStoreTests.cs

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

#### 1.3.12 NLogAppLogServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                 | 输入                                    | 期望输出                                    |
| ------ | ------ | ------ | ------------------------------------------------------ | --------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `Configure_ShouldWriteLogEntry_WhenStaticLoggerIsUsed` | 调用 `Configure` 后使用静态 logger 写入 | 目标日志文件存在且包含 `test-message-` 文本 |

#### 1.3.13 MemoryWorkInfoCacheTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                | 输入                               | 期望输出                                          |
| ------ | ------ | ------ | ----------------------------------------------------- | ---------------------------------- | ------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `TryGet_ShouldRequireFullEntry_WhenRequirementIsFull` | 仅写入 Summary 级作品缓存          | `Any` 命中，`Full` 要求不命中                     |
| [x]    | [x]    | 阶段 4 | `Set_ShouldPreserveFullEntry_WhenSummaryArrivesLater` | 先写入 Full，后写入同作品 Summary  | 缓存保留完整作品详情，不被摘要覆盖                |
| [x]    | [x]    | 阶段 4 | `TryGet_ShouldResolveNumericAlias_WhenWorkIdKnown`    | `BJ` 作品缓存 + 数值 `WorkId` 查找 | 可经数值 `WorkId` 命中同一作品缓存                |
| [x]    | [x]    | 阶段 4 | `Entries_ShouldExpirePerItem_AfterConfiguredTtl`      | 两条缓存按不同时间写入并等待过期   | 逐条按 TTL 过期，先写入项先失效，后写入项仍可命中 |

#### 1.3.14 CachedAsmrApiClientTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                                               | 期望输出                                                     |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `GetWorkInfoAsync_ShouldReturnCachedFullEntry_WithoutCallingInnerClient`   | Full 级作品缓存 + 内层 API 客户端                  | 直接返回缓存详情，不调用内层 `GetWorkInfoAsync`              |
| [x]    | [x]    | 阶段 4 | `GetWorkInfoAsync_ShouldUseCachedNumericId_WhenSummaryWasWarmedBySearch`   | Search 结果预热 Summary + 后续详情查询             | 使用缓存内 `WorkId` 补拉完整详情并回写 Full 缓存             |
| [x]    | [x]    | 阶段 4 | `GetTracksAsync_ShouldUseCachedNumericId_WhenPopularWarmupHasSummaryEntry` | 热门结果预热 Summary + 非 `RJ` `SourceId` 轨道查询 | `tracks` 查询复用缓存内数值 `WorkId`，避免直接用 `source_id` |

#### 1.3.15 FavoriteStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                   | 输入                                         | 期望输出                                                                                          |
| ------ | ------ | ------ | ------------------------------------------------------------------------ | -------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SaveFavoriteFolderItemsAsync_ShouldRoundTripAndSkipExistingDuplicates`  | 同一收藏夹重复保存同一 `SourceId` 与新增作品 | SQLite 收藏夹按 `FolderTitle + SourceId` 去重，新增/跳过计数正确，已存在作品可更新标题与 `WorkId` |
| [x]    | [x]    | 阶段 4 | `LoadFavoriteFolderTitlesAsync_ShouldDistinctTrimAndSortCaseInsensitive` | 混合空白、大小写变体与带首尾空格的收藏夹标题 | 收藏夹标题去空白、大小写不敏感去重，并按字母序稳定返回                                            |

#### 1.3.16 MetadataSyncStoreTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                            | 输入                                                       | 期望输出                                                                     |
| ------ | ------ | ------ | --------------------------------------------------------------------------------- | ---------------------------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldTrackSyncDownloadSnapshot_AndCleanupPendingRows`         | SQLite 中预置 2 条元数据并写入一条 Completed、一条 Pending | 快照正确统计完成/待处理数量与已落盘大小，且清理 Pending 后目录与记录一并移除 |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldReturnFailedSyncDownloads_ForRetry`                      | SQLite 中预置失败/成功两类同步记录                         | 仅返回 `FAILED` 记录，并保留重试次数、失败原因与时间信息                     |
| [x]    | [x]    | 阶段 5 | `MetadataSyncStore_ShouldReturnSyncDownloadsByStatus_ForExport`                   | SQLite 中预置 Completed/Failed 两类同步记录                | 可按状态分别返回导出候选记录，并保留目录大小、失败原因与字幕标记             |
| [x]    | [x]    | 阶段 5 | `UpsertMetadataWorksAsync_ShouldUpdateExistingRows_WithoutCountingThemAsInserted` | SQLite 中已存在同 Id 记录，再次 upsert 更新标题与字幕标记  | 已有记录被正确更新，快照同步反映新字段值，且返回的“新增数”为 `0`             |

### 1.4 IntegrationTests

#### 1.4.1 ApplicationBootstrapperTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                               | 期望输出                                                  |
| ------ | ------ | ------ | ---------------------------------------------------------- | ---------------------------------- | --------------------------------------------------------- |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldFailGracefully_WhenDatabaseInitThrows` | 模拟数据库初始化异常               | `IsSuccess=false` 且错误信息可读                          |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldRequireSetup_WhenConfigMissing`        | 缺失配置场景执行 `InitializeAsync` | `RequiresSetup=true`、`IsSuccess=false`，数据库文件已创建 |
| [x]    | [x]    | 阶段 1 | `Bootstrapper_ShouldSucceed_WhenConfigValid`               | 有效配置场景执行 `InitializeAsync` | `IsSuccess=true`、`RequiresSetup=false`，同步目录存在     |

### 1.5 Wpf.Tests

#### 1.5.1 DownloadInputNormalizerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                            | 期望输出                                |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------------------- | --------------------------------------- |
| [x]    | [x]    | 阶段 4 | `NormalizeSingleInputDisplay_ShouldExtractRjIdFromWorkUrl`                | 单个输入框粘贴作品 URL                          | UI 实时显示归一化 `RJxxxx`              |
| [x]    | [x]    | 阶段 4 | `NormalizeBatchInputDisplay_ShouldNormalizeAndDeduplicateSourceIds`       | 批量输入框混合 RJID/URL/`RJ-xxxx`/纯数字/重复值 | UI 实时显示为归一化且去重后的 RJID 列表 |
| [x]    | [x]    | 阶段 4 | `NormalizeBatchInputForSubmit_ShouldSupportCommaSemicolonSpaceAndNewline` | 批量输入含逗号/分号/空格/换行混排，点击提交     | 统一归一化并去重                        |

#### 1.5.2 DownloadCommandAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                      | 输入                             | 期望输出                                                   |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------- | -------------------------------- | ---------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldToggleCommandAvailability_ByTaskState`                                      | 不同任务状态输入到命令可用性规则 | 取消/重试/立即下载按钮状态与任务状态一致                   |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancel_WhenQueuedTaskSelected`                                         | 选中 `Queued` 状态任务           | “取消选中任务”按钮可用                                     |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowCancelAndImmediateStart_WhenPendingTaskSelected`                       | 选中 `Pending` 状态任务          | “取消选中任务”与“立即下载选中任务”按钮可用                 |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldDisableRetry_ForFailedPlaceholderRow_WhenSelectionExists`                   | 选中 `TaskId=Empty` 的失败占位行 | 在存在选中项时禁用“重试失败任务”，但允许“立即下载选中任务” |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowRetryWithoutSelection_WhenAnyFailedTaskExists`                         | 无选中项，任务列表中存在失败任务 | “重试失败任务”按钮可用，并回退为批量重试全部失败任务       |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldAllowRetry_WhenSelectionContainsRetryableFailedTask_AndIgnoreOtherStatuses` | 混合选中 Failed/Completed 等状态 | 只要选中集合中存在可重试失败任务，即允许触发重试           |

#### 1.5.3 DownloadViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                       | 输入                                | 期望输出                                                                                                                             |
| ------ | ------ | ------ | -------------------------------------------------------------------------------------------- | ----------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls`                     | 解析 `DownloadView.xaml` 的文本/XML | 关键样式资源、核心控件、“从文件导入”“从收藏夹导入”按钮、统一状态面板样式与共享 `PageStatePresenterTemplate` 存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldNotContainStagePrefixText`                                           | 解析 `DownloadView.xaml` 文本       | 页面不再包含“阶段 ”前缀文案。                                                                                                        |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldUseHeaderBorders_AndLockStatusWidthWhileLeavingProgressResizable`    | 解析 `DownloadView.xaml` 文本/XML   | 列头显示边框；状态列宽保持 `50` 且不可拖拽改宽；进度列宽保持 `96` 且允许调整；数据过宽时支持横向滚动与列重排。                       |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldContainQueueTranslationCheckbox`                                     | 解析 `DownloadView.xaml` 文本/XML   | 页面包含默认勾选的“加入翻译作品”复选框，且位于“只下载高清音频”右侧。                                                                 |
| [x]    | [x]    | 阶段 4 | `DownloadViewXaml_ShouldPlaceRunQueueBetweenStartSelectedAndRefresh_AndRemoveRetryAllButton` | 解析 `DownloadView.xaml` 文本       | “执行下载队列”位于“立即下载选中任务”和“刷新任务列表”之间，且页面不再包含“重试全部失败任务”按钮。                                     |

#### 1.5.4 MainWindowXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                            | 期望输出                                                                                                                               |
| ------ | ------ | ------ | ------------------------------------------------------------- | ------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `MainWindowXaml_ShouldUse1280x720DefaultWindowSize`           | 解析 `MainWindow.xaml` 文本/XML | 默认与最小窗口尺寸为 `1280x720`，主窗口标题为 `Asmroner`，并包含 `Library/Sync` 页签、壳层状态栏绑定与页签启用绑定，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `MainWindowXaml_ShouldNotContainVersionInWindowTitle`         | 解析 `MainWindow.xaml` 文本     | 标题不包含版本号前缀（例如 `Asmroner v`）。                                                                                            |
| [x]    | [x]    | 阶段 6 | `MainWindowXaml_ShouldPlaceLibraryTab_BetweenDownloadAndSync` | 解析 `MainWindow.xaml` 文本     | `Library` 页签位于 `Download` 与 `Sync` 之间，且存在 `LibraryHost` 宿主控件。                                                          |

#### 1.5.5 SearchViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                     | 输入                            | 期望输出                                                                                                                           |
| ------ | ------ | ------ | -------------------------------------------------------------------------- | ------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldContainUnifiedCardStyles_AndCoreControls`            | 解析 `SearchView.xaml` 文本/XML | 卡片化样式资源、核心控件、“导出到文件”“收藏作品”按钮、统一状态面板样式与共享 `PageStatePresenterTemplate` 存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 7 | `SearchViewXaml_ShouldUseAlignedComboBoxStyles`                            | 解析 `SearchView.xaml` 文本/XML | 下拉框与选项项样式基于共享壳层 `ComboBox`/`ComboBoxItem` 样式，且 XAML 可被解析。                                                  |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldNotContainStagePrefixText`                           | 解析 `SearchView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案。                                                                                                      |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldUseHeaderBorders_AndLockSubtitleAndDateColumnWidths` | 解析 `SearchView.xaml` 文本/XML | 结果列表列头显示边框；首列标题使用本地化“作品ID”；字幕/日期列宽保持 `42/75` 且不可拖拽改宽；数据过宽时支持横向滚动与列重排。       |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldUseSearchViewClassName`                              | 解析 `SearchView.xaml` 文本     | `x:Class` 为 `Asmroner.Wpf.Views.SearchView`，且根元素命名为 `Root`。                                                              |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldWireSelectionChangedHandlersForQueryOptions`         | 解析 `SearchView.xaml` 文本     | 排序/方向/字幕/页大小下拉均绑定 `SelectionChanged`                                                                                 |
| [x]    | [x]    | 阶段 3 | `SearchViewXaml_ShouldContainResultsGridContextMenuItems`                  | 解析 `SearchView.xaml` 文本     | 结果表格包含右键菜单四项操作及对应事件绑定                                                                                         |
| [x]    | [x]    | 阶段 4 | `SearchViewXaml_ShouldContainSeparateQueueTranslationCheckbox`             | 解析 `SearchView.xaml` 文本     | 搜索筛选用“包含翻译作品”与入队用“加入翻译作品”两个复选框并存，且后者位于前者右侧并默认勾选。                                       |

#### 1.5.6 SettingsViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                              | 期望输出                                                                                                                     |
| ------ | ------ | ------ | ------------------------------------------------------------- | --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldContainCardSections_AndActionButtons` | 解析 `SettingsView.xaml` 文本/XML | 卡片分区、下载目录/同步下载目录/元数据有效期输入框、核心动作按钮与共享 `PageStatePresenterTemplate` 存在，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 4 | `SettingsViewXaml_ShouldNotContainStagePrefixText`            | 解析 `SettingsView.xaml` 文本     | 页面不再包含“阶段 ”前缀文案，且根元素命名为 `Root`。                                                                         |

#### 1.5.7 DownloadTaskListComposerTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                       | 输入                                                                      | 期望输出                                                                  |
| ------ | ------ | ------ | ---------------------------------------------------------------------------- | ------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldIncludeQueuedPendingAndCanceledRows_BeforeActiveRows`     | 活跃任务 + 排队任务 + 已取消占位覆盖 + 标题映射                           | 输出按状态优先级排序（Running→Pending→Canceled），且状态/错误文案映射正确 |
| [x]    | [x]    | 阶段 4 | `From_ShouldMapCompletedStatusAndProgressText`                               | 已完成任务（含进度、目录字段）                                            | 行视图模型正确映射 `StatusText=已完成` 与 `ProgressText=100% (n/n)`       |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldResetCanceledOverrideToPending_WhenSourceRequeued`        | 已取消占位任务再次被加入下载队列                                          | 已重新入队任务状态显示为 `Pending/未下载`                                 |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldFillActiveTitleFromQueuedTitleCache_WhenTaskTitleIsEmpty` | 活跃任务标题为空且存在预取标题缓存                                        | 活跃任务行标题可从缓存回填，刷新后不丢失                                  |
| [x]    | [x]    | 阶段 4 | `ComposeRows_ShouldOrderByStatusAscending_ThenBySourceIdAscending`           | 混合6个任务，含Completed、Running、Queued、Pending、Failed、Canceled各1个 | 输出按状态排序优先级递增，同级按SourceId递增，验证Attribute驱动的排序正确 |

#### 1.5.8 DownloadTaskSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                               | 期望输出                                                              |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------------------------ | --------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetCancelable_ShouldReturnOnlyPendingQueuedRunning`                  | 含 Pending/Queued/Running/Failed/Completed/Canceled 的混合选中任务 | 仅返回 Pending、Queued、Running 三类可取消任务                        |
| [x]    | [x]    | 阶段 4 | `GetImmediateStartTargets_ShouldFilterStatusAndDeduplicateBySourceId` | 含可立即下载与不可立即下载状态，且 SourceId 重复                   | 仅返回 Pending/Failed/Canceled 且按 SourceId 去重后的立即下载目标集合 |

#### 1.5.9 DownloadExecutionArgsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                      | 期望输出                                    |
| ------ | ------ | ------ | --------------------------------------------------------------- | ----------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `NormalizeFileFilter_ShouldReturnNull_ForNullOrWhitespace`      | `null`、空字符串、纯空白字符串            | 统一归一化为 `null`，避免调用层重复空值判断 |
| [x]    | [x]    | 阶段 4 | `NormalizeFileFilter_ShouldTrimAndReturnValue_ForNonEmptyInput` | 含前后空白的筛选文本（如 `+voice;-demo`） | 返回去首尾空白后的原始筛选文本              |

#### 1.5.10 DownloadOperationPromptsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                                            | 输入                                                  | 期望输出                                                      |
| ------ | ------ | ------ | ------------------------------------------------------------------------------------------------- | ----------------------------------------------------- | ------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildCancelConfirmMessage_ShouldContainCancelableCount`                                          | 可取消任务数 `3`                                      | 返回“将取消 3 个任务，是否继续？”                             |
| [x]    | [x]    | 阶段 4 | `BuildRetryConfirmMessage_ShouldContainSelectedScopePreviewAndEllipsis_WhenExceedingPreviewLimit` | 6 条选中失败任务 SourceId，默认预览上限 5，最大并发 2 | 返回含“选中的”范围说明、前 5 项预览和省略号的批量重试确认文案 |
| [x]    | [x]    | 阶段 4 | `BuildRetryConfirmMessage_ShouldContainAllScopeWithoutEllipsis_WhenWithinPreviewLimit`            | 2 条全部失败任务 SourceId，最大并发 4                 | 返回含“全部”范围说明且不带省略号的批量重试确认文案            |

#### 1.5.11 DownloadOperationStatusTextsTests.cs

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

#### 1.5.12 DownloadConfirmationPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                              | 输入                              | 期望输出                                                     |
| ------ | ------ | ------ | --------------------------------------------------- | --------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldContinue_WhenResultIsYes`           | `MessageBoxResult.Yes`            | 返回 `ShouldContinue=true` 且 `StatusText=null`              |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldCancelOperation_WhenResultIsNotYes` | `MessageBoxResult.No/Cancel/None` | 返回 `ShouldContinue=false` 且 `StatusText=已取消本次操作。` |

#### 1.5.13 DownloadOperationPrecheckPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                | 期望输出                                                 |
| ------ | ------ | ------ | --------------------------------------------------------------------- | --------------------------------------------------- | -------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `CheckCancel_ShouldReturnPrompt_WhenSelectionEmpty`                   | 空选中集合                                          | 返回不可继续且提示“请先选择要取消的任务。”               |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldReturnSelectedFailedTargets_AndIgnoreOtherStatuses` | 选中 Failed/Completed 混合任务                      | 返回可继续，仅提取选中的失败任务作为重试目标             |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldRejectWhenSelectionHasNoRetryableFailedTargets`     | 选中占位失败行与非失败行                            | 返回不可继续且提示“选中项中没有可重试的失败任务。”       |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldFallbackToAllFailed_WhenSelectionEmpty`             | 无选中项，任务列表中存在失败任务                    | 返回可继续，并回退为全部失败任务重试目标                 |
| [x]    | [x]    | 阶段 4 | `CheckRetry_ShouldRejectWhenSelectionEmpty_AndNoFailedTasksExist`     | 无选中项，任务列表中没有失败任务                    | 返回不可继续且提示“当前没有失败任务可重试。”             |
| [x]    | [x]    | 阶段 4 | `CheckStartImmediate_ShouldReturnDeduplicatedTargets`                 | 含 Pending/Failed/Canceled/Running 且 SourceId 重复 | 返回可继续，目标集合按立即下载规则筛选且按 SourceId 去重 |

#### 1.5.14 DownloadDirectoryPathPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                                                    | 期望输出                                         |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | ------------------------------------------------------- | ------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsNull`             | `configuredPath=null`，默认目录 `C:/downloads/default`  | 返回默认目录路径                                 |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnTrimmedConfiguredPath_WhenConfiguredPathProvided` | `configuredPath="  C:/downloads/custom  "`              | 返回去首尾空白后的配置目录 `C:/downloads/custom` |
| [x]    | [x]    | 阶段 4 | `Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsWhitespace`       | `configuredPath="   "`，默认目录 `C:/downloads/default` | 返回默认目录路径                                 |

#### 1.5.15 DownloadQueueCachePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                                                            | 期望输出                                                            |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------------------------------------------------- | ------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Reconcile_ShouldMergePrefetchedTitles_AndKeepActiveTitles` | 活跃 sourceId 集合 + 预取标题 + 现有标题缓存 + 现有状态覆盖缓存 | 合并非空预取标题，保留活跃项标题缓存，仅清理活跃项状态覆盖          |
| [x]    | [x]    | 阶段 4 | `Reconcile_ShouldKeepCaseInsensitiveLookup`                 | 小写 sourceId 预取标题                                          | 输出标题字典支持不区分大小写查询（`RJxxxx` 可读取 `rjxxxx` 写入值） |

#### 1.5.16 DownloadWorkInfoTitlePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                              | 期望输出                                    |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildNonEmptyTitleMap_ShouldFilterWhitespaceTitles`                  | 含正常标题、空字符串、纯空白标题的 WorkInfo 集合  | 仅保留非空白标题映射                        |
| [x]    | [x]    | 阶段 4 | `BuildNonEmptyTitleMap_ShouldUseCaseInsensitiveKeys_AndLastWriteWins` | 同一 sourceId 的大小写变体键（`rjxxxx`/`RJxxxx`） | 输出字典大小写不敏感，重复 key 采用后写入值 |

#### 1.5.17 DownloadOperationContextTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                          | 输入                               | 期望输出                       |
| ------ | ------ | ------- | ----------------------------------------------- | ---------------------------------- | ------------------------------ |
| [x]    | [x]    | 阶段 4  | `Create_ShouldNormalizeWhitespaceFilter_ToNull` | `rawFileFilter="   "`              | `FileFilter=null`              |
| [x]    | [x]    | 阶段 4  | `Create_ShouldTrimFilter_WhenValueProvided`     | `rawFileFilter="  +voice;-demo  "` | `FileFilter="+voice;-demo"`    |
| [x]    | [x]    | 阶段 4+ | `Create_ShouldCarryHdAudioOnly_WhenFlagIsTrue`  | `hdAudioOnly=true`                 | `context.HdAudioOnly == true`  |
| [x]    | [x]    | 阶段 4+ | `Create_ShouldDefaultHdAudioOnly_ToFalse`       | 不传第二参数                       | `context.HdAudioOnly == false` |

#### 1.5.18 DownloadTaskSnapshotPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                     | 期望输出                               |
| ------ | ------ | ------ | --------------------------------------------------------------- | ---------------------------------------- | -------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetFailedTasks_ShouldReturnOnlyFailedItems`                    | 含 Pending/Failed/Completed 混合任务快照 | 仅返回 `Status=Failed` 任务集合        |
| [x]    | [x]    | 阶段 4 | `GetActiveSourceIds_ShouldReturnCaseInsensitiveDeduplicatedSet` | 含 sourceId 大小写变体与重复任务快照     | 返回去重且不区分大小写的 sourceId 集合 |

#### 1.5.19 SearchFilterValuePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                          | 输入                            | 期望输出                         |
| ------ | ------ | ------ | ----------------------------------------------- | ------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 3 | `MergeDistinct_ShouldAppendValue_WhenMissing`   | 已有值 `tag1`，新增值 `tag2`    | 合并结果为 `tag1,tag2`           |
| [x]    | [x]    | 阶段 3 | `MergeDistinct_ShouldNotDuplicateExistingValue` | 已有值 `tag1;tag2`，新增 `tag2` | 命中重复值时不追加，保持原值不变 |

#### 1.5.20 DownloadTaskStatusExtensionsTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                                    | 期望输出                                                               |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------------------- | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `GetDisplayName_ReturnsCorrectChineseLabel`（Theory，6 inline cases） | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应中文标签：未下载/待下载/下载中/已完成/已失败/已取消            |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_ReturnsCorrectSortOrder`（Theory，6 inline cases）      | Pending/Queued/Running/Completed/Failed/Canceled 枚举值 | 返回对应排序优先级：3/2/1/0/4/5                                        |
| [x]    | [x]    | 阶段 4 | `GetSortOrder_OrdersStatusesCorrectly`                                | 无序的 6 个枚举值输入                                   | 按排序优先级递增排列：Completed→Running→Queued→Pending→Failed→Canceled |

#### 1.5.21 StartupEndpointWarmupServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                         | 期望输出                                              |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | ---------------------------- | ----------------------------------------------------- |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldReturnImmediately_WhenDiscoveryIsSlow`   | Discover 慢响应              | 启动 warmup 调用快速返回，不阻塞窗口启动路径          |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldInvokeDiscoverAndPersistAsync`           | 正常 discover 依赖           | 后台流程会触发一次 DiscoverAndPersist 调用            |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldReuseInFlightWarmupTask_AndDiscoverOnce` | 两次并发 startup warmup 调用 | 复用同一 warmup 任务，且只执行一次 DiscoverAndPersist |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldNotThrow_WhenDiscoveryFails`             | Discover 抛异常              | 异常被吞吐并记录，不向上抛出                          |
| [x]    | [x]    | 阶段 2 | `StartInBackgroundAsync_ShouldRespectTimeout_AndContinue`              | Discover 超时（50ms）        | 超时后流程结束并继续，不阻塞应用                      |

---

#### 1.5.22 DownloadTaskRowViewModelTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                   | 输入                       | 期望输出                                      |
| ------ | ------ | ------- | ---------------------------------------------------------------------------------------- | -------------------------- | --------------------------------------------- |
| [x]    | [x]    | 阶段 4+ | `From_ShouldSetStatusSortOrder_MatchingGetSortOrder`（全 6 状态 Theory）                 | 各 DownloadTaskStatus 枚举 | `vm.StatusSortOrder == status.GetSortOrder()` |
| [x]    | [x]    | 阶段 4+ | `CreatePending_ShouldSetStatusSortOrder_MatchingGetSortOrder`（Queued/Pending/Canceled） | 各待定状态                 | `vm.StatusSortOrder == status.GetSortOrder()` |

#### 1.5.23 DownloadEnqueueDuplicatePolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                                | 输入                                                 | 期望输出                         |
| ------ | ------ | ------- | ------------------------------------------------------------------------------------- | ---------------------------------------------------- | -------------------------------- |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldExclude_WhenSourceIdExistsWithAnyStatus`（5 状态 Theory） | 已存在各状态任务，入队同 RJID                        | 返回空列表                       |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldInclude_WhenSourceIdNotInTaskList`                        | 已有 RJ001，入队 RJ002/RJ003                         | 返回 [RJ002, RJ003]              |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldIgnoreCase`                                               | 已有小写 rj001，入队 RJ001/RJ002                     | 跳过 RJ001，返回 [RJ002]         |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldReturnAll_WhenNoExistingTasks`                            | 空任务列表，入队 2 项                                | 返回所有 2 项                    |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldReturnEmpty_WhenAllAlreadyExist`                          | 全部已存在                                           | 返回空列表                       |
| [x]    | [x]    | 阶段 4+ | `FilterAlreadyPresent_ShouldDeduplicateIncoming`                                      | 含重复和大小写重复项 [RJ001,RJ002,rj001,RJ003,RJ002] | 返回 [RJ001,RJ002,RJ003]（3 项） |

#### 1.5.24 SearchPagingPolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                             | 输入                                  | 期望输出                        |
| ------ | ------ | ------- | -------------------------------------------------- | ------------------------------------- | ------------------------------- |
| [x]    | [x]    | 阶段 4+ | `SlicePage_ShouldClampPage_AndReturnExpectedItems` | 5 条数据，requestedPage=4，pageSize=2 | 纠正到有效页 3，并返回最后 1 条 |
| [x]    | [x]    | 阶段 4+ | `CanJump_ShouldReturnFalse_WhenOnlySinglePage`     | totalPages=1                          | 返回 false（禁用跳页）          |
| [x]    | [x]    | 阶段 4+ | `CanJump_ShouldReturnTrue_WhenMultiplePages`       | totalPages=2                          | 返回 true（允许跳页）           |

#### 1.5.25 SearchQueueCountPolicyTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                         | 输入                                   | 期望输出                                            |
| ------ | ------ | ------- | -------------------------------------------------------------- | -------------------------------------- | --------------------------------------------------- |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldCountSkippedFromExistingQueuedAndInputDuplicates` | 混合输入重复 + 已在队列 + 已在任务列表 | `ToEnqueue` 与 `SkippedCount` 均与去重/跳过规则一致 |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldSkipQueuedItems_WhenTaskListEmpty`                | 任务列表为空 + 队列已含同批 SourceId   | 返回空队列且跳过数等于输入项数                      |
| [x]    | [x]    | 阶段 4+ | `Build_ShouldReturnEmpty_WhenInputInvalid`                     | 空字符串与空白输入                     | 返回空队列且跳过数为 0                              |

#### 1.5.26 DownloadUnfinishedQueueSnapshotPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                             | 输入                                             | 期望输出                                                            |
| ------ | ------ | ------ | ------------------------------------------------------------------ | ------------------------------------------------ | ------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldIncludePendingQueuedFailedAndQueuedSourceIds` | 活跃任务含多状态 + 队列含重复与空白项            | 仅保留 Pending/Queued/Failed + 队列项，大小写去重后按 SourceId 排序 |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldReturnEmpty_WhenNoActiveAndQueueEmpty`        | 活跃任务空 + 队列空                              | 返回空快照                                                          |
| [x]    | [x]    | 阶段 4 | `BuildSnapshot_ShouldDeduplicateCaseInsensitiveAcrossSources`      | 活跃任务与队列同时包含同一 SourceId 的大小写变体 | 跨来源按大小写不敏感去重，仅保留唯一 SourceId                       |

#### 1.5.27 SearchWorkPageUrlPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                       | 输入                                     | 期望输出                                        |
| ------ | ------ | ------ | ------------------------------------------------------------ | ---------------------------------------- | ----------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldReplacePlaceholder_WithNormalizedSourceId`   | 模板含 `{RJID}` + 作品 URL 形式 sourceId | 生成标准 `https://www.asmr.one/work/RJxxxx` URL |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldAppendSourceId_WhenPlaceholderMissing`       | 模板不含占位符 + `rj1001`                | 自动追加 `/RJ1001`                              |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFallbackToDefaultTemplate_WhenTemplateEmpty` | 空模板 + 纯数字 sourceId                 | 回退默认模板并成功生成 URL                      |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFail_WhenSourceIdInvalid`                    | 空白 sourceId                            | 返回失败并给出可读错误                          |
| [x]    | [x]    | 阶段 3 | `TryBuild_ShouldFail_WhenTemplateInvalid`                    | 非法模板 `not-a-url-{RJID}`              | 返回失败并提示检查 `workPageUrlTemplate`        |

#### 1.5.28 SearchExportScopePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                | 期望输出                                           |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------- | -------------------------------------------------- |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnAllResults_WhenScopeIsAll`                             | 全量结果 + 部分选中，scope=All      | 返回全量结果且不触发回退                           |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnSelectedResults_WhenScopeIsSelectedAndSelectionExists` | 全量结果 + 非空选中，scope=Selected | 返回选中结果且不触发回退                           |
| [x]    | [x]    | 阶段 3 | `Build_ShouldFallbackToAll_WhenScopeIsSelectedAndSelectionEmpty`          | 全量结果 + 空选中，scope=Selected   | 回退到全量结果，`FallbackToAll=true`               |
| [x]    | [x]    | 阶段 3 | `Build_ShouldReturnEmpty_WhenNoResultsAndSelectionEmpty`                  | 全量空 + 选中空，scope=Selected     | 返回空结果，`FallbackToAll=true`（后续由 UI 提示） |

#### 1.5.29 AppVersionInfoTests.cs

| 已创建 | 已通过 | 阶段    | 样例名                                                                            | 输入                     | 期望输出                                    |
| ------ | ------ | ------- | --------------------------------------------------------------------------------- | ------------------------ | ------------------------------------------- |
| [x]    | [x]    | 阶段 5  | `GetDisplayVersion_ShouldReturnThreePartAssemblyVersion`                          | 当前程序集版本 `0.6.4.0` | 返回三段式版本文本 `0.6.4`                  |
| [x]    | [x]    | 阶段 1+ | `BuildSettingsVersionText_AndStartupMessage_ShouldUseDisplayVersionWithoutSuffix` | 动态版本文案构建         | Settings 文案与启动日志共用相同三段式版本号 |

#### 1.5.30 StartupUnfinishedQueueMetadataRefreshServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                        | 输入                                         | 期望输出                                                     |
| ------ | ------ | ------ | ----------------------------------------------------------------------------- | -------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `StartInBackgroundAsync_ShouldReturnImmediately_WhenRefreshIsSlow`            | WorkInfo API 慢响应                          | 启动刷新调用立即返回，不阻塞窗口启动                         |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldReturnZero_WhenQueueEmpty`                                | 未完成队列为空                               | 返回空结果，且不触发 API/缓存写入                            |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldOnlyFetchMissingOrUntitledWorkInfo_AndUpsertFetchedItems` | 队列含已缓存标题、空标题、缺失标题的混合场景 | 仅补拉缺失/空标题项，且只执行一次 Full 级缓存写回            |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldContinue_WhenSingleFetchFails`                            | 混合成功与失败的 WorkInfo 请求               | 单项失败不影响整体流程，成功项写回缓存，失败项返回可展示错误 |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldWaitForWarmupBeforeFetchingWorkInfo`                      | warmup 阻塞 + 单条缺失标题队列               | 在 warmup 完成前不发起 WorkInfo 请求，完成后再补拉并写回缓存 |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldContinue_WhenWarmupFails`                                 | warmup 抛异常 + 单条缺失标题队列             | warmup 失败时仍继续补拉作品信息并写回缓存                    |
| [x]    | [x]    | 阶段 4 | `RefreshAsync_ShouldReturnGlobalFailures_WhenTimeoutOccurs`                   | 单条缺失标题队列 + 后台补拉超时              | 返回失败明细，供 Download 页面标记为 `Failed` 占位行         |

#### 1.5.31 FavoriteFolderSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                        | 输入                                | 期望输出                                               |
| ------ | ------ | ------ | ------------------------------------------------------------- | ----------------------------------- | ------------------------------------------------------ |
| [x]    | [x]    | 阶段 4 | `NormalizeFolderTitle_ShouldTrimWhitespace`                   | 含首尾空白的收藏夹标题              | 返回去首尾空白后的标题                                 |
| [x]    | [x]    | 阶段 4 | `BuildFolderTitles_ShouldDistinctAndSortCaseInsensitive`      | 大小写变体、空白标题与混合顺序标题  | 输出大小写不敏感去重且按稳定顺序排序后的收藏夹标题列表 |
| [x]    | [x]    | 阶段 4 | `CanConfirm_ShouldAllowNewTitle_WhenCustomInputEnabled`       | 允许新建收藏夹 + 新标题             | 允许确认                                               |
| [x]    | [x]    | 阶段 4 | `CanConfirm_ShouldRejectUnknownTitle_WhenCustomInputDisabled` | 仅允许选择现有收藏夹 + 未知标题输入 | 拒绝确认                                               |

#### 1.5.32 FavoriteFolderDialogXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                      | 期望输出                                                    |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ----------------------------------------- | ----------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `FavoriteFolderDialogXaml_ShouldContainEditableComboBoxAndConfirmButtons` | 解析 `FavoriteFolderDialog.xaml` 文本/XML | 弹窗包含可编辑下拉框、保存/取消按钮，且 XAML 可被正确解析。 |

#### 1.5.33 SyncViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                          | 期望输出                                                                                                                                                                                                                                                          |
| ------ | ------ | ------ | ---------------------------------------------------------- | ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `SyncViewXaml_ShouldContainPrimaryActions_AndStatusFields` | 解析 `SyncView.xaml` 文本/XML | 页面包含两枚合并后的同步主按钮、"重试失败项""导出失败记录""导出成功记录""刷新统计"按钮、Banner 文案使用简洁用户面向描述（不含阶段 5 内部迭代文本）、根 Grid 使用 `ShellBackgroundBrush`、主按钮使用 `ActionButtonStyle`、次要按钮使用 `SecondaryActionButtonStyle`、统计卡片使用 `StatsCardBorderStyle` 与 `MutedLabelStyle`、统一状态面板样式、共享 `PageStatePresenterTemplate`、`StatusTextBlock + DownloadStatusTextBlock` 双状态文本框，且旧的独立停止按钮命名和硬编码背景色已移除。 |

#### 1.5.34 SyncCommandAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                    | 期望输出                                                                     |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------- | ---------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldExposeStartTexts_WhenIdle`                                | Sync 页面空闲态                         | 两个同步主按钮均可点击，文案分别为“开始同步元数据”“开始同步下载”             |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldKeepRefreshEnabled_WhenMetadataSyncIsRunning`             | 元数据同步运行中，其余操作空闲          | 元数据主按钮切换为“停止同步元数据”，下载/重试/导出禁用，但“刷新统计”仍可点击 |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldShowStoppingText_WhenMetadataStopAlreadyRequested`        | 元数据同步运行中，且已发起 stop request | 元数据主按钮显示“正在停止元数据...”，按钮禁用，刷新统计仍可点击              |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldShowStoppingText_WhenDownloadStopAlreadyRequested`        | 同步下载运行中，且已发起 stop request   | 下载主按钮显示“正在停止下载...”，按钮禁用，刷新统计仍可点击                  |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldDisableRefreshWhileRefreshIsRunning_ButKeepStopAvailable` | 元数据同步运行中，且刷新统计正在执行    | “刷新统计”仅在自身执行期间禁用，不影响当前同步主按钮保持 stop 语义           |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldKeepRefreshEnabled_WhenRetryFailedIsRunning`              | “重试失败项”执行中                      | 两个同步主按钮、重试与导出禁用，但“刷新统计”仍可点击                         |
| [x]    | [x]    | 阶段 5 | `Evaluate_ShouldDisableRefresh_WhenExportOperationRunning`                | 导出附属操作执行中                      | 两个同步主按钮、重试、导出与刷新统计全部禁用，避免导出与刷新并发冲突         |

#### 1.5.35 SyncActionDebouncePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                        | 期望输出                                                     |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | ------------------------------------------- | ------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldReturnStart_WhenSyncIsIdle`                                 | 同步未运行 + 任意最近开始时间               | 返回 `Start`，允许开始新的同步                               |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnoreMetadataDoubleClick_WhenSecondClickIsWithinOneSecond` | 元数据同步运行中 + 最近开始时间距今 `< 1s`  | 返回 `Ignore`，第二次快速点击不会误发 stop request           |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldRequestMetadataStop_WhenSecondClickIsAfterOneSecond`        | 元数据同步运行中 + 最近开始时间距今 `>= 1s` | 返回 `RequestStop`，超过防抖窗口后可正常进入停止流程         |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnoreDownloadDoubleClick_WhenSecondClickIsWithinOneSecond` | 同步下载运行中 + 最近开始时间距今 `< 1s`    | 返回 `Ignore`，同步下载主按钮具备与元数据相同的 1 秒防抖策略 |
| [x]    | [x]    | 阶段 5 | `Decide_ShouldIgnore_WhenStopAlreadyRequested`                            | 同步运行中且已进入 stopping 态              | 返回 `Ignore`，后续重复点击不会覆盖既有停止请求              |

#### 1.5.36 DownloadToolbarAvailabilityTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                               | 输入                      | 期望输出                                                         |
| ------ | ------ | ------ | -------------------------------------------------------------------- | ------------------------- | ---------------------------------------------------------------- |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldEnableToolbarActions_WhenNoQueueMutationIsRunning`   | Download 顶部按钮空闲态   | 入队入口、执行队列、清空任务列表与打开下载目录均可点击           |
| [x]    | [x]    | 阶段 4 | `Evaluate_ShouldKeepOpenDirectoryEnabled_WhenQueueMutationIsRunning` | “立即下载/执行队列”运行态 | 入队入口、执行队列与清空任务列表禁用，但“打开下载目录”保持可点击 |

#### 1.5.37 SyncStatusTextBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                | 输入                                | 期望输出                                      |
| ------ | ------ | ------ | ----------------------------------------------------- | ----------------------------------- | --------------------------------------------- |
| [x]    | [x]    | 阶段 5 | `BuildMetadataStatus_ShouldDescribeRunningProgress`   | 元数据同步进行中                    | 返回带“同步状态：”前缀的分页/累计处理状态文本 |
| [x]    | [x]    | 阶段 5 | `BuildMetadataStatus_ShouldDescribeStoppedProgress`   | 元数据同步 `STOPPED` 断点继续态     | 返回带“同步状态：”前缀的断点继续提示          |
| [x]    | [x]    | 阶段 5 | `BuildDownloadStatus_ShouldDescribeStoppingProgress`  | 同步下载进行中且已发起 stop request | 返回带“下载状态：”前缀的 stopping 提示        |
| [x]    | [x]    | 阶段 5 | `BuildDownloadStatus_ShouldDescribeCompletedProgress` | 同步下载 `COMPLETED` 完成态         | 返回带“下载状态：”前缀的完成摘要              |

#### 1.5.38 SyncProgressDetailsBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                    | 输入                                          | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 5 | `BuildRetryPendingDetails_ShouldDescribeRetryInFlight`                    | “重试失败项”刚进入执行态                      | 返回“正在重试失败同步下载，完成后这里会显示本次重试结果。”占位详情文本   |
| [x]    | [x]    | 阶段 5 | `BuildPersistedProgressDetails_ShouldDescribeMetadataAndDownloadProgress` | 已持久化的元数据进度 + 已持久化的同步下载进度 | 返回包含元数据与同步下载两组累计状态、容量与最近更新时间的详情区文本     |
| [x]    | [x]    | 阶段 5 | `BuildRetryRefreshDetails_ShouldDescribeCurrentRetrySnapshot`             | 重试运行中的报表快照                          | 返回当前重试态摘要，保留下载状态上下文，不再回退为旧的持久化下载进度详情 |

#### 1.5.39 LibraryViewXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                 | 输入                             | 期望输出                                                                                                                                                                                          |
| ------ | ------ | ------ | ---------------------------------------------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldContainCoreLibraryControls_AndStatusBlocks`     | 解析 `LibraryView.xaml` 文本/XML | `Library` 页包含当前副标题文案、关键筛选控件、上一页/下一页/跳页/page size 控件、独立“作品详情”区、文件树、系统打开区、状态区、共享 `PageStatePresenterTemplate` 与播放主按钮，且 XAML 可被解析。 |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldContainExpectedGridColumns_AndFileTreeTemplate` | 解析 `LibraryView.xaml` 文本/XML | 作品表格包含本地化列头 `作品ID/标题/日期/字幕/音频/文件`，且文件树使用 `LibraryFileItem` 层级模板。                                                                                               |
| [x]    | [x]    | 阶段 6 | `LibraryViewXaml_ShouldUseScrollableLayout_ForPagingAndRightPane`      | 解析 `LibraryView.xaml` 文本/XML | 右侧详情/文件树/上下文区采用受限高度与内部滚动布局，`FileTreeView` 不再固定 `220` 高度，且跳页/page size 事件绑定存在。                                                                           |

#### 1.5.40 LibraryPlaybackSelectionPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                      | 输入                                              | 期望输出                                                                             |
| ------ | ------ | ------ | --------------------------------------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `ShouldReloadContext_ShouldReturnTrue_WhenSelectedFileHasDifferentFullPath` | 同一 `SourceId` 与相同 `RelativePath`、不同根目录 | 当显式选中文件的 `FullPath` 与当前上下文不同，即使相对路径相同也必须重新载入上下文。 |

#### 1.5.41 ShellMediaLauncherTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                            | 输入                                       | 期望输出                                                                       |
| ------ | ------ | ------ | ----------------------------------------------------------------- | ------------------------------------------ | ------------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `Open_ShouldTreatNullProcessAsSuccess_WhenShellStartDoesNotThrow` | Shell 启动器底层调用返回 `null` 且未抛异常 | 仍视为成功打开，不抛出异常，并保持 `UseShellExecute=true` 与目标文件路径不变。 |
| [x]    | [x]    | 阶段 6 | `Open_ShouldRethrow_WhenShellStartThrows`                         | Shell 启动器底层调用直接抛出异常           | `Open()` 继续抛出原始异常，供上层 `PlayerService` 转换成失败提示。             |

#### 1.5.42 LibrarySelectionFeedbackPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                              | 输入                             | 期望输出                                                                 |
| ------ | ------ | ------ | ------------------------------------------------------------------- | -------------------------------- | ------------------------------------------------------------------------ |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldGuide_WhenWorkIsSelectedWithoutTreeItem`            | 仅选中作品、尚未选中文件树节点   | 返回作品级引导，提示当前作品包含可播放媒体文件数量与首个候选路径。       |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldExplainSupportedFormats_WhenWorkHasNoPlayableFiles` | 仅选中一个无可播放文件的作品     | 返回“未发现可播放媒体文件”提示，并附带共享支持格式列表。                 |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenDirectoryIsSelected`                       | 选中文件树中的目录节点           | 返回“当前选择是目录”提示，并在存在候选时给出建议选择的媒体文件。         |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenNonPlayableFileIsSelected`                 | 选中文件树中的不可播放文件       | 返回“当前选择的文件不可播放”提示，并在存在候选时给出建议选择的媒体文件。 |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldWarn_WhenPlayableFileDoesNotExist`                  | 选中一个路径缺失的可播放媒体文件 | 返回“当前选择的媒体文件不存在”提示，并在存在其它候选时给出可改选文件。   |
| [x]    | [x]    | 阶段 6 | `Evaluate_ShouldEnableActions_WhenPlayableFileExists`               | 选中一个实际存在的可播放媒体文件 | 返回“已选择可播放媒体文件”提示，并允许载入上下文与播放。                 |

#### 1.5.43 LibraryPlaybackContextTextBuilderTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                           | 输入                           | 期望输出                                                                               |
| ------ | ------ | ------ | ---------------------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 6 | `Build_ShouldProvideDefaultTexts_WhenNothingIsSelectedOrLoaded`  | 无当前选择、无已载入上下文     | “当前选择”区显示显式选择引导，“当前已载入上下文”区显示未载入状态与默认说明。           |
| [x]    | [x]    | 阶段 6 | `Build_ShouldKeepSelectionFeedbackSeparated_FromLoadedContext`   | 有作品级选择提示，尚未载入文件 | “当前选择”区保留作品级候选提示，“当前已载入上下文”区仍显示未载入状态，不与选择区混写。 |
| [x]    | [x]    | 阶段 6 | `Build_ShouldIncludeLoadedWorkAndFileDetails_WhenContextIsReady` | 已载入一个可播放媒体文件       | “当前已载入上下文”区显示状态、作品、文件与说明，“当前选择”区保持当前选择反馈。         |

#### 1.5.44 AppXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                        | 输入                     | 期望输出                                                                                                               |
| ------ | ------ | ------ | --------------------------------------------- | ------------------------ | ---------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `AppXaml_ShouldMergeShellResourcesDictionary` | 解析 `App.xaml` 文本/XML | 应用资源字典已合并 `ShellResources.xaml`，并包含 `PageStatePresenterTemplate` 与“正在处理”共享文案，且 XAML 可被解析。 |

#### 1.5.45 ShellResourcesXamlTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                    | 输入                                | 期望输出                                                                                                                         |
| ------ | ------ | ------ | --------------------------------------------------------- | ----------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ShellResourcesXaml_ShouldContainSharedShellResourceKeys` | 解析 `ShellResources.xaml` 文本/XML | 共享壳层背景、卡片、标题、输入框、按钮基样式以及 `PageStateBusyPanelStyle` / `PageStateEmptyPanelStyle` 存在，且 XAML 可被解析。 |

#### 1.5.46 ShellViewModelTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                      | 输入                  | 期望输出                                                               |
| ------ | ------ | ------ | ----------------------------------------------------------- | --------------------- | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `DefaultState_ShouldStartOnSettingsWithInitializingMessage` | 新建 `ShellViewModel` | 默认选中 `Settings`，初始化消息为“初始化中...”，且主功能页签默认禁用。 |
| [x]    | [x]    | 阶段 7 | `SelectedPageIndex_ShouldClampToValidRange`                 | 写入越界页签索引      | 页签索引会被约束到合法范围，并同步回正确的 `ShellPage`。               |

#### 1.5.47 NavigationServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                | 输入                                       | 期望输出                                                               |
| ------ | ------ | ------ | --------------------------------------------------------------------- | ------------------------------------------ | ---------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `NavigateTo_ShouldUpdateCurrentPageAndSelectedIndex`                  | 通过导航服务切换到 `Library`               | `CurrentPage` 与 `SelectedPageIndex` 同步到目标页签。                  |
| [x]    | [x]    | 阶段 7 | `SetPrimaryPagesEnabled_ShouldToggleContentTabsAndFallbackToSettings` | 先启用主内容页签，再禁用并停留在内容页签上 | Search/Download/Library/Sync 会统一禁用，且当前页会回退到 `Settings`。 |

#### 1.5.48 UiMessageServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                       | 输入                         | 期望输出                                                 |
| ------ | ------ | ------ | -------------------------------------------- | ---------------------------- | -------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ShowError_ShouldUpdateShellStatusMessage`   | 通过消息服务写入错误状态文本 | 当前壳层消息与 `ShellViewModel.StatusMessage` 同步更新。 |
| [x]    | [x]    | 阶段 7 | `ShowInfo_ShouldUpdateShellStatusMessage`    | 通过消息服务写入信息状态文本 | 当前壳层消息与 `ShellViewModel.StatusMessage` 同步更新。 |
| [x]    | [x]    | 阶段 7 | `ShowWarning_ShouldUpdateShellStatusMessage` | 通过消息服务写入警告状态文本 | 当前壳层消息与 `ShellViewModel.StatusMessage` 同步更新。 |

#### 1.5.49 DialogFileNamePolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                     | 输入                               | 期望输出                                                    |
| ------ | ------ | ------ | ---------------------------------------------------------- | ---------------------------------- | ----------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ResolveExtension_ShouldRespectFileNameAndFilterSelection` | 文件名已有/缺失后缀 + 保存筛选索引 | 能按现有文件名或当前筛选索引决策正确后缀。                  |
| [x]    | [x]    | 阶段 7 | `EnsureExtension_ShouldAppendOrReplaceExtension`           | 目标路径缺失/带错后缀              | 会自动补齐或替换成正确后缀，统一 Search/Sync 导出路径行为。 |

#### 1.5.50 PageLoadStateServiceTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                          | 输入                                                           | 期望输出                                                                              |
| ------ | ------ | ------ | --------------------------------------------------------------- | -------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `Create_ShouldReturnStateForRequestedPage`                      | 通过 `PageLoadStateService` 创建 `Search` 状态                 | 返回对应页面名的初始状态对象，且默认不处于 busy/empty。                               |
| [x]    | [x]    | 阶段 7 | `ShowBusy_AndHideBusy_ShouldKeepBusyUntilAllOperationsComplete` | `Download` 页面状态先后执行两次 `ShowBusy` 再分两次 `HideBusy` | busy 标记会持续到全部操作完成；中途释放时仍保持忙碌状态并回退到页面级通用 busy 文案。 |
| [x]    | [x]    | 阶段 7 | `ShowEmpty_AndClearEmpty_ShouldToggleEmptyState`                | `Library` 页面状态执行 `ShowEmpty/ClearEmpty`                  | empty 标记、标题与说明正确切换，并在清空后恢复为空字符串。                            |

#### 1.5.51 ShellStatusRelayPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                  | 输入                                           | 期望输出                                                 |
| ------ | ------ | ------ | ----------------------------------------------------------------------- | ---------------------------------------------- | -------------------------------------------------------- |
| [x]    | [x]    | 阶段 7 | `ShouldPublish_ShouldReturnFalse_WhenViewPolicyDisallows`               | 页面转发策略拒绝发布                           | 不向壳层状态栏发布消息。                                 |
| [x]    | [x]    | 阶段 7 | `ShouldPublish_ShouldReturnFalse_WhenMessageIsEmpty`                    | `StatusTextBlock` 文案为空白                   | 空白消息不会覆盖壳层状态。                               |
| [x]    | [x]    | 阶段 7 | `ShouldPublish_ShouldReturnFalse_WhenCurrentShellMessageIsInitializing` | 当前壳层消息为"初始化中..."，且页面处于可见态  | 初始化占位文案期间不发布页面消息，避免初始化阶段误覆盖。 |
| [x]    | [x]    | 阶段 7 | `ShouldPublish_ShouldReturnTrue_WhenMessageIsValidAndCanPublish`        | 页面可发布且消息非空，当前壳层消息非初始化占位 | 有效页面消息可同步到壳层状态栏。                         |

#### 1.5.52 SyncShellStatusRelayPolicyTests.cs

| 已创建 | 已通过 | 阶段   | 样例名                                                                        | 输入                                     | 期望输出                                   |
| ------ | ------ | ------ | ----------------------------------------------------------------------------- | ---------------------------------------- | ------------------------------------------ |
| [x]    | [x]    | 阶段 7 | `ShouldPublishMetadata_ShouldReturnFalse_WhenViewIsHidden`                    | Sync 页不可见时触发元数据状态更新        | 不发布元数据状态到壳层状态栏。             |
| [x]    | [x]    | 阶段 7 | `ShouldPublishMetadata_ShouldReturnFalse_WhenDownloadFlowOwnsStatus`          | 下载同步流程运行中同时出现元数据状态更新 | 元数据状态被抑制，避免覆盖下载态壳层消息。 |
| [x]    | [x]    | 阶段 7 | `ShouldPublishMetadata_ShouldReturnTrue_WhenViewVisibleAndNoDownloadConflict` | Sync 页可见且无下载流程冲突              | 元数据状态可发布到壳层状态栏。             |
| [x]    | [x]    | 阶段 7 | `ShouldPublishDownload_ShouldReturnTrue_WhenDownloadRunning`                  | 下载同步流程运行中                       | 下载状态可发布到壳层状态栏。               |
| [x]    | [x]    | 阶段 7 | `ShouldPublishDownload_ShouldReturnFalse_WhenViewHidden`                      | Sync 页不可见但下载流程运行中            | 隐藏页状态不会覆盖壳层状态栏。             |

## 2. 测试覆盖分析

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
- 阶段 7 壳层收口：主窗口页签绑定、共享导航/消息服务、统一文件对话框后缀策略、共享样式资源基线、共享 busy 面板与 Search/Download/Library/Sync 空态 page-state 模板，以及壳层消息桥接/Sync 转发门禁策略单测，✅ 当前自动化基线已覆盖；UI 冒烟，⬜ 待手工验证。
- 阶段 6 资源库与系统默认程序打开：目录名双格式解析、扫描去重、关键字/字幕/音频过滤、显式选中文件后的系统打开、刷新后跨根目录同相对路径文件的重载判定、Shell `null` 返回成功判定、失败提示、Library 打开控件与播放上下文分离展示，✅ 已覆盖。
- 翻译作品优先入队：当前语言识别、关联版本优先级选择、UI 勾选持久化与 Search/Download 四类入口回归，✅ 已覆盖。
- `source_id/workId` 双键兼容：BJ 等非 `RJ` 作品的详情/轨道解析、Search WorkId 透传与作品页链接生成，✅ 已覆盖。
- 启动补拉失败可视化：补拉超时/单项失败后在 Download 列表中显示 `Failed` 占位与错误信息，✅ 已覆盖。
- 限流可观测性：`RateLimiter` 步骤间节流间隔，⬜ 待落地。

## 3. 关键实现修复（作为测试补齐的附带产物）

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
- `PageLoadStateService` / `ShellStatusTextSynchronizer` / `App.xaml` / `ShellResources.xaml`：新增共享 `PageStatePresenterTemplate`、page-state 服务与状态文本桥接，把 Search/Download/Library/Settings/Sync 的 busy 态统一到页面级共享面板，为 Search/Download/Library/Sync 落地统一空态面板，并将页内 `StatusTextBlock` 的有效消息同步到壳层状态栏。
- `DialogService` / `DialogFileNamePolicy`：统一 Search 导出、Download 从文件导入与 Sync 导出保存对话框流程，并集中处理 CSV/JSON 默认后缀决策。
- `ShellResources.xaml`：抽取 Search/Download/Library/Settings 共享卡片、输入框、按钮与背景基样式，减少页面内重复视觉定义。

---
