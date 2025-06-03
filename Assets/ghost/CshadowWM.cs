using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CshadowWM : MonoBehaviour
{
    public GameObject shadowPrefab; // 影子预制体
    private float shadowSpawnTimer; // 影子生成计时器
    public float shadowSpawnInterval = 8f; // 影子生成间隔时间

    private void Update()
    {
        shadowSpawnTimer += Time.deltaTime;
        if (shadowSpawnTimer >= shadowSpawnInterval)
        {
            SpawnShadow();
            shadowSpawnTimer = 0f;
        }
    }

    void SpawnShadow()
    {
        GameObject shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
        ShadowWM shadowPatrol = shadow.AddComponent<ShadowWM>();
        // 这里可以根据需要为影子的巡逻速度、时间等属性设置初始值
        shadowPatrol.patrolSpeed = 1f;
        shadowPatrol.patrolTime = 1f;
    }
}
