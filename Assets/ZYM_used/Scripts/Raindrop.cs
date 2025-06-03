using UnityEngine;

public class Raindrop : MonoBehaviour
{
    private float speed;           // ��������ٶ�
    private int damage;            // �����ɵ��˺�
    private LayerMask playerLayer; // ������ڵ�ͼ��

    public void Initialize(float speed, int damage, LayerMask playerLayer)
    {
        this.speed = speed;
        this.damage = damage;
        this.playerLayer = playerLayer;
    }

    public void Move()
    {
        // �ƶ����
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    public void CheckCollision()
    {
        // �����������ͷ���
        Vector2 rayStart = transform.position;
        Vector2 rayDirection = Vector2.down;
        float rayLength = 0.1f; // ���߳��ȣ��Դ�����εĸ߶�

        // �������߼��
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, playerLayer);

        // �������ߣ����ڱ༭���пɼ���
        Debug.DrawRay(rayStart, rayDirection * rayLength, Color.red);

        // ����Ƿ�������
        if (hit.collider != null)
        {
            // ��ȡ��ҵ�����ֵ���������˺�
            Health playerHealth = hit.collider.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // ��λ��к�����
            Destroy(gameObject);
        }
    }
}