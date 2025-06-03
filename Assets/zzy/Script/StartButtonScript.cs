using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartButtonScript : MonoBehaviour
{
    public Button button; // ��ʼ��ť
    public string targetScene; // Ŀ�곡������
    public VideoPlayer videoPlayer; // ��Ƶ������
    public Image coverImage; // ����Image
    public RawImage videoScreen; // ��Ƶ��ʾ���򣨸�ΪRawImage��
    public Canvas mainCanvas; // ������Canvas

    private void Start()
    {
        button.onClick.AddListener(PlayVideoAndLoadScene);
        ShowCoverImage(); // ��ʼ��ʾ����
        HideVideoUI(); // ������ƵUI
    }

    private void PlayVideoAndLoadScene()
    {
        button.interactable = false;
        HideCoverImage(); // ���ط���

        // ��������UIԪ��
        foreach (Transform child in mainCanvas.transform)
        {
            if (child.GetComponent<Image>() != coverImage &&
                child.GetComponent<RawImage>() != videoScreen)
            {
                child.gameObject.SetActive(false);
            }
        }

        // ��ʾ��Ƶ������
        ShowVideoUI();
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(targetScene);
    }

    private void ShowCoverImage()
    {
        coverImage.gameObject.SetActive(true);
    }

    private void HideCoverImage()
    {
        coverImage.gameObject.SetActive(false);
    }

    private void ShowVideoUI()
    {
        videoScreen.gameObject.SetActive(true);
    }

    private void HideVideoUI()
    {
        videoScreen.gameObject.SetActive(false);
    }
}