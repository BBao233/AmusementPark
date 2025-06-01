using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

    private GameObject interactionPrompt;  // 交互提示UI
    private bool playerInRange;
    private bool mapsActivated = false;   // 记录地图是否已激活
    private SpriteRenderer switchRenderer; // 开关的精灵渲染器
    private Vector3 originalScale;         // 原始缩放比例
    private bool isScaling = false;        // 是否正在缩放

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

        if (interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, volume);
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

        // 确保字体存在 - 改进的字体加载逻辑
        Font loadedFont = null;

        // 尝试加载已知的内置字体
        string[] fontCandidates = {
            "Arial.ttf",              // 旧版本Unity支持
            "LegacyRuntime.ttf",      // 新版本Unity支持
            "Arial"                   // 直接使用字体名称
        };

        foreach (string fontName in fontCandidates)
        {
            try
            {
                // 先尝试使用资源路径加载
                loadedFont = Resources.GetBuiltinResource<Font>(fontName);
                if (loadedFont != null)
                {
                    Debug.Log($"成功加载字体: {fontName}");
                    break;
                }

                // 如果路径加载失败，尝试直接使用字体名称
                if (fontName.EndsWith(".ttf"))
                {
                    string plainFontName = fontName.Substring(0, fontName.Length - 4);
                    loadedFont = Font.CreateDynamicFontFromOSFont(plainFontName, (int)promptFontSize);
                    if (loadedFont != null)
                    {
                        Debug.Log($"成功通过字体名称加载: {plainFontName}");
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"尝试加载字体 {fontName} 失败: {e.Message}");
            }
        }

        // 如果所有尝试都失败，使用默认字体
        if (loadedFont == null)
        {
            Debug.LogError("无法加载任何内置字体，使用默认字体");
            loadedFont = Resources.GetBuiltinResource<Font>("Arial.ttf"); // 这可能会失败，但Unity会提供默认字体
        }

        textComponent.font = loadedFont;
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

    void LateUpdate()
    {
        // 确保提示位置跟随开关
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }
    }

    // 可视化交互范围（仅在编辑器中显示）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}