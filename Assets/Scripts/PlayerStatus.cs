using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int currentHp;
    public int maxHp = 100;
    //���߿� �߰�

    void Start()
    {
        currentHp = maxHp;
    }

    public void decreaseHp(int m_damage)
    {
        if (currentHp - m_damage > 0)
            currentHp = -m_damage;
        else
        {
            currentHp = 0;
            Debug.Log("�����");
        }
    }
}
