using UnityEngine;
using UnityEngine.UI;

public class WeaponChangeUI : MonoBehaviour
{
    
    public RectTransform click_Weapon_Buy_Button;  // 클릭한 구매 버튼
    [SerializeField] private GameObject[] weapon_slot_Image; // 무기 슬롯 이미지

    //필요한 컴포넌트들
    [SerializeField] RectTransform weapon_Change_UI;
    [SerializeField] private WeaponManager theWeaponManager;

    Vector2 offset = new Vector2(193, 193); // 위치 보정값

    private void OnEnable()
    {
        SlotImageChange();
        PlaceSlotAtButton();
    }

    public void PlaceSlotAtButton()
    {
            // 구매 버튼 RectTransform
            RectTransform buttonRect = click_Weapon_Buy_Button;

            // 버튼 월드 좌표 → 슬롯 부모 로컬 좌표 변환
            Vector3 worldPos = buttonRect.position;
            Vector3 localPos = weapon_Change_UI.parent.InverseTransformPoint(worldPos);

            // 오프셋 적용 (부모 스케일 영향을 그대로 반영)
            localPos += (Vector3)offset;

           // 무기 교체 UI 위치 적용
           weapon_Change_UI.localPosition = localPos;
    }

    public void SlotImageChange()
    {
        for (int i = 0; i < weapon_slot_Image.Length; i++)
        {
            // 슬롯에 무기가 없다면
            if (theWeaponManager.currentWeapons[i] == null)
            {
                //켜져있으면 끈다.
                if (weapon_slot_Image[i].GetComponent<Image>().enabled)
                     weapon_slot_Image[i].GetComponent<Image>().enabled = false;
            }
            // 슬롯에 무기가 있다면
            else
            {
                //꺼져있으면 킨다.
                if (!weapon_slot_Image[i].GetComponent<Image>().enabled)
                    weapon_slot_Image[i].GetComponent<Image>().enabled = true;
                weapon_slot_Image[i].GetComponent<Image>().sprite = theWeaponManager.currentWeapons[i].GetComponent<Gun>().gunImage;
            }
        }
    }

}
