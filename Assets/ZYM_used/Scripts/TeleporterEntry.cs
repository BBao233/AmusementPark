using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterEntry : MonoBehaviour
{
    [Header("传送设置")]
    public TeleporterExit targetExit; // 目标出口
    public float interactionRadius = 1f; // 交互范围

    [Header("提示UI设置")]
    public bool createPromptUI = true;
    public string malePromptText = "按E传送";
    public string femalePromptText = "按L传送";
    public Color malePromptColor = Color.blue;
    public Color femalePromptColor = Color.magenta;
    public float promptOffsetY = 1.2f;
    public float promptFontSize = 16f;

    [Header("字体设置")]
    public Font customFont; // 新增：自定义字体字段

    [Header("音效设置")]
    public AudioClip interactionSound;  // 交互音效
    [Range(0f, 1f)] public float volume = 1f;  // 音量控制滑块
    private AudioSource audioSource;    // 音频源组件

    private bool maleInRange = false;
    private bool femaleInRange = false;
    private GameObject malePromptUI;
    private GameObject femalePromptUI;

    void Start()
    {
        // 创建提示UI
        if (createPromptUI)
        {
            CreatePromptUI();
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Update()
    {
        // 更新提示位置
        if (malePromptUI != null && malePromptUI.activeSelf)
        {
            UpdatePromptPosition(malePromptUI);
        }
        if (femalePromptUI != null && femalePromptUI.activeSelf)
        {
            UpdatePromptPosition(femalePromptUI);
        }

        // 检查传送输入
        if (maleInRange && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer("Male");
        }

        if (femaleInRange && Input.GetKeyDown(KeyCode.L))
        {
            TeleportPlayer("Female");
        }
    }

    void TeleportPlayer(string playerTag)
    {
        if (targetExit == null)
        {
            Debug.LogError("未设置目标出口！");
            return;
        }

        // 查找玩家对象
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            // 传送玩家到出口
            player.transform.position = targetExit.GetExitPosition();
            Debug.Log($"{playerTag} 已传送到 {targetExit.gameObject.name}");

            if (interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound, volume);
            }

            // 播放传送效果（可选）
            if (targetExit.teleportEffect != null)
            {
                Instantiate(targetExit.teleportEffect, player.transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning($"未找到标签为 {playerTag} 的玩家对象");
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
            Debug.Log("男性进入传送区域");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = true;
            Debug.Log("女性进入传送区域");
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
            Debug.Log("男性离开传送区域");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = false;
            Debug.Log("女性离开传送区域");
        }

        UpdatePromptVisibility();
    }

    void UpdatePromptVisibility()
    {
        if (malePromptUI != null)
        {
            malePromptUI.SetActive(maleInRange);
        }

        if (femalePromptUI != null)
        {
            femalePromptUI.SetActive(femaleInRange);
        }
    }

    void CreatePromptUI()
    {
        // 寻找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TeleporterCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("创建新的Canvas用于传送提示");
        }

        // 创建男性提示文本
        malePromptUI = CreatePrompt(canvas.transform, "MalePrompt", malePromptText, malePromptColor);
        malePromptUI.SetActive(false);

        // 创建女性提示文本
        femalePromptUI = CreatePrompt(canvas.transform, "FemalePrompt", femalePromptText, femalePromptColor);
        femalePromptUI.SetActive(false);

        // 调整位置偏移
        if (malePromptUI != null && femalePromptUI != null)
        {
            RectTransform maleRect = malePromptUI.GetComponent<RectTransform>();
            RectTransform femaleRect = femalePromptUI.GetComponent<RectTransform>();

            // 男性提示在上方
            maleRect.anchoredPosition = new Vector2(0, 20);

            // 女性提示在下方
            femaleRect.anchoredPosition = new Vector2(0, -20);
        }

        Debug.Log("传送提示UI创建完成");
    }

    GameObject CreatePrompt(Transform parent, string name, string text, Color color)
    {
        GameObject prompt = new GameObject(name);
        prompt.transform.SetParent(parent);

        Text textComponent = prompt.AddComponent<Text>();
        textComponent.text = text;
        textComponent.color = color;
        textComponent.fontSize = (int)promptFontSize;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // 优先使用自定义字体
        if (customFont != null)
        {
            textComponent.font = customFont;
            Debug.Log($"使用自定义字体: {customFont.name}");
        }
        else
        {
            // 使用改进的字体加载逻辑
            Font loadedFont = LoadFont();
            if (loadedFont != null)
            {
                textComponent.font = loadedFont;
            }
            else
            {
                Debug.LogWarning("字体加载失败，使用默认字体");
            }
        }

        // 设置RectTransform
        RectTransform rectTransform = prompt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 30);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        return prompt;
    }

    void UpdatePromptPosition(GameObject prompt)
    {
        if (prompt == null || Camera.main == null) return;

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // 更新UI位置
        prompt.GetComponent<RectTransform>().position = screenPosition;
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

    // 在编辑器中可视化触发区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.5f, 1f, 0.5f); // 蓝色半透明
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        // 如果设置了目标出口，绘制连线
        if (targetExit != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetExit.GetExitPosition());
        }
    }
}