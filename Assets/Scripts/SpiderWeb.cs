using UnityEngine;
using UnityEngine.AI;

public class SpiderWeb : MonoBehaviour
{
    [Header("감소할 스피드 배수")]
    [SerializeField] private float slowSpeed = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        var zombie = other.GetComponent<Zombie>();
        if (zombie != null)
            zombie.SlowDown(slowSpeed);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        var zombie = other.GetComponent<Zombie>();
        if (zombie != null)
            zombie.RestoreSpeed();
    }
}
