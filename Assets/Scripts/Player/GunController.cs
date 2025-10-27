using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    
    // 활성화 여부.
    public static bool isActivate = true;

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    // 연사 속도 계산
    private float nextTimeToFire = 0f;

    // 상태 변수
    private bool isReload = false;
    private bool isZoom = false;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera playerCam;
    public PlayerMovement playerMove;

    // 피격 이펙트.
    [SerializeField]
    private GameObject hit_effect_prefab;


    void Update()
    {
       // Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * currentGun.range, Color.red);

        if (GameManager.isOpenShopMenu || GameManager.isPreviewActivated || GameManager.isPlayerDead)
            { return; }

        if (isActivate)
        {
            if (!playerMove.GetRun())
            {
                if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && !isReload)
                {
                    nextTimeToFire = Time.time + currentGun.fireRate;
                    Fire();
                }

                if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
                {
                    StartCoroutine(ReloadCoroutine());
                    return;
                }

                if (Input.GetButtonDown("Fire2") && !isReload)
                {
                    Zoom();
                }
            }
            
        }

    }

    void Fire()
    {
        if (currentGun.currentBulletCount > 0)
        { 
             currentGun.muzzleFlash.Play();

            //사운드 재생
            if(currentGun.WeaponName == "shotgun")
             SoundManager.instance.PlaySE("ShotGunFire");
            else if(currentGun.WeaponName == "pistol")
                SoundManager.instance.PlaySE("PistolFire");
            else
                SoundManager.instance.PlaySE("GunFire");

            currentGun.anim.SetTrigger("Fire");

             //쏘면 총알 감소
             currentGun.currentBulletCount--;

            //총을 쐈을때 총에 무엇인가 맞으면 실행
             RaycastHit hit;
             if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, currentGun.range))
             { 
                 // Debug.Log(hit.transform.name);

                 if (hit.transform.tag == "Enemy")
                 {
                    hit.transform.GetComponent<Zombie>().decreaseHp(currentGun.damage);
                 }

                 //총알 맞은 이펙트 생성
                  GameObject hit_effect = Instantiate(hit_effect_prefab, hit.point, Quaternion.LookRotation(hit.normal));
                  Destroy(hit_effect, 2f);
             }
        }
    }

    void Zoom()
    {
        isZoom = !isZoom;

        if (isZoom)
        {
            currentGun.anim.SetBool("Zoom", true);
        }
        else
        {
            currentGun.anim.SetBool("Zoom", false);
        }

    }

    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;

            SoundManager.instance.PlaySE("Reload");
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    public void GunChange(Gun _gun)
    {
        //다른 무기를들고 있으면 그 무기를 비활성화
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        //gun controller와 weaponmanager에서 무기를 교체
        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        //애니매이션으로 바뀐 위치를 다시 원래대로 바꿈
        currentGun.transform.localPosition = currentGun.originPos;
        currentGun.transform.localRotation = Quaternion.identity;

        //바뀐 무기를 활성화
        currentGun.gameObject.SetActive(true);
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetZoom()
    {
        return isZoom;
    }

}
