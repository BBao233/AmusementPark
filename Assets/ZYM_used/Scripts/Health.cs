using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 2;  // 最大生命值
    public int currentHealth;  // 当前生命值

    [Header("爱心Sprite设置")]
    public GameObject heart1; // 第一个爱心GameObject
    public GameObject heart2; // 第二个爱心GameObject

    [Header("音效设置")]
    public AudioClip interactionSound;  // 交互音效
    [Range(0f, 1f)] public float volume = 1f;  // 音量控制滑块
    private AudioSource audioSource;    // 音频源组件

    void Start()
    {
        // 初始化生命值
        currentHealth = maxHealth;
        UpdateHealthText();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth > 0)
        {
            if (interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound, volume);
            }
        }
        // 减少生命值
        currentHealth -= damageAmount;

        // 确保生命值不低于0
        currentHealth = Mathf.Max(0, currentHealth);

        // 更新UI显示
        UpdateHealthText();

        // 检查角色是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthText()
    {
        // 更新爱心显示状态
        if (heart1 != null)
        {
            heart1.SetActive(currentHealth >= 1);
        }

        if (heart2 != null)
        {
            heart2.SetActive(currentHealth >= 2);
        }
    }

    void Die()
    {
        // 角色死亡逻辑
        Debug.Log("Player died!");

        // 可以在这里添加角色死亡的动画、音效或其他效果

        // 禁用角色控制
        // 例如：GetComponent<PlayerController>().enabled = false;

        // 或者重新加载关卡
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}