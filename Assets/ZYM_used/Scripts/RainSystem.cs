using UnityEngine;

public class RainSystem : MonoBehaviour
{
    [Header("雨滴设置")]
    public GameObject rainDropPrefab;
    public float spawnRate = 0.1f;
    public float dropSpeed = 5f;
    public int damage = 1;
    public LayerMask playerLayer;

    [Header("雨区设置")]
    public float rainAreaWidth = 10f;
    public float rainAreaHeight = 2f;
    public Vector3 rainAreaCenter;

    [Header("定时设置")]
    public float rainDuration = 5f;
    private bool isRaining = false;
    private float rainTimer = 0f;
    private float nextSpawnTime;

    [Header("音效设置")]
    public AudioClip rainSound;      // 下雨音效
    [Range(0f, 1f)] public float volume = 1f; // 音量大小
    private AudioSource audioSource; // 音频源组件

    void Start()
    {
        // 初始化音频源
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;    // 循环播放
        audioSource.volume = volume;
    }

    void Update()
    {
        if (isRaining)
        {
            rainTimer -= Time.deltaTime;

            // 雨停止条件
            if (rainTimer <= 0f)
            {
                StopRain();
                return;
            }

            // 生成雨滴
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
        if (isRaining) return; // 已经在下雨则不再重复启动

        isRaining = true;
        rainTimer = rainDuration;
        Debug.Log($"开始下雨，持续{rainDuration}秒");

        // 播放雨声音效
        if (rainSound != null)
        {
            audioSource.clip = rainSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("未设置下雨音效！");
        }
    }

    public void StopRain()
    {
        isRaining = false;
        Debug.Log("雨已停止");

        // 停止雨声音效
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void SpawnRaindrop()
    {
        if (rainDropPrefab == null)
        {
            Debug.LogError("未设置雨滴预制体！");
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
        Destroy(drop, 10f); // 10秒后自动销毁
    }

    void UpdateRaindrops()
    {
        // 获取所有活动的雨滴
        Raindrop[] raindrops = FindObjectsOfType<Raindrop>();

        foreach (Raindrop raindrop in raindrops)
        {
            // 更新雨滴位置
            raindrop.Move();

            // 进行射线检测
            raindrop.CheckCollision();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rainAreaCenter, new Vector3(rainAreaWidth, rainAreaHeight, 0.1f));
    }
}