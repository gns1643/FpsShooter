using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject howToPlayPanel; 
    public GameObject howToPlay1;     
    public GameObject howToPlay2;
    public int currentStats = 0;

    private void Start()
    {
        SoundManager.instance.PlayBGM("TitleScene");
        currentStats = 0;
    }

    public void ClickStart(string sceneName)
    {
        howToPlayPanel.SetActive(true);
        howToPlay1.SetActive(true);
        howToPlay2.SetActive(false);
    }

    public void OnClickNext(string sceneName)
    {
        SoundManager.instance.PlaySE("ButtonClick");

        if (currentStats == 0)
        {
            howToPlay1.SetActive(false);
            howToPlay2.SetActive(true);
            currentStats++;
        }
        else if (currentStats == 1)
        {
            howToPlayPanel.SetActive(false);
            SoundManager.instance.StopBGM();
            SceneManager.LoadScene(sceneName);
            currentStats = 0;
        }
    }

    public void ClickReStart(string sceneName)
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
