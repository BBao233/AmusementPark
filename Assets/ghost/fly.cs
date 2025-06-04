using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class fly : MonoBehaviour
{
    // �ƶ�����
    public float speed = 4f;
    public float minX = -2.35f;
    public float maxX = 2.35f;
    public float minY = -1f;
    public float maxY = 1.5f;
    public static int bossFlag;

    // ������ͨ��������
    public GameObject exitDoor;

    // ����ֵ����
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider; // ����Ѫ��Slider

    private Vector2 currentDirection;
    private float directionChangeInterval = 1.2f;
    private float timeSinceLastDirectionChange = 0f;

    private void Start()
    {
        bossFlag = 0;
        currentHealth = maxHealth;
        currentDirection = Random.insideUnitCircle.normalized;

        // ��ʼ��Ѫ��
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // ȷ��ͨ���ų�ʼ״̬Ϊ����
        if (exitDoor != null)
            exitDoor.SetActive(false);
    }

    private void Update()
    {
        // �ƶ��߼�
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

        // ����ת
        if (currentDirection.x < 0)
        {
            transform.localScale = new Vector3(1.4f, transform.localScale.y, transform.localScale.z);
        }
        else if (currentDirection.x > 0)
        {
            transform.localScale = new Vector3(-1.4f, transform.localScale.y, transform.localScale.z);
        }
    }

    // ����BOSS����ֵ
    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);

        // ����Ѫ��
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            // ����ͨ����
            if (exitDoor != null)
                exitDoor.SetActive(true);

            bossFlag = 1;
            // BOSS�����߼�
            Destroy(gameObject);
        }
    }

    // ��ȡ��ǰ����ֵ����Ѫ���ı���ʾʹ�ã�
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // ��ȡ�������ֵ����Ѫ���ı���ʾʹ�ã�
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}