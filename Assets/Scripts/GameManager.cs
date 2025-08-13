using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isOpenShopMenu = false; //상점 메뉴 활성화 여부
    public static bool isPreviewActivated = false; //건축 프리뷰 활성화 여부

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpenShopMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
