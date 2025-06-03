using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

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

    [Header("��������")]
    public Font customFont; // �������Զ��������ֶ�

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

    [Header("�������Ի�������")]
    public bool showDialogOnInteract = true; // �Ƿ��ڽ���ʱ��ʾ�Ի���
    public string dialogTitle = "��ʾ"; // �Ի������
    [TextArea(3, 10)] public string dialogContent = "�����ɹ���"; // �Ի�������
    public float dialogDisplayTime = 3f; // �Ի�����ʾʱ�䣨�룩��0��ʾ���Զ��ر�
    public Color dialogBackgroundColor = new Color(0, 0, 0, 0.8f); // �Ի��򱳾���ɫ
    public Color dialogTitleColor = Color.white; // ������ɫ
    public Color dialogContentColor = Color.white; // ������ɫ
    public float dialogWidth = 350f; // �Ի�����
    public float dialogHeight = 200f; // �Ի���߶�
    public float dialogOffsetY = 1.5f; // �Ի���Y��ƫ����

    [Header("��ϵͳ����")]
    public RainSystem rainSystem; // ���볡���е���ϵͳ����
    public float rainDuration = 5f; // �������ʱ�䣬�ɵ���

    [Header("�Ի���ť����")]
    public string button1Text = "ѡ��һ";
    public string button2Text = "ѡ���";
    public int correctButtonIndex = 1; // 1=��ť1��ȷ, 2=��ť2��ȷ
    public UnityEvent onCorrectChoice; // ѡ����ȷѡ��ʱ�������¼�
    public UnityEvent onIncorrectChoice; // ѡ�����ѡ��ʱ�������¼�

    private GameObject interactionPrompt;  // ������ʾUI
    private GameObject dialogUI; // �Ի���UI
    private Text dialogTitleText; // �Ի�������ı�
    private Text dialogContentText; // �Ի��������ı�
    private Button button1; // �Ի���ť1
    private Button button2; // �Ի���ť2
    private bool playerInRange;
    private bool mapsActivated = false;   // ��¼��ͼ�Ƿ��Ѽ���
    private SpriteRenderer switchRenderer; // ���صľ�����Ⱦ��
    private Vector3 originalScale;         // ԭʼ���ű���
    private bool isScaling = false;        // �Ƿ���������
    private float dialogDisplayTimer = 0f; // �Ի�����ʾ��ʱ��
    private bool dialogActive = false; // �Ի����Ƿ񼤻�


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

        // ���¶Ի����ʱ��
        UpdateDialogTimer();
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

        // ��ʾ�Ի���
        if (showDialogOnInteract)
        {
            ShowDialog();
        }

        if (interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, volume);
        }

        // ��ʾ�Ի���
        if (showDialogOnInteract && !dialogActive)
        {
            ShowDialog();
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

        // ʹ��ָ�����Զ�������
        if (customFont != null)
        {
            textComponent.font = customFont;
            Debug.Log($"ʹ���Զ�������: {customFont.name}");
        }
        else
        {
            Debug.LogWarning("δָ���Զ������壬ʹ��Ĭ������");
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

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

    // ��������ʾ�Ի���
    void ShowDialog()
    {
        Debug.Log("��ʾ�Ի���");
        dialogActive = true;

        // ����Ի����Ѵ��ڣ�������
        if (dialogUI != null)
        {
            Destroy(dialogUI);
        }

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

        // �����Ի��������
        dialogUI = new GameObject("DialogUI");
        dialogUI.transform.SetParent(canvasObject.transform);

        // ��������ͼ��
        Image backgroundImage = dialogUI.AddComponent<Image>();
        backgroundImage.color = dialogBackgroundColor;

        // ����RectTransform
        RectTransform dialogRect = dialogUI.GetComponent<RectTransform>();
        dialogRect.sizeDelta = new Vector2(dialogWidth, dialogHeight);

        // ���������ı�
        GameObject titleObject = new GameObject("DialogTitle");
        titleObject.transform.SetParent(dialogUI.transform);
        dialogTitleText = titleObject.AddComponent<Text>();
        dialogTitleText.text = dialogTitle;

        // ʹ��ָ�����Զ�������
        if (customFont != null)
        {
            dialogTitleText.font = customFont;
        }
        else
        {
            dialogTitleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        dialogTitleText.fontSize = (int)(promptFontSize * 1.2f); // ���������Դ�
        dialogTitleText.color = dialogTitleColor;
        dialogTitleText.alignment = TextAnchor.MiddleCenter;

        // ���ñ���RectTransform
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // ���������ı�
        GameObject contentObject = new GameObject("DialogContent");
        contentObject.transform.SetParent(dialogUI.transform);
        dialogContentText = contentObject.AddComponent<Text>();
        dialogContentText.text = dialogContent;

        // ʹ��ָ�����Զ�������
        if (customFont != null)
        {
            dialogContentText.font = customFont;
        }
        else
        {
            dialogContentText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        dialogContentText.fontSize = (int)promptFontSize;
        dialogContentText.color = dialogContentColor;
        dialogContentText.alignment = TextAnchor.MiddleCenter;
        dialogContentText.horizontalOverflow = HorizontalWrapMode.Wrap;
        dialogContentText.verticalOverflow = VerticalWrapMode.Truncate;

        // ��������RectTransform
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.05f, 0.4f);
        contentRect.anchorMax = new Vector2(0.95f, 0.75f);
        contentRect.offsetMin = new Vector2(10, 10);
        contentRect.offsetMax = new Vector2(-10, -10);

        // ������ť1
        GameObject button1Object = new GameObject("Button1");
        button1Object.transform.SetParent(dialogUI.transform);
        button1 = button1Object.AddComponent<Button>();
        button1.onClick.AddListener(() => OnButtonClicked(1));

        // ��Ӱ�ť����
        Image button1Image = button1Object.AddComponent<Image>();
        button1Image.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // ���ð�ť�ı�
        GameObject button1TextObject = new GameObject("ButtonText");
        button1TextObject.transform.SetParent(button1Object.transform);
        Text button1Text = button1TextObject.AddComponent<Text>();
        button1Text.text = this.button1Text;

        // ʹ��ָ�����Զ�������
        if (customFont != null)
        {
            button1Text.font = customFont;
        }
        else
        {
            button1Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        button1Text.fontSize = (int)(promptFontSize * 0.9f);
        button1Text.color = Color.black;
        button1Text.alignment = TextAnchor.MiddleCenter;

        // ���ð�ť1 RectTransform
        RectTransform button1Rect = button1Object.GetComponent<RectTransform>();
        button1Rect.anchorMin = new Vector2(0.1f, 0.1f);
        button1Rect.anchorMax = new Vector2(0.45f, 0.3f);
        button1Rect.offsetMin = new Vector2(5, 5);
        button1Rect.offsetMax = new Vector2(-5, -5);

        // ���ð�ť�ı�RectTransform
        RectTransform button1TextRect = button1TextObject.GetComponent<RectTransform>();
        button1TextRect.anchorMin = Vector2.zero;
        button1TextRect.anchorMax = Vector2.one;
        button1TextRect.offsetMin = Vector2.zero;
        button1TextRect.offsetMax = Vector2.zero;

        // ������ť2
        GameObject button2Object = new GameObject("Button2");
        button2Object.transform.SetParent(dialogUI.transform);
        button2 = button2Object.AddComponent<Button>();
        button2.onClick.AddListener(() => OnButtonClicked(2));

        // ��Ӱ�ť����
        Image button2Image = button2Object.AddComponent<Image>();
        button2Image.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // ���ð�ť�ı�
        GameObject button2TextObject = new GameObject("ButtonText");
        button2TextObject.transform.SetParent(button2Object.transform);
        Text button2Text = button2TextObject.AddComponent<Text>();
        button2Text.text = this.button2Text;

        // ʹ��ָ�����Զ�������
        if (customFont != null)
        {
            button2Text.font = customFont;
        }
        else
        {
            button2Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        button2Text.fontSize = (int)(promptFontSize * 0.9f);
        button2Text.color = Color.black;
        button2Text.alignment = TextAnchor.MiddleCenter;

        // ���ð�ť2 RectTransform
        RectTransform button2Rect = button2Object.GetComponent<RectTransform>();
        button2Rect.anchorMin = new Vector2(0.55f, 0.1f);
        button2Rect.anchorMax = new Vector2(0.9f, 0.3f);
        button2Rect.offsetMin = new Vector2(5, 5);
        button2Rect.offsetMax = new Vector2(-5, -5);

        // ���ð�ť�ı�RectTransform
        RectTransform button2TextRect = button2TextObject.GetComponent<RectTransform>();
        button2TextRect.anchorMin = Vector2.zero;
        button2TextRect.anchorMax = Vector2.one;
        button2TextRect.offsetMin = Vector2.zero;
        button2TextRect.offsetMax = Vector2.zero;

        // ���¶Ի���λ��
        UpdateDialogPosition();

        // �����Ի����ʱ��
        if (dialogDisplayTime > 0)
        {
            dialogDisplayTimer = dialogDisplayTime;
        }

        Debug.Log("�Ի��򴴽����");
    }

    // ��ť����¼�����
    void OnButtonClicked(int buttonIndex)
    {
        Debug.Log($"��ť {buttonIndex} �����");

        if (buttonIndex == correctButtonIndex)
        {
            Debug.Log("ѡ������ȷѡ��");
            onCorrectChoice?.Invoke();
        }
        else
        {
            Debug.Log("ѡ���˴���ѡ��");
            onIncorrectChoice?.Invoke();
            TriggerRain();
        }

        HideDialog();
    }

    // ���������ضԻ���
    void HideDialog()
    {
        if (dialogUI != null)
        {
            Destroy(dialogUI);
            dialogUI = null;
            dialogActive = false;
            Debug.Log("�Ի����ѹر�");
        }
    }

    // ���������¶Ի���λ��
    void UpdateDialogPosition()
    {
        if (dialogUI == null)
        {
            Debug.LogWarning("���Ը��¶Ի���λ�ã����Ի���UIΪ��");
            return;
        }

        // ȷ���������
        if (Camera.main == null)
        {
            Debug.LogError("�Ҳ�����������޷����¶Ի���λ��");
            return;
        }

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, dialogOffsetY, 0));

        // ����UIλ��
        RectTransform dialogRect = dialogUI.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.anchoredPosition = Vector2.zero;
    }

    // ���������¶Ի����ʱ��
    void UpdateDialogTimer()
    {
        if (dialogDisplayTimer > 0)
        {
            dialogDisplayTimer -= Time.deltaTime;
            if (dialogDisplayTimer <= 0 && dialogUI != null)
            {
                Debug.Log("�Ի���ʱ���Զ�ѡ�����");
                onIncorrectChoice?.Invoke();
                TriggerRain(); // ��Ӵ�����
                HideDialog();
            }
        }
    }

    void TriggerRain()
    {
        if (rainSystem == null)
        {
            // �����Զ�����
            rainSystem = FindObjectOfType<RainSystem>();
            if (rainSystem == null)
            {
                Debug.LogError("������δ�ҵ�RainSystem��");
                return;
            }
        }

        // ȷ����ϵͳ������
        if (!rainSystem.gameObject.activeInHierarchy)
        {
            rainSystem.gameObject.SetActive(true);
        }

        rainSystem.rainDuration = this.rainDuration;
        rainSystem.StartRain();

        Debug.Log($"�Ѵ������꣬����{rainDuration}��");
    }

    void LateUpdate()
    {
        // ȷ����ʾλ�ø��濪��
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }

        // ȷ���Ի���λ�ø��濪��
        if (dialogUI != null)
        {
            UpdateDialogPosition();
        }
    }

    // ���ӻ�������Χ�����ڱ༭������ʾ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}