using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartButtonScript : MonoBehaviour
{
    public Button button; // 开始按钮
    public string targetScene; // 目标场景名称
    public VideoPlayer videoPlayer; // 视频播放器
    public Image coverImage; // 封面Image
    public RawImage videoScreen; // 视频显示区域（改为RawImage）
    public Canvas mainCanvas; // 主界面Canvas

    private void Start()
    {
        button.onClick.AddListener(PlayVideoAndLoadScene);
        ShowCoverImage(); // 初始显示封面
        HideVideoUI(); // 隐藏视频UI
    }

    private void PlayVideoAndLoadScene()
    {
        button.interactable = false;
        HideCoverImage(); // 隐藏封面

        // 隐藏其他UI元素
        foreach (Transform child in mainCanvas.transform)
        {
            if (child.GetComponent<Image>() != coverImage &&
                child.GetComponent<RawImage>() != videoScreen)
            {
                child.gameObject.SetActive(false);
            }
        }

        // 显示视频并播放
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