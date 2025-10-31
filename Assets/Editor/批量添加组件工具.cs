using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// 批量添加预制体工具
/// 可以为选中对象的子物体批量实例化预制体
/// </summary>
public class 批量添加组件工具 : EditorWindow
{
    private List<GameObject> 父对象列表 = new List<GameObject>();
    private Vector2 父对象列表滚动位置;
    private Vector2 预览滚动位置;
    
    // 要添加的预制体
    private GameObject 要添加的预制体;
    
    // 筛选条件
    private bool 只添加到特定名称 = false;
    private string 目标物体名称包含 = "";
    private bool 包含子对象 = true;
    private bool 跳过已有子物体 = false;
    private string 检查子物体名称 = "";
    
    // 实例化选项
    private bool 保持预制体连接 = true;
    private bool 重置本地位置 = true;
    private bool 重置本地旋转 = true;
    private bool 重置本地缩放 = false;
    
    // 子物体位置选项
    private enum 子物体位置
    {
        第一个 = 0,
        最后一个 = 1,
        指定索引 = 2
    }
    private 子物体位置 插入位置 = 子物体位置.第一个;
    private int 指定索引值 = 0;
    
    // 预览结果
    private List<添加项> 预览列表 = new List<添加项>();
    private bool 已预览 = false;

