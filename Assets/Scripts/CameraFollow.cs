using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform targetToFollow; // 目标角色
    public float smoothSpeed = 0.125f; // 跟随速度
    public Vector3 offset; // 摄像机与目标之间的偏移量
    void LateUpdate()
    {
        if (targetToFollow != null)
        {
            // 计算期望的位置
            Vector3 desiredPosition = targetToFollow.position + offset;
            // 只在x和y轴上进行插值，保持z轴不变
            Vector3 smoothedPosition = new Vector3(
                Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeed),
                Mathf.Lerp(transform.position.y, desiredPosition.y, smoothSpeed),
                transform.position.z);
            transform.position = smoothedPosition;
        }
    }
}