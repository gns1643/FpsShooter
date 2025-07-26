using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;

    public float fireRate = 15f;
    private float nextTimeToFire = 0f;

    public int maxAmmo = 10;
    private int currentAmmo = -1;       
    public float reloadTime = 2.3f;
    private bool isReloading = false;

    private bool isZoom = false;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public Animator anim;
    public GameObject impactEffect;
    public PlayerMovement PlayerMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(currentAmmo == -1)
            currentAmmo = maxAmmo;
    }

    //장전 도중 무기 교체 후 다시 무기로 돌아올때

    // Update is called once per frame
    void Update()
    {
        if (isReloading || PlayerMove.GetRun())
            return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Zoom();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        anim.SetTrigger("Fire");

        currentAmmo--;

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            //타켓에서 타켓이 맞으면 데미지가 들어가는 public 코드를 작성 필요
            if(hit.transform.tag == "Enemy")
            {
                hit.transform.GetComponent<Zombie>().decreaseHp((int)damage);
            }
            else
            {
                Debug.Log("ㅇㅇ");
            }


                //총알 맞은 이펙트 생성
                GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 2f);
        }

    }

    void Zoom()
    {

        isZoom = !isZoom;

        if(isZoom)
        {
            anim.SetBool("Zoom",true);
        }
        else
        {
            anim.SetBool("Zoom", false);
        }

    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading..");

        anim.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

    }
}
