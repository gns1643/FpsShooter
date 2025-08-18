
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
    [SerializeField] private float walkSpeed;
    public float applySpeed;
    [SerializeField] private float runRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int money; //좀비가 죽으면 주는 돈
    [SerializeField] private int zombieDamage;
    [SerializeField] private bool IsCopZombie;

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
        FollowPlayer();
    }
  

    void FollowPlayer()
    {
        if (nav != null && nav.enabled && nav.isOnNavMesh)
        {
            if (isAttack)
                return;

            nav.SetDestination(playerTransform.position);

            if (IsCopZombie)
                Walking();

            else
            {
                //플레이어와의 거리 계산
                float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

                //플레이어와의 거리에따라 뛰기, 공격, 걷기 전환
                if (distanceToPlayer >= runRange)
                    Running();
                else
                    Walking();
            }

            nav.speed = applySpeed;
        }
    }

    private void TryAttack()
    {
        if(Physics.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), transform.forward, out hit, attackRange))
        {

            if (hit.transform.tag == "Wall" || hit.transform.tag == "Player")
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
        if (!isRunning && !IsCopZombie)
        {
            isRunning = true;
            anim.SetBool("Running", true);
        }

        nav.isStopped = false;

        if(!isSlowDown)
        applySpeed = runSpeed;
    }
    void Walking()
    {
        if (isRunning&& !IsCopZombie)
        {
            isRunning = false;
            anim.SetBool("Running", false);
        }
        nav.isStopped = false;

        if (!isSlowDown)
        applySpeed = walkSpeed;
    }

    public void SetPool(ObjectPool<GameObject> pool) 
    { // 좀비 생성시 호출
        this.pool = pool; 
    }

    IEnumerator  Die()
    { // 좀비 사망시 호출
        isDead = true;
        zombieCollider.enabled = false;
        nav.isStopped = true;

        PlaySE(sound_zombie_Dead);
        anim.SetTrigger("Dead");
        GameManager.AddMoney(money);

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
