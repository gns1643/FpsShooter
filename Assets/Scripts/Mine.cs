using UnityEditor;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("폭발 관련 변수들")]
    [SerializeField] private float radius = 5f;        // 폭발 반경
    [SerializeField] private int damage = 50;          // 피해량
    [SerializeField] private LayerMask hitMask;        // 영향 받을 레이어
    [SerializeField] private GameObject vfxPrefab;     // 폭발 이펙트 프리팹

    private bool isExplode = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || isExplode) return;
        Explode();
        
    }
    void Explode()
    {
        Debug.Log("펑");
        isExplode = true;
        Vector3 center = transform.position;

        var cols = Physics.OverlapSphere(center, radius, hitMask);
        foreach (var col in cols)
        {
            var zombie = col.GetComponentInParent<Zombie>();
            Debug.Log(zombie);
            if (zombie != null)
                zombie.decreaseHp(damage);
        }
        if (vfxPrefab != null)
            Instantiate(vfxPrefab, center, Quaternion.identity);
        Destroy(gameObject);
    }
#if UNITY_EDITOR
    // 에디터에서 반경 가시화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius); // [web:50]
    }
#endif
}
