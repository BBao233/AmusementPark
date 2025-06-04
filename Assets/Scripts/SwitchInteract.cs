using UnityEngine;
using UnityEngine.UI;

public class SwitchInteractFinal : MonoBehaviour
{
    [Header("开关设置")]
    public GameObject door; // 要打开的门对象
    public string promptText = "按下 E/L 激活开关"; // 提示文字
    public string actionText = "某处的铁门打开了"; // 动作提示
    public AudioClip switchSound; // 开关音效

    [Header("UI 设置")]
    public Text displayText;       // 共用的 Text 组件
    public RectTransform uiPanel;  // UI面板整体
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // 相对偏移量

    private bool isViewing = false;
    private bool doorOpened = false;
    private AudioSource audioSource; // 音频播放源

    void Start()
    {
        HideUIElements();
        // 获取或添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 确保不自动播放音效
    }

    void Update()
    {
        if (isViewing && !doorOpened)
        {
            UpdateUIPosition();

            // 不管是谁，只要按 E 或 L 就可以触发
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.L))
            {
                TriggerSwitch();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isViewing = true;
            ShowPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isViewing = false;
            HideUIElements();
        }
    }

    void TriggerSwitch()
    {
        doorOpened = true;

        // 关闭门对象（即让精灵消失）
        if (door != null)
        {
            door.SetActive(false);
        }

        // 播放开关音效
        if (switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

        // 显示“铁门打开了”提示
        displayText.text = actionText;

        // 3秒后隐藏提示
        Invoke(nameof(HideUIElements), 3f);
    }

    void ShowPrompt()
    {
        displayText.text = promptText;
        displayText.gameObject.SetActive(true);
        uiPanel.gameObject.SetActive(true);
        UpdateUIPosition();
    }

    void HideUIElements()
    {
        displayText.gameObject.SetActive(false);
        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(false);
        }
    }

    void UpdateUIPosition()
    {
        Vector3 worldPosition = transform.position + uiOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        uiPanel.position = screenPosition;
    }
}
