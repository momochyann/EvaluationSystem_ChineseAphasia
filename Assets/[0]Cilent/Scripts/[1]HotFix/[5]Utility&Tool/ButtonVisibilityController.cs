using System;
using UnityEngine;

[ExecuteAlways]
public class ButtonVisibilityController : MonoBehaviour
{
	// When true, all child NovaImageFiller button(s) will be hidden
	public bool hideButtons = false;

	void OnValidate()
	{
		ApplyToChildren();
	}

	void Start()
	{
		ApplyToChildren();
	}

	// Apply the visibility to all child NovaImageFiller components (includes inactive children)
	public void ApplyToChildren()
	{
		var fillers = GetComponentsInChildren<NovaImageFiller>(true);
		foreach (var f in fillers)
		{
			if (f == null) continue;
			f.SetButtonVisible(!hideButtons);
		}
	}
}



