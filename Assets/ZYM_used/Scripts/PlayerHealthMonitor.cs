using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthMonitor : MonoBehaviour
{
    [Header("玩家设置")]
    public Health player1Health;  // 玩家1的Health组件
    public Health player2Health;  // 玩家2的Health组件

    [Header("场景设置")]
    public string gameOverSceneName = "GameOver";  // 游戏结束场景名称
    public float sceneSwitchDelay = 2f;  // 死亡后切换场景的延迟时间

    private bool isChecking = true;

    void Update()
    {
        if (!isChecking) return;

        // 检查两个玩家是否都死亡
        if (player1Health.currentHealth <= 0 && player2Health.currentHealth <= 0)
        {
            AllPlayersDead();
        }
    }

    void AllPlayersDead()
    {
        isChecking = false;  // 停止检测
        Debug.Log("所有玩家都已死亡，即将切换场景");

        // 延迟后切换场景
        Invoke("SwitchToGameOverScene", sceneSwitchDelay);
    }

    void SwitchToGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}
