using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.PlayBGM("TitleScene");
    }

    public void ClickStart(string sceneName)
    {
        SoundManager.instance.PlaySE("ButtonClick");
        SoundManager.instance.StopBGM();
        SceneManager.LoadScene(sceneName);
    }


    public void ClickExit()
    {
        SoundManager.instance.PlaySE("ButtonClick");
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
