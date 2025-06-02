using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterExit : MonoBehaviour
{
    [Header("����Ч��")]
    public GameObject teleportEffect; // ������Ч����ѡ��

    // ��ȡ����λ�ã�����ƫ�ƣ�
    public Vector3 GetExitPosition()
    {
        return transform.position;
    }

    // �ڱ༭���п��ӻ�����
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1f, 0.5f, 0.7f); // ��ɫ��͸��
        Gizmos.DrawCube(transform.position, new Vector3(1f, 1f, 1f));

        // ���Ʒ���ָʾ��
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1.5f);
        Gizmos.DrawLine(transform.position + transform.up * 1.5f,
                         transform.position + transform.up * 1.2f + transform.right * 0.3f);
        Gizmos.DrawLine(transform.position + transform.up * 1.5f,
                         transform.position + transform.up * 1.2f - transform.right * 0.3f);
    }
}