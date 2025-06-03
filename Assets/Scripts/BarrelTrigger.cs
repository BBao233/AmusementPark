using UnityEngine;
using UnityEngine.UI;

public class BarrelTrigger : MonoBehaviour
{
    [Header("���ƶ���")]
    public GameObject barrier;       // ���˶���

    [Header("UI ����")]
    public Text displayText;         // ���õ� Text ���
    public RectTransform uiPanel;    // UI�������
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // ���ƫ����

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

            // ��ɫ1�� E ���ɫ2�� L �����Դ���
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

        // ������ʧ
        if (barrier != null)
            barrier.SetActive(false);

        // ��ʾ��ʾ��Ϣ
        displayText.text = "���˴򿪣���������";

        // 3���������ʾ
        Invoke(nameof(HideUIElements), 3f);
    }

    void ShowPrompt()
    {
        displayText.text = "���� E/L �����";
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