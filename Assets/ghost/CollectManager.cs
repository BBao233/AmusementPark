using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectManager : MonoBehaviour
{
    public GameObject itemPrefab;         // 物品预制体
    public Transform[] spawnPoints;       // 预设的刷新位置数组
    public float respawnDelay = 1f;       // 重新刷新延迟时间（秒）

    private GameObject currentItem;       // 当前生成的物品实例

    private void Start()
    {
        // 游戏开始时生成第一个物品
        SpawnItem();
    }

    // 生成物品到随机位置
    private void SpawnItem()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("未设置物品刷新位置！");
            return;
        }

        // 随机选择一个刷新点
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;

        // 生成物品
        currentItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

        // 获取物品上的Collect脚本并注册回调
        Collect itemCollectScript = currentItem.GetComponent<Collect>();
        if (itemCollectScript != null)
        {
            itemCollectScript.OnItemCollected += OnItemCollected;
        }
        else
        {
            Debug.LogWarning("物品预制体缺少Collect脚本！");
        }
    }

    // 物品被收集时的回调
    private void OnItemCollected()
    {
        // 延迟后重新生成物品
        StartCoroutine(RespawnItemAfterDelay());
    }

    // 延迟后重新生成物品
    private IEnumerator RespawnItemAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnItem();
    }
}
