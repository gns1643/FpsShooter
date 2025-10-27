
using UnityEngine;


[System.Serializable]
public class Weapon
{
    public string WeaponName; // 이름
    public GameObject shopWeaponPrefab; //무기 프리펩
    public RectTransform weapon_Buy_Button; //무기 구매 버튼                           
    public int shopWeaponMoney; //무기 가격
}

public class WeaponShopTab : MonoBehaviour
{
    //무기 교체 UI
    [SerializeField] private GameObject weapon_Change_UI;
    // 상점에서 판매하는 무기들
    [SerializeField] private Weapon[] weapons;

    //구매 버튼을 클릭한 무기 프리펩
    private GameObject buyWeaponPrefab;

    public enum BuyWeaponType
    {
        carbine
    }

    //필요한 컴포넌트
    [SerializeField] private WeaponManager theWeaponManager;

    //무기 구매 버튼을 클릭했을때
    public void BuyButtonClick(string buyWeaponName)
    {
        //무기교체ui가 켜져있으면 구매버튼을 이미 누른 경우이므로
        //구매버튼을 다시 누르면 무기 교체 ui를 끈다.
        if (weapon_Change_UI.activeSelf)
        {
            weapon_Change_UI.SetActive(false);

        }
        else
        {
            int slotIndex;
            //구매할 무기 찾기
            for (int i = 0; i < weapons.Length; i++)
            {
                if (buyWeaponName == weapons[i].WeaponName)
                {
                    slotIndex = i;

                    // 이부분에 player돈 과 관련한 조건문을 걸어서 player돈 이상이면 아래코드를 실행하고 아니면 돈 부족이라고 뜸
                    if (GameManager.UseMoney(weapons[slotIndex].shopWeaponMoney))
                    {
                        //구매할 무기 프리펩 가져오기
                        buyWeaponPrefab = weapons[slotIndex].shopWeaponPrefab;
                        //클릭한 구매 버튼을 가져오기
                        weapon_Change_UI.GetComponent<WeaponChangeUI>().click_Weapon_Buy_Button = weapons[slotIndex].weapon_Buy_Button;
                        //무기교체UI 켜주기
                        weapon_Change_UI.SetActive(true);
                    }

                    return;
                }
            }
        }

    }

    // 무기를 교체할 슬롯 버튼을 선택했을 때
    public void Slot_Click(int slotIndex)
    {
        theWeaponManager.StartCoroutine(theWeaponManager.EquipWeaponSlot(buyWeaponPrefab, slotIndex));
        weapon_Change_UI.SetActive(false);
    }

  
}
