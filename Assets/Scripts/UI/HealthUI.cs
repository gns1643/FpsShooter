using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public TMP_Text hp_Text;
    public PlayerStatus thePlayerStatus;
    public void HpUpdate()
    {
        hp_Text.text = thePlayerStatus.currentHp.ToString();
    }
}
