using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform targetToFollow; // Ŀ���ɫ
    public float smoothSpeed = 0.125f; // �����ٶ�
    public Vector3 offset; // �������Ŀ��֮���ƫ����
    void LateUpdate()
    {
        if (targetToFollow != null)
        {
            // ����������λ��
            Vector3 desiredPosition = targetToFollow.position + offset;
            // ֻ��x��y���Ͻ��в�ֵ������z�᲻��
            Vector3 smoothedPosition = new Vector3(
                Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeed),
                Mathf.Lerp(transform.position.y, desiredPosition.y, smoothSpeed),
                transform.position.z);
            transform.position = smoothedPosition;
        }
    }
}