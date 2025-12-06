/// <summary>
/// 文件选择桥接接口（主程序集）
/// 用于在HybridCLR热更新代码中调用Windows文件选择对话框
/// </summary>
public interface IFilePickerBridge
{
    /// <summary>
    /// 打开文件选择对话框
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="filter">文件过滤器，例如："图片文件|*.png;*.jpg;*.jpeg"</param>
    /// <param name="initialDir">初始目录</param>
    /// <returns>选择的文件路径，如果取消则返回null</returns>
    string OpenFileDialog(string title = "选择文件", string filter = null, string initialDir = null);
    
    /// <summary>
    /// 打开保存文件对话框
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="defaultFileName">默认文件名</param>
    /// <param name="filter">文件过滤器，例如："JSON文件|*.json|所有文件|*.*"</param>
    /// <param name="initialDir">初始目录</param>
    /// <returns>保存的文件路径，如果取消则返回null</returns>
    string SaveFileDialog(string title = "保存文件", string defaultFileName = "备份.json", string filter = null, string initialDir = null);
}

