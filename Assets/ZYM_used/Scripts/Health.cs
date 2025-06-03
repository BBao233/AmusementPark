using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;

    [Header("爱心Sprite设置")]
    public GameObject heart1;
    public GameObject heart2;

    [Header("音效设置")]
    public AudioClip interactionSound;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;

    [Header("共享设置")]
    public Health sharedHealth; // 如果设置了，就使用这个共享血量对象

    void Start()
    {
        if (sharedHealth == null)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthText();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    public void TakeDamage(int damageAmount)
    {
        if (sharedHealth != null)
        {
            sharedHealth.TakeDamage(damageAmount);
            return;
        }

        if (currentHealth > 0 && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, volume);
        }
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthText()
    {
        if (sharedHealth != null)
        {
            sharedHealth.UpdateHealthText();
            return;
        }

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
        Debug.Log("Player died!");
        // 可添加控制器禁用、动画等
    }
}
