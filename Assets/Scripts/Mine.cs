using UnityEditor;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("폭발 관련 변수들")]
    [SerializeField] private float radius = 5f;        // 폭발 반경
    [SerializeField] private int damage = 50;          // 피해량
    [SerializeField] private LayerMask hitMask;        // 영향 받을 레이어
    [SerializeField] private GameObject vfxPrefab;     // 폭발 이펙트 프리팹

    [Header("Audio Effects")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private Vector3 explosionParticleOffset = new Vector3(0, 1, 0);
    [SerializeField] private GameObject audioSourcePrefab;

    private bool isExplode = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || isExplode) return;
        Explode();
        
    }
    void Explode()
    {
        isExplode = true;
        Vector3 center = transform.position;

        var cols = Physics.OverlapSphere(center, radius, hitMask);
        foreach (var col in cols)
        {
            var zombie = col.GetComponentInParent<Zombie>();
            //Debug.Log(zombie);
            if (zombie != null)
                zombie.decreaseHp(damage);
        }
        if (vfxPrefab != null)
            Instantiate(vfxPrefab, center, Quaternion.identity);
        PlaySoundAtPosition(explosionSound);
        Destroy(gameObject);
    }

    void PlaySoundAtPosition(AudioClip clip)
    {
        GameObject audioSourceObject = Instantiate(audioSourcePrefab, transform.position + explosionParticleOffset, Quaternion.identity);
        AudioSource instantiatedAudioSource = audioSourceObject.GetComponent<AudioSource>();
        instantiatedAudioSource.clip = clip;
        instantiatedAudioSource.spatialBlend = 1;
        instantiatedAudioSource.Play();

        Destroy(audioSourceObject, instantiatedAudioSource.clip.length);
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
