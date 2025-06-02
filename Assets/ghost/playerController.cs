using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // ��ȡˮƽ���루A/D �� ���Ҽ�ͷ����
        horizontalInput = Input.GetAxis("Horizontal");

        // ���ƽ�ɫ����
        if (horizontalInput > 0.1f) // �����ƶ�
        {
            transform.localScale = new Vector3(1.8f, 1.8f, 1.8f); // ������
        }
        else if (horizontalInput < -0.1f) // �����ƶ�
        {
            transform.localScale = new Vector3(-1.8f, 1.8f, 1.8f); // ������
        }
    }

    private void FixedUpdate()
    {
        // Ӧ��ˮƽ�ƶ�
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }
}
