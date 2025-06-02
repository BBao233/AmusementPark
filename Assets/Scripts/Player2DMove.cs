using UnityEngine;

public class Player2DMove : MonoBehaviour
{
    public enum PlayerID { Player1, Player2 }

    [Header("�������")]
    public PlayerID playerID = PlayerID.Player1;

    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("��ȫ�߾�����")]
    public Vector2 safeAreaMargin = new Vector2(0.1f, 0.1f); // ��ȫ�߾ࣨ�ӿ����꣩

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpCount = 0;

    // ȫ�ֱ߽�����״̬
    private static class BoundaryLock
    {
        public static bool Player1BlockedTop = false;
        public static bool Player1BlockedBottom = false;
        public static bool Player1BlockedLeft = false;
        public static bool Player1BlockedRight = false;

        public static bool Player2BlockedTop = false;
        public static bool Player2BlockedBottom = false;
        public static bool Player2BlockedLeft = false;
        public static bool Player2BlockedRight = false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = rb.velocity.y;

        if (playerID == PlayerID.Player1)
        {
            moveX = Input.GetAxisRaw("P1_Horizontal"); // A/D
        }
        else if (playerID == PlayerID.Player2)
        {
            moveX = Input.GetAxisRaw("P2_Horizontal"); // �� / ��
        }

        // Ԥ����λ��
        Vector2 predictedVelocity = new Vector2(moveX * moveSpeed, moveY);
        Vector3 predictedPosition = transform.position + new Vector3(predictedVelocity.x * Time.deltaTime, predictedVelocity.y * Time.deltaTime, 0);

        // ������������ɫ�� z ����
        float zDistance = transform.position.z - Camera.main.transform.position.z;

        // ���ӿڱ߾�ת��Ϊ��������
        Vector3 leftBorderWorld = Camera.main.ViewportToWorldPoint(new Vector3(safeAreaMargin.x, 0, zDistance));
        Vector3 rightBorderWorld = Camera.main.ViewportToWorldPoint(new Vector3(1 - safeAreaMargin.x, 0, zDistance));
        Vector3 bottomBorderWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, safeAreaMargin.y, zDistance));
        Vector3 topBorderWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, 1 - safeAreaMargin.y, zDistance));

        float leftBorder = leftBorderWorld.x;
        float rightBorder = rightBorderWorld.x;
        float bottomBorder = bottomBorderWorld.y;
        float topBorder = topBorderWorld.y;

        // ��ʼ���Ƿ��赲��־
        bool blockedLeft = false;
        bool blockedRight = false;
        bool blockedTop = false;
        bool blockedBottom = false;

        // ���±߽�״̬
        if (playerID == PlayerID.Player2)
        {
            // �ж� Player2 �Ƿ�Խ��
            blockedLeft = predictedPosition.x < leftBorder && moveX < 0;
            blockedRight = predictedPosition.x > rightBorder && moveX > 0;
            blockedBottom = predictedPosition.y < bottomBorder && moveY < 0;
            blockedTop = predictedPosition.y > topBorder && moveY > 0;

            // ����״̬
            BoundaryLock.Player2BlockedLeft = predictedPosition.x < leftBorder;
            BoundaryLock.Player2BlockedRight = predictedPosition.x > rightBorder;
            BoundaryLock.Player2BlockedBottom = predictedPosition.y < bottomBorder;
            BoundaryLock.Player2BlockedTop = predictedPosition.y > topBorder;

            // ����ֹ�����ƶ������������ƶ�
            if (blockedLeft || blockedRight || blockedBottom || blockedTop)
            {
                if (blockedLeft) predictedVelocity.x = 0;
                if (blockedRight) predictedVelocity.x = 0;
                if (blockedBottom) predictedVelocity.y = 0;
                if (blockedTop) predictedVelocity.y = 0;
            }

            rb.velocity = predictedVelocity;
        }
        else if (playerID == PlayerID.Player1)
        {
            // �ж� Player1 �Ƿ�Խ��
            blockedLeft = predictedPosition.x < leftBorder && moveX < 0;
            blockedRight = predictedPosition.x > rightBorder && moveX > 0;
            blockedBottom = predictedPosition.y < bottomBorder && moveY < 0;
            blockedTop = predictedPosition.y > topBorder && moveY > 0;

            // ����״̬
            BoundaryLock.Player1BlockedLeft = predictedPosition.x < leftBorder;
            BoundaryLock.Player1BlockedRight = predictedPosition.x > rightBorder;
            BoundaryLock.Player1BlockedBottom = predictedPosition.y < bottomBorder;
            BoundaryLock.Player1BlockedTop = predictedPosition.y > topBorder;

            // ����Ƿ񱻶Է��赲��˫�����ƣ�
            bool shouldBlockMove = false;

            // X�᷽��
            if ((BoundaryLock.Player2BlockedLeft && moveX > 0) ||
                (BoundaryLock.Player2BlockedRight && moveX < 0))
            {
                shouldBlockMove = true;
            }

            // Y�᷽�� - �޸����߼�
            if ((BoundaryLock.Player2BlockedTop && moveY < 0) ||    // Player2���ϱ߽磬Player1��������
                (BoundaryLock.Player2BlockedBottom && moveY > 0))  // Player2���±߽磬Player1��������
            {
                shouldBlockMove = true;
            }

            // ͬ������Լ��Ƿ��ڱ߽磬��ֹ������Զ
            if ((BoundaryLock.Player1BlockedLeft && moveX > 0) ||
                (BoundaryLock.Player1BlockedRight && moveX < 0) ||
                (BoundaryLock.Player1BlockedTop && moveY < 0) ||
                (BoundaryLock.Player1BlockedBottom && moveY > 0))
            {
                shouldBlockMove = true;
            }

            if (!shouldBlockMove)
            {
                rb.velocity = predictedVelocity;
            }
            else
            {
                // �޸��������赲ʱ��ֻ���Ʊ��赲������ƶ�
                if (shouldBlockMove)
                {
                    if ((BoundaryLock.Player2BlockedLeft && moveX > 0) ||
                        (BoundaryLock.Player2BlockedRight && moveX < 0) ||
                        (BoundaryLock.Player1BlockedLeft && moveX > 0) ||
                        (BoundaryLock.Player1BlockedRight && moveX < 0))
                    {
                        predictedVelocity.x = 0;
                    }

                    if ((BoundaryLock.Player2BlockedTop && moveY < 0) ||
                        (BoundaryLock.Player2BlockedBottom && moveY > 0) ||
                        (BoundaryLock.Player1BlockedTop && moveY < 0) ||
                        (BoundaryLock.Player1BlockedBottom && moveY > 0))
                    {
                        predictedVelocity.y = 0;
                    }
                }

                rb.velocity = predictedVelocity;
            }
        }

        // ��ɫ��ת
        if (moveX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = (moveX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // ��Ծ�߼�
        bool jumpPressed = false;
        if (playerID == PlayerID.Player1)
        {
            jumpPressed = Input.GetButtonDown("Jump_P1");
        }
        else if (playerID == PlayerID.Player2)
        {
            jumpPressed = Input.GetButtonDown("Jump_P2");
        }

        if (jumpPressed)
        {
            if (isGrounded)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpCount = 1;
            }
            else if (jumpCount == 1)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpCount = 2;
            }
        }
    }

    void FixedUpdate()
    {
        // ������
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0; // ��غ�������Ծ����
        }

        // ǿ��������ת
        rb.rotation = 0f;
    }
}