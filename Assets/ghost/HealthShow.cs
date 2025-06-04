using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthShow : MonoBehaviour
{
    public TMP_Text healthText;
    public fly bossController; // 引用BOSS控制器

    private void Start()
    {
        UpdateHealthText();
    }

    private void Update()
    {
        // 仅当BOSS存在时更新血条
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
            // 从BOSS控制器获取生命值数据
            float currentHealth = bossController.GetCurrentHealth();
            float maxHealth = bossController.GetMaxHealth();

            // 更新文本显示
            healthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString("F0");
        }
    }
}
