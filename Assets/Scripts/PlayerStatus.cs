using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatus : MonoBehaviour
{
    public int currentHp;
    public int maxHp = 100;

    public GameObject bloodyScreen;

    public Animator DeathAnimator;
    public GameObject WeaponManager;

    public HealthUI theHealthUI;

    //나중에 추가

    void Start()
    {
        currentHp = maxHp;
        theHealthUI.HpUpdate();
    }
    
    public void TakeDamage(int m_damage)
    {
        //
        if (currentHp - m_damage > 0)
        {
            StartCoroutine(BloodyScreenEffect());
            currentHp -= m_damage;
            theHealthUI.HpUpdate();
            Debug.Log(currentHp);
        }
        else
        {
            currentHp = 0; 
            theHealthUI.HpUpdate();
            PlayerDie();
            Debug.Log("사망함");
        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 2f;
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
    }

    private void PlayerDie()
    {
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        WeaponManager.SetActive(true);
        DeathAnimator.enabled = true;
    }


}
