using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public GunController gunController;
    public GrenadeController grenadeController;
    public PlayerStatus playerStatus;
    public PlayerMovement playerMovement;
    private bool isSpeedUp = false;

    public void ReloadGrenade()
    {
        if (GameManager.UseMoney(30))
        {
            grenadeController.grenadecount += 1;
        }
    }
    public void Heal()
    {
        if (GameManager.UseMoney(150))
        {
            SoundManager.instance.PlaySE("Heal");
            playerStatus.currentHp = playerStatus.maxHp;
            playerStatus.theHealthUI.HpUpdate();
        }
    }

    public void ReloadAmmo()
    {

        if (GameManager.UseMoney(100))
        {
            Gun currentGun = gunController.GetGun();
            SoundManager.instance.PlaySE("Reload");
            currentGun.currentBulletCount = currentGun.reloadBulletCount;
            currentGun.carryBulletCount = currentGun.maxBulletCount - currentGun.reloadBulletCount;
        }
    }

    public void ReSheild()
    {
        if (GameManager.UseMoney(80))
        {
            playerStatus.currentShield = playerStatus.maxShield;
            playerStatus.theHealthUI.ShieldUpdate();
        }
    }

    public void SpeedUp()
    {
        if (GameManager.UseMoney(40))
        {
            if (!isSpeedUp)
                StartCoroutine(SpeedUpBuff());
        }
    }

    private IEnumerator SpeedUpBuff()
    {
        isSpeedUp = true;
        playerMovement.walkSpeed += 4;
        playerMovement.runSpeed += 4;

        yield return new WaitForSeconds(5f);

        playerMovement.walkSpeed -= 4;
        playerMovement.runSpeed -= 4;
        isSpeedUp = false;
    }

}
