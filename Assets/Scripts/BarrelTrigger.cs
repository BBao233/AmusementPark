using UnityEngine;
using UnityEngine.UI;

public class BarrelTrigger : MonoBehaviour
{
    [Header("控制对象")]
    public GameObject barrier;       // 栏杆对象

    [Header("UI 设置")]
    public Text displayText;         // 共用的 Text 组件
    public RectTransform uiPanel;    // UI面板整体
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // 相对偏移量

    private bool isViewing = false;
    private bool triggerActivated = false;

    void Start()
    {
        HideUIElements();
    }

    void Update()
    {
        if (isViewing && !triggerActivated)
        {
            UpdateUIPosition();

            // 角色1按 E 或角色2按 L 都可以触发
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.L))
            {
                TriggerAction();
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

    void TriggerAction()
    {
        triggerActivated = true;

        // 栏杆消失
        if (barrier != null)
            barrier.SetActive(false);

        // 显示提示信息
        displayText.text = "栏杆打开，箱子落下";

        // 3秒后隐藏提示
        Invoke(nameof(HideUIElements), 3f);
    }

    void ShowPrompt()
    {
        displayText.text = "按下 E/L 激活开关";
        displayText.gameObject.SetActive(true);
        uiPanel.gameObject.SetActive(true);
        UpdateUIPosition();
    }

    void HideUIElements()
    {
        displayText.gameObject.SetActive(false);
        if (uiPanel != null)
            uiPanel.gameObject.SetActive(false);
    }

    void UpdateUIPosition()
    {
        Vector3 worldPosition = transform.position + uiOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        uiPanel.position = screenPosition;
    }
}