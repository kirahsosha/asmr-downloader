namespace Asmroner.Core.Interfaces;

/// <summary>
/// 应用级日志服务。负责在初始化阶段配置底层日志提供程序（NLog），
/// 使后续通过 ILogger&lt;T&gt; 注入的日志请求写入结构化 JSON 滚动文件。
/// </summary>
public interface IAppLogService
{
    /// <summary>
    /// 配置日志输出目录。目录不存在时将自动创建。
    /// 应在 Host.Build() 之后、首次业务日志输出之前调用。
    /// </summary>
    void Configure(string logDirectory);
}
