# 通用执行约束

> 本文档是针对操作本仓库的 AI 代理的**通用执行约束**，执行任何操作都**必须**严格遵守以下全部规则。
> 除非明确说明，否则 AI **禁止**修改本文档。

---

## 1. 全局指令

- **范围约束**：除非用户明确要求，否则**禁止**修改与当前任务无关的已有功能。
- **注释规范**：生成的注释**必须**使用中文，并采用 UTF-8 编码。
- **编码检查**：生成或修改包含中文的内容后，**必须**检查是否存在乱码；若发现乱码，**必须**修正。
- **函数修改**：修改函数实现前，**必须**先理解原有逻辑；修改时**必须**在保留原有逻辑的基础上进行，**禁止**直接移除原有逻辑。
- **环境认知**：当前操作环境为 Windows 系统。
- **测试与文档**：除非用户明确要求，否则**禁止**编写测试脚本或专门的项目说明 Markdown 文档。
- **Fallback 策略**：写代码时**不考虑** fallback 逻辑。
- **字符规范**：代码中**禁止**出现 emoji。
- **`rtk`检查**：所有命令行语句均执行 SKILL `rtk`。
- **CRG 构建触发**：满足以下任一条件时，**必须**执行 `rtk code-review-graph build`：
  - 首次进入仓库
  - 切换分支后
  - 执行 `git pull` 后
  - 进行大规模重构（跨目录移动、重命名、批量改函数签名）
- **CRG 命令约束**：所有 `code-review-graph` 相关命令**必须**通过 `rtk` 执行，允许的命令白名单：
  - `rtk code-review-graph build`
  - `rtk code-review-graph install --platform codex`
  - `rtk code-review-graph serve`（仅在显式需要时）
- **CRG 新项目初始化**：对尚未配置 MCP 的新项目，首次使用**必须**按顺序执行：
  1. `rtk code-review-graph install --platform codex`
  2. `rtk code-review-graph build`
  完成后再进入常规任务流程。

## 2. 任务规划

- **MUST** 在开始任何非平凡任务前生成详细的 todo list。
- **MUST** 严格按照 todo list 的步骤执行。
- **MUST** 在任务状态发生变化时立即更新 todo list。

## 3. 上下文排除（强制不读不写）

以下文件**禁止**加载到分析上下文中，也**禁止**被修改：

- `docs/prompts.md`
- `docs/legacy-go.md`
- `docs/review/*`
