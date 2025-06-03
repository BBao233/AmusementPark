using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CshadowWM : MonoBehaviour
{
    public GameObject shadowPrefab; // Ӱ��Ԥ����
    private float shadowSpawnTimer; // Ӱ�����ɼ�ʱ��
    public float shadowSpawnInterval = 8f; // Ӱ�����ɼ��ʱ��

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
        // ������Ը�����ҪΪӰ�ӵ�Ѳ���ٶȡ�ʱ����������ó�ʼֵ
        shadowPatrol.patrolSpeed = 1f;
        shadowPatrol.patrolTime = 1f;
    }
}
