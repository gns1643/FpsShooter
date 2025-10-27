
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] private float attackRadius = 0.8f;   // Overlap 캡슐 반경 
    [SerializeField] private float attackLength = 0.9f;   // 전방 길이 [
    [SerializeField] private LayerMask attackMask;         // 공격가능 레이어

    private DollStatus targetDoll; // 공격 타겟이 인형일 때 저장
    private DollStatus prevTargetDoll; // 이전 인형 타겟 저장

    [Header("좀비의 사운드")]
    [SerializeField] private AudioClip sound_zombie_attack;
    [SerializeField] private AudioClip sound_zombie_Dead;
    [SerializeField] private AudioClip sound_zombie_normal;

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
        
        targetDoll = FindNearbyDoll();
        //if (targetDoll != prevTargetDoll)
        //{
        //    // 이전 타겟이 존재했다면 currentHolders 감소
        //    if (prevTargetDoll != null)
        //        prevTargetDoll.currentHolders--;
        //    // 새 타겟이 있다면 currentHolders 증가
        //    if (targetDoll != null)
        //        targetDoll.currentHolders++;
        //    // prevTargetDoll 갱신
        //    prevTargetDoll = targetDoll;
        //}
        if (targetDoll == null)
            FollowPlayer();
        else
            FollowDoll();
        TryAttack();
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
        // 타깃 우선순위: Doll 있으면 Doll,  없으면 Player
        Transform target = null;
        if (targetDoll != null) target = targetDoll.transform;
        else if (playerTransform != null) target = playerTransform;

        //타겟이 플레이어 일때 벽을 감지하면 공격하도록 함
        if (target == playerTransform)
        {
            RaycastHit hitInfo;
            Vector3 start = transform.position + Vector3.up * 1.0f; // 좀비 눈높이
            Vector3 direction = (playerTransform.position - start).normalized;
            float dist = Vector3.Distance(start, playerTransform.position);

            if (Physics.Raycast(start, direction, out hitInfo, dist, attackMask))
            {
                if (hitInfo.collider.CompareTag("Wall"))
                {
                    // 벽을 우선 타겟으로 설정
                    target = hitInfo.collider.transform;
                }
            }
        }

        if (target == null) return;

        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget <= attackRange)
        {
            if (!isAttack) isAttack = true;

            // 이동 정지 및 타겟 바라보기
            if (nav) nav.isStopped = true;
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir);

            // 쿨타임 체크 후 애니메이션 트리거
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if(!GameManager.isPlayerDead) //플레이어가 살아 있을때만 공격 실행
                    PlaySE(sound_zombie_attack);
                anim.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            //처음에 navmesh를 enemyspawner에서 껐다 키기 때문에 처음에 navmesh가 꺼져서 오류가나기때문에 이 예외상황을 막기위해 적음
            if (nav.enabled == false)
                return;
            // 사정거리 밖이면 이동 재개
            if (nav) 
                nav.isStopped = false;
        }

    }

    public void Attack()
    {
        if (GameManager.isPlayerDead)
            return;
        Vector3 start = transform.position + new Vector3(0f, 1.2f, 0f);
        Vector3 end = start + transform.forward * attackLength;

        Collider[] hits = Physics.OverlapCapsule(start, end, attackRadius, attackMask);
        if (hits == null || hits.Length == 0) return;

        foreach(var c in hits)
        {
            if (!c)
                return;
            if(c.CompareTag("Doll"))
                c.transform.GetComponent<DollStatus>().TakeDamage(zombieDamage);
            else if (c.CompareTag("Wall"))
                c.transform.GetComponent<WallStatus>().TakeDamage(zombieDamage);
            else if (c.CompareTag("Player"))
                c.transform.GetComponent<PlayerStatus>().ShieldDamage(zombieDamage);
        }
        
    }
    

    public void FinishAttack()
    {
        isAttack = false;
        if (nav) nav.isStopped = false;
    }

    void Running()
    {
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
            currentHp -= m_damage;
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
