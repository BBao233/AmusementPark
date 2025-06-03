using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPushBox : MonoBehaviour
{
    [Header("�������")]
    public float pushSpeed = 1f;      // �ƶ��ٶ�
    public LayerMask boxLayer;        // �������ڵĲ�
    public float groundCheckDistance = 0.1f; // ����Ƿ�վ�������ϵľ���
    public Transform feetPosition;    // �����ж��Ƿ�վ�����ӻ������

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canPush = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckIfGrounded();
    }

    void FixedUpdate()
    {
        if (!canPush) return;

        float moveX = Input.GetAxisRaw("Horizontal");

        if (moveX != 0)
        {
            Vector2 direction = new Vector2(moveX, 0);

            // ���ǰ���Ƿ�������
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.6f, boxLayer);

            if (hit.collider != null)
            {
                Rigidbody2D boxRb = hit.collider.attachedRigidbody;

                if (boxRb != null)
                {
                    // �ƶ�����
                    boxRb.velocity = new Vector2(pushSpeed * moveX, boxRb.velocity.y);
                }
            }
        }
    }

    void CheckIfGrounded()
    {
        // �������߼���Ƿ�վ�ڵ��ϻ�������
        isGrounded = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance, ~boxLayer);

        if (!isGrounded)
        {
            // ���û��վ�ڵ��ϣ�����Ƿ�վ��������
            isGrounded = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance + 0.1f, boxLayer);
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}