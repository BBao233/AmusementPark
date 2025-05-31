using System.Collections;
using System.Collections.Generic;
using System; // ���� Action
using UnityEngine;

public class CollectingManager : MonoBehaviour
{
    // ����ģʽ
    public static CollectingManager Instance { get; private set; }

    // ������Ʒ�ռ�״̬
    public Dictionary<ItemType, bool> collectedItems = new Dictionary<ItemType, bool>();

    // �ռ�����¼�
    public event Action OnAllItemsCollected;
    // �ռ����ȸ����¼�
    public event Action<int> OnCollectionProgressUpdated;

    private void Awake()
    {
        // ������ʼ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // ��ʼ���ռ�״̬
        collectedItems[ItemType.Page1] = false;
        collectedItems[ItemType.Page2] = false;
        collectedItems[ItemType.Page3] = false;
        collectedItems[ItemType.Page4] = false;
    }

    // �ռ���Ʒ
    public void CollectItem(ItemType type, CollectibleItem item)
    {
        if (collectedItems.ContainsKey(type) && !collectedItems[type])
        {
            collectedItems[type] = true;
            Debug.Log($"�ռ���Ʒ: {type}");

            // ��ȡ��ǰ�ռ�����
            int currentCount = CollectedCount;

            // ��ӡ�ռ����ȵ�����̨
            Debug.Log($"��ǰ�ռ�����: {currentCount}/4");

            // �������ȸ����¼�
            OnCollectionProgressUpdated?.Invoke(currentCount);

            // ����Ƿ�������Ʒ�����ռ�
            CheckAllItemsCollected();
        }
    }

    // ����Ƿ�������Ʒ�����ռ�
    private void CheckAllItemsCollected()
    {
        foreach (var item in collectedItems.Values)
        {
            if (!item)
            {
                // ����δ�ռ�����Ʒ��ֱ�ӷ���
                return;
            }
        }

        // ������Ʒ�����ռ�
        Debug.Log("��ϲ��������Ʒ�ռ���ɣ�");
        OnAllItemsCollected?.Invoke();
    }

    // ��ȡ�ռ�����
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