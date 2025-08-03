using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    private TMP_Text hp_Text;
    public PlayerStatus thePlayerStatus;
    private void Start()
    {
        hp_Text = GetComponentInChildren<TMP_Text>();
    }
    public void HpUpdate()
    {
        hp_Text.text = thePlayerStatus.currentHp.ToString();
    }
}
