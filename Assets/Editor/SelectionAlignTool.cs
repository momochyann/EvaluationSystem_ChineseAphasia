using System;
using System.Collections.Generic;
using System.Linq;
using Nova;
using UnityEditor;
using UnityEngine;

public static class SelectionAlignTool
{
    private const string RootMenu = "工具/对齐/";

    [MenuItem(RootMenu + "X 对齐到第一个")]
    private static void AlignXToFirst()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float targetX = transforms[0].position.x;
        ApplyTransformChange(transforms, "X 对齐到第一个", t =>
        {
            Vector3 p = t.position;
            p.x = targetX;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "Y 对齐到第一个")]
    private static void AlignYToFirst()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float targetY = transforms[0].position.y;
        ApplyTransformChange(transforms, "Y 对齐到第一个", t =>
        {
            Vector3 p = t.position;
            p.y = targetY;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "左对齐 (最小X)")]
    private static void AlignLeft()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float minX = transforms.Min(t => t.position.x);
        ApplyTransformChange(transforms, "左对齐", t =>
        {
            Vector3 p = t.position;
            p.x = minX;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "右对齐 (最大X)")]
    private static void AlignRight()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float maxX = transforms.Max(t => t.position.x);
        ApplyTransformChange(transforms, "右对齐", t =>
        {
            Vector3 p = t.position;
            p.x = maxX;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "下对齐 (最小Y)")]
    private static void AlignBottom()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float minY = transforms.Min(t => t.position.y);
        ApplyTransformChange(transforms, "下对齐", t =>
        {
            Vector3 p = t.position;
            p.y = minY;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "上对齐 (最大Y)")]
    private static void AlignTop()
    {
        if (!TryGetSelection(out var transforms, minCount: 2))
        {
            return;
        }

        float maxY = transforms.Max(t => t.position.y);
        ApplyTransformChange(transforms, "上对齐", t =>
        {
            Vector3 p = t.position;
            p.y = maxY;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "水平分布")]
    private static void DistributeHorizontal()
    {
        if (!TryGetSelection(out var transforms, minCount: 3))
        {
            return;
        }

        var sorted = transforms.OrderBy(t => t.position.x).ToArray();
        float minX = sorted[0].position.x;
        float maxX = sorted[sorted.Length - 1].position.x;
        float step = (maxX - minX) / (sorted.Length - 1);

        ApplyTransformChange(sorted, "水平分布", (t, index) =>
        {
            Vector3 p = t.position;
            p.x = minX + (step * index);
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "垂直分布")]
    private static void DistributeVertical()
    {
        if (!TryGetSelection(out var transforms, minCount: 3))
        {
            return;
        }

        var sorted = transforms.OrderBy(t => t.position.y).ToArray();
        float minY = sorted[0].position.y;
        float maxY = sorted[sorted.Length - 1].position.y;
        float step = (maxY - minY) / (sorted.Length - 1);

        ApplyTransformChange(sorted, "垂直分布", (t, index) =>
        {
            Vector3 p = t.position;
            p.y = minY + (step * index);
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "CheckBox 子物体左对齐")]
    private static void AlignCheckBoxChildrenLeft()
    {
        if (!TryGetSelection(out var transforms, minCount: 1))
        {
            return;
        }

        var checkBoxChildren = transforms
            .SelectMany(t => t.GetComponentsInChildren<Transform>(true))
            .Where(t => t != null && t.name.IndexOf("CheckBox", StringComparison.OrdinalIgnoreCase) >= 0)
            .Distinct()
            .ToArray();

        if (checkBoxChildren.Length == 0)
        {
            Debug.LogWarning("[对齐工具] 选中范围内未找到名为 CheckBox 的子物体。");
            return;
        }

        float minX = checkBoxChildren.Min(t => t.position.x);
        ApplyTransformChange(checkBoxChildren, "CheckBox 子物体左对齐", t =>
        {
            Vector3 p = t.position;
            p.x = minX;
            t.position = p;
        });
    }

    [MenuItem(RootMenu + "拆分 ClipMask 内容与框线层")]
    private static void SplitMaskAndBorderLayer()
    {
        if (!TryGetSelection(out var transforms, minCount: 1))
        {
            return;
        }

        int successCount = 0;
        List<string> skipNames = new List<string>();

        foreach (var src in transforms)
        {
            if (src == null)
            {
                continue;
            }

            var srcBlock = src.GetComponent<UIBlock2D>();
            if (srcBlock == null)
            {
                skipNames.Add(src.name);
                continue;
            }

            Undo.RegisterFullObjectHierarchyUndo(src.gameObject, "拆分 ClipMask 内容与框线层");

            GameObject borderGO = UnityEngine.Object.Instantiate(src.gameObject, src.parent, false);
            borderGO.name = src.name + "框线";
            borderGO.transform.SetSiblingIndex(Mathf.Min(src.GetSiblingIndex() + 1, src.parent != null ? src.parent.childCount - 1 : 0));
            Undo.RegisterCreatedObjectUndo(borderGO, "创建框线层");

            // 删除复制体的所有子物体，只保留根节点边框
            for (int i = borderGO.transform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(borderGO.transform.GetChild(i).gameObject);
            }

            // 框线层不需要裁剪组件
            var clipMasks = borderGO.GetComponents<ClipMask>();
            foreach (var clipMask in clipMasks)
            {
                Undo.DestroyObjectImmediate(clipMask);
            }

            var borderBlock = borderGO.GetComponent<UIBlock2D>();
            if (borderBlock == null)
            {
                borderBlock = Undo.AddComponent<UIBlock2D>(borderGO);
            }

            // 框线层：透明填充 + 外边框
            borderBlock.BodyEnabled = false;
            Color bc = borderBlock.Color;
            bc.a = 0f;
            borderBlock.Color = bc;

            Border overlayBorder = srcBlock.Border;
            overlayBorder.Enabled = true;
            overlayBorder.Direction = BorderDirection.Out;
            ref Border borderRef = ref borderBlock.Border;
            borderRef = overlayBorder;

            // 原内容层：取消自身边框，保留Mask和内容
            Border srcBorder = srcBlock.Border;
            srcBorder.Enabled = false;
            ref Border srcBorderRef = ref srcBlock.Border;
            srcBorderRef = srcBorder;

            PrefabUtility.RecordPrefabInstancePropertyModifications(src);
            PrefabUtility.RecordPrefabInstancePropertyModifications(srcBlock);
            PrefabUtility.RecordPrefabInstancePropertyModifications(borderGO.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(borderBlock);
            EditorUtility.SetDirty(srcBlock);
            EditorUtility.SetDirty(borderBlock);

            successCount++;
        }

        if (skipNames.Count > 0)
        {
            Debug.LogWarning($"[对齐工具] 以下对象没有 UIBlock2D，已跳过: {string.Join(", ", skipNames)}");
        }

        Debug.Log($"[对齐工具] 拆分完成：成功 {successCount} 个。");
    }

    private static bool TryGetSelection(out Transform[] transforms, int minCount)
    {
        transforms = Selection.transforms;
        if (transforms == null || transforms.Length < minCount)
        {
            Debug.LogWarning($"[对齐工具] 请至少选择 {minCount} 个物体。");
            return false;
        }

        return true;
    }

    private static void ApplyTransformChange(Transform[] transforms, string undoName, Action<Transform> apply)
    {
        Undo.RecordObjects(transforms, undoName);
        foreach (var t in transforms)
        {
            if (t == null)
            {
                continue;
            }

            apply(t);
            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            EditorUtility.SetDirty(t);
        }
    }

    private static void ApplyTransformChange(Transform[] transforms, string undoName, Action<Transform, int> apply)
    {
        Undo.RecordObjects(transforms, undoName);
        for (int i = 0; i < transforms.Length; i++)
        {
            var t = transforms[i];
            if (t == null)
            {
                continue;
            }

            apply(t, i);
            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            EditorUtility.SetDirty(t);
        }
    }
}
