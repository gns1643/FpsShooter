using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //플레이어의 움직임 제어
    public static bool isOpenCraftMenu = false; //건축 메뉴 활성화 여부

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpenCraftMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }
    }
}
