using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ItemCollectedEventHandler();

public class Collect : MonoBehaviour
{
    public float damageToBoss = 10f;      // ÿ���ռ���ɵ��˺�
    public event ItemCollectedEventHandler OnItemCollected; // ��Ʒ���ռ��¼�

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // ������Ⱦ������
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"��Ʒ {gameObject.name} ������ռ�");

            // �ҵ�BOSS������˺�
            fly boss = FindObjectOfType<fly>();
            if (boss != null)
            {
                boss.TakeDamage(damageToBoss);
                Debug.Log($"��BOSS��� {damageToBoss} ���˺�");
            }
            else
            {
                Debug.LogWarning("������δ�ҵ�BOSS��");
            }

            // �����ռ��¼�
            OnItemCollected?.Invoke();

            // ������Ⱦ������Ʒ������ʧ��
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // ������Ʒ���ӳ�0.1��ȷ���¼�������ɣ�
            Destroy(gameObject, 0.1f);
        }
    }
}
