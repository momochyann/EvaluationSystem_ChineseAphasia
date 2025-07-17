using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Nova;
using NovaSamples.UIControls;
using UnityEngine.Events;

public class NovaUItest : MonoBehaviour
{
    [Header("NovaUI 组件示例")]
    [Space(10)]
    [TextArea(3, 5)]
    public string instructions = "这是一个 NovaUI 组件使用示例。请根据需要在场景中创建对应的 UI 组件，并将它们赋值给下面的字段。";
    
    [Header("Button 示例")]
    public UIBlock2D buttonBackground;
    public TextBlock buttonLabel;
    public Button buttonComponent;
    
    [Header("TextBlock 示例")]
    public TextBlock titleText;
    public TextBlock descriptionText;
    
    [Header("Image 示例")]
    public UIBlock2D imageContainer;
    public Sprite sampleSprite;
    public Texture2D sampleTexture;
    
    [Header("InputField 示例")]
    public TextField inputField;
    public UIBlock2D inputBackground;
    public TextBlock inputPlaceholder;
    
    [Header("UI 根容器")]
    public UIBlock rootContainer;

    void Start()
    {
        Debug.Log("NovaUI 测试示例启动");
        
        SetupButtonExample();
        // SetupTextExample();
        // SetupImageExample();
        // SetupInputFieldExample();
        
        ShowInstructions();
    }

    void Update()
    {
        
    }
    
    /// <summary>
    /// 显示使用说明
    /// </summary>
    private void ShowInstructions()
    {
        Debug.Log("=== NovaUI 使用示例说明 ===");
        Debug.Log("1. Button 示例：需要创建 UIBlock2D（背景）+ TextBlock（文本）+ Button（组件）");
        Debug.Log("2. Text 示例：需要创建 TextBlock 组件");
        Debug.Log("3. Image 示例：需要创建 UIBlock2D 组件并设置图片");
        Debug.Log("4. InputField 示例：需要创建 TextField + UIBlock2D（背景）+ TextBlock（占位符）");
        Debug.Log("5. 请确保已正确引用 Nova 命名空间后再使用具体的实现方法");
    }
    
    /// <summary>
    /// 设置 Button 示例
    /// </summary>
    private void SetupButtonExample()
    {
        if (buttonComponent == null || buttonBackground == null || buttonLabel == null)
            return;
            
        // 设置按钮背景样式
        buttonBackground.Color = new Color(0.2f, 0.6f, 1f, 1f); // 蓝色背景
        buttonBackground.CornerRadius = 10f; // 圆角
        buttonBackground.Size = new Length3(200f, 50f, 0f); // 宽200, 高50
        
        // 设置按钮文本
        buttonLabel.Text = "点击我";
        buttonLabel.Color = Color.white;
        buttonLabel.TMP.fontSize = 18f;
        buttonLabel.TMP.alignment = TMPro.TextAlignmentOptions.Center;
        
        // 添加点击事件
        buttonComponent.OnClicked.AddListener(() => {
            Debug.Log("按钮被点击了！");
            // 可以在这里添加按钮点击的逻辑
            OnButtonClicked();
        });
    }
    
    /// <summary>
    /// 设置 Text 示例
    /// </summary>
    private void SetupTextExample()
    {
        // 设置标题文本
        if (titleText != null)
        {
            titleText.Text = "NovaUI 示例标题";
            titleText.Color = new Color(0.1f, 0.1f, 0.1f, 1f); // 深色文本
            titleText.TMP.fontSize = 24f;
            titleText.TMP.fontStyle = TMPro.FontStyles.Bold;
            titleText.TMP.alignment = TMPro.TextAlignmentOptions.Center;
            titleText.Size = new Length3(400f, 40f, 0f);
        }
        
        // 设置描述文本
        if (descriptionText != null)
        {
            descriptionText.Text = "这是一个使用 NovaUI 架构的示例。NovaUI 提供了强大的 UI 渲染能力，支持高级样式和布局。";
            descriptionText.Color = new Color(0.3f, 0.3f, 0.3f, 1f); // 灰色文本
            descriptionText.TMP.fontSize = 14f;
            descriptionText.TMP.alignment = TMPro.TextAlignmentOptions.Left;
            descriptionText.TMP.enableWordWrapping = true; // 启用文本换行
            descriptionText.Size = new Length3(400f, 80f, 0f);
        }
    }
    
