using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [Header("场景设置")]
    public string nextSceneName = "Level2"; // 下一关场景名称
    public KeyCode interactKey1 = KeyCode.E, interactKey2 = KeyCode.L; // 交互按键

    [Header("提示UI设置")]
    public bool createPromptUI = true;
    public string promptText = "前往下一关";
    public Color promptColor = Color.green;
    public float promptOffsetY = 1.5f;
    public float promptFontSize = 20;

    private bool maleInRange = false;
    private bool femaleInRange = false;
    private GameObject promptUI;
    private bool allItemsCollected = false;
    private bool bothPlayersInZone = false;
    private bool canExit = false;

    void Start()
    {
        // 监听收集完成事件
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.OnAllItemsCollected += OnAllItemsCollected;
        }
        else
        {
            Debug.LogWarning("CollectingManager 实例未找到！");
        }

        // 创建提示UI
        if (createPromptUI)
        {
            CreatePromptUI();
        }
    }

    void OnDestroy()
    {
        // 取消事件订阅
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.OnAllItemsCollected -= OnAllItemsCollected;
        }
    }

    void OnAllItemsCollected()
    {
        allItemsCollected = true;
        Debug.Log("所有物品收集完成，可以通关");
        UpdatePromptVisibility();
    }

    void Update()
    {
        // 更新提示位置
        if (promptUI != null && promptUI.activeSelf)
        {
            UpdatePromptPosition();
        }

        // 检查是否可以退出
        bothPlayersInZone = maleInRange && femaleInRange;
        canExit = allItemsCollected && maleInRange && femaleInRange;

        // 如果满足条件且按下交互键，则进入下一关
        if (canExit && (Input.GetKeyDown(interactKey1) || Input.GetKeyDown(interactKey2)))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只检测Player层的对象
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        if (other.CompareTag("Male"))
        {
            maleInRange = true;
            Debug.Log("男性进入通关区域");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = true;
            Debug.Log("女性进入通关区域");
        }

        UpdatePromptVisibility();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 只检测Player层的对象
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        if (other.CompareTag("Male"))
        {
            maleInRange = false;
            Debug.Log("男性离开通关区域");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = false;
            Debug.Log("女性离开通关区域");
        }

        UpdatePromptVisibility();
    }

    void UpdatePromptVisibility()
    {
        if (promptUI == null) return;

        // 只有当所有物品收集完成时才考虑显示提示
        bool shouldShow = allItemsCollected;

        if (promptUI.activeSelf != shouldShow)
        {
            promptUI.SetActive(shouldShow);
            Debug.Log($"通关提示显示状态: {shouldShow}");
        }

        // 更新提示文本
        if (shouldShow)
        {
            Text text = promptUI.GetComponent<Text>();

            if (maleInRange && femaleInRange)
            {
                text.text = $"按{interactKey1}键或{interactKey2}键\n进入{nextSceneName}";
                text.color = Color.white;
            }
            else if (maleInRange && !femaleInRange)
            {
                text.text = "等待女性角色...";
                text.color = Color.white;
            }
            else if (femaleInRange && !maleInRange)
            {
                text.text = "等待男性角色...";
                text.color = Color.white;
            }
            else
            {
                text.text = "进入下一关";
                text.color = Color.white;
            }
        }
    }

    void CreatePromptUI()
    {
        // 寻找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("创建新的Canvas用于通关提示");
        }

        // 创建提示文本
        promptUI = new GameObject("ExitPrompt");
        promptUI.transform.SetParent(canvas.transform);
        Text textComponent = promptUI.AddComponent<Text>();
        textComponent.text = "需要收集所有物品";
        textComponent.color = Color.red;
        textComponent.fontSize = (int)promptFontSize;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // 使用改进的字体加载逻辑
        Font loadedFont = LoadFont();
        textComponent.font = loadedFont;

        // 设置RectTransform
        RectTransform rectTransform = promptUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 50);
        promptUI.SetActive(false); // 默认隐藏

        Debug.Log("通关提示UI创建完成");
    }

    // 改进的字体加载方法
    private Font LoadFont()
    {
        Font loadedFont = null;

        // 字体候选列表
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
                    return loadedFont;
                }

                // 如果路径加载失败，尝试直接使用字体名称
                if (fontName.EndsWith(".ttf"))
                {
                    string plainFontName = fontName.Substring(0, fontName.Length - 4);
                    loadedFont = Font.CreateDynamicFontFromOSFont(plainFontName, (int)promptFontSize);
                    if (loadedFont != null)
                    {
                        Debug.Log($"成功通过字体名称加载: {plainFontName}");
                        return loadedFont;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"尝试加载字体 {fontName} 失败: {e.Message}");
            }
        }

        // 如果所有尝试都失败，使用默认字体
        Debug.LogError("无法加载任何内置字体，使用默认字体");
        return Font.CreateDynamicFontFromOSFont("Arial", (int)promptFontSize);
    }

    void UpdatePromptPosition()
    {
        if (promptUI == null || Camera.main == null) return;

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // 更新UI位置
        promptUI.GetComponent<RectTransform>().position = screenPosition;
    }

    // 在编辑器中可视化触发区域
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // 绿色半透明
        Collider2D collider = GetComponent<Collider2D>();

        if (collider is BoxCollider2D boxCollider)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
        }
    }
}