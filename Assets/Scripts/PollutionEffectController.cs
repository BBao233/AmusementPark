using UnityEngine;
using UnityEngine.UI;
public class PollutionEffectController : MonoBehaviour
{
    public Image pollutionImage; // ��ȾͼƬ���
    public GameObject pollutionPanel; // ��Ⱦ��壨���ⲿ������ʾ���ڲ����������أ�
    [Tooltip("�����Ⱦ����ʱ�䣨�룩")]
    public float maxClearTime = 10f;
    public float clearSpeedSingle = 1.5f; // ��������ٶ�
    public float clearSpeedDouble = 3f; // ˫������ٶ�
    private float currentPollutionValue = 1f; // ��ʼ��Ⱦֵ100%
    private bool isPollutionActive = false; // ��Ⱦ�Ƿ񼤻�
    void Awake()
    {
        // ����ʼ����Ⱦֵ��������������壨���ⲿ���ƣ�
        currentPollutionValue = 1f;
        isPollutionActive = false;
        if (pollutionImage != null)
        {
            // ��ʼ͸���������δ��ʾʱ��͸����Ӱ�죩
            pollutionImage.color = new Color(
                pollutionImage.color.r,
                pollutionImage.color.g,
                pollutionImage.color.b,
                0f
            );
        }
    }
    void Update()
    {
        if (!isPollutionActive) return;
        bool player1PressE = Input.GetKey(KeyCode.E);
        bool player2PressL = Input.GetKey(KeyCode.L);
        HandlePollutionClear(player1PressE, player2PressL);
        UpdatePollutionVisual();
    }
    public void StartPollution()
    {
        isPollutionActive = true;
        currentPollutionValue = 1f; // ����Ϊ100%��Ⱦ
        // ������ʾ��ȫ��͸������ȾͼƬ
        if (pollutionImage != null)
        {
            pollutionImage.color = new Color(
                pollutionImage.color.r,
                pollutionImage.color.g,
                pollutionImage.color.b,
                1f // ��ȫ��͸��
            );
        }
    }
    private void HandlePollutionClear(bool p1Press, bool p2Press)
    {
        float delta = Time.deltaTime / maxClearTime; // ��׼������ٶ�
        if (p1Press || p2Press)
        {
            if (p1Press && p2Press) // ˫���������
            {
                currentPollutionValue -= delta * clearSpeedDouble;
            }
            else // �������
            {
                currentPollutionValue -= delta * clearSpeedSingle;
            }
        }
        currentPollutionValue = Mathf.Clamp01(currentPollutionValue);
        if (currentPollutionValue <= 0.01f)
        {
            EndPollution();
        }
    }
    private void UpdatePollutionVisual()
    {
        if (pollutionImage == null) return;
        pollutionImage.color = new Color(
            pollutionImage.color.r,
            pollutionImage.color.g,
            pollutionImage.color.b,
            currentPollutionValue // ��Ⱦֱֵ�Ӷ�Ӧ͸����
        );
    }
    private void EndPollution()
    {
        isPollutionActive = false;
        // �����ɺ�������壨�������ڳ�ʼ��ʱ���أ�
        if (pollutionPanel != null)
        {
            pollutionPanel.SetActive(false);
        }
        Debug.Log("��Ⱦ�������");
    }
}