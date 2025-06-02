using UnityEngine;
using UnityEngine.UI;

public class PollutionEffectController : MonoBehaviour
{
    public Image pollutionImage; // ��ȾͼƬ���
    public GameObject pollutionPanel; // ��Ⱦ��壨����������أ�

    [Tooltip("��Ⱦ����ʱ�䣨����0������ʱ�䣩")]
    public float maxPollutionTime = 5f;

    private float currentPollutionTime = 0f;

    private bool isPolluting = true;

    void Start()
    {
        if (pollutionImage != null)
        {
            pollutionImage.color = new Color(
                pollutionImage.color.r,
                pollutionImage.color.g,
                pollutionImage.color.b,
                0f); // ��ʼ͸��
        }

        currentPollutionTime = 0f;
        isPolluting = true;
    }

    void Update()
    {
        if (!isPolluting) return;

        HandlePollutionEffect();
    }

    private void HandlePollutionEffect()
    {
        bool pressingE = Input.GetKey(KeyCode.E);
        bool pressingL = Input.GetKey(KeyCode.L);

        float changeSpeed = 0f;

        if (pressingE && pressingL)
        {
            changeSpeed = -Time.deltaTime * 2f; // ͬʱ���£��������
        }
        else if (pressingE || pressingL)
        {
            changeSpeed = -Time.deltaTime; // �������£��������
        }
        else
        {
            changeSpeed = Time.deltaTime; // ������������Ⱦ
        }

        currentPollutionTime += changeSpeed;

        // ���Ʒ�Χ
        currentPollutionTime = Mathf.Clamp(currentPollutionTime, 0f, maxPollutionTime);

        // ����͸���ȣ�0~1��
        float alpha = currentPollutionTime / maxPollutionTime;

        // ������ɫ
        if (pollutionImage != null)
        {
            Color color = pollutionImage.color;
            color.a = alpha;
            pollutionImage.color = color;
        }

        // �����Ⱦ������
        if (currentPollutionTime <= 0f)
        {
            EndPollution();
        }
    }

    private void EndPollution()
    {
        isPolluting = false;

        if (pollutionPanel != null)
        {
            pollutionPanel.SetActive(false);
        }

        Debug.Log("��Ⱦ�������");
    }
}