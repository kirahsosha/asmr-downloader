# asmr-downloader 迁移为 WPF Windows 客户端需求分析

## 1. 文档目标

本文档用于指导后续基于 AI 的代码迁移工作，目标是将当前以 Go 命令行程序为核心、附带本地 Web 播放界面的项目，迁移为一个面向 Windows 的 WPF 桌面客户端程序。

本文档包含以下内容：

- 现有项目目录结构分析
- 关键代码文件与模块职责分析
- 迁移目标架构设计
- 详细迁移方案与实施步骤
- 推荐的项目拆分方式
- 风险、边界、验收标准

约束说明：

- 本阶段只产出分析文档，不修改任何现有业务代码
- 后续 AI 迁移应以本文档为需求基线执行

---

## 2. 当前项目总体结论

### 2.1 当前项目本质

当前仓库本质上是一个以 Go 实现的多功能桌面侧下载工具，现阶段的交互方式并不是传统 GUI，而是以下两种组合：

1. 命令行界面（CLI）
2. 本地启动 Gin Web 服务后，在浏览器中打开播放页面

因此，现有项目已经具备完整的业务能力，但缺少真正的原生桌面客户端表现层。迁移到 WPF 的核心，不是简单“加一个壳”，而是把现有 CLI 和 WebUI 的交互逻辑改造成 Windows 桌面原生交互模型。

### 2.2 当前系统已具备的业务能力

现有 Go 项目已经实现了后续 WPF 客户端所需的大部分核心能力：

- 配置初始化与读取
- asmr.one 站点登录认证
- 搜索作品
- 按 RJID 下载单个或批量作品
- 热门作品下载
- 元数据同步到本地 SQLite
- 同步下载、失败重试、结果导出、统计报表
- 本地目录扫描与播放列表展示

### 2.3 当前系统缺少的能力

现有项目缺少以下桌面客户端必需能力：

- 原生 Windows 图形界面
- 统一的页面导航与状态管理
- 可视化任务进度列表
- 下载、同步、播放、配置的可视化操作流
- 更清晰的后台任务与 UI 线程分离机制

### 2.4 对迁移策略的总体判断

推荐采用“业务逻辑重构迁移 + WPF 前端重建”的方案，而不是“WPF 调用现有 Go CLI 可执行文件”的壳式封装方案。

原因如下：

- 当前项目的核心逻辑已经按模块分层，适合迁移为 .NET 服务层
- 如果只在 WPF 中调用 Go CLI，后续任务状态、日志、取消、进度绑定会很难做细
- 本项目目标是 Windows 原生客户端，不是给现有 CLI 补一个启动器
- 后续可维护性、可测试性、MVVM 绑定能力都要求业务逻辑进入 .NET 进程内

因此，推荐的最终形态应为：

- WPF 负责表现层与用户交互
- .NET 类库负责配置、API 调用、下载、同步、数据库、播放数据装配
- Go 代码仅作为迁移参考，不作为长期运行依赖

---

## 3. 现有目录结构分析

## 3.1 仓库顶层目录结论

| 路径        | 结论             | 说明                                         |
| ----------- | ---------------- | -------------------------------------------- |
| `main.go`   | Go 应用入口      | 初始化目录、日志、命令注册、执行根命令       |
| `cmd/`      | CLI 表现层       | 封装 config/search/download/sync/listen 命令 |
| `internal/` | 核心业务层       | 配置、模型、数据库、下载引擎、日志、工具函数 |
| `webui/`    | 浏览器端播放界面 | 内嵌静态页面、播放器资源、Gin 静态服务入口   |
| `docs/`     | 迁移文档目录     | 存放需求分析与迁移设计文档                   |
| `api.http`  | 接口调试文件     | 可作为后续 .NET API 对接时的参考             |
| `README.md` | 用户说明文档     | 说明 CLI 使用方式和现有 WebUI 玩法           |
| `build.sh`  | 构建脚本         | 偏 Go 工程构建，不是 WPF 方案核心            |

## 3.2 当前实际可执行层级

当前系统的执行层级为：

1. `main.go` 启动程序
2. `cmd/root.go` 初始化配置、数据库、同步目录
3. 各子命令调用 `internal/engine`、`internal/database`、`internal/model` 等模块
4. `listen` 命令额外启动 `Gin + 内嵌 WebUI` 提供浏览器端播放界面

