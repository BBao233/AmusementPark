using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPushBox : MonoBehaviour
{
    [Header("玩家设置")]
    public float pushSpeed = 1f;      // 推动速度
    public LayerMask boxLayer;        // 箱子所在的层
    public float groundCheckDistance = 0.1f; // 检查是否站在箱子上的距离
    public Transform feetPosition;    // 用于判断是否站在箱子或地面上

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

            // 检测前方是否有箱子
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.6f, boxLayer);

            if (hit.collider != null)
            {
                Rigidbody2D boxRb = hit.collider.attachedRigidbody;

                if (boxRb != null)
                {
                    // 推动箱子
                    boxRb.velocity = new Vector2(pushSpeed * moveX, boxRb.velocity.y);
                }
            }
        }
    }

    void CheckIfGrounded()
    {
        // 向下射线检测是否站在地上或箱子上
        isGrounded = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance, ~boxLayer);

        if (!isGrounded)
        {
            // 如果没有站在地上，检查是否站在箱子上
            isGrounded = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance + 0.1f, boxLayer);
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}