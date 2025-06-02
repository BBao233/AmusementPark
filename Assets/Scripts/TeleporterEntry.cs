using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterEntry : MonoBehaviour
{
    [Header("��������")]
    public TeleporterExit targetExit; // Ŀ�����
    public float interactionRadius = 1f; // ������Χ

    [Header("��ʾUI����")]
    public bool createPromptUI = true;
    public string malePromptText = "��E����";
    public string femalePromptText = "��L����";
    public Color malePromptColor = Color.blue;
    public Color femalePromptColor = Color.magenta;
    public float promptOffsetY = 1.2f;
    public float promptFontSize = 16f;

    [Header("��������")]
    public Font customFont; // �������Զ��������ֶ�

    [Header("��Ч����")]
    public AudioClip interactionSound;  // ������Ч
    [Range(0f, 1f)] public float volume = 1f;  // �������ƻ���
    private AudioSource audioSource;    // ��ƵԴ���

    private bool maleInRange = false;
    private bool femaleInRange = false;
    private GameObject malePromptUI;
    private GameObject femalePromptUI;

    void Start()
    {
        // ������ʾUI
        if (createPromptUI)
        {
            CreatePromptUI();
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Update()
    {
        // ������ʾλ��
        if (malePromptUI != null && malePromptUI.activeSelf)
        {
            UpdatePromptPosition(malePromptUI);
        }
        if (femalePromptUI != null && femalePromptUI.activeSelf)
        {
            UpdatePromptPosition(femalePromptUI);
        }

        // ��鴫������
        if (maleInRange && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer("Male");
        }

        if (femaleInRange && Input.GetKeyDown(KeyCode.L))
        {
            TeleportPlayer("Female");
        }
    }

    void TeleportPlayer(string playerTag)
    {
        if (targetExit == null)
        {
            Debug.LogError("δ����Ŀ����ڣ�");
            return;
        }

        // ������Ҷ���
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            // ������ҵ�����
            player.transform.position = targetExit.GetExitPosition();
            Debug.Log($"{playerTag} �Ѵ��͵� {targetExit.gameObject.name}");

            if (interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound, volume);
            }

            // ���Ŵ���Ч������ѡ��
            if (targetExit.teleportEffect != null)
            {
                Instantiate(targetExit.teleportEffect, player.transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning($"δ�ҵ���ǩΪ {playerTag} ����Ҷ���");
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
            Debug.Log("���Խ��봫������");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = true;
            Debug.Log("Ů�Խ��봫������");
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
            Debug.Log("�����뿪��������");
        }
        else if (other.CompareTag("Female"))
        {
            femaleInRange = false;
            Debug.Log("Ů���뿪��������");
        }

        UpdatePromptVisibility();
    }

    void UpdatePromptVisibility()
    {
        if (malePromptUI != null)
        {
            malePromptUI.SetActive(maleInRange);
        }

        if (femalePromptUI != null)
        {
            femalePromptUI.SetActive(femaleInRange);
        }
    }

    void CreatePromptUI()
    {
        // Ѱ��Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TeleporterCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("�����µ�Canvas���ڴ�����ʾ");
        }

        // ����������ʾ�ı�
        malePromptUI = CreatePrompt(canvas.transform, "MalePrompt", malePromptText, malePromptColor);
        malePromptUI.SetActive(false);

        // ����Ů����ʾ�ı�
        femalePromptUI = CreatePrompt(canvas.transform, "FemalePrompt", femalePromptText, femalePromptColor);
        femalePromptUI.SetActive(false);

        // ����λ��ƫ��
        if (malePromptUI != null && femalePromptUI != null)
        {
            RectTransform maleRect = malePromptUI.GetComponent<RectTransform>();
            RectTransform femaleRect = femalePromptUI.GetComponent<RectTransform>();

            // ������ʾ���Ϸ�
            maleRect.anchoredPosition = new Vector2(0, 20);

            // Ů����ʾ���·�
            femaleRect.anchoredPosition = new Vector2(0, -20);
        }

        Debug.Log("������ʾUI�������");
    }

    GameObject CreatePrompt(Transform parent, string name, string text, Color color)
    {
        GameObject prompt = new GameObject(name);
        prompt.transform.SetParent(parent);

        Text textComponent = prompt.AddComponent<Text>();
        textComponent.text = text;
        textComponent.color = color;
        textComponent.fontSize = (int)promptFontSize;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // ����ʹ���Զ�������
        if (customFont != null)
        {
            textComponent.font = customFont;
            Debug.Log($"ʹ���Զ�������: {customFont.name}");
        }
        else
        {
            // ʹ�øĽ�����������߼�
            Font loadedFont = LoadFont();
            if (loadedFont != null)
            {
                textComponent.font = loadedFont;
            }
            else
            {
                Debug.LogWarning("�������ʧ�ܣ�ʹ��Ĭ������");
            }
        }

        // ����RectTransform
        RectTransform rectTransform = prompt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 30);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        return prompt;
    }

    void UpdatePromptPosition(GameObject prompt)
    {
        if (prompt == null || Camera.main == null) return;

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, promptOffsetY, 0));

        // ����UIλ��
        prompt.GetComponent<RectTransform>().position = screenPosition;
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

    // �ڱ༭���п��ӻ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.5f, 1f, 0.5f); // ��ɫ��͸��
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        // ���������Ŀ����ڣ���������
        if (targetExit != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetExit.GetExitPosition());
        }
    }
}