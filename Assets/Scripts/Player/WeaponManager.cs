using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class WeaponManager : MonoBehaviour
{
    
    // 무기 중복 교체 실행 방지.
    public static bool isChangeWeapon = false;

    //현재 장착하고 있는 무기 컴포넌트
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    //현재 장착하고 있는 무기의 타입.
    [SerializeField]
    private string currentWeaponType;

    //현재 장착중인 무기의 슬롯 번호
    public int currentWeaponSlot;

    // 무기 교체 딜레이
    [SerializeField]
    private float changeWeaponDelayTime;

    //슬롯에 장착하고 있는 무기들.
    public GameObject[] currentWeapons = new GameObject[2];

    //무기 장착 슬롯들
    [SerializeField] 
    private Transform[] weaponSlots = new Transform[2];

    //필요한 컴포넌트.
    [SerializeField]
    private GunController theGunController;
    public GameObject examplePistol;


    void Start()
    {
        StartCoroutine(EquipWeaponSlot(examplePistol, 0));
    }
    

    void Update()
    {
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapons[0] != null)
            {
                StartCoroutine(ChangeWeaponCoroutine(0));
            }
            if(Input.GetKeyDown(KeyCode.Alpha2) && currentWeapons[1] != null)
            {
                StartCoroutine(ChangeWeaponCoroutine(1));
            }
        }
    }
    
    // 무기 교체 코루틴.
    public IEnumerator ChangeWeaponCoroutine(int slotIndex)
    {
        Gun _gun = currentWeapons[slotIndex].GetComponent<Gun>();

        isChangeWeapon = true;
        CancelPreWeaponAction();

        yield return new WaitForSeconds(changeWeaponDelayTime);

        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        WeaponChange( _gun);

        currentWeaponSlot = slotIndex;
        currentWeaponType = _gun.WeaponType;

        GunController.isActivate = true;
        isChangeWeapon = false;
    }

    // 무기 취소 함수.
    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;

        }
    }

    // 무기 교체 함수.
    private void WeaponChange(Gun _gun)
    {
        if (_gun.WeaponType == "GUN")
            theGunController.GunChange(_gun);
       
    }


    // 상점에서 무기 구매 후 무기 장착 코루틴
    public IEnumerator EquipWeaponSlot(GameObject weaponPrefab, int slotIndex)
    {
        isChangeWeapon = true;
        GunController.isActivate = false;

        currentWeaponSlot = slotIndex;

        // 기존 무기 삭제
        if (currentWeapons[slotIndex] != null)
        {
            Destroy(currentWeapons[slotIndex]);
        }

        // 새로운 무기 생성
        GameObject newWeapon = Instantiate(weaponPrefab, weaponSlots[slotIndex]);
        currentWeapons[slotIndex] = newWeapon;
        WeaponChange(currentWeapons[slotIndex].GetComponent<Gun>());

        yield return new WaitForSeconds(1.06f);

        currentWeaponType = currentWeapons[slotIndex].GetComponent<Gun>().WeaponType;
        GunController.isActivate = true;
        isChangeWeapon = false;
    }

}
