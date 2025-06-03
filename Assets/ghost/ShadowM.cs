using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowM : MonoBehaviour
{
    public float patrolSpeed = 1f;    // Ѳ���ٶ�
    public float patrolTime = 2f;     // ��һ������Ѳ�ߵ�ʱ��
    private float timer;              // ��ʱ
    private bool isMovingLeft;        // �Ƿ������ƶ�
    private Vector2 startPosition;    // ��ʼλ��
    private SpriteRenderer spriteRenderer; // Ӱ�ӵľ�����Ⱦ�������ڻ�ȡ����

    private void Start()
    {
        startPosition = transform.position;
        isMovingLeft = true;
        timer = 0f;

        // ��ȡ��Ⱦ���������ȷ��Ӱ��Ԥ�������SpriteRenderer��
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Ӱ��Ԥ����ȱ��SpriteRenderer�����");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= patrolTime)
        {
            isMovingLeft = !isMovingLeft;
            timer = 0f;

            // ת��ʱ��ת���鳯��
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = isMovingLeft; // ����ʱ��ת������ʱ�ָ�
            }
        }

        // �����ƶ��������λ��
        Vector2 moveDirection = isMovingLeft ? Vector2.left : Vector2.right;
        transform.Translate(moveDirection * patrolSpeed * Time.deltaTime);
    }
}
