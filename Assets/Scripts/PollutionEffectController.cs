using UnityEngine;
using UnityEngine.UI;

public class PollutionEffectController : MonoBehaviour
{
    public Image pollutionImage; // 污染图片组件
    public GameObject pollutionPanel; // 污染面板（用于最后隐藏）

    [Tooltip("污染持续时间（即从0到满的时间）")]
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
                0f); // 初始透明
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
            changeSpeed = -Time.deltaTime * 2f; // 同时按下，加速清除
        }
        else if (pressingE || pressingL)
        {
            changeSpeed = -Time.deltaTime; // 单独按下，正常清除
        }
        else
        {
            changeSpeed = Time.deltaTime; // 不按，继续污染
        }

        currentPollutionTime += changeSpeed;

        // 限制范围
        currentPollutionTime = Mathf.Clamp(currentPollutionTime, 0f, maxPollutionTime);

        // 计算透明度（0~1）
        float alpha = currentPollutionTime / maxPollutionTime;

        // 更新颜色
        if (pollutionImage != null)
        {
            Color color = pollutionImage.color;
            color.a = alpha;
            pollutionImage.color = color;
        }

        // 如果污染清除完毕
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

        Debug.Log("污染已清除！");
    }
}