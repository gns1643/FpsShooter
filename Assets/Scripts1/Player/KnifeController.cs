using UnityEngine;

public class KnifeController : MonoBehaviour
{
    // 활성화 여부.
    public static bool isActivate = true;

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    // 연사 속도 계산
    private float nextTimeToFire = 0f;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera playerCam;
    public PlayerMovement playerMove;


    void Update()
    {
        if (isActivate || playerMove.GetRun())
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / currentGun.fireRate;
                Fire();
            }

        }

    }

    void Fire()
    {
        if (currentGun.currentBulletCount > 0)
        {
            currentGun.anim.SetTrigger("Fire");

            //쏘면 총알 감소
            currentGun.currentBulletCount--;

            //총을 쐈을때 총에 무엇인가 맞으면 실행
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, currentGun.range))
            {
                Debug.Log(hit.transform.name);

                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponent<Zombie>().decreaseHp(currentGun.damage);
                }
            }
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

}