因此，当前项目的 UI 不是独立桌面应用，而是：

- 业务核心在 Go 进程内
- 图形交互只存在于浏览器页面中

---

## 4. 现有代码文件功能分析

## 4.1 入口与命令层

### `main.go`

职责：

- 确保元数据目录存在
- 初始化日志系统
- 注册 `version` 命令
- 调用命令根入口

结论：

- 这是典型 CLI 入口
- 迁移到 WPF 后，其职责将由 `App.xaml`、主窗口启动逻辑和依赖注入容器替代

### `cmd/root.go`

职责：

- 定义根命令 `asmroner`
- 在 `PersistentPreRun` 中统一执行初始化
- 读取配置文件
- 反序列化配置到 `model.AppConfig`
- 初始化 SQLite 数据库
- 确保同步数据目录存在

结论：

- 这里实际上承担了“应用启动初始化管线”的职责
- 迁移后应拆分为：
  - `ConfigurationService`
  - `DatabaseInitializer`
  - `ApplicationBootstrapper`

### `cmd/config.go`

职责：

- 交互式初始化配置文件
- 通过命令行逐项询问账号、代理、并发、限流、目录等配置
- 写入 TOML 配置

迁移判断：

- 该文件的核心业务不是“命令行输入”，而是“配置项采集和配置持久化”
- WPF 中应迁移为“设置页面”或“首次启动向导”

### `cmd/search.go`

职责：

- 解析搜索词与高级查询语法
- 调用搜索接口
- 把结果输出为终端表格
- 提供搜索后下载、搜索后导出两个子命令

迁移判断：

- 搜索能力保留
- 表格输出逻辑废弃，替换为 WPF DataGrid 或列表视图绑定
- `search download`、`search export` 迁移为搜索结果页上的按钮操作

### `cmd/download.go`

职责：

- 支持单个 RJID、批量 RJID、`hot100` 热门下载
- 创建下载目录
- 初始化下载引擎
- 调用下载流程

迁移判断：

- 是 WPF “下载页 / 批量任务页” 的直接业务来源
- CLI 参数解析部分废弃
- 下载参数改由 UI 表单输入

### `cmd/sync.go`

职责：

- 同步元数据
- 同步并下载
- 重试失败任务
- 导出下载状态
- 打印同步统计信息

迁移判断：

- 是 WPF “同步管理页” 的核心需求来源
- `report` 子命令应迁移为统计卡片和数据面板
- `export` 子命令应迁移为导出按钮和文件保存对话框

### `cmd/listen.go`

职责：

- 启动 Gin Web 服务器
- 提供 `/api/list` 浏览接口
- 扫描下载目录并存入内存数据库
- 启动浏览器打开本地页面
- 静态提供播放资源和前端页面

迁移判断：

- 这是现有“播放器/作品浏览器”的临时 UI 方案
- 迁移为 WPF 后，不再需要 Gin、本地 HTTP 服务、自动打开浏览器
- 应改造成桌面端本地媒体库页面

## 4.2 核心业务层

### `internal/engine/engine.go`

职责：

- 创建下载引擎管理器
- 配置 Resty 客户端、代理、重试、传输策略
- 自动登录 asmr.one 获取 JWT
- 获取作品信息与音轨列表
- 下载单个作品 / 批量作品 / 热门作品
- 按偏好过滤音频格式
- 同步站点元数据到 SQLite
- 搜索接口分页聚合
- 下载文件并执行重试逻辑

结论：

- 这是整个项目最核心的业务模块
- 后续迁移时应优先拆解为 .NET 服务层，而不是把逻辑继续放在 UI 层

建议拆分为以下 .NET 服务：

- `AsmrApiClient`
- `AuthService`
- `SearchService`
- `DownloadService`
- `SyncService`
- `HotListService`
- `TaskSchedulerService`

### `internal/engine/checker.go`

职责：

- 获取可用 asmr.one 域名
- 检测最快响应站点
- 生成 API 基地址

迁移判断：

- 应迁移为独立的 `EndpointDiscoveryService`
- 启动阶段可执行，也可在设置页手动触发

### `internal/engine/limiter.go`

职责：

- 通过令牌桶 + 随机抖动实现限流

迁移判断：

