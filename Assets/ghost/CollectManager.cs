using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectManager : MonoBehaviour
{
    public GameObject itemPrefab;         // ��ƷԤ����
    public Transform[] spawnPoints;       // Ԥ���ˢ��λ������
    public float respawnDelay = 1f;       // ����ˢ���ӳ�ʱ�䣨�룩

    private GameObject currentItem;       // ��ǰ���ɵ���Ʒʵ��

    private void Start()
    {
        // ��Ϸ��ʼʱ���ɵ�һ����Ʒ
        SpawnItem();
    }

    // ������Ʒ�����λ��
    private void SpawnItem()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("δ������Ʒˢ��λ�ã�");
            return;
        }

        // ���ѡ��һ��ˢ�µ�
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;

        // ������Ʒ
        currentItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

        // ��ȡ��Ʒ�ϵ�Collect�ű���ע��ص�
        Collect itemCollectScript = currentItem.GetComponent<Collect>();
        if (itemCollectScript != null)
        {
            itemCollectScript.OnItemCollected += OnItemCollected;
        }
        else
        {
            Debug.LogWarning("��ƷԤ����ȱ��Collect�ű���");
        }
    }

    // ��Ʒ���ռ�ʱ�Ļص�
    private void OnItemCollected()
    {
        // �ӳٺ�����������Ʒ
        StartCoroutine(RespawnItemAfterDelay());
    }

    // �ӳٺ�����������Ʒ
    private IEnumerator RespawnItemAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnItem();
    }
}
