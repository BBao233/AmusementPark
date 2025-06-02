using UnityEngine;
using UnityEngine.UI;
public class PollutionTrigger : MonoBehaviour
{
    public GameObject pollutionPanel; // ȫ����Ⱦ��壨��Trigger������ʾ��
    public Image pollutionImage;      // ��ȾͼƬ���
    public GameObject dialogPanel;    // Ҫ���صĶԻ���
    public void OnYesButtonClicked()
    {
        // 1. �����ضԻ���
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
        // 2. ��ʾ��Ⱦ��壨��Trigger��ȫ���ƣ�
        if (pollutionPanel != null)
            pollutionPanel.SetActive(true);
        // 3. ������ȾЧ����ȷ���������ʾ����ã�
        if (pollutionImage != null)
        {
            PollutionEffectController controller =
                pollutionImage.GetComponent<PollutionEffectController>();
            if (controller != null)
            {
                controller.StartPollution(); // ������ȾЧ��
            }
        }
    }
}