- 是下载与同步的基础能力
- .NET 中可用 `RateLimiter`、`SemaphoreSlim`、`Channel`、`Task.Delay` 实现等效能力

### `internal/database/db.go`

职责：

- 初始化 SQLite 数据库
- 自动迁移表结构
- 提供内存数据库给 listen 模块临时使用

迁移判断：

- 应迁移为 EF Core SQLite 的初始化入口
- `NewInMemoryDb` 仅服务于现有 WebUI，本身不是长期保留能力

### `internal/model/config.go`

职责：

- 定义配置结构
- 使用 Viper 加载配置

迁移判断：

- 直接映射为 .NET 配置模型
- 推荐保留字段语义，减少业务迁移成本

### `internal/model/model.go`

职责：

- 定义 `MetadataWork` 与 `WorkSyncInfo`
- 描述站点元数据与同步下载状态

迁移判断：

- 这是 EF Core 实体模型的主要来源
- 后续数据库结构建议尽量保持兼容

### `internal/model/search.go`

职责：

- 定义搜索接口响应和搜索展示模型

迁移判断：

- 直接转为 DTO 与 ViewModel 输入结构

### `internal/model/workinfo.go`

职责：

- 定义作品详情接口模型

迁移判断：

- 转为作品详情 DTO
- 可支撑详情页、下载预览、播放页头部信息

### `internal/model/track.go`

职责：

- 定义音轨目录树结构

迁移判断：

- 是构建本地文件树和下载任务树的重要模型

### `internal/model/queryparam.go`

职责：

- 解析高级搜索字符串
- 生成 asmr.one 查询参数

迁移判断：

- 是搜索页的重要业务逻辑
- 迁移时建议保留“高级搜索语法兼容”能力
- 同时可以在 WPF 上再提供图形化查询构造器

### `internal/utils/utils.go`

职责：

- 文件大小转换
- 路径正规化
- 导出 CSV/JSON
- 目录大小统计
- 随机 User-Agent
- 空目录清理

迁移判断：

- 迁移为若干通用 helper 和基础服务
- 不建议继续保留为一个巨大工具类

### `internal/consts/consts.go`

职责：

- 定义目录名、数据库名、默认 API 地址、接口路径、ID 正则等常量

迁移判断：

- 可保留为 .NET 常量类或选项配置
- 接口路径建议集中到 `ApiEndpoints` 或 `AsmrApiOptions`

### `internal/logger/log.go`

职责：

- 为命令行输出封装日志、任务、进度、失败记录

迁移判断：

- 迁移为 .NET 日志基础设施，同时增加 UI 可绑定日志流
- 推荐使用 `ILogger` + 文件日志 + UI 任务日志缓冲

## 4.3 WebUI 层

### `webui/webui.go`

职责：

- 嵌入静态资源
- 提供静态文件系统
- 读取首页资源内容

结论：

- 该模块只服务于浏览器端播放模式
- 在纯 WPF 目标下，不应继续作为主要交互层保留

### `webui/public/`

职责：

- 提供 HTML、Tailwind、Plyr 播放器静态资源

迁移判断：

- 可作为过渡期 UI 参考
- 不建议作为最终桌面端 UI 主体

---

## 5. 当前系统的数据流与交互流分析

## 5.1 启动与初始化流

1. 程序启动
2. 创建 `.asmroner-data`
3. 加载 `config.toml`
4. 初始化 SQLite：`.asmroner-data/asmroner.db`
5. 确保同步目录存在
6. 根据命令执行搜索、下载、同步或本地 Web 播放

迁移启示：

- WPF 启动也必须保留这套初始化流程
- 只是触发方式从 `PersistentPreRun` 改为应用启动引导

## 5.2 搜索流

1. 用户输入 RJID 或高级查询语句
2. `QueryParams` 解析输入
3. 拼接 asmr.one 搜索 URL
4. 拉取一页或多页结果
5. 终端表格展示 / 导出 / 批量下载

迁移启示：

- WPF 搜索页需要支持：
  - 简单搜索
  - 高级搜索
  - 结果分页
  - 结果导出
  - 勾选后下载

## 5.3 下载流

1. 校验 RJID
2. 获取作品信息
3. 获取音轨目录树
4. 按规则生成本地目录名
5. 创建目录结构
6. 过滤优先音频格式
7. 并发下载文件
8. 重试失败请求
9. 清理空目录

