using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public GunController gunController;
    public PlayerStatus playerStatus;
    public PlayerMovement playerMovement;

    public void Heal()
    {
        SoundManager.instance.PlaySE("Heal");
        playerStatus.currentHp = playerStatus.maxHp;
        playerStatus.theHealthUI.HpUpdate();
    }

    public void ReloadAmmo()
    {

        Gun currentGun = gunController.GetGun();
        SoundManager.instance.PlaySE("Reload");
        currentGun.currentBulletCount = currentGun.reloadBulletCount;
        currentGun.carryBulletCount = currentGun.maxBulletCount - currentGun.reloadBulletCount;
    }

    public void ReSheild()
    {
        playerStatus.currentShield = playerStatus.maxShield;
        playerStatus.theHealthUI.HpUpdate();
    }

    public void SpeedUp()
    {
        StopCoroutine(SpeedUpBuff());//중복방지
        StartCoroutine(SpeedUpBuff());
    }

    private IEnumerator SpeedUpBuff()
    {
        playerMovement.walkSpeed += 3;
        playerMovement.runSpeed += 3;

        yield return new WaitForSeconds(5f);

        playerMovement.walkSpeed -= 3;
        playerMovement.runSpeed -= 3;
    }

}
