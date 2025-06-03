using UnityEngine;
using UnityEngine.UI;

public class ExitButtonScript : MonoBehaviour
{
    public Button exitButton; // ������Inspector�й�����ť���

    private void Start()
    {
        // Ϊ��ť��ӵ���¼�������������ť�����ʱ����ExitGame����
        exitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        // ��Unity�༭��������ʱ��ֹͣ��Ϸ����
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        // �ڷ��������Ϸ�У��ر���Ϸ����
        Application.Quit();
#endif
    }
}