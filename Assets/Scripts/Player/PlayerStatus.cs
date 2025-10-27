using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatus : MonoBehaviour
{
    public int currentHp;
    public int maxHp = 100;


    public int currentShield;
    public int maxShield = 50;

    public GameObject bloodyScreen;

    public Animator DeathAnimator;
    public GameObject GunController;

    public HUD theHealthUI;
    public GameObject deadUI;
    public GameObject winUI;
    public GameObject restartButton;

    private bool isBlood;
    //나중에 추가

    void Start()
    {
        currentHp = maxHp;
        currentShield = maxShield;
        theHealthUI.HpUpdate();
        theHealthUI.ShieldUpdate();
    }

    //죽으면 데미지를 더이상 받지 않음
    public void ShieldDamage(int m_damage)
    {
        if (currentShield - m_damage >= 0)
        {
            currentShield -= m_damage;
            theHealthUI.ShieldUpdate();
        }
        else //(currentHp - m_damage < 0)
        {
            int realDamge = m_damage - currentShield;
            TakeDamage(realDamge);
            currentShield = 0;
            theHealthUI.ShieldUpdate();
        }

    }

    public void TakeDamage(int m_damage)
    {
        if (currentHp - m_damage > 0)
        {
            if (!isBlood)
                StartCoroutine(BloodyScreenEffect());
            currentHp -= m_damage;
            theHealthUI.HpUpdate();
        }
        else
        {
            GameManager.isPlayerDead = true;
            currentHp = 0;
            theHealthUI.HpUpdate();
            PlayerDie();
        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        isBlood = true;

        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 0.7f;
        float elapsedTime = 0f;

        while (elapsedTime < duration) 
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if(bloodyScreen.activeInHierarchy)
        {
         
            bloodyScreen.SetActive(false);
        }

        isBlood = false;
    }

     private void PlayerDie()
    {
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        DeathAnimator.enabled = true;

        SoundManager.instance.StopAllSE();
        SoundManager.instance?.PlaySE("PlayerScream");

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);

        deadUI.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void GameEnd()
    {
        GameManager.isPlayerDead = true;

        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        winUI.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

    }
 

}
