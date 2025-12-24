using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class NameSyncText : MonoBehaviour
{
	[SerializeField, HideInInspector]
	private string _lastSyncedName;

	void OnValidate()
	{
		SyncNameToText();
	}

	void Update()
	{
		// Only run in edit mode per user's request (不在运行时同步)
		if (Application.isPlaying) return;
		SyncNameToText();
	}

	private void SyncNameToText()
	{
		var currentName = gameObject.name;
		if (currentName == _lastSyncedName) return;

		// 1) Try UnityEngine.UI.Text
		var uiText = GetComponent<Text>();
		if (uiText != null)
		{
			if (uiText.text != currentName) uiText.text = currentName;
			uiText.alignment = TextAnchor.MiddleCenter;
			_lastSyncedName = currentName;
#if UNITY_EDITOR
			// Ensure editor updates the serialized UI.Text after change
			if (!Application.isPlaying) EditorUtility.SetDirty(uiText);
#endif
			return;
		}

		// 2) Try TextMeshProUGUI via reflection so we don't require a compile-time dependency
		var components = GetComponents<Component>();
		foreach (var comp in components)
		{
			var t = comp.GetType();
			if (t.Name == "TextMeshProUGUI")
			{
				// set text property
				var textProp = t.GetProperty("text");
				if (textProp != null) textProp.SetValue(comp, currentName);

				// try to set alignment to Center (uses TMPro.TextAlignmentOptions.Center)
				var alignProp = t.GetProperty("alignment");
				if (alignProp != null)
				{
					var enumType = alignProp.PropertyType;
					try
					{
						// "Center" is the enum name used by TextAlignmentOptions for center alignment
						var centerVal = Enum.Parse(enumType, "Center");
						alignProp.SetValue(comp, centerVal);
					}
					catch
					{
						// ignore if parsing fails
					}
				}

				// Try to call TMP refresh methods to force visual update in editor
				var setAllDirtyMethod = t.GetMethod("SetAllDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				if (setAllDirtyMethod != null)
				{
					try { setAllDirtyMethod.Invoke(comp, null); } catch { }
				}
				else
				{
					var setVerticesDirty = t.GetMethod("SetVerticesDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					if (setVerticesDirty != null)
					{
						try { setVerticesDirty.Invoke(comp, null); } catch { }
					}
				}

#if UNITY_EDITOR
				// Ensure editor updates the serialized TMP object after change
				if (!Application.isPlaying) EditorUtility.SetDirty(comp);
#endif

				_lastSyncedName = currentName;
				return;
			}
			// Support Nova's TextBlock (Nova.TextBlock)
			if (t.Name == "TextBlock")
			{
				// set Text property
				var textProp = t.GetProperty("Text");
				if (textProp != null) textProp.SetValue(comp, currentName);

				// Try to access TMP property on TextBlock and update alignment / refresh
				var tmpProp = t.GetProperty("TMP");
				if (tmpProp != null)
				{
					var tmpObj = tmpProp.GetValue(comp);
					if (tmpObj != null)
					{
						// try set alignment on TMP (enum name "Center")
						var tmpType = tmpObj.GetType();
						var alignPropOnTmp = tmpType.GetProperty("alignment");
						if (alignPropOnTmp != null)
						{
							var enumType = alignPropOnTmp.PropertyType;
							try
							{
								var centerVal = Enum.Parse(enumType, "Center");
								alignPropOnTmp.SetValue(tmpObj, centerVal);
							}
							catch { }
						}

						// try calling rebuild/force update methods on TMP
						var rebuildMethod = tmpType.GetMethod("Rebuild");
						if (rebuildMethod != null)
						{
							try { rebuildMethod.Invoke(tmpObj, new object[] { UnityEngine.UI.CanvasUpdate.PreRender }); } catch { }
						}
						else
						{
							var forceMethod = tmpType.GetMethod("ForceMeshUpdate");
							if (forceMethod != null)
							{
								try { forceMethod.Invoke(tmpObj, null); } catch { }
							}
						}

#if UNITY_EDITOR
						if (!Application.isPlaying) EditorUtility.SetDirty(tmpObj as UnityEngine.Object);
#endif
					}
				}

#if UNITY_EDITOR
				// Try invoke internal EditorOnly_MarkDirty on TextBlock to notify Nova systems
				var markDirtyMethod = t.GetMethod("EditorOnly_MarkDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (markDirtyMethod != null)
				{
					try { markDirtyMethod.Invoke(comp, null); } catch { }
				}

				if (!Application.isPlaying) EditorUtility.SetDirty(comp);
#endif

				_lastSyncedName = currentName;
				return;
			}
		}

		// No supported text component found; still remember name to avoid repeated work
		_lastSyncedName = currentName;
	}
}


