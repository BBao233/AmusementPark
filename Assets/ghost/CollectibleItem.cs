using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum CollectibleType
    {
        MaleOnly,   // ֻ�������ռ�
        FemaleOnly  // ֻ��Ů���ռ�
    }

    [Header("�ռ�������")]
    public CollectibleType collectibleType;
    public ItemType itemType;  // ��Ʒ���ͣ��ռ�ҳ��1~4��
    public bool isCollected = false; // ��ֹ�ظ��ռ�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return; // ���ռ�����Ʒ������Ӧ

        // �����ײ�����Ƿ������Խ�ɫ
        if (collectibleType == CollectibleType.MaleOnly && other.CompareTag("Male"))
        {
            CollectItem();
        }
        // �����ײ�����Ƿ���Ů�Խ�ɫ
        else if (collectibleType == CollectibleType.FemaleOnly && other.CompareTag("Female"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        isCollected = true;

        // ֪ͨGameManager���Ӽ���
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.CollectItem(itemType, this);
        }

        // ���ز���������
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.1f); // �ӳ�������������Ч��
    }
}

public enum ItemType { Page1, Page2, Page3, Page4 }