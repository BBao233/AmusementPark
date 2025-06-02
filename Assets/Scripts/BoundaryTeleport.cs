using UnityEngine;

public class BoundaryTeleport : MonoBehaviour
{
    // ��ͼ���ұ߽����꣨ʵ�ʼ��ı߽磩
    public float leftBoundary = -10f;   // ��ഥ���߽磨��ɫx�ܴ�ֵʱ�������ͣ�
    public float rightBoundary = 10f;   // �Ҳഥ���߽磨��ɫx�ݴ�ֵʱ�������ͣ�

    // ��ȫ������루���ͺ���ԭ�߽����С���룩
    public float bufferDistance = 0.5f; // �Ƽ���0.3f���������ɫ�ƶ��ٶȹ����ٴδ���

    // ��ɫ��ࣨ�����ص���
    public float minSeparation = 1.5f;  // ������ɫ���ͺ����Сˮƽ���

    // ������ɫ������
    public Transform player1;
    public Transform player2;

    // ��ֹ�������͵���ȴʱ�䣨���ౣ����
    public float teleportCooldown = 0.3f;
    private float lastTeleportTime;

    void Start()
    {
        lastTeleportTime = -teleportCooldown; // ��ʼ״̬������
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        // ��ȴʱ��δ�����������
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        // ����ɫ�Ƿ񴥷�����
        bool p1Left = player1.position.x <= leftBoundary;
        bool p1Right = player1.position.x >= rightBoundary;
        bool p2Left = player2.position.x <= leftBoundary;
        bool p2Right = player2.position.x >= rightBoundary;

        // ���ȴ����Ҳഥ��������ͬʱ�������ࣩ
        if (p1Right || p2Right)
        {
            TeleportToLeftSide(); // ���͵���ȫ����ԭ��߽���ࣩ
            lastTeleportTime = Time.time;
        }
        // �ٴ�����ഥ��
        else if (p1Left || p2Left)
        {
            TeleportToRightSide(); // ���͵��Ұ�ȫ����ԭ�ұ߽��Ҳࣩ
            lastTeleportTime = Time.time;
        }
    }

    // ���͵���ȫ����ԭ��߽���ࣩ
    private void TeleportToLeftSide()
    {
        // ����������ɫ��ԭʼ����˳��ȷ�����ͺ󱣳����λ�ã�
        bool isPlayer1Left = player1.position.x < player2.position.x;

        // ��ȫ���Ļ�׼λ�ã�ԭ��߽� - bufferDistance��Զ����߽磩
        float baseX = leftBoundary - bufferDistance;

        // ��ɫ1��Ŀ��x����ԭ������ȡbaseX������ȡbaseX + minSeparation�����ּ�ࣩ
        float p1X = isPlayer1Left ? baseX : baseX + minSeparation;
        // ��ɫ2��Ŀ��x�����ɫ1�෴��ȷ�����ص���
        float p2X = isPlayer1Left ? baseX + minSeparation : baseX;

        // ����λ�ã�����y��z���䣩
        player1.position = new Vector3(p1X, player1.position.y, player1.position.z);
        player2.position = new Vector3(p2X, player2.position.y, player2.position.z);

        Debug.Log($"���͵���ȫ������ɫ1λ��: {p1X}, ��ɫ2λ��: {p2X}");
    }

    // ���͵��Ұ�ȫ����ԭ�ұ߽��Ҳࣩ
    private void TeleportToRightSide()
    {
        // ����������ɫ��ԭʼ����˳��
        bool isPlayer1Left = player1.position.x < player2.position.x;

        // �Ұ�ȫ���Ļ�׼λ�ã�ԭ�ұ߽� + bufferDistance��Զ���ұ߽磩
        float baseX = rightBoundary + bufferDistance;

        // ��ɫ1��Ŀ��x����ԭ������ȡbaseX - minSeparation�����ּ�ࣩ������ȡbaseX
        float p1X = isPlayer1Left ? baseX - minSeparation : baseX;
        // ��ɫ2��Ŀ��x�����ɫ1�෴
        float p2X = isPlayer1Left ? baseX : baseX - minSeparation;

        // ����λ��
        player1.position = new Vector3(p1X, player1.position.y, player1.position.z);
        player2.position = new Vector3(p2X, player2.position.y, player2.position.z);

        Debug.Log($"���͵��Ұ�ȫ������ɫ1λ��: {p1X}, ��ɫ2λ��: {p2X}");
    }
}