迁移启示：

- WPF 下载页需要可视化呈现：
  - 队列状态
  - 当前作品
  - 当前文件
  - 已完成数量
  - 错误信息
  - 取消 / 重试能力

## 5.4 同步流

1. 查询站点元数据总量
2. 对比本地数据库数量
3. 确认是否需要同步
4. 分页抓取元数据并写入 SQLite
5. 可选继续进行同步下载
6. 记录下载状态和统计信息

迁移启示：

- 同步页应区分两个动作：
  - 元数据同步
  - 同步后批量下载

## 5.5 播放流

1. 扫描本地同步目录
2. 按目录命名规则推断元信息
3. 构建内存数据库
4. Gin 输出文件列表接口
5. 浏览器页面发起请求并播放文件

迁移启示：

- WPF 中不需要 HTTP 服务器
- 应直接扫描本地目录并构建 ViewModel
- 媒体播放应改用 WPF 原生或 .NET 媒体组件

## 5.6 原有 Go API 清单（URL / Request / Response）

本小节基于现有 Go 实现实际调用路径整理，覆盖上游 asmr.one 接口与本地 Gin 接口。

### 5.6.1 上游 asmr.one 业务接口

说明：

- API 基地址优先取配置 `downloader.api_url`，为空时默认 `https://api.asmr-300.com`。
- 除登录接口外，其余接口均通过 `Authorization: Bearer <token>` 访问。

| 接口名         | Method | URL 模板                                                                                                                                          | Request（主要字段）                                                                                           | Response（主要结构）                                                                     |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| 登录获取 Token | POST   | `{ApiUrl}/api/auth/me`                                                                                                                            | Body(JSON): `{ "name": string, "password": string }`                                                          | JSON 对象，核心字段：`token: string`（代码中以 `map[string]interface{}` 读取）           |
| 作品详情       | GET    | `{ApiUrl}/api/work/{id}`                                                                                                                          | Path: `id`（示例：`RJ123456`）；Header: `Authorization`                                                       | `WorkInfo`：如 `id/title/release/has_subtitle/source_id/circle/tags/vas/mainCoverUrl...` |
| 音轨树         | GET    | `{ApiUrl}/api/tracks/{id}`                                                                                                                        | Path: `id`；Header: `Authorization`                                                                           | `[]Track`：如 `type/title/children/mediaStreamUrl/mediaDownloadUrl`                      |
| 搜索           | GET    | `{ApiUrl}/api/search/{encodedQuery}?order={order}&sort={sort}&page={page}&pageSize={pageSize}&subtitle={subtitle}&includeTranslationWorks={bool}` | Path: `encodedQuery`（由 `QueryParams` 构建）；Query 参数见 URL                                               | `SearchResult`：`works[] + pagination{currentPage,pageSize,totalCount}`                  |
| 元数据分页同步 | GET    | `{ApiUrl}/api/works?order=release&sort=desc&page={page}&pageSize={pageSize}[&subtitle=1]`                                                         | Query: `order/sort/page/pageSize/subtitle`；Header: `Authorization`                                           | `MetadataWorkResponse`：`works[] + pagination{currentPage,pageSize,totalCount}`          |
| 热门作品列表   | POST   | `{ApiUrl}/api/recommender/popular`                                                                                                                | Body(JSON): `{ keyword:"", page:1, pageSize:100, subtitle:0, localSubtitledWorks:[], withPlaylistStatus:[] }` | `MetadataWorkResponse`（随后提取 `works[].source_id` 进入下载）                          |

补充：下载文件阶段还会直接请求音轨中的 `mediaDownloadUrl`（文件直链，非固定 API 路径），以流式方式落盘，不走统一 JSON 响应。

实测说明（2026-03-16）：

- 以下 sample request/response 均来自本地实际调用 `https://api.asmr-300.com`。
- 鉴权 token 为安全考虑已做脱敏，仅保留前缀示例。

#### 5.6.1.1 登录获取 Token（POST `/api/auth/me`）

Sample Request:

```http
POST https://api.asmr-300.com/api/auth/me
Content-Type: application/json
Origin: https://asmr.one
Referer: https://asmr.one/

{
  "name": "guest",
  "password": "guest"
}
```

Sample Response（节选）:

