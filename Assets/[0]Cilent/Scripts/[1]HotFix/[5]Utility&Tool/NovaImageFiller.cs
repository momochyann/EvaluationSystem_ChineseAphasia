using System;
using UnityEngine;


using Nova;
using NovaSamples.UIControls;

[ExecuteAlways]
public class NovaImageFiller : MonoBehaviour
{
	// The clickable Nova UI button GameObject (can be any GameObject that receives pointer clicks)
	public GameObject novaButton;

	// The source sprite (directly assigned)
	public Sprite sourceSprite;

	// The target UI block (UIBlock2D) which will receive the image/sprite/fill
	public GameObject targetBlock2D;

	// When true the inspector will hide the button GameObject
	[HideInInspector]
	public bool hideButton = false;
	public bool hideButtonOnStart = false;

	private NovaSamples.UIControls.Button _novaButton;
	private void Awake()
	{
		if (hideButtonOnStart)
		{
			novaButton = null;
			gameObject.SetActive(false);
		}
		// if (targetBlock2D == null)
		// {
		// 	targetBlock2D = transform.parent.parent.parent.parent.parent.parent.parent.GetChild(1).gameObject;
		// }
	}
	private void Start()
	{
		//HookButton();

		this.GetComponent<Button>().OnClicked.AddListener(OnButtonPressed);
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




	// Main action: copy source sprite into the target UIBlock2D when button is pressed
	public void OnButtonPressed()
	{
		Debug.Log("NovaImageFiller: OnButtonPressed called");

		if (sourceSprite == null)
		{
			Debug.LogWarning("NovaImageFiller: sourceSprite is null");
			return;
		}

		if (targetBlock2D == null)
		{
			Debug.LogWarning("NovaImageFiller: targetBlock2D is null");
			return;
		}

		var uiBlock2D = targetBlock2D.transform.GetChild(1).GetComponent<Nova.UIBlock2D>();
		if (uiBlock2D == null)
		{
			Debug.LogWarning("NovaImageFiller: No UIBlock2D component found on targetBlock2D");
			return;
		}

		// 先显示目标对象
		targetBlock2D.SetActive(true);
		Debug.Log("NovaImageFiller: targetBlock2D set to active");

		// 然后替换图片
		uiBlock2D.SetImage(sourceSprite);
		Debug.Log("NovaImageFiller: Sprite set successfully to UIBlock2D");
	}
}


