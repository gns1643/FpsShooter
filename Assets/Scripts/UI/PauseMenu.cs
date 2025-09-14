using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject BaseUi;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.isPause)
                CallMenu();
            else
                CloseMenu();
        }
    }   

    private void CallMenu()
    {
        GameManager.isPause = true;
        BaseUi.SetActive(true);
        Time.timeScale = 0f;
    }

    private void CloseMenu()
    {
        GameManager.isPause = false;
        BaseUi.SetActive(false);
        Time.timeScale = 1f;
    }
}
