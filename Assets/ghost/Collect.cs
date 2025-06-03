using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ItemCollectedEventHandler();

public class Collect : MonoBehaviour
{
    public float damageToBoss = 10f;      // 每次收集造成的伤害
    public event ItemCollectedEventHandler OnItemCollected; // 物品被收集事件

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 缓存渲染器引用
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"物品 {gameObject.name} 被玩家收集");

            // 找到BOSS并造成伤害
            fly boss = FindObjectOfType<fly>();
            if (boss != null)
            {
                boss.TakeDamage(damageToBoss);
                Debug.Log($"对BOSS造成 {damageToBoss} 点伤害");
            }
            else
            {
                Debug.LogWarning("场景中未找到BOSS！");
            }

            // 触发收集事件
            OnItemCollected?.Invoke();

            // 禁用渲染（让物品立即消失）
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // 销毁物品（延迟0.1秒确保事件处理完成）
            Destroy(gameObject, 0.1f);
        }
    }
}
