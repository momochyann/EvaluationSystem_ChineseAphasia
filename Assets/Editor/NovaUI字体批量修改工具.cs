using UnityEngine;
using UnityEditor;
using Nova;
using NovaSamples.UIControls;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Nova UI 字体大小批量修改工具
/// 可以批量修改预制体中所有 TextField 和 TextBlock 的 TMP 字体大小
/// </summary>
public class NovaUI字体批量修改工具 : EditorWindow
{
    private GameObject 目标预制体;
    private float 新字体大小 = 24f;
    private bool 包含子对象 = true;
    private bool 只修改TextField = false;
    private bool 只修改TextBlock = false;
    
    // 字体大小过滤
    private bool 启用字体大小过滤 = false;
    private float 只修改此大小 = 18f;
    private float 大小容差 = 0.1f;  // 允许误差范围
    
    private Vector2 滚动位置;
    
    // 预览结果
    private List<修改项> 修改列表 = new List<修改项>();
    private bool 已预览 = false;

    [MenuItem("MomoTools/Nova UI 字体批量修改 &F")]
    public static void ShowWindow()
    {
        var window = GetWindow<NovaUI字体批量修改工具>("Nova字体修改");
        window.minSize = new Vector2(500, 400);
        window.Show();
    }

    private void OnEnable()
    {
        // 自动检测当前选中的预制体
        if (Selection.activeGameObject != null)
        {
            目标预制体 = Selection.activeGameObject;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Nova UI 字体批量修改工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("批量修改预制体中所有 TextField 和 TextBlock 的字体大小", MessageType.Info);
        EditorGUILayout.Space(10);

        // 配置区域
        DrawConfigSection();
        
        EditorGUILayout.Space(10);
        
        // 操作按钮
        DrawActionButtons();
        
        EditorGUILayout.Space(10);
        
        // 预览结果
        if (已预览)
        {
            DrawPreviewSection();
        }
    }

    /// <summary>
    /// 绘制配置区域
    /// </summary>
    private void DrawConfigSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("配置", EditorStyles.boldLabel);
        
        // 目标预制体
        GameObject 新预制体 = (GameObject)EditorGUILayout.ObjectField(
            "目标预制体", 
            目标预制体, 
            typeof(GameObject), 
            true
        );
        
        if (新预制体 != 目标预制体)
        {
            目标预制体 = 新预制体;
            已预览 = false; // 重置预览
        }
        
        // 字体大小
        float 新大小 = EditorGUILayout.FloatField("新字体大小", 新字体大小);
        if (新大小 != 新字体大小)
        {
            新字体大小 = Mathf.Max(1f, 新大小);
            已预览 = false;
        }
        
        EditorGUILayout.Space(5);
        
        // 选项
        包含子对象 = EditorGUILayout.Toggle("包含子对象", 包含子对象);
        
        EditorGUILayout.BeginHorizontal();
        只修改TextField = EditorGUILayout.Toggle("只修改 TextField", 只修改TextField);
        只修改TextBlock = EditorGUILayout.Toggle("只修改 TextBlock", 只修改TextBlock);
        EditorGUILayout.EndHorizontal();
        
        if (只修改TextField && 只修改TextBlock)
        {
            EditorGUILayout.HelpBox("不能同时只修改两种类型，已自动取消限制", MessageType.Warning);
            只修改TextField = false;
            只修改TextBlock = false;
        }
        
        EditorGUILayout.Space(5);
        
        // 字体大小过滤
        EditorGUILayout.BeginVertical("box");
        启用字体大小过滤 = EditorGUILayout.Toggle("只修改特定字体大小", 启用字体大小过滤);
        
        if (启用字体大小过滤)
        {
            EditorGUI.indentLevel++;
            只修改此大小 = EditorGUILayout.FloatField("原字体大小", 只修改此大小);
            大小容差 = EditorGUILayout.Slider("容差范围", 大小容差, 0f, 5f);
            EditorGUILayout.HelpBox($"只会修改字体大小在 {只修改此大小 - 大小容差:F1} ~ {只修改此大小 + 大小容差:F1} 之间的文本", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制操作按钮
    /// </summary>
    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = 目标预制体 != null;
        
        // 预览按钮
        if (GUILayout.Button("预览修改", GUILayout.Height(30)))
        {
            预览修改();
        }
        
        // 应用按钮
        GUI.enabled = 目标预制体 != null && 已预览 && 修改列表.Count > 0;
        if (GUILayout.Button("应用修改", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog(
                "确认修改", 
                $"确定要修改 {修改列表.Count} 个文本组件的字体大小吗？", 
                "确定", 
                "取消"))
            {
                应用修改();
            }
        }
        
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制预览区域
    /// </summary>
    private void DrawPreviewSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"预览结果 (共 {修改列表.Count} 项)", EditorStyles.boldLabel);
        
        if (修改列表.Count == 0)
        {
            EditorGUILayout.HelpBox("未找到任何 TextField 或 TextBlock 组件", MessageType.Warning);
        }
        else
        {
            滚动位置 = EditorGUILayout.BeginScrollView(滚动位置, GUILayout.Height(200));
            
            foreach (var 项 in 修改列表)
            {
                EditorGUILayout.BeginHorizontal("box");
                
                // 组件类型图标
                string 图标 = 项.类型 == "TextField" ? "📝" : "📄";
                EditorGUILayout.LabelField(图标, GUILayout.Width(20));
                
                // 对象名称（可点击定位）
                if (GUILayout.Button(项.对象名称, EditorStyles.linkLabel, GUILayout.Width(200)))
                {
                    Selection.activeGameObject = 项.组件.gameObject;
                    EditorGUIUtility.PingObject(项.组件.gameObject);
                }
                
                // 类型
                EditorGUILayout.LabelField(项.类型, GUILayout.Width(80));
                
                // 原字体大小
                EditorGUILayout.LabelField($"{项.原字体大小:F1}", GUILayout.Width(50));
                
                // 箭头
                EditorGUILayout.LabelField("→", GUILayout.Width(20));
                
                // 新字体大小
                Color 原颜色 = GUI.color;
                GUI.color = 项.原字体大小 != 新字体大小 ? Color.green : Color.gray;
                EditorGUILayout.LabelField($"{新字体大小:F1}", GUILayout.Width(50));
                GUI.color = 原颜色;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            // 统计信息
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            int textFieldCount = 修改列表.Count(x => x.类型 == "TextField");
            int textBlockCount = 修改列表.Count(x => x.类型 == "TextBlock");
            EditorGUILayout.LabelField($"TextField: {textFieldCount} 个", GUILayout.Width(150));
            EditorGUILayout.LabelField($"TextBlock: {textBlockCount} 个", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 预览修改
    /// </summary>
    private void 预览修改()
    {
        修改列表.Clear();
        
        if (目标预制体 == null)
        {
            Debug.LogWarning("请先选择目标预制体");
            return;
        }

        int textBlockCount = 0;
        int textFieldCount = 0;
        
        // 直接查找 TextBlock 组件
        if (!只修改TextField)
        {
            var textBlocks = 包含子对象
                ? 目标预制体.GetComponentsInChildren<TextBlock>(true)
                : 目标预制体.GetComponents<TextBlock>();
            
            Debug.Log($"找到 {textBlocks.Length} 个 TextBlock 组件");
            
            foreach (var tb in textBlocks)
            {
                if (tb.TMP != null)
                {
                    // 检查字体大小过滤
                    if (启用字体大小过滤)
                    {
                        float 差值 = Mathf.Abs(tb.TMP.fontSize - 只修改此大小);
                        if (差值 > 大小容差)
                        {
                            continue; // 跳过不符合大小要求的
                        }
                    }
                    
                    修改列表.Add(new 修改项
                    {
                        组件 = tb.TMP,
                        类型 = "TextBlock",
                        对象名称 = GetFullPath(tb.transform),
                        原字体大小 = tb.TMP.fontSize
                    });
                    textBlockCount++;
                }
                else
                {
                    Debug.LogWarning($"TextBlock '{tb.name}' 没有 TMP 组件");
                }
            }
        }
        
        // 直接查找 TextField 组件
        if (!只修改TextBlock)
        {
            var textFields = 包含子对象
                ? 目标预制体.GetComponentsInChildren<TextField>(true)
                : 目标预制体.GetComponents<TextField>();
            
            Debug.Log($"找到 {textFields.Length} 个 TextField 组件");
            
            foreach (var tf in textFields)
            {
                // TextField 可能也有 TMP
                var tmp = tf.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    // 检查字体大小过滤
                    if (启用字体大小过滤)
                    {
                        float 差值 = Mathf.Abs(tmp.fontSize - 只修改此大小);
                        if (差值 > 大小容差)
                        {
                            continue; // 跳过不符合大小要求的
                        }
                    }
                    
                    修改列表.Add(new 修改项
                    {
                        组件 = tmp,
                        类型 = "TextField",
                        对象名称 = GetFullPath(tf.transform),
                        原字体大小 = tmp.fontSize
                    });
                    textFieldCount++;
                }
            }
        }

        已预览 = true;
        Debug.Log($"预览完成：找到 {修改列表.Count} 个文本组件 (TextBlock: {textBlockCount}, TextField: {textFieldCount})");
    }

    /// <summary>
    /// 应用修改
    /// </summary>
    private void 应用修改()
    {
        if (修改列表.Count == 0)
        {
            Debug.LogWarning("没有需要修改的组件");
            return;
        }

        // 记录 Undo
        Undo.RecordObject(目标预制体, "批量修改字体大小");

        int 成功数量 = 0;
        foreach (var 项 in 修改列表)
        {
            if (项.组件 == null)
            {
                Debug.LogWarning($"组件为空: {项.对象名称}");
                continue;
            }
            
            Undo.RecordObject(项.组件, "修改字体大小");
            项.组件.fontSize = 新字体大小;
            EditorUtility.SetDirty(项.组件);
            成功数量++;
        }

        // 标记预制体为脏
        EditorUtility.SetDirty(目标预制体);
        
        // 如果是预制体，保存更改
        if (PrefabUtility.IsPartOfPrefabAsset(目标预制体) || PrefabUtility.IsPartOfPrefabInstance(目标预制体))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(目标预制体);
        }

        Debug.Log($"✅ 修改完成：成功修改 {成功数量} 个文本组件的字体大小为 {新字体大小}");
        
        // 刷新预览
        预览修改();
    }

    /// <summary>
    /// 获取对象的完整路径
    /// </summary>
    private string GetFullPath(Transform transform)
    {
        string path = transform.name;
        Transform parent = transform.parent;
        
        while (parent != null && parent != 目标预制体.transform)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }

    /// <summary>
    /// 修改项数据结构
    /// </summary>
    private class 修改项
    {
        public TMP_Text 组件;  // 使用 TMP_Text 基类，兼容 TextMeshPro 和 TextMeshProUGUI
        public string 类型;
        public string 对象名称;
        public float 原字体大小;
    }
}

/// <summary>
/// 右键菜单快捷方式
/// </summary>
public static class NovaUI字体修改右键菜单
{
    [MenuItem("GameObject/Nova UI/批量修改字体大小", false, 0)]
    private static void 批量修改字体大小()
    {
        NovaUI字体批量修改工具.ShowWindow();
    }

    [MenuItem("GameObject/Nova UI/批量修改字体大小", true)]
    private static bool 验证批量修改字体大小()
    {
        return Selection.activeGameObject != null;
    }
}

