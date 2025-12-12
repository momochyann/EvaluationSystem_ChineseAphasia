/// <summary>
/// 窗口标题桥接接口（主程序集）
/// 为热更新层提供安全的窗口标题更新能力
/// </summary>
public interface IWindowTitleBridge
{
    /// <summary>
    /// 设置窗口标题。传入空值时可使用默认标题。
    /// </summary>
    /// <param name="title">目标标题</param>
    /// <param name="defaultTitle">备用标题（可为空则使用产品名）</param>
    void SetTitle(string title, string defaultTitle = null);
}