```json
{
  "tokenPrefix": "eyJhbGciOiJIUzI1NiIsInR5",
  "tokenLength": 259
}
```

#### 5.6.1.2 作品详情（GET `/api/work/{id}`）

Sample Request:

```http
GET https://api.asmr-300.com/api/work/01426915
Authorization: Bearer <token>
```

Sample Response（节选）:

```json
{
  "id": 1426915,
  "title": "【無声音囁き特化♡全編ハメっぱなし】あま媚び囁きご奉仕猫メイド♪異世界ハメっぱなしスローセックス添い寝ライフ♡【✅豪華12大特典✅】",
  "source_id": "RJ01426915",
  "has_subtitle": false,
  "release": "2025-10-22",
  "mainCoverUrl": "https://api.asmr-300.com/api/cover/1426915.jpg?type=main"
}
```

#### 5.6.1.3 音轨树（GET `/api/tracks/{id}`）

Sample Request:

```http
GET https://api.asmr-300.com/api/tracks/01426915
Authorization: Bearer <token>
```

Sample Response（节选）:

```json
{
  "count": 5,
  "first": {
    "type": "folder",
    "title": "05_特典",
    "mediaStreamUrl": null,
    "mediaDownloadUrl": null
  }
}
```

#### 5.6.1.4 搜索（GET `/api/search/{encodedQuery}`）

Sample Request:

```http
GET https://api.asmr-300.com/api/search/%20nurse%20?order=dl_count&sort=desc&page=1&pageSize=3&subtitle=0&includeTranslationWorks=true
Authorization: Bearer <token>
```

Sample Response（节选）:

```json
{
  "pagination": {
    "totalCount": 574,
    "pageSize": 3
  },
  "works": [
    {
      "id": 241335,
      "title": "癒し系専属ナースのケアを体験できちゃうバイノーラル",
      "source_id": "RJ241335",
      "dl_count": 184684
    }
  ]
}
```

#### 5.6.1.5 元数据分页同步（GET `/api/works`）

Sample Request:

```http
GET https://api.asmr-300.com/api/works?order=release&sort=desc&page=1&pageSize=3
Authorization: Bearer <token>
```

Sample Response（节选）:

```json
{
  "pagination": {
    "totalCount": 59889,
    "pageSize": 3
  },
  "works": [
    {
      "id": 1551159,
      "title": "【4時間超】マイク直接耳舐め特化 耳舐め堕ち♡ 眠りながら身体が反応するASMR",
      "source_id": "RJ01551159",
      "release": "2026-01-23"
    }
  ]
}
```

#### 5.6.1.6 热门作品列表（POST `/api/recommender/popular`）

Sample Request:

```http
POST https://api.asmr-300.com/api/recommender/popular
Authorization: Bearer <token>
Content-Type: application/json

{
  "keyword": "",
  "page": 1,
  "pageSize": 5,
  "subtitle": 0,
  "localSubtitledWorks": [],
  "withPlaylistStatus": []
}
```

Sample Response（节选）:

```json
{
  "pagination": {
    "totalCount": 100,
    "pageSize": 5
  },
  "works": [
    {
      "id": 1551159,
      "title": "【4時間超】マイク直接耳舐め特化 耳舐め堕ち♡ 眠りながら身体が反応するASMR",
      "source_id": "RJ01551159",
      "dl_count": 21333
    }
  ]
}
```

### 5.6.2 上游站点发现与探测速率接口

这部分用于自动发现可用站点与最快 API 域名。

| 接口名                 | Method | URL                                          | Request | Response                                        |
| ---------------------- | ------ | -------------------------------------------- | ------- | ----------------------------------------------- |
| 获取发布页（主）       | GET    | `https://as.mr`                              | 无      | HTML 页面                                       |
| 获取发布页（代理回退） | GET    | `https://as.131433.xyz`                      | 无      | HTML 页面                                       |
| 获取发布页 JS 资源     | GET    | `{latestPublishSite}/assets/index.[hash].js` | 无      | JS 文本，正则提取 `link:"https://..."` 域名列表 |

程序随后并发探测候选域名响应时间，选择最快域名并转换为 `https://api.{domain}` 作为 API 地址。

### 5.6.3 本地 WebUI（Gin）接口

说明：本地接口仅在 `listen` 命令启动后可用，服务默认监听 `http://localhost:9999`。

