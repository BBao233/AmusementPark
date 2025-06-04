using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject scythePrefab; // 镰刀预制体
    public float launchForce = 8f; // 发射力度
    public Transform[] playerTransforms; // 玩家的Transform数组
    public float launchInterval = 1.3f; // 发射间隔
    public float activationRange = 6f; // 激活发射的范围
    public LayerMask groundLayer; // 地面图层
    private float alltime;
    public static int flag = 0;
    

    private float timer;
    private bool isPlayerInRange;
    private Transform targetPlayer; // 当前目标玩家
    private Dictionary<Transform, float> playerEnterTime = new Dictionary<Transform, float>(); // 记录玩家进入时间

    private void Start()
    {
        alltime = 0;
    }

    private void Update()
    {
        flag = fly.bossFlag;
        if(flag == 1 )
        {
            Destroy( gameObject );
        }
        CheckPlayersInRange();

        if (isPlayerInRange)
        {
            alltime += Time.deltaTime;
            if (alltime >= 10)
            {
                flag = 1;
            }
            timer += Time.deltaTime;
            if (timer >= launchInterval)
            {
                ShootScythe();
                timer = 0;
            }
        }
    }

    private void CheckPlayersInRange()
    {
        isPlayerInRange = false;
        float earliestTime = float.MaxValue;
        Transform earliestPlayer = null;

        foreach (Transform player in playerTransforms)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance <= activationRange)
                {
                    // 记录玩家进入时间
                    if (!playerEnterTime.ContainsKey(player))
                    {
                        playerEnterTime[player] = Time.time;
                    }

                    // 检查是否为最早进入的玩家
                    if (playerEnterTime[player] < earliestTime)
                    {
                        earliestTime = playerEnterTime[player];
                        earliestPlayer = player;
                        isPlayerInRange = true;
                    }
                }
                else
                {
                    // 玩家离开范围，从记录中移除
                    if (playerEnterTime.ContainsKey(player))
                    {
                        playerEnterTime.Remove(player);
                    }
                }
            }
        }

        // 更新目标玩家
        targetPlayer = earliestPlayer;
    }

    private void ShootScythe()
    {
        if (scythePrefab == null || targetPlayer == null)
        {
            Debug.LogError("镰刀预制体或玩家Transform未设置！");
            return;
        }

        // 实例化镰刀
        GameObject scythe = Instantiate(scythePrefab, transform.position, Quaternion.identity);

        // 确保只添加一个销毁脚本实例
        ScytheDestroyOnCollision destroyScript = scythe.GetComponent<ScytheDestroyOnCollision>();
        if (destroyScript == null)
        {
            destroyScript = scythe.AddComponent<ScytheDestroyOnCollision>();
        }
        destroyScript.groundLayer = groundLayer;
        destroyScript.boss = this; // 传递Boss引用

        // 添加旋转控制脚本
        ScytheRotation scytheRotation = scythe.GetComponent<ScytheRotation>();
        if (scytheRotation == null)
        {
            scytheRotation = scythe.AddComponent<ScytheRotation>();
        }
        scytheRotation.rotationSpeed = 720f;

        Rigidbody2D scytheRb = scythe.GetComponent<Rigidbody2D>();
        if (scytheRb != null)
        {
            // 计算朝向玩家的方向
            Vector2 directionToPlayer = (Vector2)(targetPlayer.position - transform.position).normalized;
            scytheRb.velocity = directionToPlayer * launchForce;

            // 设置初始旋转方向
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            scythe.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogError("镰刀预制体没有Rigidbody2D组件！");
        }
    }
}

// 镰刀旋转控制脚本
public class ScytheRotation : MonoBehaviour
{
    public float rotationSpeed = 720f; // 旋转速度（度/秒）

    private void Update()
    {
        // 绕Z轴旋转（2D场景通常绕Z轴旋转）
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}

// 修改后的碰撞检测脚本
public class ScytheDestroyOnCollision : MonoBehaviour
{
    public LayerMask groundLayer; // 地面图层
    public Animator scytheAnimator; // 镰刀的动画控制器
    public float destroyDelay = 0.5f; // 销毁延迟时间（秒）
    public Boss boss; // 引用Boss脚本

    private bool isDestroying = false; // 是否正在销毁中
    private Rigidbody2D rb;
    private Collider2D scytheCollider;
    private SpriteRenderer spriteRenderer; // 新增：用于隐藏镰刀精灵

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scytheCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取精灵渲染器

        // 确保动画控制器已设置
        if (scytheAnimator == null)
        {
            scytheAnimator = GetComponent<Animator>();
            if (scytheAnimator == null)
            {
                Debug.LogWarning("镰刀预制体缺少Animator组件，销毁动画将无法播放");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为地面图层，并且镰刀尚未处于销毁状态
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !isDestroying)
        {
            StartDestroySequence();
        }
    }

    private void StartDestroySequence()
    {
        isDestroying = true;

        // 停止镰刀运动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 禁用碰撞器和精灵渲染，防止视觉残留
        if (scytheCollider != null)
        {
            scytheCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // 立即隐藏镰刀
        }

        // 禁用旋转脚本
        ScytheRotation rotationScript = GetComponent<ScytheRotation>();
        if (rotationScript != null)
        {
            rotationScript.enabled = false;
        }

        // 播放销毁动画（如果有）
        if (scytheAnimator != null)
        {
            // 创建一个子对象用于显示爆炸特效
            GameObject explosionEffect = new GameObject("ExplosionEffect");
            explosionEffect.transform.position = transform.position;
            explosionEffect.transform.rotation = transform.rotation;
            explosionEffect.transform.SetParent(transform);

            // 添加精灵渲染器用于显示爆炸动画
            SpriteRenderer effectRenderer = explosionEffect.AddComponent<SpriteRenderer>();
            effectRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            effectRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

            // 添加动画控制器
            Animator effectAnimator = explosionEffect.AddComponent<Animator>();
            effectAnimator.runtimeAnimatorController = scytheAnimator.runtimeAnimatorController;

            // 播放销毁动画
            effectAnimator.SetTrigger("Destroy");

            // 设置销毁时间
            Destroy(explosionEffect, destroyDelay);
        }

        // 延迟后销毁对象
        Destroy(gameObject, destroyDelay);
    }
}