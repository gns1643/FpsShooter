using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explision Prefab")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private Vector3 explosionParticleOffset = new Vector3(0, 1, 0);
    [SerializeField] private GameObject audioSourcePrefab;

    [Header("Explosion Setting")]
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 50;          // 피해량
    [SerializeField] private LayerMask hitMask;        // 영향 받을 레이어

    [Header("Audio Effects")]
    [SerializeField] private AudioClip explosionSound;

    private float countdown;
    private bool hasExploded = false;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdown = explosionDelay;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
       if(!hasExploded)
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0f)
            {
                Explode();
                hasExploded = true; 
            }
        }
    }

    void Explode()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position + explosionParticleOffset, Quaternion.identity);
        Destroy(explosionEffect, 3f);
        PlaySoundAtPosition(explosionSound);
        NearDamage();
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

    void NearDamage()
    {
        Vector3 center = transform.position;

        var cols = Physics.OverlapSphere(center, explosionRadius, hitMask);
        foreach (var col in cols)
        {
            var zombie = col.GetComponentInParent<Zombie>();
            if (zombie != null)
                zombie.decreaseHp(damage);
        }
    }
    

}
