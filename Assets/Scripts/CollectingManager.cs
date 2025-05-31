using System.Collections;
using System.Collections.Generic;
using System; // 包含 Action
using UnityEngine;

public class CollectingManager : MonoBehaviour
{
    // 单例模式
    public static CollectingManager Instance { get; private set; }

    // 所有物品收集状态
    public Dictionary<ItemType, bool> collectedItems = new Dictionary<ItemType, bool>();

    // 收集完成事件
    public event Action OnAllItemsCollected;
    // 收集进度更新事件
    public event Action<int> OnCollectionProgressUpdated;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化收集状态
        collectedItems[ItemType.Page1] = false;
        collectedItems[ItemType.Page2] = false;
        collectedItems[ItemType.Page3] = false;
        collectedItems[ItemType.Page4] = false;
    }

    // 收集物品
    public void CollectItem(ItemType type, CollectibleItem item)
    {
        if (collectedItems.ContainsKey(type) && !collectedItems[type])
        {
            collectedItems[type] = true;
            Debug.Log($"收集物品: {type}");

            // 获取当前收集进度
            int currentCount = CollectedCount;

            // 打印收集进度到控制台
            Debug.Log($"当前收集进度: {currentCount}/4");

            // 触发进度更新事件
            OnCollectionProgressUpdated?.Invoke(currentCount);

            // 检查是否所有物品都已收集
            CheckAllItemsCollected();
        }
    }

    // 检查是否所有物品都已收集
    private void CheckAllItemsCollected()
    {
        foreach (var item in collectedItems.Values)
        {
            if (!item)
            {
                // 还有未收集的物品，直接返回
                return;
            }
        }

        // 所有物品都已收集
        Debug.Log("恭喜！所有物品收集完成！");
        OnAllItemsCollected?.Invoke();
    }

    // 获取收集进度
    public int CollectedCount
    {
        get
        {
            int count = 0;
            foreach (var collected in collectedItems.Values)
            {
                if (collected) count++;
            }
            return count;
        }
    }
}