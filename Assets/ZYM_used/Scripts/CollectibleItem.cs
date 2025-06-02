using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum CollectibleType
    {
        MaleOnly,   // 只能男性收集
        FemaleOnly  // 只能女性收集
    }

    [Header("收集物设置")]
    public CollectibleType collectibleType;
    public ItemType itemType;  // 物品类型（日记页面1~4）
    public bool isCollected = false; // 防止重复收集

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return; // 已收集的物品不再响应

        // 检查碰撞对象是否是男性角色
        if (collectibleType == CollectibleType.MaleOnly && other.CompareTag("Male"))
        {
            CollectItem();
        }
        // 检查碰撞对象是否是女性角色
        else if (collectibleType == CollectibleType.FemaleOnly && other.CompareTag("Female"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        isCollected = true;

        // 通知GameManager增加计数
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.CollectItem(itemType, this);
        }

        // 隐藏并销毁物体
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.1f); // 延迟销毁以允许播放效果
    }
}

public enum ItemType { Page1, Page2, Page3, Page4 }