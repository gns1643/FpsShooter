using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopTab : MonoBehaviour
{
    //무기 교체 UI
    [SerializeField] private GameObject weapon_Change_UI;
    //상점 무기 버튼
    [SerializeField] private RectTransform[] weapon_Buy_Button;
    //상점에서 판매하는 무기 프리펩
    [SerializeField] private GameObject[] shopWeaponPrefab;

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
            // 이부분에 player돈 과 관련한 조건문을 걸어서 player돈 이상이면 아래코드를 실행하고 아니면 돈 부족이라고 뜸

            //구매할 무기 프리펩과 버튼이 몇 번째 슬롯에 있는지 찾기
            BuyWeaponType buyWeaponType;
            Enum.TryParse(buyWeaponName, out buyWeaponType);
            int slotIndex = (int)buyWeaponType;

            //구매할 무기 프리펩 가져오기
            buyWeaponPrefab = shopWeaponPrefab[slotIndex];

            //클릭한 구매 버튼을 가져오기
            weapon_Change_UI.GetComponent<WeaponChangeUI>().click_Weapon_Buy_Button = weapon_Buy_Button[slotIndex];

            //무기교체UI 켜주기
            weapon_Change_UI.SetActive(true);
        }

    }

    // 무기를 교체할 슬롯 버튼을 선택했을 때
    public void Slot_Click(int slotIndex)
    {
        theWeaponManager.StartCoroutine(theWeaponManager.EquipWeaponSlot(buyWeaponPrefab, slotIndex));
        weapon_Change_UI.SetActive(false);
    }

  
}
