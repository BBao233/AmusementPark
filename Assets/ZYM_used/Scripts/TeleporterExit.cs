using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterExit : MonoBehaviour
{
    [Header("传送效果")]
    public GameObject teleportEffect; // 传送特效（可选）

    // 获取出口位置（考虑偏移）
    public Vector3 GetExitPosition()
    {
        return transform.position;
    }

    // 在编辑器中可视化出口
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1f, 0.5f, 0.7f); // 绿色半透明
        Gizmos.DrawCube(transform.position, new Vector3(1f, 1f, 1f));

        // 绘制方向指示器
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1.5f);
        Gizmos.DrawLine(transform.position + transform.up * 1.5f,
                         transform.position + transform.up * 1.2f + transform.right * 0.3f);
        Gizmos.DrawLine(transform.position + transform.up * 1.5f,
                         transform.position + transform.up * 1.2f - transform.right * 0.3f);
    }
}