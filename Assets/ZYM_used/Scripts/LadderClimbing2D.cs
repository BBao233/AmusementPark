using UnityEngine;

public class LadderClimbing2D_Exit : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask ladderLayer;

    [Header("���ݲ���")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float ladderOffset = 0.2f;

    // �Ƿ���������
    private bool isOnLadder;
    // ����λ��
    private float ladderXPosition;
    // ��ǰ������ײ��
    private Collider2D currentLadderCollider;

    private void Update()
    {
        if (isOnLadder)
        {
            ClimbLadder();

            // ��ʱ����ͨ����Ծ�˳�
            if (Input.GetButtonDown("Jump"))
            {
                ExitLadder();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����Ƿ���������
        if ((ladderLayer & (1 << collision.gameObject.layer)) != 0)
        {
            if (Input.GetAxisRaw("Vertical") >= 0)
            {
                EnterLadder(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �뿪������ײ����ʱ
        if ((ladderLayer & (1 << collision.gameObject.layer)) != 0 &&
            collision == currentLadderCollider)
        {
            // ���û���������룬�˳�����
            if (Input.GetAxisRaw("Vertical") <= 0)
            {
                ExitLadder();
            }
        }
    }

    private void EnterLadder(Collider2D ladderCollider)
    {
        isOnLadder = true;
        currentLadderCollider = ladderCollider;

        // ��ȡ���ӵ�Xλ�ò����ƫ��
        ladderXPosition = ladderCollider.bounds.center.x + ladderOffset * Mathf.Sign(transform.position.x - ladderCollider.bounds.center.x);

        // ��������
        rb.gravityScale = 0;

        // ���¶��� - ��ʹ��isClimbing����
        if (animator != null)
        {
            animator.SetBool("isClimbing", true);
        }
    }

    private void ClimbLadder()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // �д�ֱ����ʱ�������������ϲ��ƶ�
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            // ����Xλ�õ�����
            Vector2 newPosition = new Vector2(ladderXPosition, transform.position.y);
            transform.position = newPosition;

            // ���ô�ֱ�ٶ�
            rb.velocity = new Vector2(0, verticalInput * climbSpeed);
        }
        else
        {
            // ������ʱ����������������ӣ�����������״̬��
            rb.velocity = Vector2.zero;
        }
    }

    private void ExitLadder()
    {
        isOnLadder = false;
        currentLadderCollider = null;

        // �ָ�����
        rb.gravityScale = 1;

        // Ӧ��΢С�������ٶȣ����������ȵ����ӵײ���ײ��
        rb.velocity = new Vector2(rb.velocity.x, 0.5f);

        // ���¶��� - ��ʹ��isClimbing����
        if (animator != null)
        {
            animator.SetBool("isClimbing", false);
        }
    }
}