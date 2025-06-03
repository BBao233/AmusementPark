using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class FemaleSwitchController : MonoBehaviour
{
    [Header("目标瓦片地图")]
    public Tilemap[] targetTilemaps;  // 可控制多个瓦片地图
    public bool showOnInteract = true;  // 交互时显示还是隐藏

    [Header("新增：目标GameObjects")]
    public GameObject[] targetGameObjects;  // 新增：可控制的GameObjects

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.L;
    public float interactionRadius = 1f;

    [Header("新增：交互提示UI")]
    public bool createPromptUI = true;  // 是否自动创建提示UI
    public string promptText = "按L开启";  // 提示文本内容
    public Color promptColor = Color.white;  // 文本颜色
    public float promptOffsetY = 0.5f;  // 文本Y轴偏移量
    public float promptFontSize = 16f;  // 文本字体大小
    public bool alwaysShowPrompt = true;  // 新增：是否始终显示提示

    [Header("字体设置")]
    public Font customFont; // 新增：自定义字体字段

    [Header("新增：开关图像设置")]
    public Sprite activeSprite;         // 激活状态的精灵图像
    public Sprite inactiveSprite;       // 未激活状态的精灵图像
    public bool scaleOnActivate = true; // 是否在激活时缩放
    public Vector3 activationScale = new Vector3(1f, 1f, 1f); // 激活时的缩放比例
    public float scaleDuration = 0f;  // 缩放动画持续时间

    [Header("新增：渲染设置")]
    public string playerSortingLayer = "Player"; // 玩家的渲染层级
    public int playerOrderInLayer = 10;          // 玩家的层级顺序
    public bool alwaysRenderBehindPlayer = true; // 是否始终渲染在玩家后方

    [Header("音效设置")]
    public AudioClip interactionSound;  // 交互音效
    [Range(0f, 1f)] public float volume = 1f;  // 音量控制滑块
    private AudioSource audioSource;    // 音频源组件

    [Header("新增：对话框设置")]
    public bool showDialogOnInteract = true; // 是否在交互时显示对话框
    public string dialogTitle = "提示"; // 对话框标题
    [TextArea(3, 10)] public string dialogContent = "操作成功！"; // 对话框内容
    public float dialogDisplayTime = 3f; // 对话框显示时间（秒），0表示不自动关闭
    public Color dialogBackgroundColor = new Color(0, 0, 0, 0.8f); // 对话框背景颜色
    public Color dialogTitleColor = Color.white; // 标题颜色
    public Color dialogContentColor = Color.white; // 内容颜色
    public float dialogWidth = 350f; // 对话框宽度
    public float dialogHeight = 200f; // 对话框高度
    public float dialogOffsetY = 1.5f; // 对话框Y轴偏移量

    [Header("雨系统设置")]
    public RainSystem rainSystem; // 拖入场景中的雨系统对象
    public float rainDuration = 5f; // 下雨持续时间，可调整

    [Header("对话框按钮设置")]
    public string button1Text = "选项一";
    public string button2Text = "选项二";
    public int correctButtonIndex = 1; // 1=按钮1正确, 2=按钮2正确
    public UnityEvent onCorrectChoice; // 选择正确选项时触发的事件
    public UnityEvent onIncorrectChoice; // 选择错误选项时触发的事件

    private GameObject interactionPrompt;  // 交互提示UI
    private GameObject dialogUI; // 对话框UI
    private Text dialogTitleText; // 对话框标题文本
    private Text dialogContentText; // 对话框内容文本
    private Button button1; // 对话框按钮1
    private Button button2; // 对话框按钮2
    private bool playerInRange;
    private bool mapsActivated = false;   // 记录地图是否已激活
    private SpriteRenderer switchRenderer; // 开关的精灵渲染器
    private Vector3 originalScale;         // 原始缩放比例
    private bool isScaling = false;        // 是否正在缩放
    private float dialogDisplayTimer = 0f; // 对话框显示计时器
    private bool dialogActive = false; // 对话框是否激活


    void Start()
    {
        // 获取精灵渲染器
        switchRenderer = GetComponent<SpriteRenderer>();

        // 确保开关渲染在玩家后方
        SetupRendererOrder();

        // 保存原始缩放比例
        originalScale = transform.localScale;

        // 初始化开关图像
        UpdateSwitchSprite();

        // 初始化：默认隐藏所有目标瓦片地图和GameObjects
        ToggleTilemaps(false);
        ToggleGameObjects(false);

        // 新增：创建交互提示UI
        if (createPromptUI)
            CreateInteractionPrompt();

        // 如果有交互提示UI，根据设置决定是否显示
        UpdatePromptVisibility();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        Debug.Log($"开关 {gameObject.name} 初始化完成");
    }

    // 新增：设置渲染顺序
    void SetupRendererOrder()
    {
        if (switchRenderer == null || !alwaysRenderBehindPlayer) return;

        // 设置开关的渲染层级和顺序，确保在玩家后方
        switchRenderer.sortingLayerName = playerSortingLayer;
        switchRenderer.sortingOrder = playerOrderInLayer - 1;
    }

    void Update()
    {
        // 检测玩家是否在交互范围内
        CheckPlayerInRange();

        // 更新提示UI状态
        UpdatePromptVisibility();

        // 处理交互逻辑
        if (playerInRange && Input.GetKeyDown(interactKey) && !mapsActivated)
        {
            ActivateTilemaps();
        }

        // 更新缩放动画
        UpdateScaling();

        // 更新对话框计时器
        UpdateDialogTimer();
    }

    void CheckPlayerInRange()
    {
        // 使用LayerMask进行更精确的检测（可选）
        int layerMask = LayerMask.GetMask("Player"); // 假设"Female"角色在"Player"层

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius,
            layerMask
        );

        playerInRange = false;
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Female"))
            {
                playerInRange = true;
                break;
            }
        }
    }

    void UpdatePromptVisibility()
    {
        if (interactionPrompt == null) return;

        // 修改：只有当玩家在范围内且未激活时才显示提示
        bool shouldShow = !mapsActivated && playerInRange;

        if (interactionPrompt.activeSelf != shouldShow)
        {
            interactionPrompt.SetActive(shouldShow);
            Debug.Log($"提示UI状态更新为: {shouldShow}");
        }
    }

    void ActivateTilemaps()
    {
        mapsActivated = true;
        ToggleTilemaps(showOnInteract);
        ToggleGameObjects(showOnInteract);  // 新增：控制GameObjects

        // 更新开关图像
        UpdateSwitchSprite();

        // 开始缩放动画
        if (scaleOnActivate)
            StartScaling();

        // 隐藏提示UI
        UpdatePromptVisibility();

        // 显示对话框
        if (showDialogOnInteract)
        {
            ShowDialog();
        }

        if (interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, volume);
        }

        // 显示对话框
        if (showDialogOnInteract && !dialogActive)
        {
            ShowDialog();
        }

        Debug.Log("瓦片地图和GameObjects已激活");
    }

    void ToggleTilemaps(bool active)
    {
        foreach (var tilemap in targetTilemaps)
        {
            if (tilemap != null)
            {
                tilemap.gameObject.SetActive(active);
            }
        }
    }

    // 新增：控制GameObjects的方法
    void ToggleGameObjects(bool active)
    {
        foreach (var gameObject in targetGameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(active);
            }
        }
    }

    // 新增：更新开关图像的方法
    void UpdateSwitchSprite()
    {
        if (switchRenderer == null)
        {
            Debug.LogWarning("未找到开关的SpriteRenderer组件");
            return;
        }

        // 根据激活状态设置精灵图像
        if (mapsActivated && activeSprite != null)
        {
            switchRenderer.sprite = activeSprite;
        }
        else if (!mapsActivated && inactiveSprite != null)
        {
            switchRenderer.sprite = inactiveSprite;
        }
    }

    // 新增：开始缩放动画
    void StartScaling()
    {
        isScaling = true;
        transform.localScale = originalScale;
    }

    // 新增：更新缩放动画
    void UpdateScaling()
    {
        if (!isScaling) return;

        // 使用平滑插值实现缩放动画
        float progress = Mathf.Min(Time.deltaTime / scaleDuration, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, activationScale, progress);

        // 缩放完成后恢复原始大小
        if (Vector3.Distance(transform.localScale, activationScale) < 0.01f)
        {
            isScaling = false;
            transform.localScale = originalScale;
        }
    }

    // 新增：创建交互提示UI的方法
    void CreateInteractionPrompt()
    {
        Debug.Log("开始创建交互提示UI");

        // 创建Canvas
        GameObject canvasObject = GameObject.Find("MainCanvas");
        if (canvasObject == null)
        {
            Debug.Log("未找到MainCanvas，创建新的Canvas");
            canvasObject = new GameObject("MainCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            Debug.Log("新Canvas创建完成");
        }
        else
        {
            Debug.Log("找到现有MainCanvas");
        }

        // 创建提示文本
        interactionPrompt = new GameObject("InteractionPrompt");
        interactionPrompt.transform.SetParent(canvasObject.transform);

        Text textComponent = interactionPrompt.AddComponent<Text>();
        textComponent.text = promptText;

        // 使用指定的自定义字体
        if (customFont != null)
        {
            textComponent.font = customFont;
            Debug.Log($"使用自定义字体: {customFont.name}");
        }
        else
        {
            Debug.LogWarning("未指定自定义字体，使用默认字体");
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        textComponent.fontSize = (int)promptFontSize;
        textComponent.color = promptColor;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // 设置RectTransform
        RectTransform rectTransform = interactionPrompt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);

        // 更新位置
        UpdatePromptPosition();

        Debug.Log("交互提示UI创建完成");
    }

    // 新增：更新提示位置的方法
    void UpdatePromptPosition()
    {
        if (interactionPrompt == null)
        {
            Debug.LogWarning("尝试更新提示位置，但UI对象为空");
            return;
        }

        // 确保相机存在
        if (Camera.main == null)
        {
            Debug.LogError("找不到主相机，无法更新提示位置");
            return;
        }

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // 检查坐标是否在屏幕范围内
        bool isOnScreen = screenPosition.z > 0 &&
                          screenPosition.x > 0 && screenPosition.x < Screen.width &&
                          screenPosition.y > 0 && screenPosition.y < Screen.height;

        Debug.Log($"提示位置更新: 屏幕坐标=({screenPosition.x}, {screenPosition.y}, {screenPosition.z}), 是否在屏幕上={isOnScreen}");

        // 更新UI位置
        RectTransform rectTransform = interactionPrompt.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
    }

    // 新增：显示对话框
    void ShowDialog()
    {
        Debug.Log("显示对话框");
        dialogActive = true;

        // 如果对话框已存在，先销毁
        if (dialogUI != null)
        {
            Destroy(dialogUI);
        }

        // 创建Canvas
        GameObject canvasObject = GameObject.Find("MainCanvas");
        if (canvasObject == null)
        {
            Debug.Log("未找到MainCanvas，创建新的Canvas");
            canvasObject = new GameObject("MainCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            Debug.Log("新Canvas创建完成");
        }
        else
        {
            Debug.Log("找到现有MainCanvas");
        }

        // 创建对话框根对象
        dialogUI = new GameObject("DialogUI");
        dialogUI.transform.SetParent(canvasObject.transform);

        // 创建背景图像
        Image backgroundImage = dialogUI.AddComponent<Image>();
        backgroundImage.color = dialogBackgroundColor;

        // 设置RectTransform
        RectTransform dialogRect = dialogUI.GetComponent<RectTransform>();
        dialogRect.sizeDelta = new Vector2(dialogWidth, dialogHeight);

        // 创建标题文本
        GameObject titleObject = new GameObject("DialogTitle");
        titleObject.transform.SetParent(dialogUI.transform);
        dialogTitleText = titleObject.AddComponent<Text>();
        dialogTitleText.text = dialogTitle;

        // 使用指定的自定义字体
        if (customFont != null)
        {
            dialogTitleText.font = customFont;
        }
        else
        {
            dialogTitleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        dialogTitleText.fontSize = (int)(promptFontSize * 1.2f); // 标题字体稍大
        dialogTitleText.color = dialogTitleColor;
        dialogTitleText.alignment = TextAnchor.MiddleCenter;

        // 设置标题RectTransform
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // 创建内容文本
        GameObject contentObject = new GameObject("DialogContent");
        contentObject.transform.SetParent(dialogUI.transform);
        dialogContentText = contentObject.AddComponent<Text>();
        dialogContentText.text = dialogContent;

        // 使用指定的自定义字体
        if (customFont != null)
        {
            dialogContentText.font = customFont;
        }
        else
        {
            dialogContentText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        dialogContentText.fontSize = (int)promptFontSize;
        dialogContentText.color = dialogContentColor;
        dialogContentText.alignment = TextAnchor.MiddleCenter;
        dialogContentText.horizontalOverflow = HorizontalWrapMode.Wrap;
        dialogContentText.verticalOverflow = VerticalWrapMode.Truncate;

        // 设置内容RectTransform
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.05f, 0.4f);
        contentRect.anchorMax = new Vector2(0.95f, 0.75f);
        contentRect.offsetMin = new Vector2(10, 10);
        contentRect.offsetMax = new Vector2(-10, -10);

        // 创建按钮1
        GameObject button1Object = new GameObject("Button1");
        button1Object.transform.SetParent(dialogUI.transform);
        button1 = button1Object.AddComponent<Button>();
        button1.onClick.AddListener(() => OnButtonClicked(1));

        // 添加按钮背景
        Image button1Image = button1Object.AddComponent<Image>();
        button1Image.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // 设置按钮文本
        GameObject button1TextObject = new GameObject("ButtonText");
        button1TextObject.transform.SetParent(button1Object.transform);
        Text button1Text = button1TextObject.AddComponent<Text>();
        button1Text.text = this.button1Text;

        // 使用指定的自定义字体
        if (customFont != null)
        {
            button1Text.font = customFont;
        }
        else
        {
            button1Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        button1Text.fontSize = (int)(promptFontSize * 0.9f);
        button1Text.color = Color.black;
        button1Text.alignment = TextAnchor.MiddleCenter;

        // 设置按钮1 RectTransform
        RectTransform button1Rect = button1Object.GetComponent<RectTransform>();
        button1Rect.anchorMin = new Vector2(0.1f, 0.1f);
        button1Rect.anchorMax = new Vector2(0.45f, 0.3f);
        button1Rect.offsetMin = new Vector2(5, 5);
        button1Rect.offsetMax = new Vector2(-5, -5);

        // 设置按钮文本RectTransform
        RectTransform button1TextRect = button1TextObject.GetComponent<RectTransform>();
        button1TextRect.anchorMin = Vector2.zero;
        button1TextRect.anchorMax = Vector2.one;
        button1TextRect.offsetMin = Vector2.zero;
        button1TextRect.offsetMax = Vector2.zero;

        // 创建按钮2
        GameObject button2Object = new GameObject("Button2");
        button2Object.transform.SetParent(dialogUI.transform);
        button2 = button2Object.AddComponent<Button>();
        button2.onClick.AddListener(() => OnButtonClicked(2));

        // 添加按钮背景
        Image button2Image = button2Object.AddComponent<Image>();
        button2Image.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // 设置按钮文本
        GameObject button2TextObject = new GameObject("ButtonText");
        button2TextObject.transform.SetParent(button2Object.transform);
        Text button2Text = button2TextObject.AddComponent<Text>();
        button2Text.text = this.button2Text;

        // 使用指定的自定义字体
        if (customFont != null)
        {
            button2Text.font = customFont;
        }
        else
        {
            button2Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        button2Text.fontSize = (int)(promptFontSize * 0.9f);
        button2Text.color = Color.black;
        button2Text.alignment = TextAnchor.MiddleCenter;

        // 设置按钮2 RectTransform
        RectTransform button2Rect = button2Object.GetComponent<RectTransform>();
        button2Rect.anchorMin = new Vector2(0.55f, 0.1f);
        button2Rect.anchorMax = new Vector2(0.9f, 0.3f);
        button2Rect.offsetMin = new Vector2(5, 5);
        button2Rect.offsetMax = new Vector2(-5, -5);

        // 设置按钮文本RectTransform
        RectTransform button2TextRect = button2TextObject.GetComponent<RectTransform>();
        button2TextRect.anchorMin = Vector2.zero;
        button2TextRect.anchorMax = Vector2.one;
        button2TextRect.offsetMin = Vector2.zero;
        button2TextRect.offsetMax = Vector2.zero;

        // 更新对话框位置
        UpdateDialogPosition();

        // 启动对话框计时器
        if (dialogDisplayTime > 0)
        {
            dialogDisplayTimer = dialogDisplayTime;
        }

        Debug.Log("对话框创建完成");
    }

    // 按钮点击事件处理
    void OnButtonClicked(int buttonIndex)
    {
        Debug.Log($"按钮 {buttonIndex} 被点击");

        if (buttonIndex == correctButtonIndex)
        {
            Debug.Log("选择了正确选项");
            onCorrectChoice?.Invoke();
        }
        else
        {
            Debug.Log("选择了错误选项");
            onIncorrectChoice?.Invoke();
            TriggerRain();
        }

        HideDialog();
    }

    // 新增：隐藏对话框
    void HideDialog()
    {
        if (dialogUI != null)
        {
            Destroy(dialogUI);
            dialogUI = null;
            dialogActive = false;
            Debug.Log("对话框已关闭");
        }
    }

    // 新增：更新对话框位置
    void UpdateDialogPosition()
    {
        if (dialogUI == null)
        {
            Debug.LogWarning("尝试更新对话框位置，但对话框UI为空");
            return;
        }

        // 确保相机存在
        if (Camera.main == null)
        {
            Debug.LogError("找不到主相机，无法更新对话框位置");
            return;
        }

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, dialogOffsetY, 0));

        // 更新UI位置
        RectTransform dialogRect = dialogUI.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.anchoredPosition = Vector2.zero;
    }

    // 新增：更新对话框计时器
    void UpdateDialogTimer()
    {
        if (dialogDisplayTimer > 0)
        {
            dialogDisplayTimer -= Time.deltaTime;
            if (dialogDisplayTimer <= 0 && dialogUI != null)
            {
                Debug.Log("对话框超时，自动选择错误");
                onIncorrectChoice?.Invoke();
                TriggerRain(); // 添加触发雨
                HideDialog();
            }
        }
    }

    void TriggerRain()
    {
        if (rainSystem == null)
        {
            // 尝试自动查找
            rainSystem = FindObjectOfType<RainSystem>();
            if (rainSystem == null)
            {
                Debug.LogError("场景中未找到RainSystem！");
                return;
            }
        }

        // 确保雨系统已启用
        if (!rainSystem.gameObject.activeInHierarchy)
        {
            rainSystem.gameObject.SetActive(true);
        }

        rainSystem.rainDuration = this.rainDuration;
        rainSystem.StartRain();

        Debug.Log($"已触发下雨，持续{rainDuration}秒");
    }

    void LateUpdate()
    {
        // 确保提示位置跟随开关
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }

        // 确保对话框位置跟随开关
        if (dialogUI != null)
        {
            UpdateDialogPosition();
        }
    }

    // 可视化交互范围（仅在编辑器中显示）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}