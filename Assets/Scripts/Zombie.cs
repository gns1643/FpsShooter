using GLTF.Schema;
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Zombie : MonoBehaviour
{

    [Header("필요한 컴포넌트들")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private Transform playerTransform;
    private PlayerStatus playerStat;

    [Header("좀비의 스탯")]
    [SerializeField] int maxHp;
    [SerializeField] private int money; //좀비가 죽으면 주는 돈
    public int currentHp;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    private CapsuleCollider zombieCollider;
    private RaycastHit hit;

    bool isRunning = false;
    bool isAttack = false;
    float lastAttackTime = -1f;
    private ObjectPool<GameObject> pool;

    private bool isDead;

    private float slowMultiplier = 1f;
    void Awake()
    {
        zombieCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        isDead = false;
        zombieCollider.enabled = true;
        currentHp = maxHp;
        nav = gameObject.GetComponent<NavMeshAgent>();
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
        if (nav != null)
        {
            if (isAttack)
                return;

            nav.SetDestination(playerTransform.position);
            //플레이어와의 거리 계산
            float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

            //플레이어와의 거리에따라 뛰기, 공격, 걷기 전환
            if (distanceToPlayer >= runRange)
                Running();
            else
                Walking();
        }
        else
        {
            Debug.Log("좀비가 쫓을 목표가 없음");
        }
    }

    private void TryAttack()
    {
        if(Physics.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), transform.forward, out hit, attackRange))
        {

            if (hit.transform.tag == "Wall" || hit.transform.tag == "Player")
            {
                isAttack = true;

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    anim.SetTrigger("Attack");

                    if (hit.transform.tag == "Wall")
                    {
                        hit.transform.GetComponent<WallStatus>().TakeDamage(10);
                    }
                    else if (hit.transform.tag == "Player")
                    {
                        //플레이어가 죽지 않았으면 데미지를 준다.
                        if(!GameManager.isPlayerDead)
                             playerStat.TakeDamage(10);
                    }

                    lastAttackTime = Time.time;
                }

                nav.isStopped = true; // 공격 중 이동 정지
            }
            //감지했어도 wall이나 palyer가 아니었을 경우
            else
            {
                isAttack = false;
            }
        }
        //감지를 못했을 경우
        else
        {
            isAttack = false;
        }

    }

    void Running()
    {
        if (!isRunning)
        {
            isRunning = true;
            anim.SetBool("Running", true);
        }
        nav.isStopped = false;
        nav.speed = runSpeed;
    }
    void Walking()
    {
        if (isRunning)
        {
            isRunning = false;
            anim.SetBool("Running", false);
        }
        nav.isStopped = false;
        nav.speed = walkSpeed;

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
        anim.SetTrigger("Dead");
        GameManager.AddMoney(money);

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
        slowMultiplier = multiplier;
        if (isRunning) ApplySpeed(runSpeed);
        else ApplySpeed(walkSpeed);
    }

    // ★ 원래 속도로 복원
    public void RestoreSpeed()
    {
        slowMultiplier = 1f;
        if (isRunning) ApplySpeed(runSpeed);
        else ApplySpeed(walkSpeed);
    }

    // ★ 기본 속도에 감속 배수 적용
    private void ApplySpeed(float baseSpeed)
    {
        nav.speed = baseSpeed * slowMultiplier;
    }


}
