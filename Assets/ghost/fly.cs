using UnityEngine;

public class EagleRandomMovement : MonoBehaviour
{
    public float speed = 4f; // 老鹰飞行的速度
    public float minX = -2.35f; // 矩形区域最小X坐标
    public float maxX = 2.35f; // 矩形区域最大X坐标
    public float minY = -1f; // 矩形区域最小Y坐标
    public float maxY = 2.2f; // 矩形区域最大Y坐标

    private Vector2 currentDirection; // 当前飞行方向向量
    private float directionChangeInterval = 1.2f; // 改变方向的时间间隔（单位：秒）
    private float timeSinceLastDirectionChange = 0f; // 距离上次改变方向经过的时间

    private void Start()
    {
        // 初始化一个随机的飞行方向向量
        currentDirection = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        // 计算距离上次改变方向经过的时间
        timeSinceLastDirectionChange += Time.deltaTime;

        // 判断是否到了改变方向的时间
        if (timeSinceLastDirectionChange >= directionChangeInterval)
        {
            // 重新生成一个随机的飞行方向向量
            currentDirection = Random.insideUnitCircle.normalized;
            timeSinceLastDirectionChange = 0f;
        }

        // 根据当前方向和速度计算位移量
        Vector2 movement = currentDirection * speed * Time.deltaTime;

        // 更新老鹰的位置
        Vector2 newPosition = (Vector2)transform.position + movement;

        // 限制老鹰在指定矩形区域内
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        // 根据飞行方向调整老鹰的面向方向（通过localScale的x分量来控制）
        if (currentDirection.x < 0)
        {
            transform.localScale = new Vector3(1.4f, transform.localScale.y, transform.localScale.z);
        }
        else if (currentDirection.x > 0)
        {
            transform.localScale = new Vector3(-1.4f, transform.localScale.y, transform.localScale.z);
        }
    }
}