    /// <summary>
    /// 设置 Image 示例
    /// </summary>
    private void SetupImageExample()
    {
        if (imageContainer == null)
            return;
            
        // 设置图片容器样式
        imageContainer.Size = new Length3(150f, 150f, 0f); // 150x150 的正方形
        imageContainer.CornerRadius = 15f; // 圆角
        
        // 如果有 Sprite，使用 Sprite
        if (sampleSprite != null)
        {
            imageContainer.SetImage(sampleSprite);
            imageContainer.ImageAdjustment.ScaleMode = ImageScaleMode.Fit; // 适配模式
        }
        // 否则如果有 Texture，使用 Texture
        else if (sampleTexture != null)
        {
            imageContainer.SetImage(sampleTexture);
            imageContainer.ImageAdjustment.ScaleMode = ImageScaleMode.Fit;
        }
        else
        {
            // 如果没有图片，显示占位符颜色
            imageContainer.Color = new Color(0.8f, 0.8f, 0.8f, 1f); // 浅灰色占位符
        }
        
        // 添加边框效果
        imageContainer.Border = new Border()
        {
            Width = 2f,
            Color = new Color(0.6f, 0.6f, 0.6f, 1f) // 灰色边框
        };
    }
    
    /// <summary>
    /// 设置 InputField 示例
    /// </summary>
    private void SetupInputFieldExample()
    {
        if (inputField == null || inputBackground == null)
            return;
            
        // 设置输入框背景
        inputBackground.Color = Color.white;
        inputBackground.CornerRadius = 8f;
        inputBackground.Size = new Length3(300f, 40f, 0f);
        
        // 添加边框
        inputBackground.Border = new Border()
        {
            Width = 1f,
            Color = new Color(0.7f, 0.7f, 0.7f, 1f) // 灰色边框
        };
        
        // 设置占位符文本
        if (inputPlaceholder != null)
        {
            inputPlaceholder.Text = "请输入文本...";
            inputPlaceholder.Color = new Color(0.6f, 0.6f, 0.6f, 1f); // 灰色占位符
            inputPlaceholder.TMP.fontSize = 14f;
        }
        
        // 添加输入事件监听
        inputField.OnTextChanged += OnInputFieldTextChanged;
        
        // 设置初始文本
        inputField.Text = "";
    }
    
    /// <summary>
    /// 按钮点击事件处理
    /// </summary>
    private void OnButtonClicked()
    {
        // 改变按钮颜色作为反馈
        if (buttonBackground != null)
        {
            buttonBackground.Color = new Color(0.1f, 0.8f, 0.3f, 1f); // 绿色
            
            // 0.5秒后恢复原色
            StartCoroutine(ResetButtonColor());
        }
        
        // 更新文本内容
        if (titleText != null)
        {
            titleText.Text = "按钮已被点击！";
        }
    }
    
    /// <summary>
    /// 输入框文本改变事件处理
    /// </summary>
    private void OnInputFieldTextChanged()
    {
        if (inputField != null && descriptionText != null)
        {
            descriptionText.Text = $"当前输入内容：{inputField.Text}";
        }
    }
    
    /// <summary>
    /// 重置按钮颜色
    /// </summary>
    private IEnumerator ResetButtonColor()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (buttonBackground != null)
        {
            buttonBackground.Color = new Color(0.2f, 0.6f, 1f, 1f); // 恢复蓝色
        }
        
