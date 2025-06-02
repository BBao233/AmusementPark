using UnityEngine;

public class Raindrop : MonoBehaviour
{
    private float speed;           // 雨滴下落速度
    private int damage;            // 雨滴造成的伤害
    private LayerMask playerLayer; // 玩家所在的图层

    public void Initialize(float speed, int damage, LayerMask playerLayer)
    {
        this.speed = speed;
        this.damage = damage;
        this.playerLayer = playerLayer;
    }

    public void Move()
    {
        // 移动雨滴
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    public void CheckCollision()
    {
        // 定义射线起点和方向
        Vector2 rayStart = transform.position;
        Vector2 rayDirection = Vector2.down;
        float rayLength = 0.1f; // 射线长度，略大于雨滴的高度

        // 进行射线检测
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, playerLayer);

        // 绘制射线（仅在编辑器中可见）
        Debug.DrawRay(rayStart, rayDirection * rayLength, Color.red);

        // 检查是否击中玩家
        if (hit.collider != null)
        {
            // 获取玩家的生命值组件并造成伤害
            Health playerHealth = hit.collider.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 雨滴击中后销毁
            Destroy(gameObject);
        }
    }
}