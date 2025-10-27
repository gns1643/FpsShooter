using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 총알 개수 텍스트
    [SerializeField]
    private TMP_Text[] Bullet_text;
    [SerializeField]
    private TMP_Text Grenade_text;
    [SerializeField]
    private Sprite Grenade_image;

    //HP 텍스트
    [SerializeField]
    private TMP_Text hp_Text;

    //Shield 텍스트
    [SerializeField]
    private TMP_Text Shield_Text;

    //서브 무기들의 이미지와 슬롯 번호 텍스트
    [SerializeField]
    private GameObject[] theSubWeaponImage;
    [SerializeField]
    private TMP_Text[] theSubSlotNumber;

    //현재 장착하고 있는 무기의 이미지와 슬롯 번호 텍스트
    [SerializeField]
    private GameObject theCurrentWeaponImage;
    [SerializeField]
    private TMP_Text theCurrentSlotNumber;

    // 필요한 컴포넌트
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private GrenadeController theGrenadeController;
    public PlayerStatus thePlayerStatus;

    void Update()
    {
        CheckWeaponImage(theWeaponManager.currentWeaponSlot);
        CheckWeaponText();
        CheckBullet();
    }

    public void HpUpdate()
    {
        hp_Text.text = thePlayerStatus.currentHp.ToString();
    }

    public void ShieldUpdate()
    {
        Shield_Text.text = thePlayerStatus.currentShield.ToString();
    }

    public void CheckBullet()
    {
        if (theWeaponManager.currentWeaponType == "GUN")
        {
            Bullet_text[0].text = theWeaponManager.currentWeapons[theWeaponManager.currentWeaponSlot].GetComponent<Gun>().carryBulletCount.ToString();
            Bullet_text[1].text = theWeaponManager.currentWeapons[theWeaponManager.currentWeaponSlot].GetComponent<Gun>().currentBulletCount.ToString();
            Bullet_text[2].text = theWeaponManager.currentWeapons[theWeaponManager.currentWeaponSlot].GetComponent<Gun>().reloadBulletCount.ToString();
        }
        else if (theWeaponManager.currentWeaponType == "GRENADE")
            Grenade_text.text = theGrenadeController.grenadecount.ToString();

    }

    public void CheckWeaponImage(int currentWeaponSlotIndex)
    {
        //현재 장착 중인 무기 이미지 관리
        if (theWeaponManager.currentWeaponType == "GUN")
            theCurrentWeaponImage.GetComponent<Image>().sprite = theWeaponManager.currentWeapons[currentWeaponSlotIndex].GetComponent<Gun>().gunImage;
        else if (theWeaponManager.currentWeaponType == "GRENADE")
            theCurrentWeaponImage.GetComponent<Image>().sprite = Grenade_image;
        theCurrentSlotNumber.text = (currentWeaponSlotIndex + 1).ToString();

        //서브 무기 이미지들 관리
        List<int> slotIndex = new List<int> { 0, 1, 2 };
        slotIndex.Remove(currentWeaponSlotIndex);

        for (int i = 0; i < slotIndex.Count; i++)
        {
            if (theWeaponManager.currentWeapons[slotIndex[i]] == null)
                theSubWeaponImage[i].GetComponent<Image>().enabled = false;
            else
            {
                theSubWeaponImage[i].GetComponent<Image>().enabled = true;

                if (theWeaponManager.currentWeapons[slotIndex[i]].GetComponent<Gun>() != null)
                    theSubWeaponImage[i].GetComponent<Image>().sprite = theWeaponManager.currentWeapons[slotIndex[i]].GetComponent<Gun>().gunImage;
                else if (theWeaponManager.currentWeapons[slotIndex[i]].GetComponent<Gun>() == null)
                    theSubWeaponImage[i].GetComponent<Image>().sprite = Grenade_image;
            }

            theSubSlotNumber[i].text = (slotIndex[i] + 1).ToString();
        }
    }

    public void CheckWeaponText()
    {

        if (theWeaponManager.currentWeaponType == "GUN")
        {
            for (int i = 0; i < 3; i++)
                Bullet_text[i].gameObject.SetActive(true);
            Grenade_text.gameObject.SetActive(false);
        }
        else if (theWeaponManager.currentWeaponType == "GRENADE")
        {
            for (int i = 0; i < 3; i++)
                Bullet_text[i].gameObject.SetActive(false);
            Grenade_text.gameObject.SetActive(true);
        }

    }

}
