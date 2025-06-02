using UnityEngine.UI;
using UnityEngine;
public class Signboard : MonoBehaviour
{
    [TextArea(3, 10)]
    public string signMessage = "����һ����Ҫ֪ͨ��";
    public Text displayText;

    // ������UI������ã��������嶨λ
    public RectTransform uiPanel;
    // ������UI����ڸ�ʾ�Ƶ�ƫ����
    public Vector3 uiOffset = new Vector3(0, 2f, 0);
    private bool isViewing = false;
    private void Start()
    {
        // ��ʼʱ��������UIԪ��
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
        // ��UI��ʾʱ������������λ��
        if (isViewing && uiPanel != null)
        {
            UpdateUIPosition();
        }
    }
    private void UpdateUIPosition()
    {
        // ����ʾ�Ƶ���������ת��Ϊ��Ļ����
        Vector3 worldPosition = transform.position + uiOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // ����UI���λ��
        uiPanel.position = screenPosition;
    }
    private void ShowUIElements()
    {
        if (displayText != null)
        {
            displayText.text = signMessage;
            displayText.gameObject.SetActive(true);
        }

        // ȷ��UI����Ѽ���
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
        if (uiPanel != null)
            uiPanel.gameObject.SetActive(false);
    }

}