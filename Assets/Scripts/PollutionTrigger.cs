using UnityEngine;
using UnityEngine.UI;
public class PollutionTrigger : MonoBehaviour
{
    public GameObject pollutionPanel; // 全屏污染面板（由Trigger控制显示）
    public Image pollutionImage;      // 污染图片组件
    public GameObject dialogPanel;    // 要隐藏的对话框
    public void OnYesButtonClicked()
    {
        // 1. 先隐藏对话框
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
        // 2. 显示污染面板（由Trigger完全控制）
        if (pollutionPanel != null)
            pollutionPanel.SetActive(true);
        // 3. 启动污染效果（确保面板已显示后调用）
        if (pollutionImage != null)
        {
            PollutionEffectController controller =
                pollutionImage.GetComponent<PollutionEffectController>();
            if (controller != null)
            {
                controller.StartPollution(); // 触发污染效果
            }
        }
    }
}