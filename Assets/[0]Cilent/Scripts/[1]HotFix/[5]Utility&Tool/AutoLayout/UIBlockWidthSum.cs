using System;
using UnityEngine;
using Nova;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class UIBlockWidthSum : MonoBehaviour
{
	[Tooltip("Target Nova UIBlock (UIBlock2D). If null, the script will use the UIBlock on this GameObject.")]
	public UIBlock targetBlock;

	[Tooltip("Parent transform that contains the child columns/rows to sum. If null, uses this GameObject.")]
	public Transform childrenParent;

	[Tooltip("Extra padding (units) added to the computed width.")]
	public float extraPadding = 0f;

	[Tooltip("Epsilon threshold to avoid frequent small updates.")]
	public float updateThreshold = 0.5f;
	[Tooltip("If true the script will automatically recompute on changes. If false, use 'Apply Width Now' from the context menu.")]
	public bool autoApply = false;

	[SerializeField]
	[Tooltip("Computed width sum (units)")]
	private float sum = 0f;
	[SerializeField, HideInInspector]
	private float lastApplied = -1f;

	void Reset()
	{
		if (targetBlock == null) targetBlock = GetComponent<UIBlock>();
		if (childrenParent == null && transform.childCount > 0) childrenParent = transform;
	}

	void OnValidate()
	{
		if (autoApply) ComputeAndApply();
	}

	void Update()
	{
		if (autoApply) ComputeAndApply();
	}

	void OnTransformChildrenChanged()
	{
		if (autoApply) ComputeAndApply();
	}

	[ContextMenu("Apply Width Now")]
	public void ApplyWidthNow()
	{
		ComputeAndApply();
	}

	private void ComputeAndApply()
	{
		if (targetBlock == null)
		{
			targetBlock = GetComponent<UIBlock>();
			if (targetBlock == null) return;
		}

		if (childrenParent == null) childrenParent = transform;

		sum = 0f;
		bool missingWidth = false;

		for (int i = 0; i < childrenParent.childCount; i++)
		{
			var child = childrenParent.GetChild(i);
			if (child == null) continue;

			// Prefer Nova UIBlock (gives calculated layout size)
			var ub = child.GetComponent<UIBlock>();
			if (ub != null)
			{
				try { ub.CalculateLayout(); } catch { }
				sum += ub.CalculatedSize.Value.y;
				continue;
			}

			// Fallback to renderer bounds if available
			var renderer = child.GetComponent<Renderer>();
			if (renderer != null)
			{
				sum += renderer.bounds.size.y;  
				continue;
			}

			// No reliable width for this child: abort and keep current target width unchanged
			missingWidth = true;
			break;
		}

		sum += extraPadding;

		if (missingWidth) return;

		if (Mathf.Abs(sum - lastApplied) <= updateThreshold) return;

		// Apply to targetBlock.Size.Value.x
		try
		{
			var size = targetBlock.Size.Value;
			size.y = sum;
			targetBlock.Size.Value = size;
			targetBlock.CalculateLayout();
		}
		catch
		{
			// ignore
		}

		lastApplied = sum;

#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			EditorUtility.SetDirty(targetBlock);
			EditorUtility.SetDirty(gameObject);
		}
#endif
	}
}


