using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isOpenShopMenu = false; //상점 메뉴 활성화 여부
    public static bool isPreviewActivated = false; //건축 프리뷰 활성화 여부
    public static bool isPause = false;
    public static bool isPlayerDead;

    [SerializeField] private TextMeshProUGUI moneyUI;

    public static int playerMoney;



    void Start()
    {
        SoundManager.instance.PlayBGM("CountDownStart");
        isPlayerDead = false;
        playerMoney = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpenShopMenu || isPause || isPlayerDead)
        {
            MoneyUIUpdate();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    static public void AddMoney(int amount)
    {
        playerMoney += amount;
        // UI 갱신 등 추가 작업이 필요할 수 있습니다.
        Debug.Log("현재 돈: " + playerMoney); // 콘솔에 현재 돈을 출력합니다.
    }

    // 돈을 사용하는 함수
    static public bool UseMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            Debug.Log("사용 후 돈: " + playerMoney); // 콘솔에 현재 돈을 출력합니다.
            return true;
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }
    }

    void MoneyUIUpdate()
    {
        int currentMoney = playerMoney;
   
        string fullText = "$"+ " " + currentMoney.ToString();

        moneyUI.text = fullText;
    }
}
