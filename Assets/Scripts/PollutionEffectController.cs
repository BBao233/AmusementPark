using UnityEngine;
using UnityEngine.UI;
public class PollutionEffectController : MonoBehaviour
{
    public Image pollutionImage; // 污染图片组件
    public GameObject pollutionPanel; // 污染面板（由外部触发显示，内部不主动隐藏）
    [Tooltip("清除污染所需时间（秒）")]
    public float maxClearTime = 10f;
    public float clearSpeedSingle = 1.5f; // 单键清除速度
    public float clearSpeedDouble = 3f; // 双键清除速度
    private float currentPollutionValue = 1f; // 初始污染值100%
    private bool isPollutionActive = false; // 污染是否激活
    void Awake()
    {
        // 仅初始化污染值，不主动隐藏面板（由外部控制）
        currentPollutionValue = 1f;
        isPollutionActive = false;
        if (pollutionImage != null)
        {
            // 初始透明（但面板未显示时，透明不影响）
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
        currentPollutionValue = 1f; // 重置为100%污染
        // 立即显示完全不透明的污染图片
        if (pollutionImage != null)
        {
            pollutionImage.color = new Color(
                pollutionImage.color.r,
                pollutionImage.color.g,
                pollutionImage.color.b,
                1f // 完全不透明
            );
        }
    }
    private void HandlePollutionClear(bool p1Press, bool p2Press)
    {
        float delta = Time.deltaTime / maxClearTime; // 标准化清除速度
        if (p1Press || p2Press)
        {
            if (p1Press && p2Press) // 双键加速清除
            {
                currentPollutionValue -= delta * clearSpeedDouble;
            }
            else // 单键清除
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
            currentPollutionValue // 污染值直接对应透明度
        );
    }
    private void EndPollution()
    {
        isPollutionActive = false;
        // 清除完成后隐藏面板（而不是在初始化时隐藏）
        if (pollutionPanel != null)
        {
            pollutionPanel.SetActive(false);
        }
        Debug.Log("污染已清除！");
    }
}