        if (titleText != null)
        {
            titleText.Text = "NovaUI 示例标题";
        }
    }
    
    /// <summary>
    /// 创建动态 UI 元素的示例方法
    /// </summary>
    public void CreateDynamicButton(Vector3 position, string text, UnityAction onClick)
    {
        // 创建新的 GameObject
        GameObject buttonGO = new GameObject("DynamicButton");
        buttonGO.transform.SetParent(rootContainer.transform);
        buttonGO.transform.localPosition = position;
        
        // 添加 UIBlock2D 作为背景
        UIBlock2D background = buttonGO.AddComponent<UIBlock2D>();
        background.Color = new Color(1f, 0.5f, 0.2f, 1f); // 橙色
        background.CornerRadius = 12f; 
        background.Size = new Length3(180f, 45f, 0f);
        
        // 创建文本子对象
        GameObject textGO = new GameObject("Label");
        textGO.transform.SetParent(buttonGO.transform);
        textGO.transform.localPosition = Vector3.zero;
        
        TextBlock label = textGO.AddComponent<TextBlock>();
        label.Text = text;
        label.Color = Color.white;
        label.TMP.fontSize = 16f;
        label.TMP.alignment = TMPro.TextAlignmentOptions.Center;
        
        // 添加 Button 组件
        Button button = buttonGO.AddComponent<Button>();
        button.OnClicked.AddListener(onClick);
        
        Debug.Log($"动态创建了按钮：{text}");
    }
    
    /// <summary>
    /// 创建动态图片的示例方法
    /// </summary>
    public void CreateDynamicImage(Vector3 position, Sprite sprite)
    {
        GameObject imageGO = new GameObject("DynamicImage");
        imageGO.transform.SetParent(rootContainer.transform);
        imageGO.transform.localPosition = position;
        
        UIBlock2D image = imageGO.AddComponent<UIBlock2D>();
        image.SetImage(sprite);
        image.Size = new Length3(100f, 100f, 0f);
        image.CornerRadius = 10f;
        image.ImageAdjustment.ScaleMode = ImageScaleMode.Envelope;
        
        Debug.Log("动态创建了图片");
    }
    
    /// <summary>
    /// 创建动态文本的示例方法
    /// </summary>
    public void CreateDynamicText(Vector3 position, string text, Color color, float fontSize = 16f)
    {
        GameObject textGO = new GameObject("DynamicText");
        textGO.transform.SetParent(rootContainer.transform);
        textGO.transform.localPosition = position;
        
        TextBlock textBlock = textGO.AddComponent<TextBlock>();
        textBlock.Text = text;
        textBlock.Color = color;
        textBlock.TMP.fontSize = fontSize;
        textBlock.TMP.alignment = TMPro.TextAlignmentOptions.Center;
        textBlock.Size = new Length3(200f, 50f, 0f);
        
        Debug.Log($"动态创建了文本：{text}");
    }
    
    /// <summary>
    /// 创建带背景的动态输入框
    /// </summary>
    public void CreateDynamicInputField(Vector3 position, string placeholder = "输入文本...")
    {
        // 创建输入框容器
        GameObject containerGO = new GameObject("DynamicInputField");
        containerGO.transform.SetParent(rootContainer.transform);
        containerGO.transform.localPosition = position;
        
        // 添加背景
        UIBlock2D background = containerGO.AddComponent<UIBlock2D>();
        background.Color = Color.white;
        background.CornerRadius = 8f;
        background.Size = new Length3(250f, 35f, 0f);
        background.Border = new Border()
        {
            Width = 1f,
            Color = new Color(0.7f, 0.7f, 0.7f, 1f)
        };
        
        // 添加 TextField 组件
        TextField textField = containerGO.AddComponent<TextField>();
        textField.Text = "";
        
        // 创建占位符文本
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(containerGO.transform);
        placeholderGO.transform.localPosition = Vector3.zero;
        
        TextBlock placeholderText = placeholderGO.AddComponent<TextBlock>();
        placeholderText.Text = placeholder;
        placeholderText.Color = new Color(0.6f, 0.6f, 0.6f, 1f);
        placeholderText.TMP.fontSize = 14f;
        
        // 设置输入事件
        textField.OnTextChanged += () => {
            placeholderText.Visible = string.IsNullOrEmpty(textField.Text);
            Debug.Log($"输入框文本变化：{textField.Text}");
        };
        
        Debug.Log("动态创建了输入框");
    }
    
    // 在 Inspector 中可以调用的测试方法
    [ContextMenu("测试动态创建按钮")]
    public void TestCreateDynamicButton()
    {
        CreateDynamicButton(new Vector3(0, -100, 0), "动态按钮", () => {
            Debug.Log("动态按钮被点击了！");
        });
    }
    
    [ContextMenu("测试动态创建图片")]
    public void TestCreateDynamicImage()
    {
        if (sampleSprite != null)
        {
            CreateDynamicImage(new Vector3(200, 0, 0), sampleSprite);
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中设置 sampleSprite");
        }
    }
    
    [ContextMenu("测试动态创建文本")]
    public void TestCreateDynamicText()
    {
        CreateDynamicText(new Vector3(-200, 0, 0), "动态文本", Color.red, 20f);
    }
    
    [ContextMenu("测试动态创建输入框")]
    public void TestCreateDynamicInputField()
    {
        CreateDynamicInputField(new Vector3(0, 50, 0), "请输入内容...");
    }
    
    /// <summary>
    /// 高级样式示例：创建带阴影和渐变的按钮
    /// </summary>
    [ContextMenu("创建高级样式按钮")]
    public void CreateAdvancedStyledButton()
    {
        GameObject buttonGO = new GameObject("AdvancedStyledButton");
        buttonGO.transform.SetParent(rootContainer.transform);
        buttonGO.transform.localPosition = new Vector3(0, -200, 0);
        
        UIBlock2D background = buttonGO.AddComponent<UIBlock2D>();
        
        // 设置基础样式
        background.Size = new Length3(220f, 60f, 0f);
        background.CornerRadius = 15f;
        background.Color = new Color(0.3f, 0.7f, 1f, 1f);
        
        // 添加渐变效果
        background.Gradient = new RadialGradient()
        {
            Enabled = true,
            Color = new Color(0.8f, 0.9f, 1f, 1f),
            Center = new Vector2(0, 0.3f),
            //Radius = 0.8f
        };
        
        // 添加阴影效果
        background.Shadow = new Shadow()
        {
            Enabled = true,
            Color = new Color(0, 0, 0, 0.3f),
            Offset = Length2.FixedValue(0, -3f),
            Blur = 8f,
            Width = 2f
        };
        
        // 添加边框
        background.Border = new Border()
        {
            Width = 2f,
            Color = new Color(0.2f, 0.5f, 0.8f, 1f)
        };
        
        // 创建文本
        GameObject textGO = new GameObject("Label");
        textGO.transform.SetParent(buttonGO.transform);
        textGO.transform.localPosition = Vector3.zero;
        
        TextBlock label = textGO.AddComponent<TextBlock>();
        label.Text = "高级样式按钮";
        label.Color = Color.white;
        label.TMP.fontSize = 18f;
        label.TMP.fontStyle = TMPro.FontStyles.Bold;
        label.TMP.alignment = TMPro.TextAlignmentOptions.Center;
        
        // 添加按钮功能
        Button button = buttonGO.AddComponent<Button>();
        button.OnClicked.AddListener(() => {
            Debug.Log("高级样式按钮被点击了！");
            
            // 点击动画效果
            StartCoroutine(ButtonClickAnimation(background));
        });
        
        Debug.Log("创建了高级样式按钮");
    }
    
    /// <summary>
    /// 按钮点击动画效果
    /// </summary>
    private IEnumerator ButtonClickAnimation(UIBlock2D button)
    {
        Vector3 originalScale = button.transform.localScale;
        
        // 缩小效果
        button.transform.localScale = originalScale * 0.95f;
        yield return new WaitForSeconds(0.1f);
        
        // 恢复原大小
        button.transform.localScale = originalScale;
    }
}
