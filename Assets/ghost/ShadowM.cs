using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowM : MonoBehaviour
{
    public float patrolSpeed = 1f;    // 巡逻速度
    public float patrolTime = 2f;     // 向一个方向巡逻的时间
    private float timer;              // 计时
    private bool isMovingLeft;        // 是否向左移动
    private Vector2 startPosition;    // 起始位置
    private SpriteRenderer spriteRenderer; // 影子的精灵渲染器（用于获取朝向）

    private void Start()
    {
        startPosition = transform.position;
        isMovingLeft = true;
        timer = 0f;

        // 获取渲染器组件（需确保影子预制体包含SpriteRenderer）
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("影子预制体缺少SpriteRenderer组件！");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= patrolTime)
        {
            isMovingLeft = !isMovingLeft;
            timer = 0f;

            // 转向时翻转精灵朝向
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = isMovingLeft; // 向左时翻转，向右时恢复
            }
        }

        // 根据移动方向更新位置
        Vector2 moveDirection = isMovingLeft ? Vector2.left : Vector2.right;
        transform.Translate(moveDirection * patrolSpeed * Time.deltaTime);
    }
}
