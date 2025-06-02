using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject scythePrefab; // 镰刀预制体
    public float launchForce = 8f; // 发射力度
    public Transform playerTransform; // 玩家的Transform
    public float launchInterval = 1.3f; // 发射间隔
    public float activationRange = 6f; // 激活发射的范围
    public LayerMask groundLayer; // 地面图层
    private float alltime;
    public static int flag = 0;

    private float timer;
    private bool isPlayerInRange;

    private void Start()
    {
        alltime = 0;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerInRange = distance <= activationRange;
        }

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

    private void ShootScythe()
    {
        if (scythePrefab == null || playerTransform == null)
        {
            Debug.LogError("镰刀预制体或玩家Transform未设置！");
            return;
        }

        // 实例化镰刀
        GameObject scythe = Instantiate(scythePrefab, transform.position, Quaternion.identity);

        // 添加旋转控制脚本
        ScytheRotation scytheRotation = scythe.AddComponent<ScytheRotation>();
        scytheRotation.rotationSpeed = 720f; // 设置旋转速度（可调整）

        Rigidbody2D scytheRb = scythe.GetComponent<Rigidbody2D>();
        if (scytheRb != null)
        {
            // 计算朝向玩家的方向
            Vector2 directionToPlayer = (Vector2)(playerTransform.position - transform.position).normalized;
            scytheRb.velocity = directionToPlayer * launchForce;

            // 设置初始旋转方向
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            scythe.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogError("镰刀预制体没有Rigidbody2D组件！");
        }

        // 添加碰撞检测脚本（传递地面图层）
        ScytheDestroyOnCollision destroyScript = scythe.AddComponent<ScytheDestroyOnCollision>();
        destroyScript.groundLayer = groundLayer;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为地面图层
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}