| 接口名           | Method | URL                                         | Request                                      | Response                                                                                                       |
| ---------------- | ------ | ------------------------------------------- | -------------------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| 首页             | GET    | `/`                                         | 无                                           | 返回 `index.html`                                                                                              |
| 资源列表分页接口 | GET    | `/api/list?page={page}&pageSize={pageSize}` | Query: `page` 默认 `1`，`pageSize` 默认 `20` | `ResultResp`：`{ code, msg, data }`，成功时 `data={ infos, total, page, pageSize }`；`infos` 为 `[]FolderInfo` |
| 静态资源         | GET    | `/public/*`                                 | 无                                           | 前端静态文件                                                                                                   |
| 本地媒体目录映射 | GET    | `/{syncFolderName}/*`                       | 无                                           | 本地文件直出（音频/字幕等）                                                                                    |

`FolderInfo` 主要字段：`id/name/mediaId/date/hasSubtitles/title/baseDir/files`。

`FileInfo` 主要字段：`id/folderId/path/name/isDir`。

---

## 6. 迁移目标定义

## 6.1 目标产品形态

迁移后的目标产品应为一个 Windows 桌面客户端，具备以下能力：

- 原生 WPF 主窗口
- 左侧导航或顶部导航
- 搜索页面
- 下载管理页面
- 同步管理页面
- 本地资源库页面
- 播放页面
- 设置页面
- 日志 / 任务中心

## 6.2 推荐技术路线

推荐目标栈：

- .NET 8
- WPF
- MVVM
- EF Core + SQLite
- HttpClient
- CommunityToolkit.Mvvm
- Serilog 或 Microsoft.Extensions.Logging
- 可选：Polly、LibVLCSharp、NAudio

## 6.3 不推荐路线

### 不推荐路线 A：WPF 通过 `Process` 调 Go CLI

不推荐原因：

- 难以与 UI 精细绑定
- 进度、取消、重试、日志解析都不稳定
- 后续维护会变成“双栈共存、耦合更重”

### 不推荐路线 B：继续用浏览器 WebUI，只在 WPF 中嵌 WebView2

不推荐原因：

- 本质仍然是 Web 包壳
- 不能真正完成原生桌面化
- 后续仍要维护 Web 端与本地服务

### 可接受的过渡路线

如果希望降低第一次迁移风险，可以短期采用：

- 先重建核心服务层
- 播放页初期允许使用 WebView2 或简单播放器控件过渡
- 但最终仍要回收本地 HTTP 服务和 Gin 依赖

---

## 7. 推荐迁移后架构

## 7.1 总体架构

推荐将项目拆成两个主要工程：

1. `dotnet/Asmroner.Backend/`
  用于放置 .NET 业务类库或应用服务层

2. `dotnet/Asmroner.Wpf/`
   用于放置 WPF 客户端项目

推荐逻辑分层：

- `Domain`：实体、枚举、值对象
- `Application`：搜索、下载、同步、配置、播放、导出等应用服务
- `Infrastructure`：HTTP、SQLite、文件系统、日志
- `Presentation.Wpf`：View、ViewModel、导航、命令绑定

## 7.2 推荐项目结构

建议未来形成类似结构：

```text
dotnet/
  Asmroner.Backend/
    Asmroner.Core/
      Models/
      Interfaces/
      Services/
      Options/
      Helpers/
    Asmroner.Infrastructure/
      Api/
      Persistence/
      Repositories/
      Logging/
      FileSystem/
    Asmroner.Application/
      Search/
      Download/
      Sync/
      Config/
      Library/
  Asmroner.Wpf/
    Asmroner.Wpf/
      App.xaml
      Views/
      ViewModels/
      Services/
      Helpers/
      Resources/
      Converters/
      Controls/
```

说明：

- `dotnet/` 为建议新增根目录，用于容纳全部 .NET 迁移工程
- `Asmroner.Backend/` 与 `Asmroner.Wpf/` 采用并列分层，避免 UI 与业务层耦合
- `Asmroner.Wpf/Asmroner.Wpf/` 为示意层级，可按实际 `.sln` 组织调整

## 7.3 页面与功能对应关系

