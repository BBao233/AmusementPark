using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthMonitor : MonoBehaviour
{
    [Header("����Ѫ������")]
    public Health sharedHealth;  // �����Health���

    [Header("��������")]
    public string gameOverSceneName = "GameOver";  // ��Ϸ������������
    public float sceneSwitchDelay = 2f;  // �������л��������ӳ�ʱ��

    private bool hasTriggeredGameOver = false;

    void Update()
    {
        if (hasTriggeredGameOver || sharedHealth == null) return;

        if (sharedHealth.currentHealth <= 0)
        {
            hasTriggeredGameOver = true;
            Debug.Log("����Ѫ���ľ����л�����Ϸ��������");
            Invoke("SwitchToGameOverScene", sceneSwitchDelay);
        }
    }

    void SwitchToGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}