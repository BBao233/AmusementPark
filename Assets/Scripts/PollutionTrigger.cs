using UnityEngine;
using UnityEngine.UI;

public class PollutionTrigger : MonoBehaviour
{
    // 污染面板组件（Screen Space - Overlay）
    public GameObject pollutionPanel; // 全屏污染面板
    public Image pollutionImage;      // 污染图片组件

    // 要隐藏的对话框部分
    public GameObject dialogPanel;    // 整个对话UI的根对象

    public void OnYesButtonClicked()
    {
        // 显示污染图片
        if (pollutionPanel != null && pollutionImage != null)
        {
            pollutionPanel.SetActive(true);
            pollutionImage.color = new Color(pollutionImage.color.r, pollutionImage.color.g, pollutionImage.color.b, 1f);
        }

        // 隐藏对话框
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }
}