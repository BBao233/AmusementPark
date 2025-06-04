using UnityEngine;

public class LadderClimbing2D_Exit : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask ladderLayer;

    [Header("爬梯参数")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float ladderOffset = 0.2f;

    // 是否在梯子上
    private bool isOnLadder;
    // 梯子位置
    private float ladderXPosition;
    // 当前梯子碰撞器
    private Collider2D currentLadderCollider;

    private void Update()
    {
        if (isOnLadder)
        {
            ClimbLadder();

            // 随时可以通过跳跃退出
            if (Input.GetButtonDown("Jump"))
            {
                ExitLadder();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检测是否碰到梯子
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
        // 离开梯子碰撞区域时
        if ((ladderLayer & (1 << collision.gameObject.layer)) != 0 &&
            collision == currentLadderCollider)
        {
            // 如果没有向上输入，退出梯子
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

        // 获取梯子的X位置并添加偏移
        ladderXPosition = ladderCollider.bounds.center.x + ladderOffset * Mathf.Sign(transform.position.x - ladderCollider.bounds.center.x);

        // 禁用重力
        rb.gravityScale = 0;

        // 更新动画 - 仅使用isClimbing参数
        if (animator != null)
        {
            animator.SetBool("isClimbing", true);
        }
    }

    private void ClimbLadder()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // 有垂直输入时，锁定在梯子上并移动
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            // 锁定X位置到梯子
            Vector2 newPosition = new Vector2(ladderXPosition, transform.position.y);
            transform.position = newPosition;

            // 设置垂直速度
            rb.velocity = new Vector2(0, verticalInput * climbSpeed);
        }
        else
        {
            // 无输入时，允许玩家脱离梯子（但保持爬梯状态）
            rb.velocity = Vector2.zero;
        }
    }

    private void ExitLadder()
    {
        isOnLadder = false;
        currentLadderCollider = null;

        // 恢复重力
        rb.gravityScale = 1;

        // 应用微小的向上速度，避免立即踩到梯子底部碰撞器
        rb.velocity = new Vector2(rb.velocity.x, 0.5f);

        // 更新动画 - 仅使用isClimbing参数
        if (animator != null)
        {
            animator.SetBool("isClimbing", false);
        }
    }
}