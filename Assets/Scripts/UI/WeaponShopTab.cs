using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static WeaponShopTab;

public class WeaponShopTab : MonoBehaviour
{

    private bool isActivated = false;

    [SerializeField] private GameObject Base_UI;
    [SerializeField] private GameObject Slot_UI;
    [SerializeField] private GameObject[] slot_Image;

    //상점 무기 버튼 - 판매하는 무기 프리펩
    [SerializeField] private GameObject[] buy_Button;
    [SerializeField] private GameObject[] shopWeaponPrefab;

    //살려고 하는 무기 프리펩
    private GameObject buyWeaponPrefab;

    public enum BuyWeaponType
    {
        carbine
    }

    //필요한 컴포넌트
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private RectTransform canvasRect;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenWindow();
        }

    }

    void OpenWindow()
    {
        if (!isActivated)
        {
            isActivated = true;
            Base_UI.SetActive(true);
        }
        else
        {
            isActivated = false;
            Base_UI.SetActive(false);
            Slot_UI.SetActive(false);
        }

    }

    //무기 구매 버튼을 클릭했을때
    public void BuyButtonClick(string buyWeaponName)
    {
        // 이부분에 player돈 과 관련한 조건문을 걸어서 player돈 이상이면 아래코드를 실행하고 아니면 돈 부족이라고 뜸

        //구매할 무기 프리펩 가져오기
        BuyWeaponType buyWeaponType;
        Enum.TryParse(buyWeaponName, out buyWeaponType);
        int slotindex = (int)buyWeaponType;
        buyWeaponPrefab = shopWeaponPrefab[(int)slotindex];

        // 스크린 좌표 → 캔버스 로컬 좌표 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, buy_Button[(int)slotindex].transform.position, null, out localPoint);

        // 구매한 무기 교체 UI 생성
        Slot_UI.GetComponent<RectTransform>().anchoredPosition = new Vector2(localPoint.x + 193, localPoint.y - 193);
        SlotImageChange(0);
        SlotImageChange(1);
        Slot_UI.SetActive(true);

    }


    // 무기 교체 슬롯 버튼을 선택했을 때
    public void Slot_Click(int slotIndex)
    {
        theWeaponManager.StartCoroutine(theWeaponManager.EquipWeaponSlot(buyWeaponPrefab, slotIndex));
        Slot_UI.SetActive(false);

    }

    public void SlotImageChange(int slotIndex)
    {
            if (theWeaponManager.currentWeapons[slotIndex] == null)
            {
                slot_Image[slotIndex].GetComponent<Image>().enabled = false;
            }
            else
            {
                 slot_Image[slotIndex].GetComponent<Image>().enabled = true;
                 slot_Image[slotIndex].GetComponent<Image>().sprite = theWeaponManager.currentWeapons[slotIndex].GetComponent<Gun>().gunImage;
            }
    }
}
