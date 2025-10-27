using UnityEngine;
using UnityEngine.XR;

public class GrenadeController : MonoBehaviour
{
    //처음 플레이어 소환시 pistol이 장착되어 있으므로 grenadecontroller가 작동하지 않도록 꺼준다.
    public static bool isActivate = false;

    public int grenadecount = 1;

    [Header("Grenabe Prefab")]
    [SerializeField] private GameObject grenadePrefab;

    [Header("Grenade Setting")]
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;
    [SerializeField] private Transform throwPosition;
    [SerializeField] private Vector3 throwDirection = new Vector3(0, 1, 0);

    [Header("Grenade Force")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float maxForce = 20f;

    [Header("Trajectory Setting")]
    [SerializeField] private LineRenderer trajectoryLine;

    [SerializeField] private Camera playerCam;
    [SerializeField] private Animator grenadeAnim;

    private bool isCharging = false;
    private float chargeTime = 0f;

    private void Start()
    {
        isActivate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isOpenShopMenu || GameManager.isPreviewActivated || GameManager.isPlayerDead )
        { return; }

        if (isActivate)
        {
                if (grenadecount > 0)
                {
                    if (Input.GetKeyDown(throwKey))
                    {
                        StartThrowing();
                    }
                    if (isCharging)
                    {
                        ChargeThrow();
                    }
                    if (Input.GetKeyUp(throwKey))
                    {
                        ReleaseThrow();
                    }

                }
        }
    }

    void StartThrowing()
    {
        isCharging = true;
        chargeTime = 0f;

        trajectoryLine.enabled = true;
    }

    void ChargeThrow()
    {
        chargeTime += Time.deltaTime;

        Vector3 grenadeVelocity = (playerCam.transform.forward + throwDirection).normalized * Mathf.Min(chargeTime * throwForce, maxForce);
        ShowTrajectory(throwPosition.position + throwPosition.forward, grenadeVelocity);
    }

    void ReleaseThrow()
    {
        grenadeAnim.SetTrigger("Throw");
        ThrowGrenade(Mathf.Min(chargeTime * throwForce, maxForce));
        isCharging = false;

        grenadecount -= 1;

        trajectoryLine.enabled = false;
    }

    void ThrowGrenade(float force)
    {
        Vector3 spawnPosition = throwPosition.position + playerCam.transform.forward;
        GameObject grenade = Instantiate(grenadePrefab, spawnPosition, playerCam.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Vector3 finalThrowDirection = (playerCam.transform.forward + throwDirection).normalized;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);

        SoundManager.instance.PlaySE("Throw");
    }

    void ShowTrajectory(Vector3 origin , Vector3 speed)
    {
        float maxTime = 2.5f;

        Vector3[] points = new Vector3[50];
        trajectoryLine.positionCount = points.Length;
       for (int i = 0; i < points.Length; i++)
        {
            float time = i * (maxTime / 50);
            //float time = i * 0.1f;
            points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;
        }

        trajectoryLine.SetPositions(points); 
    }

    public void GrenadeChange(GameObject _GrenadeArm)
   {
        //다른 무기를들고 있으면 그 무기를 비활성화
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        //gun controller와 weaponmanager에서 무기를 교체
        WeaponManager.currentWeapon = _GrenadeArm.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = _GrenadeArm.GetComponent<Animator>();

        //바뀐 무기를 활성화
        _GrenadeArm.gameObject.SetActive(true);
    }
}