| 目标页面      | 对应现有能力                               | 说明                                    |
| ------------- | ------------------------------------------ | --------------------------------------- |
| 首页 / 仪表盘 | `sync report`                              | 显示元数据量、下载量、失败数、同步进度  |
| 搜索页        | `search`                                   | 搜索、分页、筛选、导出、加入下载        |
| 下载页        | `download`、`search download`              | 单项下载、批量下载、热门下载、队列状态  |
| 同步页        | `sync`、`sync download`、`retry`、`export` | 元数据同步、失败重试、状态导出          |
| 资源库页      | `listen` 的目录扫描逻辑                    | 展示本地作品目录、文件树、筛选          |
| 播放页        | `listen + WebUI`                           | 音频播放、章节树、字幕支持、切换文件    |
| 设置页        | `config`                                   | 账号、密码、API、代理、目录、限流、并发 |
| 日志页        | `logger`                                   | 实时日志、错误日志、任务明细            |

---

## 8. Go 模块到 .NET 模块映射建议

| 现有 Go 模块      | 目标 .NET 模块                                      | 迁移说明                         |
| ----------------- | --------------------------------------------------- | -------------------------------- |
| `main.go`         | `App.xaml` / Bootstrapper                           | 启动入口改为 WPF 应用生命周期    |
| `cmd/root.go`     | `ApplicationBootstrapper`                           | 统一初始化配置、数据库、目录     |
| `cmd/config.go`   | `SettingsViewModel` + `ConfigurationService`        | CLI 交互改为设置页与首次向导     |
| `cmd/search.go`   | `SearchViewModel` + `SearchService`                 | 终端表格改为可绑定列表           |
| `cmd/download.go` | `DownloadViewModel` + `DownloadService`             | 下载请求改为 UI 命令触发         |
| `cmd/sync.go`     | `SyncViewModel` + `SyncService`                     | 统计、重试、导出改为页面操作     |
| `cmd/listen.go`   | `LibraryViewModel` + `PlayerViewModel`              | 不再启动 HTTP 服务，直接本地扫描 |
| `engine.go`       | `AsmrApiClient` + `DownloadService` + `SyncService` | 核心迁移重点                     |
| `checker.go`      | `EndpointDiscoveryService`                          | 启动检测与手动切换可用域名       |
| `limiter.go`      | `RateLimitService`                                  | 限流与抖动保留                   |
| `db.go`           | `DbContext` + `DatabaseInitializer`                 | 改为 EF Core                     |
| `model/*.go`      | DTO + Entity + ViewModel                            | 逐类映射                         |
| `utils.go`        | Helpers / Utility services                          | 按职责拆分                       |
| `webui/`          | `Views/PlayerPage.xaml` 等                          | 前端界面重写                     |

## 9. Go 模块更新记录

### 9.1 本次检查范围

- 检查日期：2026-03-18
- 对比区间：`origin/v2..upstream/v2`
- `origin/v2`：`5a4d1fca3458cd9c91069b6dc833bc82efcac87d`
- `upstream/v2`：`ab8036d63d917f4043c059af5f5e0f45548731ae`
- 核对命令：`git rev-list --count origin/v2..upstream/v2`
- 核对结果：`0`

结论：本次检查未发现 `upstream/v2` 相对 `origin/v2` 的新增提交，按用户要求以“空区间审计记录”方式留档，不生成伪造的逐 commit 迁移条目。

### 9.2 更新记录

| 序号 | 提交名称                 | 提交描述                                                                 | 提交时间   | 更新文件列表 | 代码逻辑更新描述                                                 | 是否需要迁移到 dotnet | 是否已完成迁移 |
| ---- | ------------------------ | ------------------------------------------------------------------------ | ---------- | ------------ | ---------------------------------------------------------------- | --------------------- | -------------- |
| 1    | 空区间审计（无新增提交） | 已核对 `origin/v2..upstream/v2`，当前未发现 `upstream/v2` 独有的新增提交 | 2026-03-18 | 无           | 本次仅确认上游无新增 Go 逻辑变更进入待迁移范围，本地无需追加迁移 | 否                    | 不适用         |

### 9.3 备注

- `upstream/v2` 当前最新引用虽为 `ab8036d63d917f4043c059af5f5e0f45548731ae`，但不构成 `origin/v2..upstream/v2` 的新增提交区间。
- 后续若再次执行上游同步检查，应继续以 `origin/v2..upstream/v2` 为准，并按实际新增 commit 逐条追加到本章。

---
