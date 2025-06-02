using UnityEngine;

public class EagleRandomMovement : MonoBehaviour
{
    public float speed = 4f; // ��ӥ���е��ٶ�
    public float minX = -2.35f; // ����������СX����
    public float maxX = 2.35f; // �����������X����
    public float minY = -1f; // ����������СY����
    public float maxY = 2.2f; // �����������Y����

    private Vector2 currentDirection; // ��ǰ���з�������
    private float directionChangeInterval = 1.2f; // �ı䷽���ʱ��������λ���룩
    private float timeSinceLastDirectionChange = 0f; // �����ϴθı䷽�򾭹���ʱ��

    private void Start()
    {
        // ��ʼ��һ������ķ��з�������
        currentDirection = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        // ��������ϴθı䷽�򾭹���ʱ��
        timeSinceLastDirectionChange += Time.deltaTime;

        // �ж��Ƿ��˸ı䷽���ʱ��
        if (timeSinceLastDirectionChange >= directionChangeInterval)
        {
            // ��������һ������ķ��з�������
            currentDirection = Random.insideUnitCircle.normalized;
            timeSinceLastDirectionChange = 0f;
        }

        // ���ݵ�ǰ������ٶȼ���λ����
        Vector2 movement = currentDirection * speed * Time.deltaTime;

        // ������ӥ��λ��
        Vector2 newPosition = (Vector2)transform.position + movement;

        // ������ӥ��ָ������������
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        // ���ݷ��з��������ӥ��������ͨ��localScale��x���������ƣ�
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