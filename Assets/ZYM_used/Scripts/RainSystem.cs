using UnityEngine;

public class RainSystem : MonoBehaviour
{
    [Header("�������")]
    public GameObject rainDropPrefab;
    public float spawnRate = 0.1f;
    public float dropSpeed = 5f;
    public int damage = 1;
    public LayerMask playerLayer;

    [Header("��������")]
    public float rainAreaWidth = 10f;
    public float rainAreaHeight = 2f;
    public Vector3 rainAreaCenter;

    [Header("��ʱ����")]
    public float rainDuration = 5f;
    private bool isRaining = false;
    private float rainTimer = 0f;
    private float nextSpawnTime;

    [Header("��Ч����")]
    public AudioClip rainSound;      // ������Ч
    [Range(0f, 1f)] public float volume = 1f; // ������С
    private AudioSource audioSource; // ��ƵԴ���

    void Start()
    {
        // ��ʼ����ƵԴ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;    // ѭ������
        audioSource.volume = volume;
    }

    void Update()
    {
        if (isRaining)
        {
            rainTimer -= Time.deltaTime;

            // ��ֹͣ����
            if (rainTimer <= 0f)
            {
                StopRain();
                return;
            }

            // �������
            if (Time.time >= nextSpawnTime)
            {
                SpawnRaindrop();
                nextSpawnTime = Time.time + spawnRate;
            }
        }
        UpdateRaindrops();
    }
    
    public void StartRain()
    {
        if (isRaining) return; // �Ѿ������������ظ�����

        isRaining = true;
        rainTimer = rainDuration;
        Debug.Log($"��ʼ���꣬����{rainDuration}��");

        // ����������Ч
        if (rainSound != null)
        {
            audioSource.clip = rainSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("δ����������Ч��");
        }
    }

    public void StopRain()
    {
        isRaining = false;
        Debug.Log("����ֹͣ");

        // ֹͣ������Ч
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void SpawnRaindrop()
    {
        if (rainDropPrefab == null)
        {
            Debug.LogError("δ�������Ԥ���壡");
            return;
        }

        Vector3 spawnPos = new Vector3(
            rainAreaCenter.x + Random.Range(-rainAreaWidth / 2, rainAreaWidth / 2),
            rainAreaCenter.y + rainAreaHeight / 2,
            rainAreaCenter.z
        );

        GameObject drop = Instantiate(rainDropPrefab, spawnPos, Quaternion.identity);
        Raindrop raindrop = drop.AddComponent<Raindrop>();
        raindrop.Initialize(dropSpeed, damage, playerLayer);
        Destroy(drop, 10f); // 10����Զ�����
    }

    void UpdateRaindrops()
    {
        // ��ȡ���л�����
        Raindrop[] raindrops = FindObjectsOfType<Raindrop>();

        foreach (Raindrop raindrop in raindrops)
        {
            // �������λ��
            raindrop.Move();

            // �������߼��
            raindrop.CheckCollision();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rainAreaCenter, new Vector3(rainAreaWidth, rainAreaHeight, 0.1f));
    }
}