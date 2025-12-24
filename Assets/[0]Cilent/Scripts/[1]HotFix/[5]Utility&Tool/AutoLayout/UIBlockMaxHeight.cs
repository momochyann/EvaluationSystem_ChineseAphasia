using System;
using UnityEngine;
using Nova;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class UIBlockMaxHeight : MonoBehaviour
{
	[Tooltip("Target Nova UIBlock (UIBlock2D). If null, the script will use the UIBlock on this GameObject.")]
	public UIBlock targetBlock;

	[Tooltip("Parent transform that contains the child elements to consider. If null, uses this GameObject.")]
	public Transform childrenParent;

	[Tooltip("If true the script will automatically recompute on changes. If false, use 'Apply Height Now' from the context menu.")]
	public bool autoApply = false;

	[Tooltip("If true and Specific Child is assigned, the target height will follow that child's height instead of the max of all children.")]
	public bool followSpecificChild = false;

	[Tooltip("Specific child to follow when Follow Specific Child is enabled.")]
	public Transform specificChild;

	[Tooltip("Extra padding (units) added to the computed height.")]
	public float extraPadding = 0f;

	[Tooltip("Epsilon threshold to avoid frequent small updates.")]
	public float updateThreshold = 0.5f;

	[SerializeField]
	[Tooltip("Computed max child height (units)")]
	private float maxHeight = 0f;

	[SerializeField, HideInInspector]
	private float lastApplied = -1f;
	private int lastChildCount = -1;
	private float[] lastChildHeights = null;
	private float lastSpecificHeight = float.NaN;

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
		// Always detect child changes and recompute when necessary
		if (childrenParent == null) childrenParent = transform;

		// If following a specific child, only monitor that child's height
		if (followSpecificChild && specificChild != null)
		{
			float h = GetChildHeightOrNaN(specificChild);
			if (!float.IsNaN(h) && (float.IsNaN(lastSpecificHeight) || Mathf.Abs(h - lastSpecificHeight) > updateThreshold))
			{
				ComputeAndApplySpecificChild();
			}
			return;
		}

		int count = childrenParent.childCount;
		if (count != lastChildCount)
		{
			ComputeAndApply();
			return;
		}

		// If we don't have cached heights yet, compute now
		if (lastChildHeights == null || lastChildHeights.Length != count)
		{
			ComputeAndApply();
			return;
		}

		// Check if any child's height changed
		bool anyChanged = false;
		for (int i = 0; i < count; i++)
		{
			var child = childrenParent.GetChild(i);
			if (child == null) continue;
			float h = GetChildHeightOrNaN(child);
			if (float.IsNaN(h))
			{
				// missing info -> abort detection and rely on ComputeAndApply when available
				anyChanged = false;
				break;
			}
			if (Mathf.Abs(h - lastChildHeights[i]) > updateThreshold)
			{
				anyChanged = true;
				break;
			}
		}

		if (anyChanged)
		{
			ComputeAndApply();
		}
	}

	void OnTransformChildrenChanged()
	{
		// Always respond to children added/removed
		if (followSpecificChild && specificChild != null)
		{
			ComputeAndApplySpecificChild();
		}
		else
		{
			ComputeAndApply();
		}
	}

	[ContextMenu("Apply Height Now")]
	public void ApplyHeightNow()
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

		maxHeight = 0f;
		bool missingHeight = false;
		int childCount = childrenParent.childCount;
		// ensure cache array
		if (lastChildHeights == null || lastChildHeights.Length != childCount)
		{
			lastChildHeights = new float[childCount];
		}

		for (int i = 0; i < childCount; i++)
		{
			var child = childrenParent.GetChild(i);
			if (child == null) continue;

			// Prefer Nova UIBlock (gives calculated layout size)
			var ub = child.GetComponent<UIBlock>();
			if (ub != null)
			{
				try { ub.CalculateLayout(); } catch { }
				float h = ub.CalculatedSize.Value.y;
				if (h > maxHeight) maxHeight = h;
				continue;
			}

			// Fallback to renderer bounds if available
			var renderer = child.GetComponent<Renderer>();
			if (renderer != null)
			{
				float h = renderer.bounds.size.y;
				if (h > maxHeight) maxHeight = h;
				lastChildHeights[i] = h;
				continue;
			}

			// Missing reliable height -> abort and keep current target height unchanged
			missingHeight = true;
			break;
		}

		if (missingHeight) return;

		maxHeight += extraPadding;

		if (Mathf.Abs(maxHeight - lastApplied) <= updateThreshold) return;

		// Apply to targetBlock.Size.Value.y
		try
		{
			var size = targetBlock.Size.Value;
			size.y = maxHeight;
			targetBlock.Size.Value = size;
			targetBlock.CalculateLayout();
		}
		catch
		{
			// ignore
		}

		lastApplied = maxHeight;
		// update child count and cache
		lastChildCount = childCount;
		for (int i = 0; i < childCount; i++)
		{
			var child = childrenParent.GetChild(i);
			if (child == null) lastChildHeights[i] = float.NaN;
			else lastChildHeights[i] = GetChildHeightOrNaN(child);
		}

#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			EditorUtility.SetDirty(targetBlock);
			EditorUtility.SetDirty(gameObject);
		}
#endif
	}
	/// <summary>
	/// Returns the child's height in world units or NaN if not available.
	/// </summary>
	private float GetChildHeightOrNaN(Transform child)
	{
		if (child == null) return float.NaN;

		var ub = child.GetComponent<UIBlock>();
		if (ub != null)
		{
			try { ub.CalculateLayout(); } catch { }
			return ub.CalculatedSize.Value.y;
		}

		var renderer = child.GetComponent<Renderer>();
		if (renderer != null)
		{
			return renderer.bounds.size.y;
		}

		return float.NaN;
	}
	// When following a specific child, compute using that child
	private void ComputeAndApplySpecificChild()
	{
		if (specificChild == null) return;
		float h = GetChildHeightOrNaN(specificChild);
		if (float.IsNaN(h)) return;

		maxHeight = h + extraPadding;
		if (Mathf.Abs(maxHeight - lastApplied) <= updateThreshold) return;

		try
		{
			var size = targetBlock.Size.Value;
			size.y = maxHeight;
			targetBlock.Size.Value = size;
			targetBlock.CalculateLayout();
		}
		catch { }

		lastApplied = maxHeight;
		lastSpecificHeight = h;
	}
}


