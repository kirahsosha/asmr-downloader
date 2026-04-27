# 版本更新专项约束

> 本文档是针对操作本仓库的 AI 代理的**版本更新专项约束**。
> 涉及版本号升级、版本口径同步或 `docs/wpf-migration-progress.md` / `docs/wpf-migration-history.md` / `docs/wpf-migration-tests.md` 的版本维护时，**必须**完整读取并严格遵守以下全部规则。
> 除非明确说明，否则 AI **禁止**修改本文档。

---

## 1. 版本号更新规范

执行本类任务时，除本文件外，**必须**同时遵守 `docs/prompts/document_special_prompts.md` 与 `docs/prompts/submission_policy_prompts.md` 中的相关规则。

### 1.1 适用范围

- 将 WPF 运行时版本统一升级到新的 `主版本.次版本.补丁版本`。
- 同步更新当前口径文档与验证记录。

### 1.2 更新范围

**必须更新**：

- 四个运行时项目的 `.csproj` 版本字段：
  - `dotnet/Asmroner.Backend/Asmroner.Core/Asmroner.Core.csproj`
  - `dotnet/Asmroner.Backend/Asmroner.Application/Asmroner.Application.csproj`
  - `dotnet/Asmroner.Backend/Asmroner.Infrastructure/Asmroner.Infrastructure.csproj`
  - `dotnet/Asmroner.Wpf/Asmroner.Wpf/Asmroner.Wpf.csproj`
- `README.md` 中的当前版本文案。
- `docs/wpf-migration-progress.md` 中的当前跟踪版本、阶段口径、`§2.1` 待提交行、`§3` 手工测试清单。
- `docs/wpf-migration-history.md` 中的 `§1` 追加记录。
- `docs/wpf-migration-tests.md` 中的 `§1` 单元测试清单基线与受影响样例说明。

**禁止更新**：

- 历史 Go CLI / WebUI 的版本号与构建脚本口径。
- `bin/`、`obj/` 生成产物。
- `docs/wpf-migration-history.md` 中已存在的历史 `§1` 记录正文。

### 1.3 版本号格式

每个 `.csproj` 必须同步更新以下字段：

| 字段                     | 格式示例  | 说明   |
| ------------------------ | --------- | ------ |
| `<Version>`              | `0.4.9`   | 三段式 |
| `<InformationalVersion>` | `0.4.9`   | 三段式 |
| `<AssemblyVersion>`      | `0.4.9.0` | 四段式 |
| `<FileVersion>`          | `0.4.9.0` | 四段式 |

### 1.4 执行步骤

1. **建立 todo list**：按 `docs/prompts/global_prompts.md` 的 `§2` 要求。
2. **更新版本号**：
   - 更新四个运行时项目的版本字段（见 `§1.3`）。
   - 同步更新 `README.md` 的当前版本显示。
   - 当前项目通过 `AppVersionInfo` 动态读取程序集版本，优先保证 `.csproj` 正确；仅在确有硬编码残留时，才额外修改运行时代码或测试。
3. **同步 progress 文档**（按顺序）：
   - 检查 `docs/wpf-migration-progress.md` 顶部当前跟踪版本，以及 `1.2` / `1.3` / `1.4` / `1.5` 是否需要变更。
   - 在 `docs/wpf-migration-history.md` 的 `1` 追加新记录，不修改旧记录。
   - 在 `docs/wpf-migration-tests.md` 的顶部基线说明与 `1` 更新当前测试样例基线。
   - 在 `docs/wpf-migration-progress.md` 的 `2.1` 新增或合并单一待提交行（见 `docs/prompts/document_special_prompts.md` 的 `§1.3` 与 `docs/prompts/submission_policy_prompts.md` 的 `§1`）。
   - 在 `docs/wpf-migration-progress.md` 的 `3` 将受影响项重置为未勾选，并新增必要验证项（见 `docs/prompts/document_special_prompts.md` 的 `§1.4`）。
4. **验证**（建议顺序）：
   - `Application.Tests`
   - `Infrastructure.Tests`
   - `Wpf.Tests`
   - `dotnet/Asmroner.sln`
   - 若 WPF 测试或构建被进程锁定阻塞，应记录原因并补充手工验证结果。

### 1.5 检查清单

- [ ] 四个运行时项目版本字段已统一（三段式/四段式正确）。
- [ ] `README.md` 当前版本已同步。
- [ ] `docs/wpf-migration-progress.md` 顶部当前跟踪版本已同步。
- [ ] `docs/wpf-migration-progress.md` 顶部版本口径与 `1.2`/`1.3`/`1.4` 已检查并更新。
- [ ] `docs/wpf-migration-history.md` 的 `1` 已追加记录，未误动历史记录。
- [ ] `docs/wpf-migration-tests.md` 的 `1` 已同步当前测试基线。
- [ ] `2.1` 已保持单一待提交行。
- [ ] `3` 受影响项已重置为未勾选。
- [ ] 目标测试项目通过。
- [ ] 解决方案级回归通过或已记录阻塞原因。
- [ ] Settings 页面版本文案与启动日志显示新版本。
- [ ] 未误改 `bin/`、`obj/` 产物。
- [ ] 未误动历史 Go 版本口径。
- [ ] 未读取或修改被排除的文件（见 `docs/prompts/global_prompts.md` 的 `§3` 与 `docs/prompts/document_special_prompts.md` 的 `§1.1`）。
