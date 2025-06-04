using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthShow : MonoBehaviour
{
    public TMP_Text healthText;
    public fly bossController; // ����BOSS������

    private void Start()
    {
        UpdateHealthText();
    }

    private void Update()
    {
        // ����BOSS����ʱ����Ѫ��
        if (bossController != null)
        {
            UpdateHealthText();
        }
        else
        {
            healthText.text = "0" + " / " + "100";
        }
    }

    void UpdateHealthText()
    {
        if (bossController != null && healthText != null)
        {
            // ��BOSS��������ȡ����ֵ����
            float currentHealth = bossController.GetCurrentHealth();
            float maxHealth = bossController.GetMaxHealth();

            // �����ı���ʾ
            healthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0");
        }
    }
}
