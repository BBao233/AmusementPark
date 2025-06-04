using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject scythePrefab; // ����Ԥ����
    public float launchForce = 8f; // ��������
    public Transform[] playerTransforms; // ��ҵ�Transform����
    public float launchInterval = 1.3f; // ������
    public float activationRange = 6f; // �����ķ�Χ
    public LayerMask groundLayer; // ����ͼ��
    private float alltime;
    public static int flag = 0;
    

    private float timer;
    private bool isPlayerInRange;
    private Transform targetPlayer; // ��ǰĿ�����
    private Dictionary<Transform, float> playerEnterTime = new Dictionary<Transform, float>(); // ��¼��ҽ���ʱ��

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
                    // ��¼��ҽ���ʱ��
                    if (!playerEnterTime.ContainsKey(player))
                    {
                        playerEnterTime[player] = Time.time;
                    }

                    // ����Ƿ�Ϊ�����������
                    if (playerEnterTime[player] < earliestTime)
                    {
                        earliestTime = playerEnterTime[player];
                        earliestPlayer = player;
                        isPlayerInRange = true;
                    }
                }
                else
                {
                    // ����뿪��Χ���Ӽ�¼���Ƴ�
                    if (playerEnterTime.ContainsKey(player))
                    {
                        playerEnterTime.Remove(player);
                    }
                }
            }
        }

        // ����Ŀ�����
        targetPlayer = earliestPlayer;
    }

    private void ShootScythe()
    {
        if (scythePrefab == null || targetPlayer == null)
        {
            Debug.LogError("����Ԥ��������Transformδ���ã�");
            return;
        }

        // ʵ��������
        GameObject scythe = Instantiate(scythePrefab, transform.position, Quaternion.identity);

        // ȷ��ֻ���һ�����ٽű�ʵ��
        ScytheDestroyOnCollision destroyScript = scythe.GetComponent<ScytheDestroyOnCollision>();
        if (destroyScript == null)
        {
            destroyScript = scythe.AddComponent<ScytheDestroyOnCollision>();
        }
        destroyScript.groundLayer = groundLayer;
        destroyScript.boss = this; // ����Boss����

        // �����ת���ƽű�
        ScytheRotation scytheRotation = scythe.GetComponent<ScytheRotation>();
        if (scytheRotation == null)
        {
            scytheRotation = scythe.AddComponent<ScytheRotation>();
        }
        scytheRotation.rotationSpeed = 720f;

        Rigidbody2D scytheRb = scythe.GetComponent<Rigidbody2D>();
        if (scytheRb != null)
        {
            // ���㳯����ҵķ���
            Vector2 directionToPlayer = (Vector2)(targetPlayer.position - transform.position).normalized;
            scytheRb.velocity = directionToPlayer * launchForce;

            // ���ó�ʼ��ת����
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            scythe.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogError("����Ԥ����û��Rigidbody2D�����");
        }
    }
}

// ������ת���ƽű�
public class ScytheRotation : MonoBehaviour
{
    public float rotationSpeed = 720f; // ��ת�ٶȣ���/�룩

    private void Update()
    {
        // ��Z����ת��2D����ͨ����Z����ת��
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}

// �޸ĺ����ײ���ű�
public class ScytheDestroyOnCollision : MonoBehaviour
{
    public LayerMask groundLayer; // ����ͼ��
    public Animator scytheAnimator; // �����Ķ���������
    public float destroyDelay = 0.5f; // �����ӳ�ʱ�䣨�룩
    public Boss boss; // ����Boss�ű�

    private bool isDestroying = false; // �Ƿ�����������
    private Rigidbody2D rb;
    private Collider2D scytheCollider;
    private SpriteRenderer spriteRenderer; // ����������������������

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scytheCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��ȡ������Ⱦ��

        // ȷ������������������
        if (scytheAnimator == null)
        {
            scytheAnimator = GetComponent<Animator>();
            if (scytheAnimator == null)
            {
                Debug.LogWarning("����Ԥ����ȱ��Animator��������ٶ������޷�����");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ײ�����Ƿ�Ϊ����ͼ�㣬����������δ��������״̬
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !isDestroying)
        {
            StartDestroySequence();
        }
    }

    private void StartDestroySequence()
    {
        isDestroying = true;

        // ֹͣ�����˶�
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ������ײ���;�����Ⱦ����ֹ�Ӿ�����
        if (scytheCollider != null)
        {
            scytheCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // ������������
        }

        // ������ת�ű�
        ScytheRotation rotationScript = GetComponent<ScytheRotation>();
        if (rotationScript != null)
        {
            rotationScript.enabled = false;
        }

        // �������ٶ���������У�
        if (scytheAnimator != null)
        {
            // ����һ���Ӷ���������ʾ��ը��Ч
            GameObject explosionEffect = new GameObject("ExplosionEffect");
            explosionEffect.transform.position = transform.position;
            explosionEffect.transform.rotation = transform.rotation;
            explosionEffect.transform.SetParent(transform);

            // ��Ӿ�����Ⱦ��������ʾ��ը����
            SpriteRenderer effectRenderer = explosionEffect.AddComponent<SpriteRenderer>();
            effectRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            effectRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

            // ��Ӷ���������
            Animator effectAnimator = explosionEffect.AddComponent<Animator>();
            effectAnimator.runtimeAnimatorController = scytheAnimator.runtimeAnimatorController;

            // �������ٶ���
            effectAnimator.SetTrigger("Destroy");

            // ��������ʱ��
            Destroy(explosionEffect, destroyDelay);
        }

        // �ӳٺ����ٶ���
        Destroy(gameObject, destroyDelay);
    }
}