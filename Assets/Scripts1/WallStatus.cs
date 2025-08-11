using UnityEngine;

public class WallStatus : MonoBehaviour
{
    public int currentHp;
    public int maxHp = 100;
    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int m_damage)
    {
        if (currentHp - m_damage > 0)
        {
            currentHp -= m_damage;
        }
        else
        {
            currentHp = 0;
            Destroy(gameObject);
            Debug.Log("º®»Ñ°³Áü");
        }
    }
}
