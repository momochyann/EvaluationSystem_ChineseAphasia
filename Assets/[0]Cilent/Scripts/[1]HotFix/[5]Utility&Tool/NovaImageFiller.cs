using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[ExecuteAlways]
public class NovaImageFiller : MonoBehaviour
{
	// The clickable Nova UI button GameObject (can be any GameObject that receives pointer clicks)
	public GameObject novaButton;

	// The source image GameObject (expected to have a UnityEngine.UI.Image, but reflection fallbacks are attempted)
	public GameObject sourceImageObject;

	// The target UI block (UIBlock2D) which will receive the image/sprite/fill
	public GameObject targetBlock2D;

	// When true the inspector will hide the button GameObject
	public bool hideButton = false;

	[SerializeField, HideInInspector]
	private bool _isHooked = false;

	void OnValidate()
	{
		ApplyButtonVisibility();
		HookButton();
	}

	void OnEnable()
	{
		ApplyButtonVisibility();
		HookButton();
	}

	// Apply the inspector hide/show state to the assigned button
	public void ApplyButtonVisibility()
	{
		if (novaButton != null)
		{
			novaButton.SetActive(!hideButton);
		}
	}

	// External API so parent controllers can show/hide the button
	public void SetButtonVisible(bool visible)
	{
		hideButton = !visible;
		ApplyButtonVisibility();
	}

	// Ensure we get click callbacks regardless of which button implementation is used.
	private void HookButton()
	{
		if (novaButton == null) return;
		if (_isHooked) return;

		// Prefer Unity UI Button if present (adds redundancy)
		var uiButton = novaButton.GetComponent<UnityEngine.UI.Button>();
		if (uiButton != null)
		{
			uiButton.onClick.AddListener(OnButtonPressed);
			_isHooked = true;
			return;
		}

		// Otherwise add an EventTrigger entry for PointerClick so clicks are forwarded to this component
		var trigger = novaButton.GetComponent<EventTrigger>();
		if (trigger == null) trigger = novaButton.AddComponent<EventTrigger>();

		var entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback = new EventTrigger.TriggerEvent();
		UnityAction<BaseEventData> action = (data) => { OnButtonPressed(); };
		entry.callback.AddListener(action);
		trigger.triggers.Add(entry);

		_isHooked = true;
	}

	// Main action: copy source image into the target block when button is pressed
	public void OnButtonPressed()
	{
		if (sourceImageObject == null || targetBlock2D == null) return;

		// Try fast path: both source and target are UnityEngine.UI.Image
		var srcImg = sourceImageObject.GetComponent<Image>();
		var tgtImg = targetBlock2D.GetComponent<Image>();
		if (srcImg != null && tgtImg != null)
		{
			tgtImg.sprite = srcImg.sprite;
			// copy fill amount if applicable
			try { tgtImg.fillAmount = srcImg.fillAmount; } catch { }
			return;
		}

		// Fallback: copy sprite/texture via reflection from any component that exposes a 'sprite' or 'texture' property
		Sprite srcSprite = null;
		if (srcImg != null) srcSprite = srcImg.sprite;

		if (srcSprite != null)
		{
			var comps = targetBlock2D.GetComponents<Component>();
			foreach (var comp in comps)
			{
				if (comp == null) continue;
				var t = comp.GetType();
				// try property 'sprite' or 'Sprite'
				var prop = t.GetProperty("sprite");
				if (prop == null) prop = t.GetProperty("Sprite");
				if (prop != null && prop.PropertyType == typeof(Sprite))
				{
					try { prop.SetValue(comp, srcSprite); } catch { }
					return;
				}

				// try texture properties
				var texProp = t.GetProperty("texture");
				if (texProp != null && texProp.PropertyType == typeof(Texture))
				{
					try { texProp.SetValue(comp, srcSprite.texture); } catch { }
					return;
				}
			}
		}

		// If no sprite found, try to copy a raw Texture from a component named 'RawImage' or property 'texture'
		var rawSrc = sourceImageObject.GetComponent("RawImage");
		if (rawSrc != null)
		{
			var rawType = rawSrc.GetType();
			var texProp = rawType.GetProperty("texture");
			if (texProp != null)
			{
				var tex = texProp.GetValue(rawSrc) as Texture;
				if (tex != null)
				{
					var comps = targetBlock2D.GetComponents<Component>();
					foreach (var comp in comps)
					{
						if (comp == null) continue;
						var t = comp.GetType();
						var tp = t.GetProperty("texture");
						if (tp != null && tp.PropertyType == typeof(Texture))
						{
							try { tp.SetValue(comp, tex); } catch { }
							return;
						}
					}
				}
			}
		}
	}
}


