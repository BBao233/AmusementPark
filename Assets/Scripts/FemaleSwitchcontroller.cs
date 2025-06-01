using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FemaleSwitchController : MonoBehaviour
{
    [Header("Ŀ����Ƭ��ͼ")]
    public Tilemap[] targetTilemaps;  // �ɿ��ƶ����Ƭ��ͼ
    public bool showOnInteract = true;  // ����ʱ��ʾ��������

    [Header("������Ŀ��GameObjects")]
    public GameObject[] targetGameObjects;  // �������ɿ��Ƶ�GameObjects

    [Header("��������")]
    public KeyCode interactKey = KeyCode.L;
    public float interactionRadius = 1f;

    [Header("������������ʾUI")]
    public bool createPromptUI = true;  // �Ƿ��Զ�������ʾUI
    public string promptText = "��L����";  // ��ʾ�ı�����
    public Color promptColor = Color.white;  // �ı���ɫ
    public float promptOffsetY = 0.5f;  // �ı�Y��ƫ����
    public float promptFontSize = 16f;  // �ı������С
    public bool alwaysShowPrompt = true;  // �������Ƿ�ʼ����ʾ��ʾ

    [Header("����������ͼ������")]
    public Sprite activeSprite;         // ����״̬�ľ���ͼ��
    public Sprite inactiveSprite;       // δ����״̬�ľ���ͼ��
    public bool scaleOnActivate = true; // �Ƿ��ڼ���ʱ����
    public Vector3 activationScale = new Vector3(1f, 1f, 1f); // ����ʱ�����ű���
    public float scaleDuration = 0f;  // ���Ŷ�������ʱ��

    [Header("��������Ⱦ����")]
    public string playerSortingLayer = "Player"; // ��ҵ���Ⱦ�㼶
    public int playerOrderInLayer = 10;          // ��ҵĲ㼶˳��
    public bool alwaysRenderBehindPlayer = true; // �Ƿ�ʼ����Ⱦ����Һ�

    [Header("��Ч����")]
    public AudioClip interactionSound;  // ������Ч
    [Range(0f, 1f)] public float volume = 1f;  // �������ƻ���
    private AudioSource audioSource;    // ��ƵԴ���

    private GameObject interactionPrompt;  // ������ʾUI
    private bool playerInRange;
    private bool mapsActivated = false;   // ��¼��ͼ�Ƿ��Ѽ���
    private SpriteRenderer switchRenderer; // ���صľ�����Ⱦ��
    private Vector3 originalScale;         // ԭʼ���ű���
    private bool isScaling = false;        // �Ƿ���������

    void Start()
    {
        // ��ȡ������Ⱦ��
        switchRenderer = GetComponent<SpriteRenderer>();

        // ȷ��������Ⱦ����Һ�
        SetupRendererOrder();

        // ����ԭʼ���ű���
        originalScale = transform.localScale;

        // ��ʼ������ͼ��
        UpdateSwitchSprite();

        // ��ʼ����Ĭ����������Ŀ����Ƭ��ͼ��GameObjects
        ToggleTilemaps(false);
        ToggleGameObjects(false);

        // ����������������ʾUI
        if (createPromptUI)
            CreateInteractionPrompt();

        // ����н�����ʾUI���������þ����Ƿ���ʾ
        UpdatePromptVisibility();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        Debug.Log($"���� {gameObject.name} ��ʼ�����");
    }

    // ������������Ⱦ˳��
    void SetupRendererOrder()
    {
        if (switchRenderer == null || !alwaysRenderBehindPlayer) return;

        // ���ÿ��ص���Ⱦ�㼶��˳��ȷ������Һ�
        switchRenderer.sortingLayerName = playerSortingLayer;
        switchRenderer.sortingOrder = playerOrderInLayer - 1;
    }

    void Update()
    {
        // �������Ƿ��ڽ�����Χ��
        CheckPlayerInRange();

        // ������ʾUI״̬
        UpdatePromptVisibility();

        // �������߼�
        if (playerInRange && Input.GetKeyDown(interactKey) && !mapsActivated)
        {
            ActivateTilemaps();
        }

        // �������Ŷ���
        UpdateScaling();
    }

    void CheckPlayerInRange()
    {
        // ʹ��LayerMask���и���ȷ�ļ�⣨��ѡ��
        int layerMask = LayerMask.GetMask("Player"); // ����"Female"��ɫ��"Player"��

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius,
            layerMask
        );

        playerInRange = false;
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Female"))
            {
                playerInRange = true;
                break;
            }
        }
    }

    void UpdatePromptVisibility()
    {
        if (interactionPrompt == null) return;

        // �޸ģ�ֻ�е�����ڷ�Χ����δ����ʱ����ʾ��ʾ
        bool shouldShow = !mapsActivated && playerInRange;

        if (interactionPrompt.activeSelf != shouldShow)
        {
            interactionPrompt.SetActive(shouldShow);
            Debug.Log($"��ʾUI״̬����Ϊ: {shouldShow}");
        }
    }

    void ActivateTilemaps()
    {
        mapsActivated = true;
        ToggleTilemaps(showOnInteract);
        ToggleGameObjects(showOnInteract);  // ����������GameObjects

        // ���¿���ͼ��
        UpdateSwitchSprite();

        // ��ʼ���Ŷ���
        if (scaleOnActivate)
            StartScaling();

        // ������ʾUI
        UpdatePromptVisibility();

        if (interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, volume);
        }

        Debug.Log("��Ƭ��ͼ��GameObjects�Ѽ���");
    }

    void ToggleTilemaps(bool active)
    {
        foreach (var tilemap in targetTilemaps)
        {
            if (tilemap != null)
            {
                tilemap.gameObject.SetActive(active);
            }
        }
    }

    // ����������GameObjects�ķ���
    void ToggleGameObjects(bool active)
    {
        foreach (var gameObject in targetGameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(active);
            }
        }
    }

    // ���������¿���ͼ��ķ���
    void UpdateSwitchSprite()
    {
        if (switchRenderer == null)
        {
            Debug.LogWarning("δ�ҵ����ص�SpriteRenderer���");
            return;
        }

        // ���ݼ���״̬���þ���ͼ��
        if (mapsActivated && activeSprite != null)
        {
            switchRenderer.sprite = activeSprite;
        }
        else if (!mapsActivated && inactiveSprite != null)
        {
            switchRenderer.sprite = inactiveSprite;
        }
    }

    // ��������ʼ���Ŷ���
    void StartScaling()
    {
        isScaling = true;
        transform.localScale = originalScale;
    }

    // �������������Ŷ���
    void UpdateScaling()
    {
        if (!isScaling) return;

        // ʹ��ƽ����ֵʵ�����Ŷ���
        float progress = Mathf.Min(Time.deltaTime / scaleDuration, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, activationScale, progress);

        // ������ɺ�ָ�ԭʼ��С
        if (Vector3.Distance(transform.localScale, activationScale) < 0.01f)
        {
            isScaling = false;
            transform.localScale = originalScale;
        }
    }

    // ����������������ʾUI�ķ���
    void CreateInteractionPrompt()
    {
        Debug.Log("��ʼ����������ʾUI");

        // ����Canvas
        GameObject canvasObject = GameObject.Find("MainCanvas");
        if (canvasObject == null)
        {
            Debug.Log("δ�ҵ�MainCanvas�������µ�Canvas");
            canvasObject = new GameObject("MainCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            Debug.Log("��Canvas�������");
        }
        else
        {
            Debug.Log("�ҵ�����MainCanvas");
        }

        // ������ʾ�ı�
        interactionPrompt = new GameObject("InteractionPrompt");
        interactionPrompt.transform.SetParent(canvasObject.transform);

        Text textComponent = interactionPrompt.AddComponent<Text>();
        textComponent.text = promptText;

        // ȷ��������� - �Ľ�����������߼�
        Font loadedFont = null;

        // ���Լ�����֪����������
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
                    break;
                }

                // ���·������ʧ�ܣ�����ֱ��ʹ����������
                if (fontName.EndsWith(".ttf"))
                {
                    string plainFontName = fontName.Substring(0, fontName.Length - 4);
                    loadedFont = Font.CreateDynamicFontFromOSFont(plainFontName, (int)promptFontSize);
                    if (loadedFont != null)
                    {
                        Debug.Log($"�ɹ�ͨ���������Ƽ���: {plainFontName}");
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"���Լ������� {fontName} ʧ��: {e.Message}");
            }
        }

        // ������г��Զ�ʧ�ܣ�ʹ��Ĭ������
        if (loadedFont == null)
        {
            Debug.LogError("�޷������κ��������壬ʹ��Ĭ������");
            loadedFont = Resources.GetBuiltinResource<Font>("Arial.ttf"); // ����ܻ�ʧ�ܣ���Unity���ṩĬ������
        }

        textComponent.font = loadedFont;
        textComponent.fontSize = (int)promptFontSize;
        textComponent.color = promptColor;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // ����RectTransform
        RectTransform rectTransform = interactionPrompt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);

        // ����λ��
        UpdatePromptPosition();

        Debug.Log("������ʾUI�������");
    }

    // ������������ʾλ�õķ���
    void UpdatePromptPosition()
    {
        if (interactionPrompt == null)
        {
            Debug.LogWarning("���Ը�����ʾλ�ã���UI����Ϊ��");
            return;
        }

        // ȷ���������
        if (Camera.main == null)
        {
            Debug.LogError("�Ҳ�����������޷�������ʾλ��");
            return;
        }

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // ��������Ƿ�����Ļ��Χ��
        bool isOnScreen = screenPosition.z > 0 &&
                          screenPosition.x > 0 && screenPosition.x < Screen.width &&
                          screenPosition.y > 0 && screenPosition.y < Screen.height;

        Debug.Log($"��ʾλ�ø���: ��Ļ����=({screenPosition.x}, {screenPosition.y}, {screenPosition.z}), �Ƿ�����Ļ��={isOnScreen}");

        // ����UIλ��
        RectTransform rectTransform = interactionPrompt.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
    }

    void LateUpdate()
    {
        // ȷ����ʾλ�ø��濪��
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }
    }

    // ���ӻ�������Χ�����ڱ༭������ʾ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}