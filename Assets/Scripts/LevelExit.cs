using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [Header("��������")]
    public string nextSceneName = "Level2"; // ��һ�س�������
    public KeyCode interactKey1 = KeyCode.E, interactKey2 = KeyCode.L; // ��������

    [Header("��ʾUI����")]
    public bool createPromptUI = true;
    public string promptText = "ǰ����һ��";
    public Color promptColor = Color.green;
    public float promptOffsetY = 1.5f;
    public float promptFontSize = 20;

    private bool maleInRange = false;
    private bool femaleInRange = false;
    private GameObject promptUI;
    private bool allItemsCollected = false;
    private bool bothPlayersInZone = false;
    private bool canExit = false;

    void Start()
    {
        // �����ռ�����¼�
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.OnAllItemsCollected += OnAllItemsCollected;
        }
        else
        {
            Debug.LogWarning("CollectingManager ʵ��δ�ҵ���");
        }

        // ������ʾUI
        if (createPromptUI)
        {
            CreatePromptUI();
        }
    }

    void OnDestroy()
    {
        // ȡ���¼�����
        if (CollectingManager.Instance != null)
        {
            CollectingManager.Instance.OnAllItemsCollected -= OnAllItemsCollected;
        }
    }

    void OnAllItemsCollected()
    {
        allItemsCollected = true;
        Debug.Log("������Ʒ�ռ���ɣ�����ͨ��");
        UpdatePromptVisibility();
    }

    void Update()
    {
        // ������ʾλ��
        if (promptUI != null && promptUI.activeSelf)
        {
            UpdatePromptPosition();
        }

        // ����Ƿ�����˳�
        bothPlayersInZone = maleInRange && femaleInRange;
        canExit = allItemsCollected && maleInRange && femaleInRange;

        // ������������Ұ��½��������������һ��
        if (canExit && (Input.GetKeyDown(interactKey1) || Input.GetKeyDown(interactKey2)))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ֻ���Player��Ķ���
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        if (other.CompareTag("Male"))
        {
            maleInRange = true;
            Debug.Log("���Խ���ͨ������");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = true;
            Debug.Log("Ů�Խ���ͨ������");
        }

        UpdatePromptVisibility();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // ֻ���Player��Ķ���
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        if (other.CompareTag("Male"))
        {
            maleInRange = false;
            Debug.Log("�����뿪ͨ������");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = false;
            Debug.Log("Ů���뿪ͨ������");
        }

        UpdatePromptVisibility();
    }

    void UpdatePromptVisibility()
    {
        if (promptUI == null) return;

        // ֻ�е�������Ʒ�ռ����ʱ�ſ�����ʾ��ʾ
        bool shouldShow = allItemsCollected;

        if (promptUI.activeSelf != shouldShow)
        {
            promptUI.SetActive(shouldShow);
            Debug.Log($"ͨ����ʾ��ʾ״̬: {shouldShow}");
        }

        // ������ʾ�ı�
        if (shouldShow)
        {
            Text text = promptUI.GetComponent<Text>();

            if (maleInRange && femaleInRange)
            {
                text.text = $"��{interactKey1}����{interactKey2}��\n����{nextSceneName}";
                text.color = Color.white;
            }
            else if (maleInRange && !femaleInRange)
            {
                text.text = "�ȴ�Ů�Խ�ɫ...";
                text.color = Color.white;
            }
            else if (femaleInRange && !maleInRange)
            {
                text.text = "�ȴ����Խ�ɫ...";
                text.color = Color.white;
            }
            else
            {
                text.text = "������һ��";
                text.color = Color.white;
            }
        }
    }

    void CreatePromptUI()
    {
        // Ѱ��Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("�����µ�Canvas����ͨ����ʾ");
        }

        // ������ʾ�ı�
        promptUI = new GameObject("ExitPrompt");
        promptUI.transform.SetParent(canvas.transform);
        Text textComponent = promptUI.AddComponent<Text>();
        textComponent.text = "��Ҫ�ռ�������Ʒ";
        textComponent.color = Color.red;
        textComponent.fontSize = (int)promptFontSize;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // ʹ�øĽ�����������߼�
        Font loadedFont = LoadFont();
        textComponent.font = loadedFont;

        // ����RectTransform
        RectTransform rectTransform = promptUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 50);
        promptUI.SetActive(false); // Ĭ������

        Debug.Log("ͨ����ʾUI�������");
    }

    // �Ľ���������ط���
    private Font LoadFont()
    {
        Font loadedFont = null;

        // �����ѡ�б�
        string[] fontCandidates = {
            "Arial.ttf",              // �ɰ汾Unity֧��
            "LegacyRuntime.ttf",      // �°汾Unity֧��
            "Arial"                   // ֱ��ʹ����������
        };

        foreach (string fontName in fontCandidates)
        {
            try
            {
                // �ȳ���ʹ����Դ·������
                loadedFont = Resources.GetBuiltinResource<Font>(fontName);
                if (loadedFont != null)
                {
                    Debug.Log($"�ɹ���������: {fontName}");
                    return loadedFont;
                }

                // ���·������ʧ�ܣ�����ֱ��ʹ����������
                if (fontName.EndsWith(".ttf"))
                {
                    string plainFontName = fontName.Substring(0, fontName.Length - 4);
                    loadedFont = Font.CreateDynamicFontFromOSFont(plainFontName, (int)promptFontSize);
                    if (loadedFont != null)
                    {
                        Debug.Log($"�ɹ�ͨ���������Ƽ���: {plainFontName}");
                        return loadedFont;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"���Լ������� {fontName} ʧ��: {e.Message}");
            }
        }

        // ������г��Զ�ʧ�ܣ�ʹ��Ĭ������
        Debug.LogError("�޷������κ��������壬ʹ��Ĭ������");
        return Font.CreateDynamicFontFromOSFont("Arial", (int)promptFontSize);
    }

    void UpdatePromptPosition()
    {
        if (promptUI == null || Camera.main == null) return;

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // ����UIλ��
        promptUI.GetComponent<RectTransform>().position = screenPosition;
    }

    // �ڱ༭���п��ӻ���������
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // ��ɫ��͸��
        Collider2D collider = GetComponent<Collider2D>();

        if (collider is BoxCollider2D boxCollider)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
        }
    }
}