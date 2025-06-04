using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class fly : MonoBehaviour
{
    // 移动参数
    public float speed = 4f;
    public float minX = -2.35f;
    public float maxX = 2.35f;
    public float minY = -1f;
    public float maxY = 1.5f;
    public static int bossFlag;

    // 新增：通关门引用
    public GameObject exitDoor;

    // 生命值参数
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider; // 引用血条Slider

    private Vector2 currentDirection;
    private float directionChangeInterval = 1.2f;
    private float timeSinceLastDirectionChange = 0f;

    private void Start()
    {
        bossFlag = 0;
        currentHealth = maxHealth;
        currentDirection = Random.insideUnitCircle.normalized;

        // 初始化血条
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 确保通关门初始状态为禁用
        if (exitDoor != null)
            exitDoor.SetActive(false);
    }

    private void Update()
    {
        // 移动逻辑
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange >= directionChangeInterval)
        {
            currentDirection = Random.insideUnitCircle.normalized;
            timeSinceLastDirectionChange = 0f;
        }

        Vector2 movement = currentDirection * speed * Time.deltaTime;
        Vector2 newPosition = (Vector2)transform.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;

        // 方向翻转
        if (currentDirection.x < 0)
        {
            transform.localScale = new Vector3(1.4f, transform.localScale.y, transform.localScale.z);
        }
        else if (currentDirection.x > 0)
        {
            transform.localScale = new Vector3(-1.4f, transform.localScale.y, transform.localScale.z);
        }
    }

    // 减少BOSS生命值
    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);

        // 更新血条
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            // 激活通关门
            if (exitDoor != null)
                exitDoor.SetActive(true);

            bossFlag = 1;
            // BOSS死亡逻辑
            Destroy(gameObject);
        }
    }

    // 获取当前生命值（供血条文本显示使用）
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // 获取最大生命值（供血条文本显示使用）
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}