    [MenuItem("MomoTools/批量添加预制体工具 &P")]
    public static void ShowWindow()
    {
        var window = GetWindow<批量添加组件工具>("批量添加预制体");
        window.minSize = new Vector2(700, 600);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("批量添加预制体工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("为选中对象的子物体批量实例化预制体", MessageType.Info);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        
        // 左侧：父对象列表
        EditorGUILayout.BeginVertical(GUILayout.Width(280));
        DrawParentObjectList();
        EditorGUILayout.EndVertical();
        
        // 右侧：配置和预览
        EditorGUILayout.BeginVertical();
        DrawPrefabSection();
        EditorGUILayout.Space(5);
        DrawConfigSection();
        EditorGUILayout.Space(10);
        DrawActionButtons();
        EditorGUILayout.Space(10);
        if (已预览)
        {
            DrawPreviewSection();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制父对象列表
    /// </summary>
    private void DrawParentObjectList()
    {
        EditorGUILayout.LabelField("父对象列表", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical("box");
        
        // 拖放区域
        var dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "拖放父对象到这里\n(支持多选)");
        
        HandleDragAndDrop(dropArea);
        
        EditorGUILayout.Space(5);
        
        // 列表
        父对象列表滚动位置 = EditorGUILayout.BeginScrollView(父对象列表滚动位置, GUILayout.Height(300));
        
        for (int i = 0; i < 父对象列表.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");
            
            // 序号
            EditorGUILayout.LabelField($"{i + 1}.", GUILayout.Width(30));
            
            // 对象字段
            父对象列表[i] = (GameObject)EditorGUILayout.ObjectField(
                父对象列表[i], 
                typeof(GameObject), 
                true,  // 允许场景对象
                GUILayout.ExpandWidth(true)
            );
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(25)))
            {
                父对象列表.RemoveAt(i);
                已预览 = false;
                break;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space(5);
        
        // 底部按钮
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("添加空位"))
        {
            父对象列表.Add(null);
        }
        
        if (GUILayout.Button("清空列表"))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清空列表吗？", "确定", "取消"))
            {
                父对象列表.Clear();
                已预览 = false;
            }
        }
        
        if (GUILayout.Button("移除空位"))
        {
            父对象列表.RemoveAll(x => x == null);
            已预览 = false;
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // 从选中对象添加按钮
        if (GUILayout.Button("从选中对象添加"))
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                foreach (var obj in Selection.gameObjects)
                {
                    if (!父对象列表.Contains(obj))
                    {
                        父对象列表.Add(obj);
                    }
                }
                已预览 = false;
            }
        }
        
        EditorGUILayout.LabelField($"已添加: {父对象列表.Count(x => x != null)} 个对象");
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 处理拖放
    /// </summary>
    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        
        if (dropArea.Contains(evt.mousePosition))
        {
            if (evt.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
            else if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    GameObject go = obj as GameObject;
                    if (go != null && !父对象列表.Contains(go))
                    {
                        父对象列表.Add(go);
                    }
                }
                
                已预览 = false;
                evt.Use();
            }
        }
    }

    /// <summary>
    /// 绘制预制体选择区域
    /// </summary>
    private void DrawPrefabSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("要添加的预制体", EditorStyles.boldLabel);
        
        GameObject 新预制体 = (GameObject)EditorGUILayout.ObjectField(
            "预制体:", 
            要添加的预制体, 
            typeof(GameObject), 
            false,
            GUILayout.Height(20)
        );
        
        if (新预制体 != 要添加的预制体)
        {
            要添加的预制体 = 新预制体;
            已预览 = false;
        }
        
        if (要添加的预制体 != null)
        {
            EditorGUILayout.HelpBox($"将添加预制体: {要添加的预制体.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("请选择要添加的预制体", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制配置区域
    /// </summary>
    private void DrawConfigSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("添加配置", EditorStyles.boldLabel);
        
        // 筛选条件
        EditorGUILayout.LabelField("目标子物体筛选", EditorStyles.miniBoldLabel);
        包含子对象 = EditorGUILayout.Toggle("包含所有层级子对象", 包含子对象);
        
        EditorGUILayout.BeginHorizontal();
        只添加到特定名称 = EditorGUILayout.Toggle("只添加到特定名称", 只添加到特定名称);
        GUI.enabled = 只添加到特定名称;
        目标物体名称包含 = EditorGUILayout.TextField(目标物体名称包含);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        if (只添加到特定名称 && !string.IsNullOrEmpty(目标物体名称包含))
        {
            EditorGUILayout.HelpBox($"只会为名称包含 '{目标物体名称包含}' 的子物体添加预制体", MessageType.Info);
        }
        
        EditorGUILayout.BeginHorizontal();
        跳过已有子物体 = EditorGUILayout.Toggle("跳过已有指定子物体", 跳过已有子物体);
        GUI.enabled = 跳过已有子物体;
        检查子物体名称 = EditorGUILayout.TextField(检查子物体名称);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        if (跳过已有子物体 && !string.IsNullOrEmpty(检查子物体名称))
        {
            EditorGUILayout.HelpBox($"如果子物体已经有名为 '{检查子物体名称}' 的子对象，将跳过", MessageType.Info);
        }
        
        EditorGUILayout.Space(5);
        
        // 实例化选项
        EditorGUILayout.LabelField("实例化选项", EditorStyles.miniBoldLabel);
        保持预制体连接 = EditorGUILayout.Toggle("保持预制体连接", 保持预制体连接);
        重置本地位置 = EditorGUILayout.Toggle("重置本地位置", 重置本地位置);
        重置本地旋转 = EditorGUILayout.Toggle("重置本地旋转", 重置本地旋转);
        重置本地缩放 = EditorGUILayout.Toggle("重置本地缩放", 重置本地缩放);
        
        EditorGUILayout.Space(5);
        
        // 子物体位置选项
        EditorGUILayout.LabelField("子物体位置", EditorStyles.miniBoldLabel);
        插入位置 = (子物体位置)EditorGUILayout.EnumPopup("插入位置:", 插入位置);
        
        if (插入位置 == 子物体位置.指定索引)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("索引值:", GUILayout.Width(60));
            指定索引值 = EditorGUILayout.IntField(指定索引值);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("索引从0开始。如果索引超出范围，将添加到最后", MessageType.Info);
        }
        else if (插入位置 == 子物体位置.第一个)
        {
            EditorGUILayout.HelpBox("预制体将作为第一个子物体", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("预制体将作为最后一个子物体", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制操作按钮
    /// </summary>
    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = 父对象列表.Any(x => x != null) && 要添加的预制体 != null;
        
        if (GUILayout.Button("预览添加", GUILayout.Height(30)))
        {
            预览添加();
        }
        
        GUI.enabled = 已预览 && 预览列表.Count > 0;
        
        if (GUILayout.Button("应用添加", GUILayout.Height(30)))
        {
            int 将添加数量 = 预览列表.Count(x => !x.已存在组件);
            if (EditorUtility.DisplayDialog(
                "确认添加", 
                $"确定要为 {将添加数量} 个对象添加预制体 '{要添加的预制体.name}' 吗？", 
                "确定", 
                "取消"))
            {
                应用添加();
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
        EditorGUILayout.LabelField($"预览结果 (共 {预览列表.Count} 项)", EditorStyles.boldLabel);
        
        if (预览列表.Count == 0)
        {
            EditorGUILayout.HelpBox("未找到符合条件的目标对象", MessageType.Warning);
        }
        else
        {
            预览滚动位置 = EditorGUILayout.BeginScrollView(预览滚动位置, GUILayout.Height(150));
            
            foreach (var 项 in 预览列表)
            {
                EditorGUILayout.BeginHorizontal("box");
                
                // 对象名称（可点击定位）
                if (GUILayout.Button(项.对象路径, EditorStyles.linkLabel, GUILayout.Width(400)))
                {
                    Selection.activeGameObject = 项.目标对象;
                    EditorGUIUtility.PingObject(项.目标对象);
                }
                
                // 状态
                Color 原颜色 = GUI.color;
                if (项.已存在组件)
                {
                    GUI.color = Color.yellow;
                    EditorGUILayout.LabelField("已存在", GUILayout.Width(60));
                }
                else
                {
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField("将添加", GUILayout.Width(60));
                }
                GUI.color = 原颜色;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            // 统计信息
            EditorGUILayout.Space(5);
            int 新添加数量 = 预览列表.Count(x => !x.已存在组件);
            int 已存在数量 = 预览列表.Count(x => x.已存在组件);
            EditorGUILayout.LabelField($"将新添加: {新添加数量} 个预制体实例  |  已存在跳过: {已存在数量} 个");
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 预览添加
    /// </summary>
    private void 预览添加()
    {
        预览列表.Clear();
        
        if (要添加的预制体 == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择要添加的预制体", "确定");
            return;
        }
        
        // 遍历所有父对象
        foreach (var 父对象 in 父对象列表)
        {
            if (父对象 == null) continue;
            
            // 获取所有Transform
            Transform[] transforms = 包含子对象
                ? 父对象.GetComponentsInChildren<Transform>(true)
                : new Transform[] { 父对象.transform };
            
            foreach (var trans in transforms)
            {
                // 检查名称过滤
                if (只添加到特定名称 && !string.IsNullOrEmpty(目标物体名称包含))
                {
                    if (!trans.name.Contains(目标物体名称包含))
                    {
                        continue;
                    }
                }
                
                // 检查是否已存在指定名称的子物体
                bool 已存在 = false;
                if (跳过已有子物体 && !string.IsNullOrEmpty(检查子物体名称))
                {
                    已存在 = trans.Find(检查子物体名称) != null;
                }
                
                预览列表.Add(new 添加项
                {
                    目标对象 = trans.gameObject,
                    对象路径 = GetFullPath(trans, 父对象.transform),
                    已存在组件 = 已存在  // 这里复用字段表示是否已存在子物体
                });
            }
        }
        
        已预览 = true;
        Debug.Log($"预览完成：找到 {预览列表.Count} 个符合条件的对象");
    }

    /// <summary>
    /// 应用添加
    /// </summary>
    private void 应用添加()
    {
        if (预览列表.Count == 0)
        {
            Debug.LogWarning("没有需要添加的对象");
            return;
        }
        
        if (要添加的预制体 == null)
        {
            Debug.LogError("要添加的预制体为空");
            return;
        }
        
        int 成功数量 = 0;
        int 跳过数量 = 0;
        
        foreach (var 项 in 预览列表)
        {
            if (项.已存在组件)  // 这里表示已存在指定子物体
            {
                跳过数量++;
                continue;
            }
            
            try
            {
                // 实例化预制体
                GameObject 新实例;
                if (保持预制体连接)
                {
                    新实例 = (GameObject)PrefabUtility.InstantiatePrefab(要添加的预制体, 项.目标对象.transform);
                }
                else
                {
                    新实例 = Instantiate(要添加的预制体, 项.目标对象.transform);
                }
                
                Undo.RegisterCreatedObjectUndo(新实例, "添加预制体");
                
                // 设置子物体位置
                switch (插入位置)
                {
                    case 子物体位置.第一个:
                        新实例.transform.SetAsFirstSibling();
                        break;
                    case 子物体位置.最后一个:
                        新实例.transform.SetAsLastSibling();
                        break;
                    case 子物体位置.指定索引:
                        新实例.transform.SetSiblingIndex(指定索引值);
                        break;
                }
                
                // 应用Transform设置
                if (重置本地位置)
                {
                    新实例.transform.localPosition = Vector3.zero;
                }
                if (重置本地旋转)
                {
                    新实例.transform.localRotation = Quaternion.identity;
                }
                if (重置本地缩放)
                {
                    新实例.transform.localScale = Vector3.one;
                }
                
                EditorUtility.SetDirty(项.目标对象);
                成功数量++;
            }
            catch (Exception e)
            {
                Debug.LogError($"添加预制体失败: {项.对象路径}, 错误: {e.Message}");
            }
        }
        
        // 保存场景修改
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log($"✅ 添加完成：成功添加 {成功数量} 个预制体实例，跳过 {跳过数量} 个");
        
        // 刷新预览
        预览添加();
    }

    /// <summary>
    /// 获取对象的完整路径
    /// </summary>
    private string GetFullPath(Transform transform, Transform root)
    {
        string path = transform.name;
        Transform parent = transform.parent;
        
        while (parent != null && parent != root)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        if (root != null)
        {
            path = root.name + "/" + path;
        }
        
        return path;
    }

    /// <summary>
    /// 添加项数据结构
    /// </summary>
    private class 添加项
    {
        public GameObject 目标对象;
        public string 对象路径;
        public bool 已存在组件;
    }
}


