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
        // 获取水平输入（A/D 或 左右箭头键）
        horizontalInput = Input.GetAxis("Horizontal");

        // 控制角色朝向
        if (horizontalInput > 0.1f) // 向右移动
        {
            transform.localScale = new Vector3(1.8f, 1.8f, 1.8f); // 面向右
        }
        else if (horizontalInput < -0.1f) // 向左移动
        {
            transform.localScale = new Vector3(-1.8f, 1.8f, 1.8f); // 面向左
        }
    }

    private void FixedUpdate()
    {
        // 应用水平移动
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }
}
