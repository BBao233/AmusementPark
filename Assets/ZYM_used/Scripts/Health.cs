using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;

    [Header("����Sprite����")]
    public GameObject heart1;
    public GameObject heart2;

    [Header("��Ч����")]
    public AudioClip interactionSound;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;

    [Header("��������")]
    public Health sharedHealth; // ��������ˣ���ʹ���������Ѫ������

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
        // ����ӿ��������á�������
    }
}
