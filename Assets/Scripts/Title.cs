using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public void ClickStart(string sceneName)
    {
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
    }


    public void ClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
