using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject scythePrefab; // ����Ԥ����
    public float launchForce = 8f; // ��������
    public Transform playerTransform; // ��ҵ�Transform
    public float launchInterval = 1.3f; // ������
    public float activationRange = 6f; // �����ķ�Χ
    public LayerMask groundLayer; // ����ͼ��
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
            Debug.LogError("����Ԥ��������Transformδ���ã�");
            return;
        }

        // ʵ��������
        GameObject scythe = Instantiate(scythePrefab, transform.position, Quaternion.identity);

        // �����ת���ƽű�
        ScytheRotation scytheRotation = scythe.AddComponent<ScytheRotation>();
        scytheRotation.rotationSpeed = 720f; // ������ת�ٶȣ��ɵ�����

        Rigidbody2D scytheRb = scythe.GetComponent<Rigidbody2D>();
        if (scytheRb != null)
        {
            // ���㳯����ҵķ���
            Vector2 directionToPlayer = (Vector2)(playerTransform.position - transform.position).normalized;
            scytheRb.velocity = directionToPlayer * launchForce;

            // ���ó�ʼ��ת����
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            scythe.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogError("����Ԥ����û��Rigidbody2D�����");
        }

        // �����ײ���ű������ݵ���ͼ�㣩
        ScytheDestroyOnCollision destroyScript = scythe.AddComponent<ScytheDestroyOnCollision>();
        destroyScript.groundLayer = groundLayer;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ײ�����Ƿ�Ϊ����ͼ��
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}