using JetBrains.Annotations;
using UnityEngine;

public class InterActionManager : MonoBehaviour
{
    public GunController gunController;
    public PlayerStatus playerStatus;
    public Camera playerCam;

    private void Update()
    {
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * 10, Color.red);

        RaycastHit hit;

        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10))
        {

            if (hit.transform.tag == "AmmoBox" || hit.transform.tag == "AidKit")
            {
                
                if(Input.GetKeyDown(KeyCode.F))
                {
                    if (hit.transform.tag == "AmmoBox")
                    {
                        Gun currentGun = gunController.GetGun();
                        SoundManager.instance.PlaySE("Reload");
                        currentGun.currentBulletCount = currentGun.reloadBulletCount;
                        currentGun.carryBulletCount = currentGun.maxBulletCount - currentGun.reloadBulletCount;
                    }

                    if(hit.transform.tag == "AidKit")
                    {
                        SoundManager.instance.PlaySE("Heal");
                        playerStatus.currentHp = playerStatus.maxHp;
                        playerStatus.theHealthUI.HpUpdate();
                    }
                         
                }
            }
        }
    }
}
