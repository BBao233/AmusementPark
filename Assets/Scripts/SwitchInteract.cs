using UnityEngine;
using UnityEngine.UI;

public class SwitchInteractFinal : MonoBehaviour
{
    [Header("��������")]
    public GameObject door; // Ҫ�򿪵��Ŷ���
    public string promptText = "���� E/L �����"; // ��ʾ����
    public string actionText = "ĳ�������Ŵ���"; // ������ʾ
    public AudioClip switchSound; // ������Ч

    [Header("UI ����")]
    public Text displayText;       // ���õ� Text ���
    public RectTransform uiPanel;  // UI�������
    public Vector3 uiOffset = new Vector3(0, 2f, 0); // ���ƫ����

    private bool isViewing = false;
    private bool doorOpened = false;
    private AudioSource audioSource; // ��Ƶ����Դ

    void Start()
    {
        HideUIElements();
        // ��ȡ����� AudioSource ���
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // ȷ�����Զ�������Ч
    }

    void Update()
    {
        if (isViewing && !doorOpened)
        {
            UpdateUIPosition();

            // ������˭��ֻҪ�� E �� L �Ϳ��Դ���
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

        // �ر��Ŷ��󣨼��þ�����ʧ��
        if (door != null)
        {
            door.SetActive(false);
        }

        // ���ſ�����Ч
        if (switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

        // ��ʾ�����Ŵ��ˡ���ʾ
        displayText.text = actionText;

        // 3���������ʾ
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
