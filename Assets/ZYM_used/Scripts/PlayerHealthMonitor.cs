using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthMonitor : MonoBehaviour
{
    [Header("共享血量设置")]
    public Health sharedHealth;  // 共享的Health组件

    [Header("场景设置")]
    public string gameOverSceneName = "GameOver";  // 游戏结束场景名称
    public float sceneSwitchDelay = 2f;  // 死亡后切换场景的延迟时间

    private bool hasTriggeredGameOver = false;

    void Update()
    {
        if (hasTriggeredGameOver || sharedHealth == null) return;

        if (sharedHealth.currentHealth <= 0)
        {
            hasTriggeredGameOver = true;
            Debug.Log("共享血量耗尽，切换至游戏结束场景");
            Invoke("SwitchToGameOverScene", sceneSwitchDelay);
        }
    }

    void SwitchToGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}