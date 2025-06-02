using UnityEngine;
using UnityEngine.UI;
public class ClownNPCInteraction : MonoBehaviour
{
    [TextArea(3, 10)]
    public string dialogMessage = "你敢不敢按YES？";
    // 对话UI组件（World Space）
    public Text displayText;
    public Button option1Button;
    public Button option2Button;
    public RectTransform dialogPanel; // 整个对话UI的根对象
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // 对话框偏移量
    // 新增字段：按钮文本
    public string option1Text = "YES";
    public string option2Text = "NO";
    private bool isViewing = false;
    void Start()
    {
        HideDialogUI();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowDialogUI();
            isViewing = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideDialogUI();
            isViewing = false;
        }
    }
    private void Update()
    {
        if (isViewing && dialogPanel != null)
        {
            UpdateDialogPosition();
        }
    }
    private void UpdateDialogPosition()
    {
        Vector3 worldPosition = transform.position + uiOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        dialogPanel.position = screenPosition;
    }
    private void ShowDialogUI()
    {
        if (displayText != null)
        {
            displayText.text = dialogMessage;
            displayText.gameObject.SetActive(true);
        }
        if (option1Button != null)
        {
            // 设置按钮文本
            Text btnText1 = option1Button.GetComponentInChildren<Text>();
            if (btnText1 != null)
                btnText1.text = option1Text;
            option1Button.gameObject.SetActive(true);
        }
        if (option2Button != null)
        {
            // 设置按钮文本
            Text btnText2 = option2Button.GetComponentInChildren<Text>();
            if (btnText2 != null)
                btnText2.text = option2Text;
            option2Button.gameObject.SetActive(true);
        }
        if (dialogPanel != null)
        {
            dialogPanel.gameObject.SetActive(true);
        }
    }
    private void HideDialogUI()
    {
        if (displayText != null)
            displayText.gameObject.SetActive(false);
        if (option1Button != null)
            option1Button.gameObject.SetActive(false);
        if (option2Button != null)
            option2Button.gameObject.SetActive(false);
        if (dialogPanel != null)
            dialogPanel.gameObject.SetActive(false);
    }
}