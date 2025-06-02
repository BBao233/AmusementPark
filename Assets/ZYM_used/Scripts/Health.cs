using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 2;  // �������ֵ
    public int currentHealth;  // ��ǰ����ֵ

    [Header("����Sprite����")]
    public GameObject heart1; // ��һ������GameObject
    public GameObject heart2; // �ڶ�������GameObject

    [Header("��Ч����")]
    public AudioClip interactionSound;  // ������Ч
    [Range(0f, 1f)] public float volume = 1f;  // �������ƻ���
    private AudioSource audioSource;    // ��ƵԴ���

    void Start()
    {
        // ��ʼ������ֵ
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
        // ��������ֵ
        currentHealth -= damageAmount;

        // ȷ������ֵ������0
        currentHealth = Mathf.Max(0, currentHealth);

        // ����UI��ʾ
        UpdateHealthText();

        // ����ɫ�Ƿ�����
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthText()
    {
        // ���°�����ʾ״̬
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
        // ��ɫ�����߼�
        Debug.Log("Player died!");

        // ������������ӽ�ɫ�����Ķ�������Ч������Ч��

        // ���ý�ɫ����
        // ���磺GetComponent<PlayerController>().enabled = false;

        // �������¼��عؿ�
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}