using UnityEngine;
using TMPro;
using Nova;

/// <summary>
/// Attach this to a GameObject and assign a `Nova.TextBlock` (or leave empty to auto-find).
/// It computes the number of visible lines in the `TextBlock`'s TMP component (accounts for wrapping)
/// and exposes the value in the Inspector in real time.
/// </summary>
public class LineCountTextBlock : MonoBehaviour
{
	[Header("Nova TextBlock Reference")]
	[Tooltip("Assign the Nova.TextBlock whose lines you want to count. If left empty, the script will attempt to find one on this GameObject or its children.")]
	public TextBlock textBlock;

	[Space]
	[Tooltip("实时计算出的文本显示行数（只读）")]
	[SerializeField]
	private int lineCount = 1;

	/// <summary>
	/// 公开只读行数给其它脚本使用。
	/// </summary>
	public int LineCount => lineCount;

	private string previousText = "";

	void Reset()
	{
		if (textBlock == null)
		{
			textBlock = GetComponent<TextBlock>() ?? GetComponentInChildren<TextBlock>();
		}
	}

	void Awake()
	{
		UpdateLineCountImmediate();
	}

	void Update()
	{
		if (textBlock == null || textBlock.TMP == null)
			return;

		string current = textBlock.Text ?? "";
		if (current != previousText)
		{
			previousText = current;
			UpdateLineCount();
		}
	}

	private void UpdateLineCountImmediate()
	{
		if (textBlock == null || textBlock.TMP == null)
			return;
		previousText = textBlock.Text ?? "";
		UpdateLineCount();
	}

	private void UpdateLineCount()
	{
		if (textBlock == null || textBlock.TMP == null)
		{
			lineCount = 0;
			return;
		}

		// Force the TMP component to update its mesh/text info so lineCount is accurate
		textBlock.TMP.ForceMeshUpdate();
		int count = 1;
		if (textBlock.TMP.textInfo != null)
		{
			count = textBlock.TMP.textInfo.lineCount;
		}
		lineCount = Mathf.Max(1, count);
	}
}


