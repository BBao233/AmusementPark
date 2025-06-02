using UnityEngine;

public class BoundaryTeleport : MonoBehaviour
{
    // 地图左右边界坐标（实际检测的边界）
    public float leftBoundary = -10f;   // 左侧触发边界（角色x≤此值时触发传送）
    public float rightBoundary = 10f;   // 右侧触发边界（角色x≥此值时触发传送）

    // 安全缓冲距离（传送后与原边界的最小距离）
    public float bufferDistance = 0.5f; // 推荐≥0.3f，避免因角色移动速度过快再次触发

    // 角色间距（避免重叠）
    public float minSeparation = 1.5f;  // 两个角色传送后的最小水平间距

    // 两个角色的引用
    public Transform player1;
    public Transform player2;

    // 防止连续传送的冷却时间（冗余保护）
    public float teleportCooldown = 0.3f;
    private float lastTeleportTime;

    void Start()
    {
        lastTeleportTime = -teleportCooldown; // 初始状态允许传送
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        // 冷却时间未到，跳过检测
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        // 检测角色是否触发传送
        bool p1Left = player1.position.x <= leftBoundary;
        bool p1Right = player1.position.x >= rightBoundary;
        bool p2Left = player2.position.x <= leftBoundary;
        bool p2Right = player2.position.x >= rightBoundary;

        // 优先处理右侧触发（避免同时触发两侧）
        if (p1Right || p2Right)
        {
            TeleportToLeftSide(); // 传送到左安全区（原左边界左侧）
            lastTeleportTime = Time.time;
        }
        // 再处理左侧触发
        else if (p1Left || p2Left)
        {
            TeleportToRightSide(); // 传送到右安全区（原右边界右侧）
            lastTeleportTime = Time.time;
        }
    }

    // 传送到左安全区（原左边界左侧）
    private void TeleportToLeftSide()
    {
        // 计算两个角色的原始左右顺序（确保传送后保持相对位置）
        bool isPlayer1Left = player1.position.x < player2.position.x;

        // 左安全区的基准位置：原左边界 - bufferDistance（远离左边界）
        float baseX = leftBoundary - bufferDistance;

        // 角色1的目标x：若原在左，则取baseX；否则取baseX + minSeparation（保持间距）
        float p1X = isPlayer1Left ? baseX : baseX + minSeparation;
        // 角色2的目标x：与角色1相反（确保不重叠）
        float p2X = isPlayer1Left ? baseX + minSeparation : baseX;

        // 更新位置（保持y和z不变）
        player1.position = new Vector3(p1X, player1.position.y, player1.position.z);
        player2.position = new Vector3(p2X, player2.position.y, player2.position.z);

        Debug.Log($"传送到左安全区！角色1位置: {p1X}, 角色2位置: {p2X}");
    }

    // 传送到右安全区（原右边界右侧）
    private void TeleportToRightSide()
    {
        // 计算两个角色的原始左右顺序
        bool isPlayer1Left = player1.position.x < player2.position.x;

        // 右安全区的基准位置：原右边界 + bufferDistance（远离右边界）
        float baseX = rightBoundary + bufferDistance;

        // 角色1的目标x：若原在左，则取baseX - minSeparation（保持间距）；否则取baseX
        float p1X = isPlayer1Left ? baseX - minSeparation : baseX;
        // 角色2的目标x：与角色1相反
        float p2X = isPlayer1Left ? baseX : baseX - minSeparation;

        // 更新位置
        player1.position = new Vector3(p1X, player1.position.y, player1.position.z);
        player2.position = new Vector3(p2X, player2.position.y, player2.position.z);

        Debug.Log($"传送到右安全区！角色1位置: {p1X}, 角色2位置: {p2X}");
    }
}