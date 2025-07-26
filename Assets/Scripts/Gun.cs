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

    //���� ���� ���� ��ü �� �ٽ� ����� ���ƿö�

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

            //Ÿ�Ͽ��� Ÿ���� ������ �������� ���� public �ڵ带 �ۼ� �ʿ�
            if(hit.transform.tag == "Enemy")
            {
                hit.transform.GetComponent<Zombie>().decreaseHp((int)damage);
            }
            else
            {
                Debug.Log("����");
            }


                //�Ѿ� ���� ����Ʈ ����
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
