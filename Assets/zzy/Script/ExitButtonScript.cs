using UnityEngine;
using UnityEngine.UI;

public class ExitButtonScript : MonoBehaviour
{
    public Button exitButton; // 用于在Inspector中关联按钮组件

    private void Start()
    {
        // 为按钮添加点击事件监听器，当按钮被点击时调用ExitGame方法
        exitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        // 在Unity编辑器中运行时，停止游戏运行
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        // 在发布后的游戏中，关闭游戏进程
        Application.Quit();
#endif
    }
}