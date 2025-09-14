using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public void ClickStart(string sceneName)
    {
        SoundManager.instance.PlaySE("ButtonClick");
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
    }


    public void ClickExit()
    {
        SoundManager.instance.PlaySE("ButtonClick");
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
