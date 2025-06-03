using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    [Header("��������")]
    public string targetSceneName = "NextLevel"; // Ŀ�곡����������ӵ�Build Settings��
    public Vector2 boxTriggerSize = new Vector2(2f, 1.5f); // Box Collider 2D �Ĵ�С������ߣ�
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // UI������ŵ�ƫ����

    [Header("UI�ı�����")]
    public Text promptText; // ��ʾ�ı�����"��E/L������һ����"��
    public Color activeTextColor = Color.white; // ����ʱ�ı���ɫ
    public Color inactiveTextColor = Color.gray; // �Ǽ���ʱ�ı���ɫ

    [Header("��������")]
    public KeyCode player1Key = KeyCode.E; // ��ɫ1������E��
    public KeyCode player2Key = KeyCode.L; // ��ɫ2������L��

    private bool isPlayerInRange = false; // ����Ƿ��ڴ�����Χ��

    private void Start()
    {
        // ��ʼ��UI״̬������+��ɫ�ı���
        HidePrompt();
        if (promptText != null)
        {
            promptText.color = inactiveTextColor;
        }
    }

    private void OnDrawGizmos()
    {
        // �༭������ʾBox�������򣨷��������Χ��
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, boxTriggerSize);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowPrompt(); // ��ʾ��ʾ�ı���������ɫ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HidePrompt(); // �����ı����ָ���ɫ
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            // ����UIλ�ã�ʼ�������Ϸ���
            UpdateUIPosition();

            // ��ⰴ������
            if (Input.GetKeyDown(player1Key) || Input.GetKeyDown(player2Key))
            {
                EnterNextScene(); // ���������л�
            }
        }
    }

    private void UpdateUIPosition()
    {
        if (promptText == null) return;

        // ����������ת��Ϊ��Ļ����
        Vector3 worldPos = transform.position + uiOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        promptText.rectTransform.position = screenPos;
    }

    private void ShowPrompt()
    {
        if (promptText == null) return;

        promptText.gameObject.SetActive(true);
        promptText.color = activeTextColor;
        promptText.text = "�� E/L �������"; // ���Զ�����ʾ����
    }

    private void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }

    private void EnterNextScene()
    {
        // �л���������ȷ����������ӵ�Build Settings��
        SceneManager.LoadScene(targetSceneName);
        Debug.Log($"���ڼ��س�����{targetSceneName}");
    }

    // �Զ�����Box Collider 2D��ȷ������������ȷ��
    private void OnValidate()
    {
        BoxCollider2D trigger = GetComponent<BoxCollider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true; // ��Ϊ������
        }
        trigger.size = boxTriggerSize; // ͬ�����������С
        trigger.offset = Vector2.zero; // ���Ķ��루�ɸ�����Ҫ����ƫ������
    }
}