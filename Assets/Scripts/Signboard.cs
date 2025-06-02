using UnityEngine.UI;
using UnityEngine;
public class Signboard : MonoBehaviour
{
    [TextArea(3, 10)]
    public string signMessage = "这是一则重要通知！";
    public Text displayText;
    public Button option1Button;
    public Button option2Button;
    public string option1Text = "YES";
    public string option2Text = "NO";
    // 新增：UI面板引用，用于整体定位
    public RectTransform uiPanel;
    // 新增：UI相对于告示牌的偏移量
    public Vector3 uiOffset = new Vector3(0, 2f, 0);
    private bool isViewing = false;
    private void Start()
    {
        // 初始时隐藏所有UI元素
        HideUIElements();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowUIElements();
            isViewing = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideUIElements();
            isViewing = false;
        }
    }
    private void Update()
    {
        // 当UI显示时，持续更新其位置
        if (isViewing && uiPanel != null)
        {
            UpdateUIPosition();
        }
    }
    private void UpdateUIPosition()
    {
        // 将告示牌的世界坐标转换为屏幕坐标
        Vector3 worldPosition = transform.position + uiOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // 设置UI面板位置
        uiPanel.position = screenPosition;
    }
    private void ShowUIElements()
    {
        if (displayText != null)
        {
            displayText.text = signMessage;
            displayText.gameObject.SetActive(true);
        }
        if (option1Button != null)
        {
            option1Button.GetComponentInChildren<Text>().text = option1Text;
            option1Button.gameObject.SetActive(true);
        }
        if (option2Button != null)
        {
            option2Button.GetComponentInChildren<Text>().text = option2Text;
            option2Button.gameObject.SetActive(true);
        }
        // 确保UI面板已激活
        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(true);
            UpdateUIPosition();
        }
    }
    private void HideUIElements()
    {
        if (displayText != null)
            displayText.gameObject.SetActive(false);
        if (option1Button != null)
            option1Button.gameObject.SetActive(false);
        if (option2Button != null)
            option2Button.gameObject.SetActive(false);
        if (uiPanel != null)
            uiPanel.gameObject.SetActive(false);
    }
    public void OnOption1Clicked()
    {
        Debug.Log("选择了YES");
        // 处理选项1逻辑
    }
    public void OnOption2Clicked()
    {
        Debug.Log("选择了NO");
        // 处理选项2逻辑
    }
}