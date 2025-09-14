
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour
{
    [Header("필요한 컴포넌트들")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private CapsuleCollider zombieCollider;
    private AudioSource theAudio;

    [Header("플레이어 컴포넌트(스포너에서 소환시 자동 지정)")]
    [SerializeField] private Transform playerTransform;
    private PlayerStatus playerStat;

    [Header("좀비의 스탯")]
    public int maxHp;
    public int currentHp;
    [SerializeField] private float runSpeed;
    public float applySpeed;
    [SerializeField] private float runRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int money; //좀비가 죽으면 주는 돈
    [SerializeField] private float dollDetectRange = 2.5f; // 좀비가 인형을 인식할 거리
    [SerializeField] private int zombieDamage;

    private DollStatus targetDoll; // 공격 타겟이 인형일 때 저장
    private DollStatus prevTargetDoll; // 이전 인형 타겟 저장

    [Header("좀비의 사운드")]
    [SerializeField] private AudioClip sound_zombie_attack;
    [SerializeField] private AudioClip sound_zombie_Dead;
    [SerializeField] private AudioClip sound_zombie_normal;

    bool isRunning = false;
    bool isSlowDown;
    bool isAttack;
    float lastAttackTime = -1f;
    private RaycastHit hit;
    private ObjectPool<GameObject> pool;

    private bool isDead;

    private float slowMultiplier = 1f;

    void Awake()
    {
        theAudio = GetComponent<AudioSource>();
        zombieCollider = gameObject.GetComponent<CapsuleCollider>();
        nav = gameObject.GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        isSlowDown = false;
        isDead = false;
        isAttack = false;
        zombieCollider.enabled = true;
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
            return;

        Debug.DrawRay(transform.position+ new Vector3(0f, 1.5f, 0f) , transform.forward * attackRange, Color.red);

        TryAttack();
        if (isDead)
            return;
        targetDoll = FindNearbyDoll();
        if (targetDoll != prevTargetDoll)
        {
            // 이전 타겟이 존재했다면 currentHolders 감소
            if (prevTargetDoll != null)
                prevTargetDoll.currentHolders--;
            // 새 타겟이 있다면 currentHolders 증가
            if (targetDoll != null)
                targetDoll.currentHolders++;
            // prevTargetDoll 갱신
            prevTargetDoll = targetDoll;
        }
        if (targetDoll == null)
        {   //탐지되는 인형이 없다면
            Debug.Log("Follow Player");
            FollowPlayer();
        }
        else
        {   //탐지되는 인형이 있다면
            Debug.Log("Follow Doll");
            FollowDoll();
        }
    }
    DollStatus FindNearbyDoll()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dollDetectRange);
        foreach (var hit in hits)
        {
            // "Doll" 태그를 가진 오브젝트만 찾음
            if (hit.CompareTag("Doll"))
            {
                DollStatus doll = hit.GetComponent<DollStatus>();
                if (doll != null && doll.currentHp > 0 && doll.currentHolders < doll.enemyHoldLimit)
                    return doll;
            }
        }
        return null;
    }

    void FollowDoll()
    {
        float distToWall = Vector3.Distance(transform.position, targetDoll.transform.position);
        if (distToWall <= attackRange)
        {
            nav.isStopped = true;
        }
        else
        {
            nav.SetDestination(targetDoll.transform.position);
        }
    }

    void FollowPlayer()
    {
        if (nav != null && nav.enabled && nav.isOnNavMesh)
        {
            if (isAttack)
                return;

            nav.SetDestination(playerTransform.position);

            Running();
            nav.speed = applySpeed;
        }
    }

    private void TryAttack()
    {
        if(Physics.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), transform.forward, out hit, attackRange))
        {

            if (hit.transform.tag == "Wall" || hit.transform.tag == "Player" || hit.transform.tag == "Doll")
            {
                if(!isAttack)
                    isAttack = true;

                nav.isStopped = true;

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PlaySE(sound_zombie_attack);
                    anim.SetTrigger("Attack");

                    lastAttackTime = Time.time;
                }
            }
           
        }

    }

    public void Attack()
    {
        //플레이어가 움직여 공격을 피한경우
        if(hit.transform == null)
        {
            Debug.Log("공격이 빗나갔다");
            return;
        }

        if (hit.transform.tag == "Wall")
        {
            hit.transform.GetComponent<WallStatus>().TakeDamage(zombieDamage);
        }
        else if(hit.transform.tag == "Doll")
        {
            transform.LookAt(hit.transform.position);
            hit.transform.GetComponent<DollStatus>().TakeDamage(zombieDamage);
        }
        else if (hit.transform.tag == "Player")
        {
            //플레이어가 죽지 않았으면 데미지를 준다.
            if (!GameManager.isPlayerDead)
                playerStat.TakeDamage(zombieDamage);
        }
        
    }
    

    public void FinishAttack()
    {
        isAttack = false;
    }

    void Running()
    {
        isRunning = true;
        anim.SetBool("Running", true);

        nav.isStopped = false;

        if(!isSlowDown)
        applySpeed = runSpeed;
    }
    public void SetPool(ObjectPool<GameObject> pool) 
    { // 좀비 생성시 호출
        this.pool = pool; 
    }

    IEnumerator Die()
    { // 좀비 사망시 호출
        isDead = true;
        zombieCollider.enabled = false;
        nav.isStopped = true;

        PlaySE(sound_zombie_Dead);
        anim.SetTrigger("Dead");
        GameManager.AddMoney(money);

        if (targetDoll != null)
        {
            targetDoll.currentHolders--;
            targetDoll = null;
        }

        WaveManager.Instance.OnZombieKilled();
        WaveManager.Instance.PrintZombieCount(); //디버그용

        yield return new WaitForSeconds(1.5f);
        
        pool.Release(gameObject);
    }

    public void decreaseHp(int m_damage)
    {
        if(currentHp - m_damage > 0)
            currentHp = -m_damage;
        else
        {
            currentHp = 0;
            StartCoroutine(Die());
        }
    }

    public void SetPlayerTransform(Transform m_player, PlayerStatus m_playerStat)
    {
        playerTransform = m_player;
        playerStat = m_playerStat;
    }
    
    // ★ 감속 배수를 변경 (거미줄 등 느려지는 효과 적용)
    public void SlowDown(float multiplier)
    {
        Debug.Log("실행");
        isSlowDown = true;
        slowMultiplier = multiplier;
        ApplySpeed(applySpeed);
    }

    // ★ 원래 속도로 복원
    public void RestoreSpeed()
    {
        slowMultiplier = 1f;
        ApplySpeed(applySpeed);
        isSlowDown = false;
    }

    // ★ 기본 속도에 감속 배수 적용
    private void ApplySpeed(float baseSpeed)
    {
        Debug.Log("Success");
        applySpeed = baseSpeed * slowMultiplier;
    }

    private void PlaySE(AudioClip clip)
    {
        theAudio.clip = clip;
        theAudio.Play();
    }
}
