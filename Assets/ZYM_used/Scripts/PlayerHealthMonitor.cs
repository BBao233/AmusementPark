using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthMonitor : MonoBehaviour
{
    [Header("�������")]
    public Health player1Health;  // ���1��Health���
    public Health player2Health;  // ���2��Health���

    [Header("��������")]
    public string gameOverSceneName = "GameOver";  // ��Ϸ������������
    public float sceneSwitchDelay = 2f;  // �������л��������ӳ�ʱ��

    private bool isChecking = true;

    void Update()
    {
        if (!isChecking) return;

        // �����������Ƿ�����
        if (player1Health.currentHealth <= 0 && player2Health.currentHealth <= 0)
        {
            AllPlayersDead();
        }
    }

    void AllPlayersDead()
    {
        isChecking = false;  // ֹͣ���
        Debug.Log("������Ҷ��������������л�����");

        // �ӳٺ��л�����
        Invoke("SwitchToGameOverScene", sceneSwitchDelay);
    }

    void SwitchToGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}
