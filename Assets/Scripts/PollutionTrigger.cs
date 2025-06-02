using UnityEngine;
using UnityEngine.UI;

public class PollutionTrigger : MonoBehaviour
{
    // ��Ⱦ��������Screen Space - Overlay��
    public GameObject pollutionPanel; // ȫ����Ⱦ���
    public Image pollutionImage;      // ��ȾͼƬ���

    // Ҫ���صĶԻ��򲿷�
    public GameObject dialogPanel;    // �����Ի�UI�ĸ�����

    public void OnYesButtonClicked()
    {
        // ��ʾ��ȾͼƬ
        if (pollutionPanel != null && pollutionImage != null)
        {
            pollutionPanel.SetActive(true);
            pollutionImage.color = new Color(pollutionImage.color.r, pollutionImage.color.g, pollutionImage.color.b, 1f);
        }

        // ���ضԻ���
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }
}