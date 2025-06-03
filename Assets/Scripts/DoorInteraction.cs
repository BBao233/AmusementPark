using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    [Header("基础配置")]
    public string targetSceneName = "NextLevel"; // 目标场景名（需添加到Build Settings）
    public Vector2 boxTriggerSize = new Vector2(2f, 1.5f); // Box Collider 2D 的大小（宽×高）
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // UI相对于门的偏移量

    [Header("UI文本配置")]
    public Text promptText; // 提示文本（如"按E/L进入下一场景"）
    public Color activeTextColor = Color.white; // 激活时文本颜色
    public Color inactiveTextColor = Color.gray; // 非激活时文本颜色

    [Header("按键配置")]
    public KeyCode player1Key = KeyCode.E; // 角色1按键（E）
    public KeyCode player2Key = KeyCode.L; // 角色2按键（L）

    private bool isPlayerInRange = false; // 玩家是否在触发范围内

    private void Start()
    {
        // 初始化UI状态（隐藏+灰色文本）
        HidePrompt();
        if (promptText != null)
        {
            promptText.color = inactiveTextColor;
        }
    }

    private void OnDrawGizmos()
    {
        // 编辑器中显示Box触发区域（方便调整范围）
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, boxTriggerSize);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowPrompt(); // 显示提示文本并激活颜色
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HidePrompt(); // 隐藏文本并恢复灰色
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            // 更新UI位置（始终在门上方）
            UpdateUIPosition();

            // 检测按键输入
            if (Input.GetKeyDown(player1Key) || Input.GetKeyDown(player2Key))
            {
                EnterNextScene(); // 触发场景切换
            }
        }
    }

    private void UpdateUIPosition()
    {
        if (promptText == null) return;

        // 将世界坐标转换为屏幕坐标
        Vector3 worldPos = transform.position + uiOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        promptText.rectTransform.position = screenPos;
    }

    private void ShowPrompt()
    {
        if (promptText == null) return;

        promptText.gameObject.SetActive(true);
        promptText.color = activeTextColor;
        promptText.text = "按 E/L 进入鬼屋"; // 可自定义提示内容
    }

    private void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }

    private void EnterNextScene()
    {
        // 切换场景（需确保场景已添加到Build Settings）
        SceneManager.LoadScene(targetSceneName);
        Debug.Log($"正在加载场景：{targetSceneName}");
    }

    // 自动配置Box Collider 2D（确保触发区域正确）
    private void OnValidate()
    {
        BoxCollider2D trigger = GetComponent<BoxCollider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true; // 设为触发器
        }
        trigger.size = boxTriggerSize; // 同步触发区域大小
        trigger.offset = Vector2.zero; // 中心对齐（可根据需要调整偏移量）
    }
}